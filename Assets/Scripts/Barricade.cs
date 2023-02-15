using UnityEngine;
using UnityEngine.UI;

public class Barricade : MonoBehaviour, IDamage, actionObject
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
        if (Input.GetButtonDown("Action") && CheckPlayerCoins(repairCost))
        {
            primaryAction();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            target = other.transform;
            playerInRange = true;
            if (!barricadeActive)
            {
                gameManager.instance.alertText.text = $"E: Rebuild: ({repairCost})";
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if  (collision.gameObject.CompareTag("Range")
            || collision.gameObject.CompareTag("Melee")
            || collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<IDamage>().takeDamage(damage);
            collision.gameObject.GetComponent<Rigidbody>().AddForce(-transform.forward * pushBackForce);
            takeDamage(damage / 2);
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

    public void takeDamage(float damage)
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

    public void primaryAction()
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

    public void secondaryAction()
    {
        primaryAction();
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