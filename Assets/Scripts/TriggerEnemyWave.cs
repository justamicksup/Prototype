using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnemyWave : MonoBehaviour, actionObject
{
    bool playerInRange;
    public GameObject chestTrigger;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
    public void primaryAction()
    {
        if (playerInRange)
        {
            if (Input.GetButtonDown("Action"))
            {
                Destroy(chestTrigger);
                gameManager.instance.updateWave();
                gameManager.instance.updateAmmoUI();
            }
        }
    }

    public void secondaryAction()
    {
        primaryAction();
    }
}
