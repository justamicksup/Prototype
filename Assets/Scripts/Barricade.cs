using Enemy;
using Player;
using UnityEngine;
using UnityEngine.UI;

public class Barricade : MonoBehaviour, IDamage, IActionObject
{
    [Header("----- Barricade Stats -----")] 
    [SerializeField] float HP;
    private float HPOrig;
    [SerializeField] int repairCost;
    [SerializeField] int damage;
    [SerializeField] int pushBackForce;
    public Image HPBar;


    [Header("----- Components -----")]
    [SerializeField] GameObject barricade;
    [SerializeField] GameObject brokenBarricade;
    [SerializeField] BoxCollider _boxCollider;

    private bool hasCoin = false;
    private bool playerInRange;
    private bool barricadeActive;
    private Transform target = null;

    // Start is called before the first frame update
    void Start()
    {
        brokenBarricade.SetActive(false);
        HPOrig = HP;
        updateHPBar();
        HPBar.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Action") && CheckPlayerCoins(repairCost) && !gameManager.instance.isPaused)
        {
            PrimaryAction();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            target = other.transform;
            if (!barricadeActive && Vector3.Distance(transform.position, target.position) <= 4f)
            {
                playerInRange = true;
                gameManager.instance.alertText.text = $"E: Rebuild: ({repairCost})";
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        Vector3 enemyPos = -collision.transform.forward;
        if  (collision.gameObject.CompareTag("Range")
            || collision.gameObject.CompareTag("Melee")
            || collision.gameObject.CompareTag("Enemy") 
            || collision.gameObject.CompareTag("No Weapon"))
        {
            collision.gameObject.GetComponent<IDamage>().TakeDamage(damage);
            rb.AddForce(enemyPos*pushBackForce);
            TakeDamage(damage / 2);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInRange = false;
            hasCoin = false;
            gameManager.instance.alertText.text = "";
        }
    }

    public void TakeDamage(float damage)
    {
        if (HP > 0)
        {
            HP -= (int)damage;
            updateHPBar();
        }
        if (HP <= 0)
        {
            barricade.SetActive(false);
            barricadeActive = false;
            brokenBarricade.SetActive(true);
            _boxCollider.enabled = false;
            HPBar.enabled= false;
        }        
    }

    public void PrimaryAction()
    {
        if (gameManager.instance.playerScript.GetCoins() >= repairCost)
        {
            gameManager.instance.playerScript.addCoins(-repairCost);
            brokenBarricade.SetActive(false);
            barricadeActive = true;
            barricade.SetActive(true);
            _boxCollider.enabled = true;
            HP = HPOrig;
            HPBar.enabled = true;
            updateHPBar();
            gameManager.instance.alertText.text = "";
        }      
    }

    public void SecondaryAction()
    {
        PrimaryAction();
    }

    private bool CheckPlayerCoins(int cost)
    {
        if (playerInRange && gameManager.instance.playerScript.GetCoins() >= cost)
        {
            hasCoin = true;
        }

        return hasCoin;
    }

    public float GetHP()
    {
        return HP;
    }

    public void updateHPBar()
    {
        HPBar.fillAmount = HP / HPOrig;
    }
}