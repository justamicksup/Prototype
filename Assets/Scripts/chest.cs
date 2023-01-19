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
    
    [SerializeField]  GameObject weaponDisplay;
    [SerializeField] int seed;
    private Transform target = null;
    public Armory tempArmory;

    bool hasCoins;
    bool openingChest;
    bool playerInRange;
    bool choseWeapon;




    void Start()
    {
       
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
                if (weaponDisplay != null)
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

        
        
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit))
        {
            if (hit.collider.GetComponent<chest>() != null)
            {
                hit.collider.GetComponent<chest>().checkWallet();
                if (hasCoins)
                {
                    gameManager.instance.updatePlayerCoins(-chestCost);
                    
                    seed = Random.Range(0, tempArmory.MasterWeaponList.Count);
                    chestCost = (seed + 1) * 10;

                    weaponDisplay.GetComponent<MeshFilter>().sharedMesh =
                        tempArmory.MasterWeaponList[seed].Model.GetComponent<MeshFilter>().sharedMesh;
                    weaponDisplay.GetComponent<MeshRenderer>().sharedMaterials =
                        tempArmory.MasterWeaponList[seed].Model.GetComponent<MeshRenderer>().sharedMaterials;
                    
                    


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
                gameManager.instance.playerScript.WeaponPickup(tempArmory.MasterWeaponList[seed]);
              
                
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