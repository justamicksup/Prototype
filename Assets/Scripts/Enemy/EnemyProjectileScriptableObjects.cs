
using UnityEngine;
[CreateAssetMenu(fileName = "Enemy (Projectile)", menuName = "Enemy Factory/Enemy (Projectile)")]
public class EnemyProjectileScriptableObjects : MasterEnemy
    {
        public int shootDamage = 5;
        public int range = 5;
        public float shootRate = 1f;
        public float shootForce = 1;
        public int bulletSpeed;
        public int ammoCapacity = 6;
        public int ammoRemaining = 6;
        public float reloadTime = 0.1f;
        public AudioClip audGunShot;
    }
