using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour, IDamage
{
    //Common
    [SerializeField] int HP;
    [SerializeField] int rotationSpeed;
    [Range(10, 1000)] [SerializeField] int lootValue;
    public DeathEffect deathEffect;
    public int viewAngle;
    [SerializeField] private int maxHealth = 200;
    [SerializeField] private int currentHealth;
    [SerializeField] private bool isInvincible = false;
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
    [Header("----- Explosive Enemy Stats From SO -----")] [SerializeField]
    float throwingDistance = 10f;

    [SerializeField] int forceForward = 1;
    [SerializeField] int forceUpward = 1;
    [SerializeField] int torque = 1;
    [SerializeField] float coolDown = 3;
    public int throwAngle;

    [Header("----- Bomb Stats -----")] [SerializeField]
    int timer = 1;

    [SerializeField] int explosionRadius = 1;
    [SerializeField] int explosionForce = 1;
    [SerializeField] int explosiveDamage = 1;


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


    [Header("----- Variables -----")] private float healthPercentage;
    Vector3 playerDir;
    bool isSwinging;
    bool isShooting;
    bool isThrowing;
    bool playerInRange;
    float angleToPlayer;
    bool hasWeapon;
    [Range(0, 1)] [SerializeField] float audWeaponSwingtVol;
    [Range(0, 1)] [SerializeField] float audGunShotVol;


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
        playerDir = gameManager.instance.player.transform.position - headPos.position;

        agent.SetDestination(gameManager.instance.player.transform.position);
        animator.SetFloat("Speed", agent.velocity.normalized.magnitude);
        healthPercentage = (float)currentHealth / (float)maxHealth;


        if (!isInvincible)
        {
            if (playerInRange)
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

    void GetStats(BossScriptableObject _bossScriptableObject)
    {
        attack = _bossScriptableObject.attack;
        bulletSpeed = _bossScriptableObject.bulletSpeed;
        maxHealth = _bossScriptableObject.health;
        //store all the stats needed for all sequences
    }

    #endregion
    

    #region PlayerInteraction

    bool CanSeePlayer()
    {
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);


        RaycastHit hit;

        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player"))
            {
                agent.SetDestination(gameManager.instance.player.transform.position);

                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    facePlayer();
                }

                if (healthPercentage > 0.7f)
                {
                    StartCoroutine(TransitionToNextAttack());
                    animator.SetInteger("WeaponType", 2);
                    if (!isShooting && angleToPlayer <= shootAngle)
                    {
                        StartCoroutine(shoot());
                    }
                }

                else if (healthPercentage > 0.4f)
                {
                    animator.SetInteger("WeaponType", 1);
                    agent.stoppingDistance = 2;
                    GetComponent<SphereCollider>().radius = 3;

                    if (!isSwinging)
                    {
                        Debug.Log("Melee");
                        StartCoroutine(MeleeHit());
                    }
                }
                else if (healthPercentage > 0.2f) //throwing bombs
                {
                    animator.SetInteger("WeaponType", 3);
                    agent.stoppingDistance = 10;
                    GetComponent<SphereCollider>().radius = 15;
                    if (!isThrowing)
                    {
                        StartCoroutine(ThrowBomb());
                    }
                }
                else if (healthPercentage > 0.0f)
                {
                    //explosives but faster
                    animator.SetInteger("WeaponType", 3);
                    coolDown = .1f;
                    if (!isThrowing)
                    {
                        StartCoroutine(ThrowBomb());
                    }
                }
                else
                {
                    Destroy(gameObject);
                    gameManager.instance.youWin();
                }

                return true;
            }
        }

        return false;
    }

    void facePlayer()
    {
        //don't rotate up or down (Y)
        playerDir.y = 0;
        //Quaternion for a rotation to player
        Quaternion rot = Quaternion.LookRotation(playerDir);
        //make rotation smooth with Lerp
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * rotationSpeed);
    }

    public IEnumerator flashDamage()
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

    public void takeDamage(int damage)
    {
        if (!isInvincible)
        {
            weaponColliderOff();
            currentHealth -= damage;
            StartCoroutine(flashDamage());
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }


    IEnumerator TransitionToNextAttack()
    {
        isInvincible = true;
        // Perform transition sequence
        yield return new WaitForSeconds(transitionDuration);
        isInvincible = false;
    }

    #endregion
    

    #region GunLogic

    IEnumerator shoot()
    {
        isShooting = true;

        // aud.PlayOneShot(audGunShot, audGunShotVol);
        GameObject bulletClone = Instantiate(bullet, shootPos.position, bullet.transform.rotation);
        bulletClone.GetComponent<Rigidbody>().velocity =
            (gameManager.instance.player.transform.position - headPos.transform.position).normalized * bulletSpeed;
        bulletClone.GetComponent<bullet>().bulletDamage = shootDamage;
        Debug.Log(bulletClone.GetComponent<bullet>().bulletDamage);

        yield return new WaitForSeconds(shootRate);

        isShooting = false;
    }

    #endregion
    

    #region MeleeLogic

    IEnumerator MeleeHit()
    {
        isSwinging = true;

        animator.SetTrigger("MeleeHit");
        yield return new WaitForSeconds(swingRate);

        isSwinging = false;
    }

    public void weaponColliderOn()
    {
        weaponCollider.enabled = true;
    }

    public void weaponColliderOff()
    {
        weaponCollider.enabled = false;
    }

    #endregion
    

    #region ExplosivesLogic

    IEnumerator ThrowBomb()
    {
        isThrowing = true;

        animator.SetTrigger("Throw");
        yield return new WaitForSeconds(coolDown);

        isThrowing = false;
    }

    public void createBomb()
    {
        GameObject grenade = Instantiate(bomb, throwingHand.position, throwingHand.rotation);
        Rigidbody grenadeRigidbody = grenade.GetComponent<Rigidbody>();
        grenadeRigidbody.AddForce(transform.forward * forceForward + transform.up * forceUpward);
        grenadeRigidbody.AddTorque(Random.insideUnitSphere * torque);

        explosiveDamage = grenade.GetComponent<explosiveWeapon>().damage;
        explosionRadius = grenade.GetComponent<explosiveWeapon>().range;
        explosionForce = grenade.GetComponent<explosiveWeapon>().force;
        timer = grenade.GetComponent<explosiveWeapon>().timer;
    }

    #endregion
}