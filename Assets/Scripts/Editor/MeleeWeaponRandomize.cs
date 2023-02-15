#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(MeleeWeaponScriptableObjects))]


    public class MeleeWeaponRandomize : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var meleeWeaponScriptableObjects = (MeleeWeaponScriptableObjects)target;
    
            if(GUILayout.Button("Randomize", GUILayout.Height(20)))
            {
                meleeWeaponScriptableObjects.Randomize();
            }
         
        }
    }
}