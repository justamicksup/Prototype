using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour, IDamage
{
    [SerializeField] private int maxHealth = 200;
    [SerializeField] private int currentHealth;
    [SerializeField] private bool isInvincible = false;
    [SerializeField] public float transitionDuration = 2.0f;
    [SerializeField] int rotationSpeed;
    public int attack;
    
    
    [Header("----- Variables -----")]
    Vector3 playerDir;
    bool playerInRange; 
    bool isShooting;
    float angleToPlayer;
    
    [Header("----- Needed References -----")]
    public MasterEnemy masterEnemyScriptableObject;
    public Animator animator;
    public NavMeshAgent agent;
    public Transform headPos;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [SerializeField] Renderer model;
    public AudioSource aud;
    
    [Header("----- Gun Stats -----")]
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
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        GetStats((BossScriptableObject)masterEnemyScriptableObject);
        GetNavMesh();
        
    }

    // Update is called once per frame
    void Update()
    {
        playerDir = gameManager.instance.player.transform.position - headPos.position;

        agent.SetDestination(gameManager.instance.player.transform.position);
       // animator.SetFloat("Speed", agent.velocity.normalized.magnitude);
        
       
        
        if (!isInvincible)
        {
            float healthPercentage = (float)currentHealth / (float)maxHealth;
        
            if (healthPercentage > 0.7f)
            {
                if (playerInRange)
                {
                    CanSeePlayer();
                }
                
                //shooting
                //need shooting navmesh agent
                // Perform attack sequence 1
            }
            else if (healthPercentage > 0.4f)
            {
                agent.stoppingDistance = 0;
                GetComponent<SphereCollider>().radius = 3;
                
                if (playerInRange)
                {
                    gameManager.instance.playerScript.takeDamage(attack);
                }
                //StartCoroutine(TransitionToNextAttack());
                //melee
                //need melee navmesh agent
                //Perform attack sequence 2
            }
            else if (healthPercentage > 0.2f) //throwing bombs
            {
               // StartCoroutine(TransitionToNextAttack());
                //explosives
                //need explosives navmesh agent
                //Perform attack sequence 3
            }
            else if (healthPercentage > 0.0f)
            {
                //StartCoroutine(TransitionToNextAttack());
                //explosives but faster
            }
            
            else
            {
                //stop all agent movement
                //stop attacks
                // The boss has died
                // Perform death sequence
            }
        }
    }

    public void takeDamage(int damage)
    {
        if (!isInvincible)
        {
            currentHealth -= damage;
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
        //store all the stats needed for all sequences
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

        //animator.SetTrigger("Shoot");
       // aud.PlayOneShot(audGunShot, audGunShotVol);
        GameObject bulletClone = Instantiate(bullet, shootPos.position, bullet.transform.rotation);
        bulletClone.GetComponent<Rigidbody>().velocity = (gameManager.instance.player.transform.position - headPos.transform.position).normalized * bulletSpeed;
        bulletClone.GetComponent<bullet>().bulletDamage = shootDamage;
        Debug.Log(bulletClone.GetComponent<bullet>().bulletDamage);

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
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
    
    void facePlayer()
    {
        //don't rotate up or down (Y)
        playerDir.y = 0;
        //Quaternion for a rotation to player
        Quaternion rot = Quaternion.LookRotation(playerDir);
        //make rotation smooth with Lerp
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * rotationSpeed);
    }
    
}
