using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Enemy.Enemy_AI_Scripts
{
    public class ExplosiveEnemyAI : MonoBehaviour, IDamage
    {
        [FormerlySerializedAs("HP")]
        [Header("----- Explosive Enemy Stats From SO -----")] 
        [SerializeField] int hp;
        [SerializeField] int rotationSpeed;
        [Range(10, 1000)] [SerializeField] int lootValue;
        // [SerializeField] float throwingDistance = 10f;
        [SerializeField] int forceForward = 1;
        [SerializeField] int forceUpward = 1;
        [SerializeField] int torque = 1;
        [SerializeField] float coolDown = 3;
        public int throwAngle;
        public int viewAngle;
        public DeathEffect deathEffect;
    
        [Header("----- Bomb Stats -----")] 
        //   [SerializeField] int timer = 1;
        // [SerializeField] int explosionRadius = 1;
        //  [SerializeField] int explosionForce = 1;
        //[SerializeField] int explosiveDamage = 1;

    
        [Header("----- Needed References -----")]
        public MasterEnemy masterEnemyScriptableObject;
        [SerializeField] Renderer model;
        public Animator animator;
        public NavMeshAgent agent;
        public Transform headPos;
        public Transform throwingHand;
        public GameObject bomb;
        [SerializeField] GameObject weaponDrop;


        [Header("----- Variables -----")] Vector3 _playerDir;
        bool _isThrowing;
        bool _playerInRange;
        float _angleToPlayer;
        public float stoppingDistOrig;
        [SerializeField] private Vector3 offSetPlayerDir = new Vector3(0, 1, 0);
        private static readonly int Hit1 = Animator.StringToHash("Hit1");
        private static readonly int Speed = Animator.StringToHash("Speed");

        // Start is called before the first frame update
        void Start()
        {
            gameManager.instance.updateEnemyRemaining(1);
            GetStats((EnemyExplosiveScriptableObjects)masterEnemyScriptableObject);
            GetNavMesh();
            stoppingDistOrig = agent.stoppingDistance;
        
     
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
        public void takeDamage(float damage)
        {
            hp -= (int)damage;
            animator.SetTrigger(Hit1);
            StartCoroutine(FlashDamage());
            FacePlayer();
            agent.SetDestination(gameManager.instance.player.transform.position);
            if (hp <= 0)
            {
                gameManager.instance.updateEnemyRemaining(-1);
                gameManager.instance.playerScript.addCoins(lootValue);
                deathEffect.DeathByEffects();

                Destroy(gameObject);
                gameManager.instance.DropLoot(transform, weaponDrop, true, true, true);
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

        void GetStats(EnemyExplosiveScriptableObjects enemyExplosiveScriptableObject)
        {
            hp = (int)(enemyExplosiveScriptableObject.health * gameManager.instance.enemyWaveSystem.difficultyMultiplier * (gameManager.instance.enemyWaveSystem.currentWaveIndex + 1));
            bomb = enemyExplosiveScriptableObject.bomb;
            throwAngle = enemyExplosiveScriptableObject.throwAngle;
            viewAngle = enemyExplosiveScriptableObject.viewAngle;
            forceForward = enemyExplosiveScriptableObject.forceForward;
            forceUpward = enemyExplosiveScriptableObject.forceUpward;
            torque = enemyExplosiveScriptableObject.torque;
            rotationSpeed = enemyExplosiveScriptableObject.rotationSpeed;
            // throwingDistance = enemyExplosiveScriptableObject.throwingDistance;
            deathEffect.SetDeathEffect(enemyExplosiveScriptableObject.deathEffect);
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
            agent.avoidancePriority = masterEnemyScriptableObject.navMesh.AvoidancePriority;

            // Path Finding
            agent.autoTraverseOffMeshLink = masterEnemyScriptableObject.navMesh.AutoTraverseOffMeshLink;
            agent.autoRepath = masterEnemyScriptableObject.navMesh.AutoRepath;
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

        private IEnumerator FlashDamage()
        {
            var material = model.material;
            material.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            material.color = Color.white;
        }

        IEnumerator ThrowBomb()
        {
            _isThrowing = true;
            animator.SetTrigger(agent.velocity.normalized.magnitude > 0 ? "Throw" : "IdleThrow");

            yield return new WaitForSeconds(coolDown);
            _isThrowing = false;
        }

        public void CreateBomb()
        {
            GameObject grenade = Instantiate(bomb, throwingHand.position, throwingHand.rotation);
            Rigidbody grenadeRigidbody = grenade.GetComponent<Rigidbody>();
            var transform1 = transform;
            grenadeRigidbody.AddForce(transform1.forward * forceForward + transform1.up * forceUpward);
            grenadeRigidbody.AddTorque(Random.insideUnitSphere * torque);

            //explosiveDamage = grenade.GetComponent<explosiveWeapon>().damage;
            //explosionRadius = grenade.GetComponent<explosiveWeapon>().range;
            //explosionForce = grenade.GetComponent<explosiveWeapon>().force;
            // timer = grenade.GetComponent<explosiveWeapon>().timer;
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
                

                if (!_isThrowing && _angleToPlayer <= throwAngle)
                {
                    StartCoroutine(ThrowBomb());
                }
            }
        }
    
   
    
    }
}