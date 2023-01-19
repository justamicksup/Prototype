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
    public static Animation shipAnim;
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
    
    
    [Header("----- UI -----")]
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
    

    [Header("----- Game Settings -----]")]
    public int sensitivity;
    
    
    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        shipAnim = GameObject.FindGameObjectWithTag("Ships").GetComponent<Animation>();
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

    public void StartGame()
    {
        StartCoroutine(StartGameHelper());
    }

    private IEnumerator StartGameHelper()
    {
        if(shipAnim != null)
        {
            shipAnim.clip = shipAnim.GetClip("ShipsLanding");
            shipAnim.Play();
        }
        yield return new WaitForSeconds(1.5f);
        instance.updateWave();
        instance.UpdateUI();
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
                updateWave();
            }
        }
    }

    public void updatePlayerCoins(int amount)
    {
        playerScript.addCoins(amount);
    }

    public void updateWave()
    {
        waveCount++;
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
    
    public void updateAmmoUI()
    {
        for (int i = 0; i < 3; i++)
        {
            ammoCountText[i].text = playerScript.ammoRemaining.ToString() + "/" + playerScript.ammoCapacity.ToString();
        }
    }

    public void updateAmmo(int ammo)
    {
        playerScript.ammo -= ammo;
        
    }

  

    public void UpdateUI()
    {
       // update ammo on slot
        coinsText.text = playerScript.GetCoins().ToString();
        waveCountText.text = $"Wave {waveCount}";
        
    }
    
    public void updateCoinUI()
    {
        coinsText.text = playerScript.GetCoins().ToString();
    }
}