using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class RangedWeapon : ScriptableObject
{
    public GameObject gunModel;
    public AudioClip gunShotAudio;
    public float shootRate;
    public int shootDist;
    public int shootDmg;
    public float reloadSpeed;
    public float shootForce;
    public int magSize; //make private later
    public int ammoRemaining;
    public int gunAmmo;
    public int gunAmmoCap;//make private later
}
