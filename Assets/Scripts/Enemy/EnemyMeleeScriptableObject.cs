
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy (Melee)", menuName = "Enemy Factory/Enemy (Melee)")]
    public class EnemyMeleeScriptableObject: MasterEnemy
    {
        public int attack = 1;
        public float swingRate = 0.1f;
        public AudioClip audWeaponSwing;
    }
