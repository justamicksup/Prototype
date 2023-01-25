using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class PowerStat : ScriptableObject
{
    [SerializeField] public float effectDuration;
    public int speedBonus;
    public int staminaBonus;
    public int shootDmgBonus;
    public int meleeDmgBonus;
    public int goldBonus;
    public AudioClip powerAudio;
}
