using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ExplosiveEnemyAI : MonoBehaviour, IDamage
{
    [Header("----- Explosive Enemy Stats From SO -----")] 
    [SerializeField] int HP;
    [SerializeField] int rotationSpeed;
    [Range(10, 1000)] [SerializeField] int lootValue;
    [SerializeField] float throwingDistance = 10f;
    [SerializeField] int forceForward = 1;
    [SerializeField] int forceUpward = 1;
    [SerializeField] int torque = 1;
    [SerializeField] float coolDown = 3;
    public int throwAngle;
    public int viewAngle;
    public DeathEffect deathEffect;
    
    [Header("----- Bomb Stats -----")] 
    [SerializeField] int timer = 1;
    [SerializeField] int explosionRadius = 1;
    [SerializeField] int explosionForce = 1;
    [SerializeField] int explosiveDamage = 1;

    
    [Header("----- Needed References -----")]
    public MasterEnemy masterEnemyScriptableObject;
    [SerializeField] Renderer model;
    public Animator animator;
    public NavMeshAgent agent;
    public Transform headPos;
    public Transform throwingHand;
    public GameObject bomb;
    [SerializeField] GameObject weaponDrop;


    [Header("----- Variables -----")] Vector3 playerDir;
    bool isThrowing;
    bool playerInRange;
    float angleToPlayer;
    public float stoppingDistOrig;

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
        playerDir = gameManager.instance.player.transform.position - headPos.position;

        agent.SetDestination(gameManager.instance.player.transform.position);
        animator.SetFloat("Speed", agent.velocity.normalized.magnitude);

       

        if (playerInRange)
        {
            CanSeePlayer();
        }
    }

    // Need to adjust this in all AI to work like lecture
    public void takeDamage(float damage)
    {
        HP -= (int)damage;
        animator.SetTrigger("Hit1");
        StartCoroutine(flashDamage());
        facePlayer();
        agent.SetDestination(gameManager.instance.player.transform.position);
        if (HP <= 0)
        {
            gameManager.instance.updateEnemyRemaining(-1);
            gameManager.instance.playerScript.addCoins(lootValue);
            deathEffect.DeathByEffects();

            Destroy(gameObject);
            gameManager.instance.DropLoot(transform, weaponDrop, true, true, true);
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

    void GetStats(EnemyExplosiveScriptableObjects _enemyExplosiveScriptableObject)
    {
        HP = (int)(_enemyExplosiveScriptableObject.health * gameManager.instance.enemyWaveSystem.difficultyMultiplier * (gameManager.instance.enemyWaveSystem.currentWaveIndex + 1));
        bomb = _enemyExplosiveScriptableObject.bomb;
        throwAngle = _enemyExplosiveScriptableObject.throwAngle;
        viewAngle = _enemyExplosiveScriptableObject.viewAngle;
        forceForward = _enemyExplosiveScriptableObject.forceForward;
        forceUpward = _enemyExplosiveScriptableObject.forceUpward;
        torque = _enemyExplosiveScriptableObject.torque;
        rotationSpeed = _enemyExplosiveScriptableObject.rotationSpeed;
        throwingDistance = _enemyExplosiveScriptableObject.throwingDistance;
        deathEffect.SetDeathEffect(_enemyExplosiveScriptableObject.deathEffect);
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
    public IEnumerator flashDamage()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        model.material.color = Color.white;
    }

    IEnumerator ThrowBomb()
    {
        isThrowing = true;
        if (agent.velocity.normalized.magnitude > 0)
        {
            animator.SetTrigger("Throw");
        }
        else
        {
            animator.SetTrigger("IdleThrow");
        }

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
                

                if (!isThrowing && angleToPlayer <= throwAngle)
                {
                    StartCoroutine(ThrowBomb());
                }

                return true;
            }
        }

        return false;
    }
    
   
    
}