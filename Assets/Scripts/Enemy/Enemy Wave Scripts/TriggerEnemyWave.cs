using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnemyWave : MonoBehaviour
{
    bool playerInRange;
    [SerializeField] GameObject chestTrigger;
    [SerializeField] EnemyWaveSystem enemySpawnPoint;
    
    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {

            if (Input.GetButtonDown("Action"))
            {
                gameManager.instance.alertText.text = "";
                Destroy(chestTrigger); 
                gameManager.instance.StartGame();
               
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            gameManager.instance.alertText.text = "E: Take The Key";
            gameManager.instance.updateKey();
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            gameManager.instance.alertText.text = "";
        }
    }


}
