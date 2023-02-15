#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(ProjectileWeaponScriptableObjects))]


    public class ProjectileWeaponRandomize : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var projectileWeaponScriptableObjects = (ProjectileWeaponScriptableObjects)target;
    
            if(GUILayout.Button("Randomize", GUILayout.Height(20)))
            {
                projectileWeaponScriptableObjects.Randomize();
            }
         
        }
    }
}
