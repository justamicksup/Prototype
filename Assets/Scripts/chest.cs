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
    //[SerializeField] Weapon armory;
    [SerializeField] Transform chestWeapon;
    public Weapon weaponSelection;
    [SerializeField] int seed;
    private Transform target = null;
    public Armory tempArmory;

    bool hasCoins;
    bool openingChest;
    bool playerInRange;
    bool choseWeapon;




    void Start()
    {
        //weaponSelection = tempArmory.ListOfWeapons[seed];
    }
    void Update()
    {

        if (playerInRange)
        {
            
            
            if (!openingChest && Input.GetButton("Action"))
            {
                checkWallet();
                if (hasCoins)
                {
                    Debug.Log("Have Money");
                    StartCoroutine(OpenTheChest());
                    //gameManager.instance.updateAmmo();
                   
                }
                else
                {
                    Debug.Log("You're Broke");
                }
                
            }

            if (!choseWeapon && Input.GetButton("Submit"))
            {
                if (weaponSelection != null)
                {
                    Debug.Log("Got Weapon");
                    StartCoroutine(TakeWeapon());
                }
                else
                {
                    Debug.Log("Where is my weapon?");
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if ((other.tag == "Player"))
        {
            target = other.transform;
            Debug.Log("PLAYER");
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player") target = null;
        Debug.Log("NO PLAYER");
        playerInRange = false;
    }

    public void primaryAction()
    {
    }

    public void secondaryAction()
    {
    }

    IEnumerator OpenTheChest()
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
    }
}