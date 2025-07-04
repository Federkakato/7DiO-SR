using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SimpleFPSMovement : MonoBehaviour
{
    [Header("Geschwindigkeiten")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 7f;
    public float crouchSpeed = 2f;

    [Header("Springen & Gravitation")]
    public float jumpHeight = 1.2f;
    public float gravity = -20f;

    [Header("Maus‑Look")]
    public float mouseSensitivity = 2f;
    public Transform cam;                      // deine Kamera hier reinziehen

    [Header("Crouch‑Settings")]
    public float crouchHeight = 1f;

    [Header("Head Bobbing")]
    public float bobSpeedWalk = 6f;   // Frequenz beim Gehen
    public float bobSpeedSprint = 10f;  // Frequenz beim Sprinten
    public float bobAmountWalk = 0.05f;
    public float bobAmountSprint = 0.1f;


    CharacterController cc;
    Vector3 velocity;
    float standHeight;
    bool isCrouching;
    float bobbingTimer;
    Vector3 camOrigin;
    bool CanStandUp()
    {
        // Wir casten eine kleine Kapsel in der Zielhöhe nach oben
        float standCheckHeight = standHeight;
        float radius = cc.radius - 0.05f;          // etwas Puffer
        Vector3 bottom = transform.position + Vector3.up * radius;
        Vector3 top = bottom + Vector3.up * standCheckHeight - Vector3.up * 2f * radius;

        // Prüfe, ob etwas im Weg ist (Layer „Default“ oder was immer kollidiert)
        return !Physics.CheckCapsule(bottom, top, radius, ~0, QueryTriggerInteraction.Ignore);
    }

    bool IsGrounded()
    {
        // Raycast nach unten, etwas länger als die Hälfte der CharacterController-Höhe
        return Physics.Raycast(transform.position, Vector3.down, cc.height / 2 + 0.1f);
    }


    void Start()
    {
        cc = GetComponent<CharacterController>();
        standHeight = cc.height;
        if (cam == null)
        {
            Debug.LogError("Cam ist nicht zugewiesen! Bitte ziehe deine Kamera in das 'cam' Feld im Inspector.");
            // Optional: Hier könntest du abbrechen, wenn cam wichtig ist:
            // enabled = false;
            return;
        }

        camOrigin = cam.localPosition;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        BobHead();
        Look();
        Move();
        Jump();
        CrouchToggle();
        ApplyGravity();
    }

    /* ---------- Maus‑Look ---------- */
    void Look()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        if (cam)
        {
            float pitch = cam.localEulerAngles.x - mouseY;
            if (pitch > 180) pitch -= 360;
            pitch = Mathf.Clamp(pitch, -80, 80);
            cam.localEulerAngles = new Vector3(pitch, 0, 0);
        }
    }

    /* ---------- Bewegung ---------- */
    void Move()
    {
        float h = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
        float v = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);

        Vector3 dir = (transform.right * h + transform.forward * v).normalized;
        float spd = isCrouching ? crouchSpeed :
                      Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        cc.Move(dir * spd * Time.deltaTime);
    }

    /* ---------- Springen ---------- */
    void Jump()
    {
        // Check whether we are touching the ground
        bool grounded = IsGrounded();



        // Pull the character down softly so it stays grounded
        if (grounded && velocity.y < 0f)
            velocity.y = -2f;

        // Basic jump: only when grounded, not crouching, and Space pressed this frame
        if (grounded && !isCrouching && Input.GetKeyDown(KeyCode.Space))
        {
            // v = √(2gh)  ->  velocity needed to reach the desired jumpHeight
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }



    /* ---------- Ducken ---------- */
    void CrouchToggle()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
            TryToggleCrouch();
    }

    void TryToggleCrouch()
    {
        bool wantCrouch = !isCrouching;

        // Wenn wir aufstehen wollen, erst prüfen ob genug Platz ist
        if (!wantCrouch && !CanStandUp()) return;

        SetCrouch(wantCrouch);
    }


    void SetCrouch(bool state)
    {
        isCrouching = state;
        cc.height = state ? crouchHeight : standHeight;
        var c = cc.center;
        c.y = cc.height / 2f;
        cc.center = c;

        // Kamera ggf. mitbewegen (optional)
        float camOffset = state ? -0.5f : 0.5f;          // anpassen
        cam.localPosition += Vector3.up * camOffset;
    }

    /* ---------- Gravitation ---------- */
    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
    }
    void BobHead()
    {
        if (!cam) return;

        // Bewegungs-Geschwindigkeit nur auf der XZ Ebene
        Vector3 horizontalVel = cc.velocity;
        horizontalVel.y = 0;
        float speed = horizontalVel.magnitude;

        bool grounded = cc.isGrounded;
        bool moving = speed > 0.1f;

        if (grounded && moving)
        {
            bool sprinting = Input.GetKey(KeyCode.LeftShift) && !isCrouching;
            float bobSpeed = sprinting ? bobSpeedSprint : bobSpeedWalk;
            float bobAmount = sprinting ? bobAmountSprint : bobAmountWalk;

            bobbingTimer += Time.deltaTime * bobSpeed;
            float bob = Mathf.Sin(bobbingTimer) * bobAmount;

            cam.localPosition = camOrigin + Vector3.up * bob;
        }
        else
        {
            bobbingTimer = 0f;   // reset Phase
                                 // sanft zurück zur Ausgangsposition
            cam.localPosition = Vector3.Lerp(cam.localPosition,
                                             camOrigin,
                                             Time.deltaTime * bobSpeedWalk);
        }
    }

}
