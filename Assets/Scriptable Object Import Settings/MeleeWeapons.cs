using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class MeleeWeapons : Weapon
{
   
    // internal Armory.EnumWeaponCategory weaponCategory = Armory.EnumWeaponCategory.Melee;
    // public Armory.EnumMeleeWeapons meleeWeaponsType = Armory.EnumMeleeWeapons.SwordAndShield;
    
    [SerializeField] int attack = 5;
    [SerializeField] int weaponReach = 5;
    [Range(1, 10)] [SerializeField] private float knockbackForce = 1;
    [Range(0.1f, 10)] [SerializeField] float swingSpeed = 0.1f;
    [SerializeField] public GameObject model;
    [SerializeField] private int level = 1;
    
    // may need more for Game Manager to access  SerializedFields
   
}
