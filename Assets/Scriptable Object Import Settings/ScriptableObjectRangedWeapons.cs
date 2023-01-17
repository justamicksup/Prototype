using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(fileName = "New Ranged Weapon", menuName = "Scriptable Objects/Ranged Weapons")]
[System.Serializable]
public class ScriptableObjectRangedWeapons : ScriptableObject
{
    

    [SerializeField] int attack = 5;
    [SerializeField] int range = 5;
    [SerializeField] float shootRate = 1f;
    [Range(1, 10)] [SerializeField] float shootForce = 1;
    [Range(1, 100)] [SerializeField] int ammoCapacity = 1;
    [Range(0.1f, 10)] [SerializeField] float reloadTime = 0.1f;
    [SerializeField]  public GameObject model;
    [SerializeField]  int level = 1;
    [Range(1, 100)] [SerializeField] int ammoRemaining = 1;

    // may need more for Game Manager to access  SerializedFields
    public int Attack
    {
        get => attack;
        set => attack = value;
    }

    public int Range
    {
        get => range;
        set => range = value;
    }

    public float ShootRate
    {
        get => shootRate;
        set => shootRate = value;
    }

    public float ShootForce
    {
        get => shootForce;
        set => shootForce = value;
    }

    public int AmmoCapacity
    {
        get => ammoCapacity;
        set => ammoCapacity = value;
    }

    public float ReloadTime
    {
        get => reloadTime;
        set => reloadTime = value;
    }

    public int Level
    {
        get => level;
        set => level = value;
    }

    public int AmmoRemaining
    {
        get => ammoRemaining;
        set => ammoRemaining = value;
    }
   
    
}