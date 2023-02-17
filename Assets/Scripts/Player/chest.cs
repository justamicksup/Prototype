
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
            if (Input.GetButtonDown("Action") && !isRerolling && hasCoins)
            {
                SecondaryAction();
                gameManager.instance.alertText.text = $"F: Purchase Weapon ({chestCost})\n E: Reroll ({rollCost})";
                //StartCoroutine(Delay(.1f));
            }

            if (Input.GetButton("Submit") && wallet >= chestCost)
            {
                PrimaryAction();
                gameManager.instance.alertText.text = "";
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            target = other.transform;
            // Debug.Log("PLAYER");
            playerInRange = true;
            gameManager.instance.alertText.text = $"F: Purchase Weapon ({chestCost})\n E: Reroll ({rollCost})";
            // if (chestCost < wallet)
            // {
            //     hasCoins = true;
            //     weaponDisplay.SetActive(true);
            //     gameManager.instance.playerScript.inActionRange = true;
            // }
            // else
            // {
            //     hasCoins = false;
            // }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (playerInRange)
            {
                showChest = true;
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

    //commented out because issue fixed with GetButtonDown
    //IEnumerator Delay(float delay)
    //{
    //    //Delay to not reroll weapon since registering multiple rolls in 1 frame
    //    isRerolling = true;
    //    yield return new WaitForSeconds(delay);
    //    isRerolling = false;
    //}

    private void rollChest()
    {
        seed = Random.Range(0, tempArmory.MasterWeaponList.Count);
        chestCost = (seed + 1) * 100;

        weaponDisplay.GetComponent<MeshFilter>().sharedMesh =
            tempArmory.MasterWeaponList[seed].Model.GetComponent<MeshFilter>().sharedMesh;
        weaponDisplay.GetComponent<MeshRenderer>().sharedMaterials =
            tempArmory.MasterWeaponList[seed].Model.GetComponent<MeshRenderer>().sharedMaterials;
    }

    void checkWallet()
    {
        if (gameManager.instance.playerScript.GetCoins() >= chestCost)
        {
            hasCoins = true;
        }
        else
        {
            hasCoins = false;
        }
    }
}