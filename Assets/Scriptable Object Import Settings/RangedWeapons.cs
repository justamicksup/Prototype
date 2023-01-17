using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapons : Weapon
{
    
    [SerializeField] ScriptableObjectRangedWeapons _scriptableObjectRangedWeapons; 
    
    //public GameObject model;
    internal int level = 1;
    internal int attack = 5;
    internal int range = 5;
    internal float shootRate = 1f;
    internal float shootForce = 1;
    internal int ammoCapacity = 6;
    internal int ammoRemaining = 9;
    internal float reloadTime = 0.1f;
    
    
    public RangedWeapons SetStats(Weapon _rangedWeapons)
    {
        RangedWeapons rangedWeapons = (RangedWeapons)_rangedWeapons;
        
        rangedWeapons.level = _scriptableObjectRangedWeapons.Level;
        rangedWeapons.attack = _scriptableObjectRangedWeapons.Attack;
        rangedWeapons.range = _scriptableObjectRangedWeapons.Range;
        rangedWeapons.shootRate = _scriptableObjectRangedWeapons.ShootRate;
        rangedWeapons.shootForce = _scriptableObjectRangedWeapons.ShootForce;
        rangedWeapons.ammoCapacity = _scriptableObjectRangedWeapons.AmmoCapacity;
        rangedWeapons.ammoRemaining = _scriptableObjectRangedWeapons.AmmoRemaining;
        rangedWeapons.reloadTime = _scriptableObjectRangedWeapons.ReloadTime;
    
        return rangedWeapons;
        
    }
   
}