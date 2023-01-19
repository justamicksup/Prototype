
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
 
[CustomEditor(typeof(MeleeWeaponScriptableObjects))]


public class MeleeWeaponRandomize : Editor
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