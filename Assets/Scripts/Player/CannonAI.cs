using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CannonAI : MonoBehaviour
{
    [Header("----- Components -----")]
    //if parent is used, transform is offset for circular rotation
    public GameObject cannon;
    [SerializeField] GameObject cannball;
    [SerializeField] ParticleSystem smokeParticle;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject noCircle;
    [SerializeField] Image shootTime;
    [SerializeField] Image shootTimeFill;

    [Header("----- Cannon Stats -----")]
    [SerializeField] int rotSpeed;
    [SerializeField] int rotRange;
    [SerializeField] int viewAngle;//is this the same as shoot angle?
    [SerializeField] float activeTime;
    [SerializeField] int activateCost;
    [SerializeField] float coolTime;
    [SerializeField] int direction = 1;

    [Header("----- Shooting -----")]
    [SerializeField] Transform shootPos;
    [Range(0, 100)] [SerializeField] int cannSpeed;//cannonball speed
    [Range(0, 10)] [SerializeField] float shootRate;
    [Range(0, 100)] [SerializeField] int shootDist;
    [Range(0, 180)] [SerializeField] int shootAngle;//is this the same as view angle?

    [Header("----- Bomba -----")]
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
    public bool playerInRange;
    bool activeTimerOn;
    bool coolTimerOn;
    float actTimeOrig;
    float coolTimeOrig;


    // Start is called before the first frame update
    void Start()
    {
        //so distance will be set to the cannon's radius
        shootDist = (int)GetComponent<SphereCollider>().radius;
        actTimeOrig = activeTime;
        coolTimeOrig = coolTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (activeTimerOn)
        {
            activeTime -= Time.deltaTime;
            if(activeTime <= 0)
            {
                activeTimerOn= false;
                cannonActive= false;
                activeTime = actTimeOrig;
                coolTimerOn = true;
                smokeParticle.Play();
            }
        }
        if (coolTimerOn)
        {
            noCircle.SetActive(true);
            coolTime -= Time.deltaTime;
            if(coolTime <= 0)
            {
                coolTimerOn= false;
                coolTime = coolTimeOrig;
                noCircle.SetActive(false);
            }
        }
        if (cannonActive)
        {
            gameManager.instance.alertText.text = "";
            //target should be assigned when
            //Enemy, Range, Melee tags enter trigger
            if (target != null && enemyInRange)
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
        if (playerInRange && !coolTimerOn && !gameManager.instance.isPaused)
        {
            if (Input.GetButtonDown("Action") && gameManager.instance.playerScript.GetCoins() >= activateCost)
            {
                gameManager.instance.alertText.text = "";
                gameManager.instance.playerScript.addCoins(-activateCost);
                smokeParticle.Stop();
                cannonActive = true;
                activeTimerOn= true;
            }
        }
        if(cannonActive && target != null)
        {
            canSeeEnemy();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (cannonActive)
        {
            //not grabbing target
            if (other.CompareTag("Enemy") || other.CompareTag("Range") || other.CompareTag("Melee") || other.CompareTag("No Weapon") || other.CompareTag("Explosive"))
            {
                enemyInRange = true;
                enemyDir = other.transform.position;
                target = other.transform;
                canSeeEnemy();
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!coolTimerOn && Vector3.Distance(cannon.transform.position, gameManager.instance.player.transform.position) <= 2f)
            {
                playerInRange = true;
                gameManager.instance.alertText.text = $"E: Activate Cannon: ({activateCost})";
            }
            else
            {
                playerInRange= false;
                gameManager.instance.alertText.text = "";
            }

        }
        if (cannonActive && target == null)
        {
            if (other.CompareTag("Enemy") || other.CompareTag("Range") || other.CompareTag("Melee") || other.CompareTag("No Weapon") || other.CompareTag("Explosive"))
            {
                enemyInRange = true;
                enemyDir = other.transform.position;
                target = other.transform;
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
        angleToEnemy = Vector3.Angle(enemyDir, cannon.transform.forward);

        RaycastHit hit;
        if (Physics.SphereCast(shootPos.position, 1, enemyDir, out hit))
        {
            if (hit.collider.CompareTag("Range") || hit.collider.CompareTag("Melee") || hit.collider.CompareTag("Enemy") 
                || hit.collider.CompareTag("No Weapon") || hit.collider.CompareTag("Explosive"))
            {
                if (!isShooting && angleToEnemy <= viewAngle)
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
        muzzleFlash.Play();
        createBall();

        shootTime.gameObject.SetActive(true);
        StartCoroutine(UpdateUI());
        yield return new WaitForSeconds(shootRate);

        isShooting = false;
        muzzleFlash.Stop();
    }

    IEnumerator UpdateUI()
    {
        float time = 0;
        shootTimeFill.fillAmount = 0;
        while (time < shootRate)
        {
            shootTimeFill.fillAmount = Mathf.Lerp(0, 1, time / shootRate);
            time += Time.deltaTime;
            yield return null;
        }
        shootTime.gameObject.SetActive(false);
        yield break;
    }
    public void createBall()
    {
        GameObject cannBall = Instantiate(cannball, shootPos.position, cannon.transform.rotation);
        Rigidbody ballRigidbody = cannBall.GetComponent<Rigidbody>();
        cannBall.GetComponent<Rigidbody>().velocity =
            (enemyDir - cannon.transform.position).normalized * cannSpeed;
        ballRigidbody.AddForce(transform.forward * forwardForce + transform.up * upForce);

        explosiveDamage = cannBall.GetComponent<explosiveWeapon>().damage;
        explosionRadius = cannBall.GetComponent<explosiveWeapon>().range;
        explosionForce = cannBall.GetComponent<explosiveWeapon>().force;
        timer = cannBall.GetComponent<explosiveWeapon>().timer;
    }
}
