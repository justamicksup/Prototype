using System.Collections;
using Enemy.Enemy_Stats_Scripts;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Enemy.Enemy_AI_Scripts
{
    public class MeleeEnemyAI : MonoBehaviour, IDamage
    {
        [FormerlySerializedAs("HP")]
        [Header("----- Melee Enemy Stats From SO -----")]
        [SerializeField] int hp;
        [SerializeField] int rotationSpeed;
        [Range(10, 1000)] [SerializeField] int lootValue;
        [SerializeField] public int attack = 1;
        public float swingRate = 0.1f;
        public AudioClip audWeaponSwing;
        //Need to put in SO
        public int swingAngle;
        public float stoppingDistOrig;
        public int viewAngle;
        public DeathEffect deathEffect;
    
    
        [Header("----- Needed References -----")]
        public MasterEnemy masterEnemyScriptableObject;
        public Animator animator;
        public NavMeshAgent agent;
        public Transform headPos;
        public Collider weaponCollider;
        public MeleeWeapon weapon;
        [SerializeField] Renderer model;
        public AudioSource aud;
        [SerializeField] GameObject weaponDrop;

    
        [Header("----- Variables -----")]
        Vector3 _playerDir;
        bool _isSwinging;
        bool _playerInRange;
        float _angleToPlayer;
        bool _hasWeapon;
        [Range(0, 1)] [SerializeField] float audWeaponSwingVol;
        [SerializeField] private Vector3 offSetPlayerDir = new Vector3(0, 1, 0);
        private static readonly int Attack1H1 = Animator.StringToHash("Attack1h1");
        private static readonly int Hit1 = Animator.StringToHash("Hit1");
        private static readonly int Speed = Animator.StringToHash("Speed");

        // Start is called before the first frame update
        void Start()
        {
            gameManager.instance.updateEnemyRemaining(1);
            GetStats((EnemyMeleeScriptableObject)masterEnemyScriptableObject);
            GetNavMesh();
            stoppingDistOrig = agent.stoppingDistance;
            weaponCollider.enabled = false;


        }

        // Update is called once per frame
        void Update()
        {
            var position = gameManager.instance.player.transform.position;
            _playerDir = position + offSetPlayerDir - headPos.position;
       
       
            agent.SetDestination(position);
       
            animator.SetFloat(Speed, agent.velocity.normalized.magnitude);
        
       
       

            if (_playerInRange)
            {
                CanSeePlayer();
            
            }
        }
        // Need to adjust this in all AI to work like lecture
        public void TakeDamage(float damage)
        {
            hp -= (int)damage;
            animator.SetTrigger(Hit1);
            WeaponColliderOff();
            StartCoroutine(FlashDamage());
            FacePlayer();
            if (agent.isActiveAndEnabled)
            {
                agent.SetDestination(gameManager.instance.player.transform.position);
            }
            if (hp <= 0)
            {
                gameManager.instance.updateEnemyRemaining(-1);
                gameManager.instance.playerScript.addCoins(lootValue);
                deathEffect.DeathByEffects();
            
                Destroy(gameObject);
                gameManager.instance.DropLoot(transform, weaponDrop, false, true, true);
            }
        }
    
        void FacePlayer()
        {
            //don't rotate up or down (Y)
            _playerDir.y = 0;
            //Quaternion for a rotation to player
            Quaternion rot = Quaternion.LookRotation(_playerDir);
            //make rotation smooth with Lerp
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * rotationSpeed);
        }
    
        void GetStats(EnemyMeleeScriptableObject enemyMeleeScriptableObject)
        {
            hp = (int)(enemyMeleeScriptableObject.health * gameManager.instance.enemyWaveSystem.difficultyMultiplier * (gameManager.instance.enemyWaveSystem.currentWaveIndex + 1));
            //HP = _enemyMeleeScriptableObject.health;
            swingRate = enemyMeleeScriptableObject.swingRate;
            swingAngle = enemyMeleeScriptableObject.swingAngle;
            viewAngle = enemyMeleeScriptableObject.viewAngle;
            rotationSpeed = enemyMeleeScriptableObject.rotationSpeed;
            audWeaponSwing = enemyMeleeScriptableObject.audWeaponSwing[Random.Range(0,enemyMeleeScriptableObject.audWeaponSwing.Length)];
            weapon.damage = (int)(enemyMeleeScriptableObject.attack * gameManager.instance.enemyWaveSystem.difficultyMultiplier * (gameManager.instance.enemyWaveSystem.currentWaveIndex + 1));
            attack = weapon.damage;
      
            deathEffect.SetDeathEffect(enemyMeleeScriptableObject.deathEffect);
            // Haven't implemented defense yet
            // Haven't implemented boss check yet
        }
    
        void GetNavMesh()
        {
            // Overwrites attached NavMeshAgent with Scriptable Object NavMesh
            // Only variables not included 
            // Agent Type, Quality, Area Mask. (Set these in Nav Mesh Agent)
        
            // OffSet
            agent.baseOffset = masterEnemyScriptableObject.navMesh.baseOffset;
       
            // Steering
            agent.speed = masterEnemyScriptableObject.navMesh.speed;
            agent.angularSpeed = masterEnemyScriptableObject.navMesh.angularSpeed;
            agent.acceleration = masterEnemyScriptableObject.navMesh.acceleration;
            agent.stoppingDistance = masterEnemyScriptableObject.navMesh.stoppingDistance;
            agent.autoBraking = masterEnemyScriptableObject.navMesh.autoBraking;
      
            // Obstacle Avoidance
            agent.radius = masterEnemyScriptableObject.navMesh.radius;
            agent.height = masterEnemyScriptableObject.navMesh.height;
            agent.avoidancePriority = masterEnemyScriptableObject.navMesh.avoidancePriority;
        
            // Path Finding
            agent.autoTraverseOffMeshLink = masterEnemyScriptableObject.navMesh.autoTraverseOffMeshLink;
            agent.autoRepath = masterEnemyScriptableObject.navMesh.autoRepath;
        }

        private IEnumerator FlashDamage()
        {
            var material = model.material;
            material.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            material.color = Color.white;
        }

        IEnumerator MeleeHit()
        {
        
            _isSwinging = true;
            animator.SetTrigger(Attack1H1);
        
            yield return new WaitForSeconds(swingRate);
        
            _isSwinging = false;
        }

    
        public void WeaponColliderOn()
        {
            weaponCollider.enabled = true;
            aud.PlayOneShot(audWeaponSwing, audWeaponSwingVol);
        }

        private void WeaponColliderOff()
        {
            weaponCollider.enabled = false;
        }
    
        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInRange = true;
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInRange = false;
            }
        }
        void CanSeePlayer()
        {
            _playerDir = gameManager.instance.player.transform.position + offSetPlayerDir - headPos.position;
            _angleToPlayer = Vector3.Angle(_playerDir, transform.forward);


            if (!Physics.Raycast(headPos.position, _playerDir, out var hit)) return;
            if (hit.collider.CompareTag("Player"))
            {
               
                agent.SetDestination(gameManager.instance.player.transform.position);

                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    FacePlayer();
                }

                if (!_isSwinging && _angleToPlayer <= swingAngle)
                {
                  
                    StartCoroutine(MeleeHit());
                }
            }
        }

    
    }
}
