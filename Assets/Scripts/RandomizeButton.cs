using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
 
[CustomEditor(typeof(ProjectileWeaponScriptableObjects))]


public class ProjectileWeaponRandomize : Editor
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
