using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Armory", menuName = "Scriptable Objects/Armory")]
public class Armory : ScriptableObject
{
    [SerializeField] public List<MasterWeapon> MasterWeaponList;
    
}