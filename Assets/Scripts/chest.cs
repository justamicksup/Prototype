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
    [SerializeField] int seed;
    [SerializeField] int chestCost;
    [Range(100, 1000)] [SerializeField] int rollCost;
    [SerializeField] Transform chestWeapon;
    public GameObject weaponSelection;
    [SerializeField] List<RangedWeapon> chestContents;
    private Transform target = null;

    bool hasCoins;
    bool playerInRange;

    void Start()
    {
        rollChest();
        weaponSelection.SetActive(false);
    }
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            target = other.transform;
            Debug.Log("PLAYER");
            playerInRange = true;
            gameManager.instance.actionObject = this.gameObject;
            if (chestCost < gameManager.instance.playerScript.GetCoins())
            {
                hasCoins = true;
                weaponSelection.SetActive(true);
                gameManager.instance.playerScript.inActionRange = true;
            }
            else
            {
                hasCoins = false;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            target = null;
            gameManager.instance.actionObject = null;
            weaponSelection.SetActive(false);
        }
        Debug.Log("NO PLAYER");
        playerInRange = false;
        gameManager.instance.playerScript.inActionRange = false;
        hasCoins = false;
    }

    public void primaryAction()
    {
        //get weapon stats
        //destroy weapon and chest
        gameManager.instance.playerScript.addCoins(-chestCost);
        gameManager.instance.playerScript.weaponPickup(chestContents[seed]);
        Destroy(gameObject);

        //give player weapon
    }

    public void secondaryAction()
    {
        gameManager.instance.playerScript.addCoins(-rollCost);
        rollChest();
    }
    private void rollChest()
    {
        if (weaponSelection != null)
        {
            Destroy(weaponSelection);
        }
        seed = Random.Range(0, chestContents.Count - 1);
        chestCost = (seed + 1) * 100;
        weaponSelection = chestContents[seed].gunModel;
        weaponSelection = Instantiate(weaponSelection, chestWeapon.position, chestWeapon.rotation);
    }
}
    /*IEnumerator OpenTheChest()
    {
        openingChest = true;

        if (weaponSelection != null)
        {
            Debug.Log("Before Destroy");
            Destroy(weaponSelection.GameObject());
            Debug.Log("Before Destroy");
        }
       
        
        
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit))
        {
            if (hit.collider.GetComponent<chest>() != null)
            {
                hit.collider.GetComponent<chest>().checkWallet();
                if (hasCoins)
                {
                    gameManager.instance.updatePlayerCoins(-chestCost);
                    seed = Random.Range(0, tempArmory.ListOfWeapons.Count);
                    Debug.Log("Before");
                    //weaponSelection = armory.armory.ListOfWeapons[seed];
                    Debug.Log("After");
                    chestCost = (seed + 1) * 10;
                   weaponSelection = Instantiate(tempArmory.ListOfWeapons[seed], chestWeapon.position,
                        chestWeapon.transform.rotation, chestWeapon.transform);
                    Debug.Log("Last");
                    
                    
                }
            }
        }

        yield return new WaitForSeconds(.5f);
        openingChest = false;
    }

    IEnumerator TakeWeapon()
    {
        choseWeapon = true;
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit))
        {
            if (hit.collider.CompareTag("Weapon"))
            {
                //armory.armory.ListOfWeapons[seed]
                //Instantiate(armory.armory.ListOfWeapons[seed], new Vector3(0,0,0), Quaternion.identity);
                var temp = Instantiate(tempArmory.ListOfWeapons[seed], gameManager.instance.playerScript.viewModel.transform);

                gameManager.instance.updateWeaponSlots(tempArmory.ListOfWeapons[seed]);
               // gameManager.instance.playerScript.weapons[0] = armory.armory.ListOfWeapons[seed];

               // gameManager.instance.updateWeaponSlots(armory.armory.ListOfWeapons[seed]);
                
               //gameManager.instance.updateWeaponSlots(weap);
                   Destroy(gameObject);
            }
        }

        yield return new WaitForSeconds(.1f);
        choseWeapon = false;
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
    }*/