using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    public GameObject player;
    public playerController playerScript;
    public int enemiesRemaining;

    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
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
        if(enemiesRemaining<=0) { Debug.Log("You Win!!"); }
    }

    public void updatePlayerCoins(int amount)
    {
        playerScript.addCoins(amount);
    }
}
