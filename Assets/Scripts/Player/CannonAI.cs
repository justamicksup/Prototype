using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ProBuilder.Shapes;

public class CannonAI : MonoBehaviour
{
    [Header("----- Components -----")]
    //if parent is used, transform is offset for circular rotation
    public GameObject cannon;
    [SerializeField] GameObject cannball;

    [Header("----- Cannon Stats -----")]
    [SerializeField] int rotSpeed;
    [SerializeField] int rotRange;
    [SerializeField] int viewAngle;//is this the same as shoot angle?
    [SerializeField] int activeTime;
    [SerializeField] float waitTime;//pause before rotating back

    [Header("----- Shooting -----")]
    [SerializeField] Transform shootPos;
    [Range(0, 100)][SerializeField] int cannSpeed;//cannonball speed
    [Range(0, 2)][SerializeField] float shootRate;
    [Range(0, 100)][SerializeField] int shootDist;
    [Range(0, 180)][SerializeField] int shootAngle;//is this the same as view angle?
    [SerializeField] int upForce;
    [SerializeField] int forwardForce;
    [SerializeField] int explosiveDamage;
    [SerializeField] int explosionRadius;
    [SerializeField] int explosionForce;
    [SerializeField] int timer;

    public int cannonID; //if integration to gameManager is needed

    //enemy transform on trigger entry
    public Transform target = null;
    Vector3 enemyDir;
    //used for angle from start
    Quaternion lookRot;

    bool isShooting;
    bool enemyInRange;
    float angleToEnemy;
    //origin angle
    float currAngle = 0f;
    public bool cannonActive;
    bool isRotating;

    //invert rotation
    bool rotPositive;
    

    // Start is called before the first frame update
    void Start()
    {
        //so distance will be set to the cannon's radius
        shootDist = (int)GetComponent<SphereCollider>().radius;
    }

    // Update is called once per frame
    void Update()
    {
        if (cannonActive)
        {
            //target should be assigned when
            //Enemy, Range, Melee tags enter trigger
            if (target != null)
            {
                //update direction to enemy each frame
                enemyDir = transform.position - target.transform.position;
                //face enemy ignores y
                faceEnemy();
            }
            else
            {
                //if no targets, idle rotate and scan for enemies
                if (!isRotating)
                {
                    StartCoroutine(idle(rotRange));
                }
                rotPositive = !rotPositive;
                //time before rotation to other side
                //**need to account for rotation time?
                new WaitForSeconds(waitTime);

                //rotate opposite way?
                if (!isRotating && !rotPositive)
                {
                    StartCoroutine(idle(-rotRange));
                    rotPositive = !rotPositive;
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (cannonActive)
        {
            //not grabbing target
            if (other.CompareTag("Enemy") || other.CompareTag("Range") || other.CompareTag("Melee"))
            {
                enemyInRange = true;
                enemyDir = other.transform.position;
                target = other.transform;

                canSeeEnemy();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.transform == target)
        {
            target = null;
            enemyInRange = false;
        }
    }
    void canSeeEnemy()
    {
        angleToEnemy = Vector3.Angle(enemyDir, transform.forward);

        Debug.Log(angleToEnemy);
        Debug.DrawRay(shootPos.position, enemyDir);

        RaycastHit hit;
        if (Physics.Raycast(shootPos.position, enemyDir, out hit))
        {
            if ((hit.collider.CompareTag("Range") || hit.collider.CompareTag("Melee") || hit.collider.CompareTag("Enemy")) && angleToEnemy <= viewAngle)
            {
                if (!isShooting && angleToEnemy <= shootAngle)
                {
                    StartCoroutine(shoot());
                }
            }
        }
    }
    void faceEnemy()
    {
        //don't rotate up or down (Y)
        enemyDir.y = 0;
        //Quaternion for a rotation to player
        Quaternion rot = Quaternion.LookRotation(enemyDir);
        //make rotation smooth with Lerp
        cannon.transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * rotSpeed);
    }
    IEnumerator idle(int range)
    {
        //starting the rotation
        isRotating= true;        
        //clamp current angle for rotation range
        currAngle = Mathf.Clamp(currAngle+(float)rotSpeed*Time.deltaTime,-range, range);
        //invert rotation?
        if (!rotPositive)
        {
            currAngle *= -1;
        }
        //get change in angle
        lookRot = Quaternion.AngleAxis(currAngle, Vector3.up);
        //assign the rotation
        cannon.transform.rotation = lookRot;
        //scan for enemy
        canSeeEnemy();
        //10 degrees a second? 
        yield return new WaitForSeconds(0.1f);        
        //stopping rotation
        isRotating= false;
    }
    IEnumerator shoot()
    {
        isShooting = true;

        createBall();

        yield return new WaitForSeconds(shootRate);

        isShooting = false;
    }
    public void createBall()
    {
        GameObject cannBall = Instantiate(cannball, shootPos.position, shootPos.rotation);
        Rigidbody ballRigidbody = cannBall.GetComponent<Rigidbody>();
        ballRigidbody.AddForce(transform.forward * forwardForce + transform.up * upForce);

        explosiveDamage = cannBall.GetComponent<explosiveWeapon>().damage;
        explosionRadius = cannBall.GetComponent<explosiveWeapon>().range;
        explosionForce = cannBall.GetComponent<explosiveWeapon>().force;
        timer = cannBall.GetComponent<explosiveWeapon>().timer;
    }
}
