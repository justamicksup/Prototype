using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;

    public GameObject enemy;

    [Header("----- Enemy Stats -----")]
    [SerializeField] private EnemyStats enem;
    [SerializeField] Transform headPos;
    [Range(1, 15)] [SerializeField] int HP;
    [SerializeField] int rotationSpeed;
    [Range(10,1000)] [SerializeField] int lootValue;

    [Header("----- Shooting -----")]
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject bullet;
    [Range(15, 35)] [SerializeField] int bulletSpeed;
    [Range(0.1f, 2)] [SerializeField] float shootRate;
    [Range(5, 100)] [SerializeField] int shootDist;
    [Range(0, 10)] [SerializeField] int shootDamage;

    [Header("----- Melee -----")] 
    [SerializeField] float swingRate;
    
    bool isSwinging;
    bool isShooting;
    Vector3 playerDir;
    bool playerInRange;


    // Start is called before the first frame update
    void Start()
    {
        gameManager.instance.updateEnemyRemaining(1);
    }

    // Update is called once per frame
    void Update()
    {playerDir = gameManager.instance.player.transform.position - headPos.position;


        agent.SetDestination(gameManager.instance.player.transform.position);

        
                 if (playerInRange)
                 {
                     if (!isSwinging)
                     {
                         if (enemy.CompareTag("Melee"))
                         {
                             StartCoroutine(MeleeHit());
                         }
                     }
                    
                     if (agent.remainingDistance < agent.stoppingDistance)
                     {
                         facePlayer();
                     }

                     if (enemy.CompareTag("Range"))
                     {
                         if (!isShooting)
                         {
                             StartCoroutine(shoot());
                         }
                     }
                     
                 }
    }

    public void takeDamage(int damage)
    {
        HP -= damage;
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

        GameObject bulletClone = Instantiate(bullet, shootPos.position, bullet.transform.rotation);
        bulletClone.GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;
        bulletClone.GetComponent<bullet>().bulletDamage = shootDamage;

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    IEnumerator MeleeHit()
    {
        isSwinging = true;
        
        gameManager.instance.playerScript.takeDamage(2);
        

        yield return new WaitForSeconds(swingRate);
        isSwinging = false;
    }

    void facePlayer()
    {
        //don't rotate up or down (Y)
        playerDir.y= 0;
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
}
