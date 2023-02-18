
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

public class chest : MonoBehaviour, IActionObject
{
    [SerializeField] int chestCost;
    [SerializeField] int rollCost;

    [SerializeField] Transform chestWeapon;

    [SerializeField] GameObject weaponDisplay;
    [SerializeField] int seed;
    private Transform target = null;
    public Armory tempArmory;

    private bool showChest = false;

    bool hasCoins;
    bool openingChest;
    bool playerInRange;
    bool choseWeapon;
    bool isRerolling;

    public int wallet;

    void Start()
    {
        rollChest();
        weaponDisplay.SetActive(false);
    }

    void Update()
    {
        wallet = gameManager.instance.playerScript.GetCoins();
        if (rollCost <= wallet)
        {
            hasCoins = true;
            weaponDisplay.SetActive(true);
            gameManager.instance.playerScript.inActionRange = true;
        }
        else
        {
            hasCoins = false;
        }

        if(showChest && !gameManager.instance.isPaused)
        {
            if (Input.GetButtonDown("Submit") && !isRerolling && hasCoins)
            {
                SecondaryAction();
                gameManager.instance.alertText.text = "";
                gameManager.instance.alertText.text = $"E: Purchase Weapon ({chestCost})\n F: Reroll ({rollCost})";
            }

            if (Input.GetButton("Action") && wallet >= chestCost)
            {
                PrimaryAction();
                gameManager.instance.alertText.text = "";
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            target = other.transform;
            if (target != null && Vector3.Distance(transform.position, target.position) <= 4f)
            {
                playerInRange = true;
                showChest = true;
                gameManager.instance.alertText.text = $"E: Purchase Weapon ({chestCost})\n F: Reroll ({rollCost})";
            }
        }
    }   

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            showChest = false;
            target = null;
            weaponDisplay.SetActive(false);
            gameManager.instance.alertText.text = "";
            // Debug.Log("NO PLAYER");
            playerInRange = false;
            gameManager.instance.playerScript.inActionRange = false;
            hasCoins = false;
        }

    }
    
    public void PrimaryAction()
    {
        //Take the weapon you bought
        gameManager.instance.playerScript.AddWeaponToInventory(tempArmory.MasterWeaponList[seed]);
        gameManager.instance.playerScript.addCoins(-chestCost);
        // Destroy chest
        Destroy(gameObject);
    }

    public void SecondaryAction()
    {
       //Deduct coins for roll
        gameManager.instance.playerScript.addCoins(-rollCost);
        
         // Re Roll for a new Weapon
        rollChest();
        
    }

    private void rollChest()
    {
        seed = Random.Range(0, tempArmory.MasterWeaponList.Count);
        chestCost = (seed + 1) * 100;

        weaponDisplay.GetComponent<MeshFilter>().sharedMesh =
            tempArmory.MasterWeaponList[seed].Model.GetComponent<MeshFilter>().sharedMesh;
        weaponDisplay.GetComponent<MeshRenderer>().sharedMaterials =
            tempArmory.MasterWeaponList[seed].Model.GetComponent<MeshRenderer>().sharedMaterials;
    }
}