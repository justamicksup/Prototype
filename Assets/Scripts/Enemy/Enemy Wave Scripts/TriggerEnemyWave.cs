using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnemyWave : MonoBehaviour
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
            gameManager.instance.alertText.text = "E: Start Wave";
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
