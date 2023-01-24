using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class enemyAI : MonoBehaviour, IDamage
{
    [Header("----- Components -----")] [SerializeField]
    NavMeshAgent agent;

    public MasterEnemy masterEnemyScriptableObject;
    public Animator anim;
    
    [Header("----- Enemy Stats -----")] [SerializeField]
    Transform headPos;

    [SerializeField] int HP;
    [SerializeField] int rotationSpeed;
    [Range(10, 1000)] [SerializeField] int lootValue;

    [Header("----- Shooting -----")] public int shootDamage;
    public int range;
    public float shootRate;
    public float shootForce;
    public int ammoCapacity;
    public int ammoRemaining;
    public float reloadTime;
    public AudioClip audGunShot;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [Range(15, 35)] [SerializeField] int bulletSpeed;


    [Header("----- Melee -----")] [SerializeField]
    public int attack = 1;

    public float swingRate = 0.1f;
    public AudioClip audWeaponSwing;

    [Header("----- Explosive -----")] 
    [SerializeField] int throwingDistance = 1;
    [SerializeField] int explosiveDamage = 1;
    [SerializeField] int force = 1;
    [SerializeField] Transform throwingHand;
    [SerializeField] GameObject bomb;
    [SerializeField] int explosionRadius = 5;
    [SerializeField] int explosionForce = 100;
    [SerializeField] int timer = 100;
    
    bool isSwinging;
    bool isShooting;
    bool isThrowing;
    bool isRangedEnemy;
    bool isMeleeEnemy;
    bool isExplosiveEnemy;
    Vector3 playerDir;
    bool playerInRange;


    // Start is called before the first frame update
    void Start()
    {
        gameManager.instance.updateEnemyRemaining(1);


        if (masterEnemyScriptableObject.GetType() == typeof(EnemyProjectileScriptableObjects))
        {
            Debug.Log("Its the Projectile type");
            isRangedEnemy = true;
            isMeleeEnemy = false;
            isExplosiveEnemy = false;
            GetStats((EnemyProjectileScriptableObjects)masterEnemyScriptableObject);
            GetNavMesh();
        }

        if (masterEnemyScriptableObject.GetType() == typeof(EnemyMeleeScriptableObject))
        {
            isMeleeEnemy = true;
            isRangedEnemy = false;
            isExplosiveEnemy = false;

            Debug.Log("Its the Melee type");
            GetStats((EnemyMeleeScriptableObject)masterEnemyScriptableObject);
            GetNavMesh();
        }

        if (masterEnemyScriptableObject.GetType() == typeof(EnemyExplosiveScriptableObjects))
        {
            isExplosiveEnemy = true;
            isMeleeEnemy = false;
            isRangedEnemy = false;

            Debug.Log("Its the Melee type");
            GetStats((EnemyExplosiveScriptableObjects)masterEnemyScriptableObject);
            GetNavMesh();
        }
    }

    // Update is called once per frame
    void Update()
    {
        playerDir = gameManager.instance.player.transform.position - headPos.position;

        agent.SetDestination(gameManager.instance.player.transform.position);
        anim.SetFloat("Speed", agent.velocity.normalized.magnitude);


        if (agent.remainingDistance < agent.stoppingDistance)
        {
            facePlayer();
        }


        if (playerInRange)
        {
            if (isRangedEnemy)
            {
                if (!isShooting)
                {
                    Debug.Log("Shooting");
                    StartCoroutine(shoot());
                }
            }
            else if (isMeleeEnemy)
            {
                if (!isSwinging)
                {
                    Debug.Log("Swinging");
                    StartCoroutine(MeleeHit());
                }
            }
            else if (isExplosiveEnemy)
            {
                if (!isThrowing)
                {
                    Debug.Log("Throwing");
                    StartCoroutine(ThrowBomb());
                }
            }
        }
    }


    public void takeDamage(int damage)
    {
        HP -= damage;
        anim.SetTrigger("Hit1");
        facePlayer();
        agent.SetDestination(gameManager.instance.player.transform.position);
        if (HP <= 0)
        {
            gameManager.instance.updateEnemyRemaining(-1);
            gameManager.instance.playerScript.addCoins(lootValue);

            Destroy(gameObject);
        }
    }

    IEnumerator shoot()
    {
        isShooting = true;
        if (agent.velocity.normalized.magnitude > 0)
        {
            anim.SetTrigger("Shoot");
        }
        else
        {
            anim.SetTrigger("IdleShoot");
        }

        GameObject bulletClone = Instantiate(bullet, shootPos.position, bullet.transform.rotation);
        bulletClone.GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;
        bulletClone.GetComponent<bullet>().bulletDamage = shootDamage;
        Debug.Log(bulletClone.GetComponent<bullet>().bulletDamage);

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    IEnumerator MeleeHit()
    {
        isSwinging = true;
        anim.SetTrigger("Attack1h1");
        gameManager.instance.playerScript.takeDamage(attack);

        yield return new WaitForSeconds(swingRate);
        isSwinging = false;
    }

    IEnumerator ThrowBomb()
    {
        isThrowing = true;
        if (agent.velocity.normalized.magnitude > 0)
        {
            anim.SetTrigger("Throw");
        }
        else
        {
            anim.SetTrigger("IdleThrow");
        }
        
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

        GameObject grenade = Instantiate(bomb, throwingHand.position, throwingHand.rotation);
        Rigidbody grenadeRigidbody = grenade.GetComponent<Rigidbody>();
        grenadeRigidbody.AddForce(transform.forward * 500 + transform.up * 250);
        grenadeRigidbody.AddTorque(Random.insideUnitSphere * 500);
       
        grenade.GetComponent<explosiveWeapon>().damage = explosiveDamage;
        grenade.GetComponent<explosiveWeapon>().range = explosionRadius;
        grenade.GetComponent<explosiveWeapon>().force = explosionForce;
        grenade.GetComponent<explosiveWeapon>().timer = timer;

        //yield return new WaitForSeconds(0.8728814f);
        
        isThrowing = false;
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

    void GetStats(EnemyProjectileScriptableObjects _enemyProjectileScriptableObjects)
    {
        HP = _enemyProjectileScriptableObjects.health;
        shootDamage = _enemyProjectileScriptableObjects.shootDamage;
        range = _enemyProjectileScriptableObjects.range;
        shootRate = _enemyProjectileScriptableObjects.shootRate;
        shootForce = _enemyProjectileScriptableObjects.shootForce;
        bulletSpeed = _enemyProjectileScriptableObjects.bulletSpeed;
        ammoCapacity = _enemyProjectileScriptableObjects.ammoCapacity;
        ammoRemaining = _enemyProjectileScriptableObjects.ammoRemaining;
        reloadTime = _enemyProjectileScriptableObjects.reloadTime;
        audGunShot = _enemyProjectileScriptableObjects.audGunShot;
    }

    void GetStats(EnemyMeleeScriptableObject _enemyMeleeScriptableObject)
    {
        HP = _enemyMeleeScriptableObject.health;
        attack = _enemyMeleeScriptableObject.attack;
        swingRate = _enemyMeleeScriptableObject.swingRate;
        audWeaponSwing = _enemyMeleeScriptableObject.audWeaponSwing;
    }

    void GetStats(EnemyExplosiveScriptableObjects _enemyExplosiveScriptableObject)
    {
        HP = _enemyExplosiveScriptableObject.health;
    }


    void GetNavMesh()
    {
        agent.baseOffset = masterEnemyScriptableObject.navMesh.baseOffset;
        agent.speed = masterEnemyScriptableObject.navMesh.speed;
        agent.angularSpeed = masterEnemyScriptableObject.navMesh.angularSpeed;
        agent.acceleration = masterEnemyScriptableObject.navMesh.acceleration;
        agent.stoppingDistance = masterEnemyScriptableObject.navMesh.stoppingDistance;
        agent.autoBraking = masterEnemyScriptableObject.navMesh.autoBraking;
        agent.radius = masterEnemyScriptableObject.navMesh.radius;
        agent.height = masterEnemyScriptableObject.navMesh.height;
    }
}