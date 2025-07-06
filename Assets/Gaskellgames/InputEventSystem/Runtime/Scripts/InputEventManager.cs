#if GASKELLGAMES
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

#if UNITY_EDITOR
using Gaskellgames.EditorOnly;
#endif

namespace Gaskellgames.InputEventSystem
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    [AddComponentMenu("Gaskellgames/Input Event System/Input Event Manager")]
    public class InputEventManager : GgMonoBehaviour
    {
        #region Variables

        [SerializeField, Required]
        [Tooltip("Automatically enabled and disabled action assets")]
        private List<InputActionAsset> inputActionAssets;

        [SerializeField]
        public GgEvent<InputDevice> onDeviceAdded;
        
        [SerializeField, Space]
        public GgEvent<InputDevice> onDeviceRemoved;

        [SerializeField, Space]
        public GgEvent<InputControlScheme> onControlsChanged;

        [SerializeField, ShowAsTag]
        [Tooltip("Current input device name. (InputDevice.displayName)")]
        private string activeDevice;

        [SerializeField, ShowAsTag]
        [Tooltip("Current control scheme name. (InputControlScheme.name)")]
        private string activeControlScheme;

        [SerializeField, ReadOnly]
        [Tooltip("String list of all device names")]
        private List<string> deviceList = new List<string>();

        [SerializeField, ReadOnly]
        [Tooltip("String list of all control schemes")]
        private List<string> controlSchemes = new List<string>();
        
        private List<InputDevice> devices = new List<InputDevice>();
        private InputDevice lastUsedInputDevice;
        
        #endregion
        
        //----------------------------------------------------------------------------------------------------
        
        #region Game loop

#if UNITY_EDITOR

        private void Reset()
        {
            SetupInputActionManager();
        }
        
#endif

        private void OnEnable()
        {
            EnableAllInputActions();
            devices = InputSystem.devices.ToList();
            UpdateDeviceList();
            UpdateControlSchemeList();
            
            InputSystem.onDeviceChange += InputSystem_OnDeviceChange;
            InputSystem.onEvent += InputSystem_OnEvent;
        }

        private void OnDisable()
        {
            InputSystem.onDeviceChange -= InputSystem_OnDeviceChange;
            InputSystem.onEvent -= InputSystem_OnEvent;
            
            devices = new List<InputDevice>();
            UpdateDeviceList();
            UpdateControlSchemeList();
            DisableAllInputActions();
        }

        private void InputSystem_OnDeviceChange(InputDevice inputDevice, InputDeviceChange inputDeviceChange)
        {
            switch (inputDeviceChange)
            {
                case InputDeviceChange.Added:
                    devices.TryAdd(inputDevice);
                    UpdateDeviceList();
                    onDeviceAdded.Invoke(inputDevice);
                    Log(LogType.Log, "Device Added: {0}", inputDevice);
                    break;
                
                case InputDeviceChange.Removed:
                    devices.Remove(inputDevice);
                    UpdateDeviceList();
                    onDeviceRemoved.Invoke(inputDevice);
                    Log(LogType.Log, "Device Removed: {0}", inputDevice);
                    break;
            }
        }
        
        private void InputSystem_OnEvent(InputEventPtr inputEventPointer, InputDevice inputDevice)
        {
            // check for change in inputDevice
            if (lastUsedInputDevice == inputDevice) { return; }
            
            // get new inputDevice
            var eventType = inputEventPointer.type;
            if (eventType == StateEvent.Type)
            {
                if (!inputEventPointer.EnumerateChangedControls(device: inputDevice, magnitudeThreshold: 0.0001f).Any()) { return; }
            }
            lastUsedInputDevice = inputDevice;
            activeDevice = inputDevice.displayName;
            Log(LogType.Log, "Current Device: {0}", activeDevice);

            // try get control scheme for device
            if (InputSystemExtensions.TryFindControlScheme(inputActionAssets, inputDevice, out InputControlScheme scheme))
            {
                onControlsChanged.Invoke(scheme);
                activeControlScheme = scheme.ToString();
                Log(LogType.Log, "Control Scheme: {0}", activeControlScheme);
            }
        }
        
        #endregion
        
        //----------------------------------------------------------------------------------------------------
        
        #region Private Functions
        
        private void EnableAllInputActions()
        {
            if (inputActionAssets != null)
            {
                foreach (InputActionAsset asset in inputActionAssets)
                {
                    if (asset != null)
                    {
                        asset.Enable();
                    }
                }
            }
        }

        private void DisableAllInputActions()
        {
            if (inputActionAssets != null)
            {
                foreach (InputActionAsset asset in inputActionAssets)
                {
                    if (asset != null)
                    {
                        asset.Disable();
                    }
                }
            }
        }

        private void UpdateDeviceList()
        {
            deviceList = new List<string>();
            foreach (var inputDevice in devices)
            {
                deviceList.TryAdd(inputDevice.displayName);
            }
        }

        private void UpdateControlSchemeList()
        {
            controlSchemes = new List<string>();
            foreach (var inputActionAsset in inputActionAssets)
            {
                foreach (var inputControlScheme in inputActionAsset.controlSchemes)
                {
                    controlSchemes.TryAdd(inputControlScheme.name);
                }
            }
        }

        #endregion
        
        //----------------------------------------------------------------------------------------------------
        
        #region Public Functions

#if UNITY_EDITOR
        
        private const string packageRefName = "InputEventSystem";
        private const string relativePath = "/Runtime/InputActions/";
        
        public void SetupInputActionManager()
        {
            inputActionAssets = new List<InputActionAsset>();
            if (!GgPackageRef.TryGetFullFilePath(packageRefName, relativePath, out string filePath)) { return; }
            AddInputActionToManager(AssetDatabase.LoadAssetAtPath<InputActionAsset>(filePath + "InputActionsGaskellgames.inputactions"));
        }
        
#endif
        
        public void AddInputActionToManager(InputActionAsset iaa)
        {
            if (!inputActionAssets.Contains(iaa))
            {
                inputActionAssets.Add(iaa);
            }
        }
        
        public void RemoveInputActionFromManager(InputActionAsset iaa)
        {
            if (inputActionAssets.Contains(iaa))
            {
                inputActionAssets.Remove(iaa);
            }
        }

        #endregion
        
    } // class end
}
#endif