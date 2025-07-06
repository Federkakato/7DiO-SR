#if UNITY_EDITOR
using UnityEditor;

namespace Gaskellgames.InputEventSystem.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    [InitializeOnLoad]
    public class DefineSymbols_InputEventSystem
    {
        #region Variables
        
        private static readonly string[] ExtraScriptingDefineSymbols = new string[] { "GASKELLGAMES_INPUTEVENTSYSTEM" };
        
        private static readonly string link_GgCore = "<a href=\"https://assetstore.unity.com/packages/tools/utilities/ggcore-gaskellgames-304325\">GgCore</a>";
        private static readonly string error = $"{link_GgCore} not detected: The Input Event System package from Gaskellgames requires {link_GgCore}. Please add the package from the package manager, or claim it for FREE from the Unity Asset Store using the same account that has the Input Event System asset licence.";
        
        #endregion
        
        //----------------------------------------------------------------------------------------------------
        
        #region Constructors

        static DefineSymbols_InputEventSystem()
        {
#if !ENABLE_INPUT_SYSTEM
            UnityEngine.Debug.LogError("Input System not detected! Please download via the package manager.");
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