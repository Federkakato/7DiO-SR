using UnityEngine;

namespace Gaskellgames.InputEventSystem
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    public class SelectableObject : GgMonoBehaviour
    {
        #region Variables
        
        [SerializeField, ReadOnly]
        [Tooltip("")]
        private bool isSelected;
        
        /// <summary>
        /// Called when this object is selected.
        /// </summary>
        public GgEvent onSelected;
        
        /// <summary>
        /// Called when this object is deselected.
        /// </summary>
        public GgEvent onDeselected;
        
        #endregion
        
        //----------------------------------------------------------------------------------------------------
        
        #region Getter / Setter
        
        /// <summary>
        /// Set whether the object is selected or not.
        /// </summary>
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                bool previousValue = isSelected;
                isSelected = value;
                
                if (value && !previousValue)
                {
                    onSelected?.Invoke();
                }
                else if (!value && previousValue)
                {
                    onDeselected?.Invoke();
                }
            }
        }
        
        #endregion
        
    } // class end
}