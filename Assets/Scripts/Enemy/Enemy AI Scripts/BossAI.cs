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
    public NavMeshAgent agent;
    public MasterEnemy masterEnemyScriptableObject;
    bool playerInRange;
    
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInvincible)
        {
            float healthPercentage = (float)currentHealth / (float)maxHealth;

            if (healthPercentage > 0.7f) 
            {   //shooting
                //need shooting navmesh agent
                // Perform attack sequence 1
            }
            else if (healthPercentage > 0.4f) 
            {   
                StartCoroutine(TransitionToNextAttack());
                //melee
                //need melee navmesh agent
                //Perform attack sequence 2
            }
            else if (healthPercentage > 0.2f) //throwing bombs
            {
                StartCoroutine(TransitionToNextAttack());
                //explosives
                //need explosives navmesh agent
                //Perform attack sequence 3
            }
            else if (healthPercentage > 0.0f)
            {
                StartCoroutine(TransitionToNextAttack());
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
        //store all the stats needed for all sequences
    }
}
