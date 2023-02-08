using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lootChest : MonoBehaviour, actionObject
{
    [SerializeField] GameObject chest;
    [Range(1, 1000)][SerializeField] int lootCost;
    [SerializeField] Transform weaponTransform;
    [SerializeField] MasterWeapon weapon;

    [SerializeField] GameObject displayWeapon;
    bool inRange;
    bool canPurchase;

    void Start()
    {
        displayWeapon.GetComponent<MeshFilter>().sharedMesh = weapon.Model.GetComponent<MeshFilter>().sharedMesh;
        displayWeapon.GetComponent<MeshRenderer>().sharedMaterials = weapon.Model.GetComponent<MeshRenderer>().sharedMaterials;
        displayWeapon.SetActive(false);
    }

    void Update()
    {
        if (Input.GetButtonDown("Submit") && checkPlayerCoins())
        {           
            primaryAction();
        }
    }

    public void primaryAction()
    {
        if(inRange)
        {            
            if(checkPlayerCoins()) 
            {
                gameManager.instance.playerScript.AddWeaponToInventory(weapon);
                gameManager.instance.playerScript.addCoins(-lootCost);
            }
        }
    }

    public void secondaryAction() 
    { 
        //None - maybe can be upgrade actions?
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            inRange = true;
            displayWeapon.SetActive(true);
            gameManager.instance.alertText.text = $"Weapon Cost: {lootCost}";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            inRange = false;
            canPurchase = false;
            gameManager.instance.alertText.text = "";
            displayWeapon.SetActive(false);
        }
    }

    private bool checkPlayerCoins()
    {
        if(gameManager.instance.playerScript.GetCoins() >= lootCost)
        {
            canPurchase = true;
        }

        return canPurchase;
    }
}
