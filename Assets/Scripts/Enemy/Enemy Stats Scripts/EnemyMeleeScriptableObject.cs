
using UnityEngine;

namespace Enemy.Enemy_Stats_Scripts
{
    [CreateAssetMenu(fileName = "Enemy (Melee)", menuName = "Enemy Factory/Enemy (Melee)")]
    public class EnemyMeleeScriptableObject: MasterEnemy
    {
        public int attack = 1;
        public float swingRate = 0.1f;
        public int swingAngle;
        public int viewAngle;
        public int rotationSpeed = 50;
        public bool hasWeapon;
        public AudioClip[] audWeaponSwing;
        public GameObject deathEffect;
    }
}
