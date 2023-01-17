using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class ExplosiveWeapons : Weapon
{
    // internal Armory.EnumWeaponCategory weaponCategory = Armory.EnumWeaponCategory.Explosives;
    // public Armory.EnumExplosiveWeapons explosiveWeaponsType = Armory.EnumExplosiveWeapons.Grenade;

    [SerializeField] int attack = 5;
    [SerializeField] int range = 5;
    [SerializeField] float splashRadius = 5; 
    [Range(1, 10)] [SerializeField] float knockbackForce = 1;
    [Range(1, 100)] [SerializeField] int explosivesCapacity = 1;
    [Range(0.1f, 10)] [SerializeField] float cookoffTime = 0.1f;
    [SerializeField] public GameObject model;
    [SerializeField] private int level = 1;
    
    // may need more for Game Manager to access  SerializedFields
    public int Attack
    {
        get => attack;
        private set=> attack = value;
    }

   
}
