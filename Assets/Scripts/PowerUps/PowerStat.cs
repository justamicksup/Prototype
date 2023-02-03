using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class PowerStat : ScriptableObject
{
    [Range(0.1f, 30.0f)] [SerializeField] public float effectDuration;
    public int healthBonus;
    public int speedBonus;
    public int staminaBonus;
    [Range(0, 2147483000)]public int shootDmgBonus;
    [Range(0, 2147483000)]public int meleeDmgBonus;
    public int goldBonus;
    public AudioClip powerAudio;
}
