
using UnityEngine;
[CreateAssetMenu(fileName = "Enemy (Explosive)", menuName = "Enemy Factory/Enemy (Explosive)")]
    public class EnemyExplosiveScriptableObjects : MasterEnemy
    {
        
        [SerializeField] int throwingDistance;
        [SerializeField] GameObject bomb;
        
    }
