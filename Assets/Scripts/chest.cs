using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chest : MonoBehaviour, actionObject
{
    [SerializeField] int chestCost;
    [SerializeField] int rollCost;
    [SerializeField] GameObject[] weapon;
    [SerializeField] Transform chestWeapon;
    [SerializeField] GameObject weaponSelection;
    [SerializeField] int seed;
    private Transform target = null;

    bool hasCoins;


    // Start is called before the first frame update
    void Start()
    {
        secondaryAction();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") target = other.transform;
        if (gameManager.instance.playerScript.GetCoins() >= chestCost)
        {
            hasCoins = true;
        }
        else
            hasCoins = false;
        Debug.Log("PLAYER");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player") target = null;
        Debug.Log("NO PLAYER");
    }

    public void primaryAction()
    {
        if (hasCoins)
        {
            gameManager.instance.playerScript.addCoins((-chestCost));
            Destroy(weaponSelection.gameObject);
            Debug.Log("bought");

            //give weapon
        }
        else
        {
            Debug.Log("Not enough coins");
        }
    }
    public void secondaryAction()
    {
        Destroy(weaponSelection.gameObject);
        seed = (int)Random.Range(0.0f, 9.0f);
        weaponSelection = weapon[seed];
        weaponSelection = Instantiate(weaponSelection, chestWeapon.position, chestWeapon.transform.rotation, chestWeapon.transform);
        chestCost = (seed + 1) * 100;
    }
}