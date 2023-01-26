using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class MeleeEnemyAI : MonoBehaviour, IDamage
{
    [Header("----- Melee Enemy Stats From SO -----")]
    [SerializeField] int HP;
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

    
    [Header("----- Variables -----")]
    Vector3 playerDir;
    bool isSwinging;
    bool playerInRange;
    float angleToPlayer;
    bool hasWeapon;
    [Range(0, 1)] [SerializeField] float audWeaponSwingtVol;
    
    // Start is called before the first frame update
    void Start()
    {
        gameManager.instance.updateEnemyRemaining(1);
        GetStats((EnemyMeleeScriptableObject)masterEnemyScriptableObject);
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
        weaponColliderOff();
        StartCoroutine(flashDamage());
        facePlayer();
        agent.SetDestination(gameManager.instance.player.transform.position);
        if (HP <= 0)
        {
            gameManager.instance.updateEnemyRemaining(-1);
            gameManager.instance.playerScript.addCoins(lootValue);
    
                deathEffect.DeathByEffects();
            
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
    
    void GetStats(EnemyMeleeScriptableObject _enemyMeleeScriptableObject)
    {
        HP = _enemyMeleeScriptableObject.health;
        swingRate = _enemyMeleeScriptableObject.swingRate;
        swingAngle = _enemyMeleeScriptableObject.swingAngle;
        viewAngle = _enemyMeleeScriptableObject.viewAngle;
        rotationSpeed = _enemyMeleeScriptableObject.rotationSpeed;
        audWeaponSwing = _enemyMeleeScriptableObject.audWeaponSwing[Random.Range(0,_enemyMeleeScriptableObject.audWeaponSwing.Length)];
        weapon.damage = _enemyMeleeScriptableObject.attack;
        attack = weapon.damage;
      
        deathEffect.SetDeathEffect(_enemyMeleeScriptableObject.deathEffect);
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

    IEnumerator MeleeHit()
    {
        
        isSwinging = true;
        animator.SetTrigger("Attack1h1");
       
        
       
       // gameManager.instance.playerScript.takeDamage(attack);
        
        
        yield return new WaitForSeconds(swingRate);
        
        isSwinging = false;
    }

    
    public void weaponColliderOn()
    {
        weaponCollider.enabled = true;
        aud.PlayOneShot(audWeaponSwing, audWeaponSwingtVol);
    }
    
    public void weaponColliderOff()
    {
        weaponCollider.enabled = false;
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

                if (!isSwinging && angleToPlayer <= swingAngle)
                {
                  
                    StartCoroutine(MeleeHit());
                }

                return true;
            }
        }
        
        return false;
    }

    
}
