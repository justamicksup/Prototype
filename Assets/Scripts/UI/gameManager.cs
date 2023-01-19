using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Content;
#endif
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Cursor = UnityEngine.Cursor;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    
    [Header("----- Player -----")] 
    public GameObject player;
    public playerController playerScript;
    public GameObject playerSpawnPos;
    public int HP;
    public GameObject actionObject;

    [Header("----- Game Goal -----")]
    public int enemiesRemaining;
    public int waveCount;
    public int maxWave;
    public WaveController waveController;
    //Dont understand the need for a bool - rework?
    public bool nextWave;
    
    [Header("----- MENUS -----")]
    public GameObject activeMenu;
    public GameObject pauseMenu;
    public GameObject settingsMenu;
    public GameObject winMenu;
    public GameObject loseMenu;

    [Header("----- HUD -----")]
    public Image playerHPBar;
    public Image playerStaminaBar;
    [SerializeField] private Text[] ammoCountText;
    [SerializeField] private Text coinsText;
    [SerializeField] private Text waveCountText;
    [SerializeField] private TextMeshProUGUI enemiesRemainingText;
    /*
    [Header("----- Weapons -----")]
    [SerializeField] public ScriptableObject[] weaponsInScene; //maybe scriptable object will replace this.
    */
    [Header("----- Game Settings -----]")]
    public int sensitivity;
    
    public bool isPaused;
    float timeScaleOrig;
    
    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        playerScript.addCoins(2000);
        playerSpawnPos = GameObject.FindGameObjectWithTag("Player Spawn Pos");
        timeScaleOrig = Time.timeScale;
        waveController = Resources.Load("WaveController") as WaveController;
        
    }


    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
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
                updateWave();
            }
        }
    }

    public void updatePlayerCoins(int amount)
    {
        playerScript.addCoins(amount);
        coinsText.text = playerScript.GetCoins().ToString();
    }

    public void updateWave()
    {
        waveCount++;
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

    public void updateAmmoUI()
    {
        for (int i = 0; i < 3; i++)
        {
            ammoCountText[i].text = playerScript.ammoRemaining.ToString() 
                + "/" + playerScript.gunAmmo.ToString();
        }
    }

    public void updateCoinUI()
    {
        coinsText.text = playerScript.GetCoins().ToString();
    }
}