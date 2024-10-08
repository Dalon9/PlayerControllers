namespace VRTK.Editor
{
    using UnityEngine;
    using UnityEditor;
    using VRTK;
    
    [CustomEditor(typeof(VRTK_RadialMenu))]
    public class VRTK_RadialMenuEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            VRTK_RadialMenu rMenu = (VRTK_RadialMenu)target;
            if (GUILayout.Button("Regenerate Buttons"))
            {
                rMenu.RegenerateButtons();
            }
        }
    }
}