using System.Collections;
using Enemy.Enemy_Stats_Scripts;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.Enemy_AI_Scripts
{
    public class ProjectileEnemyAI : MonoBehaviour, IDamage
    {
        [Header("----- Projectile Enemy Stats From SO -----")] [SerializeField]
        int hp;

        [SerializeField] int rotationSpeed;
        [Range(10, 1000)] [SerializeField] int lootValue;
        public int shootDamage;
        public int range;
        public float shootRate;
        public float shootForce;
        public int ammoCapacity;
        public int ammoRemaining;
        public float reloadTime;
        public AudioClip audGunShot;
        public int shootAngle;
        public int viewAngle;
        [Range(15, 35)] [SerializeField] int bulletSpeed;
        public DeathEffect deathEffect;

        [Header("----- Needed References -----")]
        public MasterEnemy masterEnemyScriptableObject;

        public Animator animator;
        public NavMeshAgent agent;
        public Transform headPos;
        [SerializeField] Transform shootPos;
        [SerializeField] GameObject bullet;
        [SerializeField] Renderer model;
        public AudioSource aud;
        [SerializeField] GameObject weaponDrop;
       // [SerializeField] private EnemyWaveSystem waveStats;

        [Header("----- Variables -----")] Vector3 _playerDir;
        bool _isShooting;
        bool _playerInRange;
        float _angleToPlayer;
        public float stoppingDistOrig;
        [Range(0, 1)] [SerializeField] float audGunShotVol;

        [SerializeField] private Vector3 offSetPlayerDir = new Vector3(0, 1, 0);

        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Shoot1 = Animator.StringToHash("Shoot");

        // Start is called before the first frame update
        void Start()
        {
            gameManager.instance.updateEnemyRemaining(1);
            GetStats((EnemyProjectileScriptableObjects)masterEnemyScriptableObject);
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
            //animator.SetTrigger("Hit1");
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

        void GetStats(EnemyProjectileScriptableObjects enemyProjectileScriptableObjects)
        {
            hp = (int)(enemyProjectileScriptableObjects.health *
                       gameManager.instance.enemyWaveSystem.difficultyMultiplier *
                       (gameManager.instance.enemyWaveSystem.currentWaveIndex + 1));
            shootDamage = (int)(enemyProjectileScriptableObjects.shootDamage *
                                gameManager.instance.enemyWaveSystem.difficultyMultiplier *
                                (gameManager.instance.enemyWaveSystem.currentWaveIndex + 1));
            range = enemyProjectileScriptableObjects.range;
            shootRate = enemyProjectileScriptableObjects.shootRate;
            shootForce = enemyProjectileScriptableObjects.shootForce;
            bulletSpeed = enemyProjectileScriptableObjects.bulletSpeed;
            ammoCapacity = enemyProjectileScriptableObjects.ammoCapacity;
            ammoRemaining = enemyProjectileScriptableObjects.ammoRemaining;
            reloadTime = enemyProjectileScriptableObjects.reloadTime;
            audGunShot =
                enemyProjectileScriptableObjects.audGunShot[
                    Random.Range(0, enemyProjectileScriptableObjects.audGunShot.Length)];
            shootAngle = enemyProjectileScriptableObjects.shootAngle;
            shootAngle = enemyProjectileScriptableObjects.shootAngle;
            viewAngle = enemyProjectileScriptableObjects.viewAngle;
            rotationSpeed = enemyProjectileScriptableObjects.rotationSpeed;
            deathEffect.SetDeathEffect(enemyProjectileScriptableObjects.deathEffect);
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

        IEnumerator Shoot()
        {
            _isShooting = true;
            // if (agent.velocity.normalized.magnitude > 0)
            // {
            //     animator.SetTrigger("Shoot");
            // }
            // else
            // {
            //     animator.SetTrigger("IdleShoot");
            // }
            animator.SetTrigger(Shoot1);
            aud.PlayOneShot(audGunShot, audGunShotVol);
            GameObject bulletClone = Instantiate(bullet, shootPos.position, bullet.transform.rotation);
            bulletClone.GetComponent<Rigidbody>().velocity =
                (gameManager.instance.player.transform.position - headPos.transform.position).normalized * bulletSpeed;
            bulletClone.GetComponent<bullet>().bulletDamage = shootDamage;
            //Debug.Log(bulletClone.GetComponent<bullet>().bulletDamage);

            yield return new WaitForSeconds(shootRate);
            _isShooting = false;
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInRange = true;
            }

            Barricade b = other.GetComponent<Barricade>();
            if (other.CompareTag("Destructible") && b != null &&  b.GetHP() > 0)
            {
                StartCoroutine(Shoot());
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

                if (!_isShooting && _angleToPlayer <= shootAngle)
                {
                    //Debug.Log("Shooting player");
                    StartCoroutine(Shoot());
                }
            }
        }
    }
}