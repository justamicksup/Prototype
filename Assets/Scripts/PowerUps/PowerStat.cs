using UnityEngine;

[CreateAssetMenu]

public class PowerStat : ScriptableObject
{
    [Range(0.1f, 100.0f)] [SerializeField] public float effectDuration;
    public int healthBonus;
    public int speedBonus;
    public int staminaBonus;
    [Range(0, 2147483000)]public int shootDmgBonus;
    [Range(0, 2147483000)]public int meleeDmgBonus;
    public int goldBonus;
    public int ammoBonus;
    public MasterWeapon weapon;
    public AudioClip powerAudio;
}
