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
    public GameObject playerSpawnPos;
    public int HP;
    public bool isPaused;
    public GameObject activeMenu;
    public GameObject pauseMenu;
    public GameObject winMenu;
    public GameObject loseMenu;
    public bool nextWave;

    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        HUD = transform.parent.gameObject.GetComponent<HUD>();
        playerScript = player.GetComponent<playerController>();
        playerScript.addCoins(2000);
        playerSpawnPos = GameObject.FindGameObjectWithTag("Player Spawn Pos");
        timeScaleOrig = Time.timeScale;
       
    }


    void Update()
    {
        
        
        if (Input.GetButtonDown("Pause"))
        {
            isPaused = !isPaused;
            activeMenu = pauseMenu;
            activeMenu.SetActive(isPaused);

            if (isPaused)
                pauseGame();
            else
                unpauseGame();
        }
    }

    public void updateEnemyRemaining(int amount)
    {
        enemiesRemaining += amount;

        // check for game over (enemy <= 0)
        if (waveCount == 3 && enemiesRemaining == 0)
        {
            youWin();
        }
    }

    public void updatePlayerCoins(int amount)
    {
        playerScript.addCoins(amount);
    }
    
    public void updateWave(int amount)
    {
        waveCount += amount;
        nextWave = true;
    }

    public void pauseGame()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void unpauseGame()
    {
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        activeMenu.SetActive(false);
        activeMenu = null;
        
    }

    public void youWin()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        activeMenu = winMenu;
        activeMenu.SetActive(true);
    }
    
    public void youLose()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        pauseGame();
        activeMenu = loseMenu;
        activeMenu.SetActive(true);
    }
}
