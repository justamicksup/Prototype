using UnityEngine;

namespace Enemy.Enemy_Stats_Scripts
{
    [CreateAssetMenu(fileName = "Enemy (BOSS)", menuName = "Enemy Factory/Enemy (BOSS)")]
    public class BossScriptableObject: MasterEnemy
    {
        public int throwingDistance;
        public GameObject bomb;
        public int throwAngle = 45;
        public int viewAngle = 60;
        public int forceForward= 500;
        public int forceUpward= 250;
        public int torque= 500;
        public int rotationSpeed = 50;
        public GameObject deathEffect;
        
        public int attack = 1;
        public float swingRate = 0.1f;
        public int swingAngle;
        public bool hasWeapon;
        public AudioClip[] audWeaponSwing;
        
        
        public int shootDamage = 5;
        public int range = 5;
        public float shootRate = 1f;
        public float shootForce = 1;
        public int bulletSpeed;
        public int ammoCapacity = 6;
        public int ammoRemaining = 6;
        public float reloadTime = 0.1f;
        public int shootAngle = 45;
        public AudioClip[] audGunShot;
    }
}
