using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

[CreateAssetMenu(fileName = "Armory", menuName = "Scriptable Objects/Armory")]
public class Armory : ScriptableObject
{
    [SerializeField] public List<MasterWeapon> MasterWeaponList;
    
}