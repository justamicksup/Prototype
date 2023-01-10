using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    public GameObject player;
    public playerController playerScript;
    public HUD HUD;
    public int enemiesRemaining;
    public int waveCount;
    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        HUD = transform.parent.gameObject.GetComponent<HUD>();
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

    public void loadScene(int scene)
    {
        //Load scene by index
        SceneManager.LoadScene(scene);
    }
    public void reloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
