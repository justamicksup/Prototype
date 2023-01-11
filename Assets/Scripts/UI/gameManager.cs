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
    public HUD HUD;
    public int enemiesRemaining;
    public int waveCount;
    float timeScaleOrig;

    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        HUD = transform.parent.gameObject.GetComponent<HUD>();
        playerScript = player.GetComponent<playerController>();
        playerScript.addCoins(2000);
        timeScaleOrig = Time.timeScale;
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

    public void pauseGame()
    {
        Time.timeScale = 0;
        HUD.pauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void unpauseGame()
    {
        Time.timeScale = timeScaleOrig;
        HUD.pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }
}
