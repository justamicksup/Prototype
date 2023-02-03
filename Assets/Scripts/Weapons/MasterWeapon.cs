using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public abstract class MasterWeapon: ScriptableObject
    {
        public GameObject Model;

        public WeaponType weaponType;
        
        public enum WeaponType
        {
            none, sword, pistol, musket, bomb
        }
    }
