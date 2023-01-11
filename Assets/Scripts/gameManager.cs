using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Content;
#endif
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    public GameObject player;
    public playerController playerScript;
    public GameObject HUD;
    public int enemiesRemaining;
    public int waveCount;
    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        HUD = transform.parent.gameObject;
        playerScript = player.GetComponent<playerController>();
        playerScript.addCoins(2000);
    }


    void Update()
    {
        
    }

    public void updateEnemyRemaining(int amount)
    {
        enemiesRemaining += amount;

        // check for game over (enemy <= 0)
        if (enemiesRemaining <= 0)
        {
            Debug.Log("You Win!!");
        }
    }

    public void updatePlayerCoins(int amount)
    {
        playerScript.addCoins(amount);
    }
    
    public void updateWave(int amount)
    {
        waveCount += amount;
    }
}
