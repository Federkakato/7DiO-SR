#if UNITY_EDITOR
using UnityEditor;

namespace Gaskellgames.CameraSystem.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    [InitializeOnLoad]
    public class DefineSymbols_CameraSystem
    {
        #region Variables
        
        private static readonly string[] ExtraScriptingDefineSymbols = new string[] { "GASKELLGAMES_CAMERASYSTEM" };
        
        private static readonly string link_GgCore = "<a href=\"https://assetstore.unity.com/packages/tools/utilities/ggcore-gaskellgames-304325\">GgCore</a>";
        private static readonly string error = $"{link_GgCore} not detected: The Camera System package from Gaskellgames requires {link_GgCore}. Please add the package from the package manager, or claim it for FREE from the Unity Asset Store using the same account that has the Camera System asset licence.";
        
        private static readonly string link_InputEvent = "<a href=\"https://assetstore.unity.com/packages/tools/input-management/input-event-system-gaskellgames-305184\">Input Event System</a>";
        private static readonly string error2 = $"{link_InputEvent} not detected: The Camera System package from Gaskellgames requires {link_InputEvent}. Please add the package from the package manager, or claim it for FREE from the Unity Asset Store using the same account that has the Camera System asset licence.";
        
        #endregion
        
        //----------------------------------------------------------------------------------------------------
        
        #region Constructors

        static DefineSymbols_CameraSystem()
        {
#if !GASKELLGAMES_INPUTEVENTSYSTEM
            UnityEngine.Debug.LogError(error2);
#endif
#if GASKELLGAMES
            Gaskellgames.EditorOnly.DefineSymbols_GgCore.AddExtraScriptingDefineSymbols(ExtraScriptingDefineSymbols);
#else
            UnityEngine.Debug.LogError(error);
#endif
        }
        
        #endregion
        
    } // class end
}

#endif