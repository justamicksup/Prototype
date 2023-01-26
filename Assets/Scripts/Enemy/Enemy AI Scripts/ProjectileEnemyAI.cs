using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

    public class ProjectileEnemyAI: MonoBehaviour, IDamage
    {
        
    [Header("----- Projectile Enemy Stats From SO -----")]
    [SerializeField] int HP;
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

    [Header("----- Needed References -----")]
    public MasterEnemy masterEnemyScriptableObject;
    public Animator animator;
    public NavMeshAgent agent;
    public Transform headPos;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] Renderer model;
    
    [Header("----- Variables -----")]
    Vector3 playerDir;
    bool isShooting;
    bool playerInRange;
    float angleToPlayer;
    public float stoppingDistOrig;
      
      
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
        playerDir = gameManager.instance.player.transform.position - headPos.position;

        agent.SetDestination(gameManager.instance.player.transform.position);
        animator.SetFloat("Speed", agent.velocity.normalized.magnitude);
        
        
        if (playerInRange)
        {
            CanSeePlayer();
           
        }
    }
    // Need to adjust this in all AI to work like lecture
    public void takeDamage(int damage)
    {
        HP -= damage;
        animator.SetTrigger("Hit1");
        StartCoroutine(flashDamage());
        facePlayer();
        agent.SetDestination(gameManager.instance.player.transform.position);
        if (HP <= 0)
        {
            gameManager.instance.updateEnemyRemaining(-1);
            gameManager.instance.playerScript.addCoins(lootValue);

            Destroy(gameObject);
        }
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
        shootAngle = _enemyProjectileScriptableObjects.shootAngle;
        shootAngle = _enemyProjectileScriptableObjects.shootAngle;
        viewAngle = _enemyProjectileScriptableObjects.viewAngle;
        rotationSpeed = _enemyProjectileScriptableObjects.rotationSpeed;
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
    
    public IEnumerator flashDamage()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        model.material.color = Color.white;
    }

    IEnumerator shoot()
    {
        isShooting = true;
        // if (agent.velocity.normalized.magnitude > 0)
        // {
        //     animator.SetTrigger("Shoot");
        // }
        // else
        // {
        //     animator.SetTrigger("IdleShoot");
        // }

        animator.SetTrigger("Shoot");
        GameObject bulletClone = Instantiate(bullet, shootPos.position, bullet.transform.rotation);
        bulletClone.GetComponent<Rigidbody>().velocity = (gameManager.instance.player.transform.position - headPos.transform.position).normalized * bulletSpeed;
        bulletClone.GetComponent<bullet>().bulletDamage = shootDamage;
        Debug.Log(bulletClone.GetComponent<bullet>().bulletDamage);

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
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
    
    bool CanSeePlayer()
    {
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);
        
        Debug.Log(angleToPlayer);
        
        
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

                if (!isShooting && angleToPlayer <= shootAngle)
                { 
                    Debug.Log("Shooting player");
                    StartCoroutine(shoot());
                }

                return true;
            }
        }
        

        return false;
    }
}
    
