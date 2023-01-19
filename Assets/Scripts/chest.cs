using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class chest : MonoBehaviour, actionObject
{
    [SerializeField] int chestCost;
    [SerializeField] int rollCost;

    [SerializeField] Transform chestWeapon;

    [SerializeField] GameObject weaponDisplay;
    [SerializeField] int seed;
    private Transform target = null;
    public Armory tempArmory;

    bool hasCoins;
    bool openingChest;
    bool playerInRange;
    bool choseWeapon;
    bool isRerolling;


    void Start()
    {
        //Cameron Code

        rollChest();
        weaponDisplay.SetActive(false);
        // weaponSelection.SetActive(false);
    }

    void Update()
    {
        // if (playerInRange)
        // {
        //     
        //     
        //     if (!openingChest && Input.GetButton("Action"))
        //     {
        //         checkWallet();
        //         if (hasCoins)
        //         {
        //             Debug.Log("Have Money");
        //             StartCoroutine(OpenTheChest());
        //             //gameManager.instance.updateAmmo();
        //            
        //         }
        //         else
        //         {
        //             Debug.Log("You're Broke");
        //         }
        //         
        //     }
        //
        //     if (!choseWeapon && Input.GetButton("Submit"))
        //     {
        //         if (weaponDisplay != null)
        //         {
        //             Debug.Log("Got Weapon");
        //             StartCoroutine(TakeWeapon());
        //         }
        //         else
        //         {
        //             Debug.Log("Where is my weapon?");
        //         }
        //     }
        // }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            target = other.transform;
            Debug.Log("PLAYER");
            playerInRange = true;
            // gameManager.instance.actionObject = this.gameObject;
            if (chestCost < gameManager.instance.playerScript.GetCoins())
            {
                hasCoins = true;
                weaponDisplay.SetActive(true);
                //weaponSelection.SetActive(true);
                gameManager.instance.playerScript.inActionRange = true;
            }
            else
            {
                hasCoins = false;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (playerInRange)
            {
                if (Input.GetButton("Action") && !isRerolling)
                {
                    
                    secondaryAction();
                    StartCoroutine(Delay(.1f));

                }
            
            }


            if (playerInRange && Input.GetButton("Submit"))
            {
                primaryAction();
            }
        }
    }
    

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            target = null;
            weaponDisplay.SetActive(false);

            // gameManager.instance.actionObject = null;
            //weaponSelection.SetActive(false);
        }

        Debug.Log("NO PLAYER");
        playerInRange = false;
        gameManager.instance.playerScript.inActionRange = false;
        hasCoins = false;
    }
    
    public void primaryAction()
    {
        //Take the weapon you bought
        gameManager.instance.playerScript.WeaponPickup(tempArmory.MasterWeaponList[seed]);
        // Destroy chest
        Destroy(gameObject);

       

       


        // //get weapon stats
        // //destroy weapon and chest
        // gameManager.instance.playerScript.addCoins(-chestCost);
        // gameManager.instance.playerScript.weaponPickup(chestContents[seed]);
        // Destroy(gameObject);
        //
        // //give player weapon
    }

    public void secondaryAction()
    {
       //Deduct coins for roll
        gameManager.instance.playerScript.addCoins(-rollCost);
        
         // Re Roll for a new Weapon
        rollChest();
        
    }

    IEnumerator Delay(float delay)
    {
        //Delay to not reroll weapon since registering multiple rolls in 1 frame
        isRerolling = true;
        yield return new WaitForSeconds(delay);
        isRerolling = false;
    }

    private void rollChest()
    {
        // if (weaponSelection != null)
        // {
        //     Destroy(weaponSelection);
        // }
        // seed = Random.Range(0, chestContents.Count - 1);
        //chestCost = (seed + 1) * 100;

        //weaponSelection = chestContents[seed].gunModel;
        //weaponSelection = Instantiate(weaponSelection, chestWeapon.position, chestWeapon.rotation);

        seed = Random.Range(0, tempArmory.MasterWeaponList.Count);
        chestCost = (seed + 1) * 100;

        weaponDisplay.GetComponent<MeshFilter>().sharedMesh =
            tempArmory.MasterWeaponList[seed].Model.GetComponent<MeshFilter>().sharedMesh;
        weaponDisplay.GetComponent<MeshRenderer>().sharedMaterials =
            tempArmory.MasterWeaponList[seed].Model.GetComponent<MeshRenderer>().sharedMaterials;
    }

    // IEnumerator OpenTheChest()
    // {
    // openingChest = true;
    //
    //
    //
    // RaycastHit hit;
    // if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit))
    // {
    //     if (hit.collider.GetComponent<chest>() != null)
    //     {
    //         hit.collider.GetComponent<chest>().checkWallet();
    //         if (hasCoins)
    //         {
    //             gameManager.instance.updatePlayerCoins(-chestCost);
    //             
    //             seed = Random.Range(0, tempArmory.MasterWeaponList.Count);
    //             chestCost = (seed + 1) * 10;
    //
    //             weaponDisplay.GetComponent<MeshFilter>().sharedMesh =
    //                 tempArmory.MasterWeaponList[seed].Model.GetComponent<MeshFilter>().sharedMesh;
    //             weaponDisplay.GetComponent<MeshRenderer>().sharedMaterials =
    //                 tempArmory.MasterWeaponList[seed].Model.GetComponent<MeshRenderer>().sharedMaterials;
    //             
    //             
    //
    //
    //         }
    //     }
    // }
    //
    // yield return new WaitForSeconds(.5f);
    // openingChest = false;
    // }

    // IEnumerator TakeWeapon()
    // {
    //     choseWeapon = true;
    //     RaycastHit hit;
    //     if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit))
    //     {
    //         if (hit.collider.CompareTag("Weapon"))
    //         {
    //             gameManager.instance.playerScript.WeaponPickup(tempArmory.MasterWeaponList[seed]);
    //
    //
    //             Destroy(gameObject);
    //         }
    //     }
    //
    //     yield return new WaitForSeconds(.1f);
    //     choseWeapon = false;
    // }


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