using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Projectile Weapon", menuName = "Weapon Factory/Projectile Weapon")]
public class ProjectileWeaponScriptableObjects : MasterWeapon
{
    // all weapons share
    public int gunLevel = 1;
    public int shootDamage = 5;
    public int range = 5;
    public float shootRate = 1f;
    public float shootForce = 1;
    public int ammoCapacity = 6;
    public int ammoRemaining = 6;
    public float reloadTime = 0.1f;
    public AudioClip audGunShot;
    
    public void Randomize()
    {
        
        gunLevel = Random.Range(1, 10);
        shootDamage =Random.Range(10, 100);
        range = Random.Range(10, 100);
        shootRate = Random.Range(0.1f, 1);
        shootForce =Random.Range(1, 5);
        ammoCapacity =Random.Range(5, 30);
        ammoRemaining = ammoCapacity;
        reloadTime = Random.Range(0.1f, 2f);
    }
}