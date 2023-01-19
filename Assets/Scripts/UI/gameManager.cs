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
    public Armory armory;
    public bool nextWave;
    [SerializeField] private TextMeshProUGUI enemiesRemainingText;
    
    
    [Header("----- UI -----")] public HUD HUD;
    public GameObject activeMenu;
    public GameObject pauseMenu;
    public GameObject winMenu;
    public GameObject loseMenu;
    public Image playerHPBar;
    public Image playerStaminaBar;
    [SerializeField] private Text[] ammoCountText;
    [SerializeField] private Text coinsText;
    [SerializeField] private Text waveCountText;
    public GameObject settingsMenu;
    
    [Header("----- Weapons and Ammo -----")]
    public int ammoRemaining;
    public int weaponsInLevel;
    public Weapon currWeapon;
    //Having issue casting weapon to specific class and keeping scriptable object stats
    //goal is to have all weapons be in Weapon[] WeaponSlots and cast accordingly for logic
    public Weapon[] WeaponSlots;
    public RangedWeapons[] GunSlots;

    [Header("----- Game Settings -----]")]
    public int sensitivity;
    
    
    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        //HUD = transform.parent.gameObject.GetComponent<HUD>();
        playerScript = player.GetComponent<playerController>();
        playerScript.addCoins(2000000);
        playerSpawnPos = GameObject.FindGameObjectWithTag("Player Spawn Pos");
        timeScaleOrig = Time.timeScale;
        waveController = Resources.Load("WaveController") as WaveController;
        
        ammoCountText[0].text = "";
        ammoCountText[1].text = "";
        ammoCountText[2].text = "";
        waveCountText.text = "";
        coinsText.text = playerScript.GetCoins().ToString();
        
        
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

    public void updateAmmo(int ammo)
    {
        playerScript.ammo -= ammo;
        
    }

    public void updateWeaponSlots(Weapon _weapon)
    {
        // if (_weapon is RangedWeapons)
        // {
        //     RangedWeapons rangedWeapons = (RangedWeapons)_weapon;
        //     rangedWeapons = rangedWeapons.SetStats(rangedWeapons);
        //
        //     //temporarily putting it in hardcoded spot, will fix later
        //     GunSlots[0] = rangedWeapons;
        //
        //     
        //     UpdateUI();
        //     currWeapon = GunSlots[0];
        // }
        // else
        // {
        //     Debug.Log("Not a Ranged Weapons unfortunately");
        // }
    }

    public void UpdateUI()
    {
       // update ammo on slot
        coinsText.text = playerScript.GetCoins().ToString();
        waveCountText.text = $"Wave {waveCount}";
        
    }
}