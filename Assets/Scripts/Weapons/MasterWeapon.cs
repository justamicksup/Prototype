using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MasterWeapon: ScriptableObject
{
    public GameObject Model;

    public WeaponType weaponType;
    public Vector3 rotationOffset;
    public Vector3 positionOffset;
        
    public enum WeaponType
    {
        none, sword, pistol, musket, bomb
    }
}
