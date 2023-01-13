using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Content;
#endif
using UnityEngine;
using UnityEngine.UI;
using Cursor = UnityEngine.Cursor;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    [Header("----- Player -----")] public GameObject player;
    public playerController playerScript;


    [Header("----- Game Goal -----")] public int enemiesRemaining;
    public int waveCount;
    float timeScaleOrig;
    public GameObject playerSpawnPos;
    public int HP;
    public bool isPaused;
    public int maxWave;
    public WaveController waveController;
    public bool nextWave;
    [SerializeField] private TextMeshProUGUI enemiesRemainingText;

    [Header("----- UI -----")]
    public HUD HUD;
    public GameObject activeMenu;
    public GameObject pauseMenu;
    public GameObject settingsMenu;
    public GameObject winMenu;
    public GameObject loseMenu;
    public Image playerHPBar;
    public Image playerStaminaBar;

    [Header("----- Game Settings -----]")]
    public int sensitivity;


    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        //HUD = transform.parent.gameObject.GetComponent<HUD>();
        playerScript = player.GetComponent<playerController>();
        playerScript.addCoins(2000);
        playerSpawnPos = GameObject.FindGameObjectWithTag("Player Spawn Pos");
        timeScaleOrig = Time.timeScale;
        waveController = Resources.Load("WaveController")as WaveController;
    }


    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            activeMenu = pauseMenu;
            isPaused = !isPaused;

            if (isPaused)
            {
                Debug.Log("Pause");
                pauseGame();
            }

            else
            {
                Debug.Log("Unpause");
                unpauseGame();
            }
        }
        
    }

    public void updateEnemyRemaining(int amount)
    {
        enemiesRemaining += amount;
//        enemiesRemainingText.text = enemiesRemaining.ToString("F0");

        // check to see if game is over based on enemy count <= 0
       
        
        if (enemiesRemaining <= 0)
        {
            if (waveCount == waveController.numWaves)
            {
                pauseGame();
                activeMenu = winMenu;
                activeMenu.SetActive(true);
            }
            else
            {
                updateWave(1);
            }
           
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
        //activeMenu = pauseMenu;
        activeMenu.SetActive(true);
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
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
        Cursor.lockState = CursorLockMode.None;
        activeMenu = winMenu;
        activeMenu.SetActive(true);
    }

    public void youLose()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        pauseGame();
        activeMenu = loseMenu;
        activeMenu.SetActive(true);
    }
}