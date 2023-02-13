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
    [SerializeField] int direction = 1;

    [Header("----- Shooting -----")]
    [SerializeField] Transform shootPos;
    [Range(0, 100)] [SerializeField] int cannSpeed;//cannonball speed
    [Range(0, 2)] [SerializeField] float shootRate;
    [Range(0, 100)] [SerializeField] int shootDist;
    [Range(0, 180)] [SerializeField] int shootAngle;//is this the same as view angle?
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
                enemyDir = cannon.transform.position - target.transform.position;
                //face enemy ignores y
                faceEnemy();
            }
            else
            {
                currAngle += Time.deltaTime * rotSpeed * direction;

                if (currAngle >= rotRange) direction = -1;
                else if (currAngle <= -rotRange) direction = 1;

                cannon.transform.localRotation = Quaternion.Euler(0, currAngle, 0);                
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
        if (other.gameObject.transform == target)
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
        rot.eulerAngles = new Vector3(rot.eulerAngles.x, rot.eulerAngles.y + 180, rot.eulerAngles.z);
        //make rotation smooth with Lerp
        cannon.transform.rotation = Quaternion.Lerp(cannon.transform.rotation, rot, Time.deltaTime * rotSpeed);
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
