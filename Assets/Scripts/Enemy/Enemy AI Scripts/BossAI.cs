using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.Enemy_AI_Scripts
{
    public class BossAI : MonoBehaviour, IDamage
    {
        //Common
        //[SerializeField] int hp;
        [SerializeField] int rotationSpeed;
        // [Range(10, 1000)] [SerializeField] int lootValue;
        public DeathEffect deathEffect;
        public int viewAngle;
        [SerializeField] private int maxHealth = 200;
        [SerializeField] private int currentHealth;
        [SerializeField] private bool isInvincible;
        [SerializeField] public float transitionDuration = 2.0f;


        //Gun
        [Header("----- Projectile Enemy Stats From SO -----")]
        public int shootDamage;

        public int range;
        public float shootRate;
        public float shootForce;
        public int ammoCapacity;
        public int ammoRemaining;
        public float reloadTime;
        public AudioClip audGunShot;
        public int shootAngle;
        [SerializeField] int bulletSpeed;
        public float stoppingDistOrig;


        //Melee
        [Header("----- Melee Enemy Stats From SO -----")] [SerializeField]
        public int attack = 1;

        public float swingRate = 0.1f;
        public AudioClip audWeaponSwing;
        public int swingAngle;


        //Explosives
        [Header("----- Explosive Enemy Stats From SO -----")] 
        // [SerializeField] float throwingDistance = 10f;

        [SerializeField] int forceForward = 1;
        [SerializeField] int forceUpward = 1;
        [SerializeField] int torque = 1;
        [SerializeField] float coolDown = 3;
        public int throwAngle;

        [Header("----- Bomb Stats -----")] 
        // [SerializeField] int timer = 1;

        //[SerializeField] int explosionRadius = 1;
        //[SerializeField] int explosionForce = 1;
        // [SerializeField] int explosiveDamage = 1;


        //Drag references in
        [Header("----- Needed References -----")]
        public MasterEnemy masterEnemyScriptableObject;

        public Animator animator;
        public NavMeshAgent agent;
        public Transform headPos;
        public Collider weaponCollider;
        public MeleeWeapon weapon;
        [SerializeField] Renderer[] model;
        public AudioSource aud;
        [SerializeField] Transform shootPos;
        [SerializeField] GameObject bullet;
        public Transform throwingHand;
        public GameObject bomb;


        [Header("----- Variables -----")] private float _healthPercentage;
        Vector3 _playerDir;
        bool _isSwinging;
        bool _isShooting;
        bool _isThrowing;
        bool _playerInRange;
        float _angleToPlayer;
        bool _hasWeapon;
        //  [Range(0, 1)] [SerializeField] float audWeaponSwingVol;
        // [Range(0, 1)] [SerializeField] float audGunShotVol;
        [SerializeField] private Vector3 offSetPlayerDir = new Vector3(0, 1, 0);
        private static readonly int Throw = Animator.StringToHash("Throw");
        private static readonly int Hit = Animator.StringToHash("MeleeHit");
        private static readonly int WeaponType = Animator.StringToHash("WeaponType");
        private static readonly int Speed = Animator.StringToHash("Speed");

        // Start is called before the first frame update
        void Start()
        {
            GetStats((BossScriptableObject)masterEnemyScriptableObject);
            GetNavMesh();
            currentHealth = maxHealth;
        }

        // Update is called once per frame
        void Update()
        {
            var position = gameManager.instance.player.transform.position;
            _playerDir = position + offSetPlayerDir - headPos.position;

            agent.SetDestination(position);
            animator.SetFloat(Speed, agent.velocity.normalized.magnitude);
            _healthPercentage = (float)currentHealth / maxHealth;


            if (!isInvincible)
            {
                if (_playerInRange)
                {
                    CanSeePlayer();
                }
            }
        }

    
        #region SetStatsLogic

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

        void GetStats(BossScriptableObject bossScriptableObject)
        {
            weapon.damage = bossScriptableObject.attack;
            attack = weapon.damage;
            bulletSpeed = bossScriptableObject.bulletSpeed;
            maxHealth = bossScriptableObject.health;
            //store all the stats needed for all sequences
        }

        #endregion
    

        #region PlayerInteraction

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

                if (_healthPercentage > 0.7f)
                {
                    animator.SetInteger(WeaponType, 2);
                    if (!_isShooting && _angleToPlayer <= shootAngle)
                    {
                        StartCoroutine(Shoot());
                    }
                }

                else if (_healthPercentage > 0.4f)
                {
                    animator.SetInteger(WeaponType, 1);
                    agent.stoppingDistance = 2;
                    GetComponent<SphereCollider>().radius = 3;

                    if (!_isSwinging)
                    {
                        // Debug.Log("Melee");
                        StartCoroutine(MeleeHit());
                    }
                }
                else if (_healthPercentage > 0.2f) //throwing bombs
                {
                    animator.SetInteger(WeaponType, 3);
                    agent.stoppingDistance = 10;
                    GetComponent<SphereCollider>().radius = 50;
                    if (!_isThrowing)
                    {
                        StartCoroutine(ThrowBomb());
                    }
                }
                else if (_healthPercentage > 0.0f)
                {
                    GetComponent<SphereCollider>().radius = 50;
                    //explosives but faster
                    agent.stoppingDistance = 10;
                    animator.SetInteger(WeaponType, 3);
                    coolDown = .1f;
                    if (!_isThrowing)
                    {
                        StartCoroutine(ThrowBomb());
                    }
                }
                else
                { 
                   
                    StartCoroutine(gameManager.instance.rescueShipWin());
                    Destroy(gameObject);
                   
                }
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

        private IEnumerator FlashDamage()
        {
            foreach (var mesh in model)
            {
                mesh.material.color = Color.red;
            }

            yield return new WaitForSeconds(0.15f);

            foreach (var mesh in model)
            {
                mesh.material.color = Color.white;
            }
        }

        public void takeDamage(float damage)
        {
            if (!isInvincible)
            {
                WeaponColliderOff();
                currentHealth -= (int)damage;
                StartCoroutine(FlashDamage());
            }
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


        // IEnumerator TransitionToNextAttack()
        // {
        //     isInvincible = true;
        //     // Perform transition sequence
        //     yield return new WaitForSeconds(transitionDuration);
        //     isInvincible = false;
        // }

        #endregion
    

        #region GunLogic

        IEnumerator Shoot()
        {
            _isShooting = true;

            // aud.PlayOneShot(audGunShot, audGunShotVol);
            GameObject bulletClone = Instantiate(bullet, shootPos.position, bullet.transform.rotation);
            bulletClone.GetComponent<Rigidbody>().velocity =
                (gameManager.instance.player.transform.position - headPos.transform.position).normalized * bulletSpeed;
            bulletClone.GetComponent<bullet>().bulletDamage = shootDamage;
            // Debug.Log(bulletClone.GetComponent<bullet>().bulletDamage);

            yield return new WaitForSeconds(shootRate);

            _isShooting = false;
        }

        #endregion
    

        #region MeleeLogic

        IEnumerator MeleeHit()
        {
            _isSwinging = true;

            animator.SetTrigger(Hit);
            yield return new WaitForSeconds(swingRate);

            _isSwinging = false;
        }

        public void weaponColliderOn()
        {
            weaponCollider.enabled = true;
        }

        private void WeaponColliderOff()
        {
            weaponCollider.enabled = false;
        }

        #endregion
    

        #region ExplosivesLogic

        IEnumerator ThrowBomb()
        {
            _isThrowing = true;

            animator.SetTrigger(Throw);
            yield return new WaitForSeconds(coolDown);

            _isThrowing = false;
        }

        public void CreateBomb()
        { 
            var transform1 = transform;
            GameObject grenade = Instantiate(bomb, throwingHand.position, throwingHand.rotation);
            Rigidbody grenadeRigidbody = grenade.GetComponent<Rigidbody>();
       
            grenadeRigidbody.AddForce(transform1.forward * forceForward + transform1.up * forceUpward);
            grenadeRigidbody.AddTorque(Random.insideUnitSphere * torque);

            //explosiveDamage = grenade.GetComponent<explosiveWeapon>().damage;
            //explosionRadius = grenade.GetComponent<explosiveWeapon>().range;
            //explosionForce = grenade.GetComponent<explosiveWeapon>().force;
            //timer = grenade.GetComponent<explosiveWeapon>().timer;
        }

        #endregion
    }
}