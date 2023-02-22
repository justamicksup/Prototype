using System.Collections;
using Enemy.Enemy_Wave_Scripts;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cursor = UnityEngine.Cursor;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    public static Animation shipAnim;
    public static Animation rescueAnim;
    public static GameObject sun;
    [SerializeField] public NavMeshSurface surface;

    #region PLAYER
    [Header("----- Player -----")] 
    public GameObject player;
    public playerController playerScript;
    public int currentLevel;
    #endregion
    #region GAME GOAL
    [Header("----- Game Goal -----")] 
    public int enemiesRemaining;
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
    public int key;
    public EnemyWaveSystem enemyWaveSystem;
    private bool didWin;
    private bool didLose;
    #endregion
    #region UI
    [Header("----- UI -----")] 
    public GameObject activeMenu;
    public GameObject pauseMenu;
    public GameObject winMenu;
    public GameObject loseMenu;
    public Image playerHPBar;
    public Image playerStaminaBar;
    public GameObject instaKillIcon;
    public GameObject speedBoostIcon;
    public GameObject healingIcon;
    public GameObject screenFlash;
    float timer1; public bool t1On;
    float timer2; public bool t2On;
    float timer3; public bool t3On;
    //[SerializeField] private Text[] ammoCountText;
    [SerializeField] Image[] weaponIcons;
    [SerializeField]  TextMeshProUGUI[] weaponSlotNumber;
    [SerializeField] Text ammoText;
    [SerializeField] private Text coinsText;
    [SerializeField] internal Text waveCountText;
    [SerializeField] internal GameObject waveText;
    public Text alertText;
    public GameObject TitleScreen;
    public GameObject LoadScreen;
    public GameObject settingsScreen;
    public GameObject controlScreen;
    public Image LoadBar;
    public Text timer;
    #endregion
    #region UPGRADE MENU
    [Header("----- Upgrade Menu -----")]
    public GameObject upgradeMenu;
    public Text speed;
    public Text health;
    public Text stamina;
    public Text dmg;
    public Text maxammo;
    public Text reload;
    public Text range;
    public Text cost;
    #endregion
    #region WEAPONS & AMMO
    [Header("----- Weapons and Ammo -----")]
    public int ammoRemaining;
    public int weaponsInLevel;
    #endregion
    #region LOOT DROPS
    [Header("----- Enemy Loot Drops -----")] 
    [SerializeField] GameObject coin10;
    [SerializeField] GameObject coin25;
    [SerializeField] GameObject coin50;
    [SerializeField] GameObject coin500;
    [SerializeField] GameObject speedPowerUp;
    [SerializeField] GameObject ammoPickup;
    [SerializeField] GameObject healthPowerUp;
    [SerializeField] GameObject oneShotPowerUp;
    [SerializeField] GameObject ammoPowerUp;
    #endregion
    #region BACKGROUND
    [Header("----- Background -----")] 
    public AudioSource audBackground;
    public AudioClip[] levelMusicBackground;
    [Range(0, 1)] [SerializeField] float levelVolBackground;
    #endregion
    #region MUSIC & SETTINGS
    [Header("----- Fight Music -----")] 
    public AudioSource aud;
    public AudioClip[] levelMusic;
    [Range(0, 1)] [SerializeField] float levelVol;

    [Header("----- Game Settings -----]")] 
    public int sensitivity;
    public Material[] skyboxes = new Material[2];
    #endregion

    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        GameObject go = GameObject.FindGameObjectWithTag("Ships");
        if (go != null)
        {
            shipAnim = go.GetComponent<Animation>();
        }
        GameObject rescueShip = GameObject.FindGameObjectWithTag("Rescue Ship");
        if (rescueShip != null)
        {
            rescueAnim = rescueShip.GetComponent<Animation>();
        }
        sun = GameObject.FindGameObjectWithTag("Sun");
        //HUD = transform.parent.gameObject.GetComponent<HUD>();
        playerScript = player.GetComponent<playerController>();

        //****TURN ON FOR TESTING*****\\

        playerScript.addCoins(2000);

        //*******TURN ON FOR TESTING********\\


        playerSpawnPos = GameObject.FindGameObjectWithTag("Player Spawn Pos");
        timeScaleOrig = Time.timeScale;
        //waveController = Resources.Load("WaveController") as WaveController;

        //ammoCountText[0].text = "";
        //ammoCountText[1].text = "";
        //ammoCountText[2].text = "";
        waveCountText.text = "";
        waveText.SetActive(false);
        coinsText.text = playerScript.GetCoins().ToString();
        timer.text = "";

        if (audBackground != null)
        {
            audBackground.volume = levelVolBackground;
            audBackground.clip = levelMusicBackground[Random.Range(0, levelMusicBackground.Length)];
            audBackground.Play();
        }

        key = 0;

        if (PlayerPrefs.HasKey("sensitivity"))
        {
            sensitivity = (int)PlayerPrefs.GetFloat("sensitivity");
        }
    }

    void Update()
    {
        UpdateUI();
        if (Input.GetButtonDown("Cancel") && !didLose && !didWin)
        {

            if(upgradeMenu.activeSelf)
            {
                upgradeMenu.SetActive(false);
                isPaused = false;
            }
            else if(settingsScreen.activeSelf)
            {
                settingsScreen.SetActive(false);
                pauseMenu.SetActive(true);
            }
            else if(controlScreen.activeSelf)
            {
                controlScreen.SetActive(false);
                pauseMenu.SetActive(true);
            }
            else if(pauseMenu.activeSelf)
            {
                pauseMenu.SetActive(false);
                isPaused = false;
            }
            else
            {
                pauseMenu.SetActive(true);
                isPaused = true;
            }

            if (isPaused)
                pauseGame();
            else
                unpauseGame();
        }
        if(Input.GetButtonDown("Stats") && !didLose && !didWin)
        {
            if(upgradeMenu.activeSelf)
            {
                upgradeMenu.SetActive(false);
                unpauseGame();
                isPaused = false;
            }
            else
            {
                if(!pauseMenu.activeSelf && !controlScreen.activeSelf && !settingsScreen.activeSelf)
                {
                    isPaused = true;
                    upgradeMenu.SetActive(true);
                    pauseGame();
                }
                
            }

        }

        #region POWER UP TIMERS
        if (t1On)
        {
            timer1 -= Time.deltaTime;
            if (timer1 <= 0)
            {
                timer1 = 0;
                t1On= false;
                instaKillIcon.SetActive(false);
            }
        }
        if (t2On)
        {
            timer2 -= Time.deltaTime;
            if (timer2 <= 0)
            {
                timer2 = 0;
                t2On= false;
                healingIcon.SetActive(false);
            }
        }
        if (t3On)
        {
            timer3 -= Time.deltaTime;
            if (timer3 <= 0)
            {
                timer3 = 0; 
                t3On= false;
                speedBoostIcon.SetActive(false);
            }
        }
        #endregion
    }

    #region IENUM'S
    private IEnumerator StartGameHelper()
    {
        if (shipAnim != null)
        {
            shipAnim.clip = shipAnim.GetClip("ShipsLanding");
            shipAnim.Play();
        }
       
        if (sun != null)
        {
            RenderSettings.skybox = skyboxes[1];
            float time = 0;
            Light l = sun.GetComponent<Light>();
            Quaternion startValue = transform.rotation;
            Quaternion endValue = Quaternion.Euler(-52, 4, -145);
            while (time < 1.5f)
            {
                sun.transform.rotation = Quaternion.Lerp(startValue, endValue, time / 1.5f);
                RenderSettings.ambientIntensity = Mathf.Lerp(1, 0.5f, time / 1.5f);
                l.color = Color.Lerp(new Color(255f / 255f, 244f / 255f, 214f / 255f), new Color(129f / 255f, 182f / 255f, 255f / 255f), time / 1.5f);
                l.intensity = Mathf.Lerp(1, 0.5f, time / 1.5f);
                time += Time.deltaTime;
                yield return null;
            }

            RenderSettings.ambientIntensity = 0.5f;
            l.color = new Color(129f / 255f, 182f / 255f, 255f / 255f);
            l.intensity = 0.5f;
            sun.transform.rotation = endValue;
        }

        if (aud != null)
        {
            audBackground.Stop();
            aud.volume = levelVol;
            aud.clip = levelMusic[Random.Range(0, levelMusic.Length)];
            aud.Play();
        }

        yield return new WaitForSeconds(shipAnim.clip.length);
        enemyWaveSystem.SpawnTheWave();
        instance.updateWave();
        instance.UpdateUI();
    }
    public IEnumerator flash()
    {
        screenFlash.SetActive(true);
        yield return new WaitForSeconds(0.15f);
        screenFlash.SetActive(false);
    }
    public IEnumerator rescueShipWin()
    {
        if (rescueAnim != null)
        {
            rescueAnim.clip = rescueAnim.GetClip("ShipsRescue");
            rescueAnim.Play();
        }
        yield return new WaitForSeconds(1.5f);
    }

    #endregion

    #region UPDATE UI FUNCTIONS
    public void updatePlayerCoins(int amount)
    {
        playerScript.addCoins(amount);
    }
    public void updateAmmoUI(bool gun = true)
    {
        if (gun)
        {
            ammoText.text =
                $"{playerScript.weaponList[playerScript.currentWeapon].currentClip} / {playerScript.ammoRemaining}";
        }
        else
        {
            ammoText.text = $"\u221E / \u221E";
        }
    }
    public void updateAmmo(int ammo)
    {
        playerScript.ammo -= ammo;
    }
    public void UpdateUI()
    {
        if (playerScript.weaponList.Count > 0)
        {
            weaponIcons[0].color = new Color(255, 255, 255, 255);
            weaponIcons[1].color = new Color(255, 255, 255, 255);
            weaponIcons[2].color = new Color(255, 255, 255, 255);
            weaponIcons[0].color = new Color(255, 255, 255, 255);
            weaponIcons[1].color = new Color(255, 255, 255, 255);
            weaponIcons[2].color = new Color(255, 255, 255, 255);

            if (playerScript.currentWeapon == 0)
            {
                 weaponIcons[0].color = new Color(0, 0, 0, 255);
                 weaponSlotNumber[0].color = new Color(0, 0, 0, 255);
                // weaponIcons[1].color = new Color(0, 0, 0, 255);
                // weaponSlotNumber[1].color = new Color(255, 255, 255, 50);
                // weaponIcons[2].color = new Color(0, 0, 0, 255);
                // weaponSlotNumber[2].color = new Color(255, 255, 255, 50);
            }
            else if (playerScript.currentWeapon == 1)
            {
               // weaponIcons[0].color = new Color(255, 255, 255, 255);
               // weaponSlotNumber[0].color = new Color(255, 255, 255, 255);
                weaponIcons[1].color = new Color(0, 0, 0, 255);
                weaponSlotNumber[1].color = new Color(0, 0, 0, 255);
               // weaponIcons[2].color = new Color(0, 0, 0, 255);
               // weaponSlotNumber[2].color = new Color(255, 255, 255, 50);
            }
            else if (playerScript.currentWeapon == 2)
            {
                 // weaponIcons[0].color = new Color(0, 0, 0, 255);
                 // weaponSlotNumber[0].color = new Color(255, 255, 255, 50);
                 // weaponIcons[1].color = new Color(0, 0, 0, 255);
                 // weaponSlotNumber[1].color = new Color(255, 255, 255, 50);
                weaponIcons[2].color = new Color(0, 0, 0, 255);
                weaponSlotNumber[2].color = new Color(0, 0, 0, 255);
            }
        }

        coinsText.text = playerScript.GetCoins().ToString();
        if (waveCount > 0)
        {
            if (enemyWaveSystem.isBossSpawned)
            {
                waveCountText.text = "";
            }
            else if (enemyWaveSystem.isMiniBossSpawned)
            {
                waveCountText.text = "";
            }
            else
            {
                waveText.SetActive(true);
                 waveCountText.text = $" {waveCount}";
            }
           
        }

        //Upgrade Menu
        health.text = $"Health: {playerScript.PlayerMaxHealth}";
        stamina.text = $"Stamina: {playerScript.PlayerMaxStamina}";
        speed.text = $"Speed: {playerScript.PlayerSpeed}";

        dmg.text = $"Damage: {playerScript.GunDamage}";
        range.text = $"Range: {playerScript.GunShootRange}";
        reload.text = $"Reload Time: {playerScript.GunReloadTime}";
        maxammo.text = $"Max Ammo: {playerScript.MaxAmmo}";

        cost.text = $"Cost to Upgrade: {playerScript.upgradeCost}";
    }
    public void updateCoinUI()
    {
        coinsText.text = playerScript.GetCoins().ToString();
    }

    #endregion

    #region GAME
    public void StartGame()
    {
        StartCoroutine(StartGameHelper());
    }
    public void updateEnemyRemaining(int amount)
    {
        enemiesRemaining += amount;
        //        enemiesRemainingText.text = enemiesRemaining.ToString("F0");

        // check to see if game is over based on enemy count <= 0


        // if (enemiesRemaining <= 0)
        // {
        //     if (waveCount >= waveController.numWaves)
        //     {
        //         pauseGame();
        //         activeMenu = winMenu;
        //         activeMenu.SetActive(true);
        //     }
        //     else
        //     {
        //         updateWave();
        //     }
        // }
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
        Cursor.lockState = CursorLockMode.None;
    }
    public void unpauseGame()
    {
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void youWin()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        activeMenu = winMenu;
        didWin = true;
        activeMenu.SetActive(true);
    }
    public void youLose()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        pauseGame();
        didLose = true;
        activeMenu = loseMenu;
        activeMenu.SetActive(true);
    }
    public void DropLoot(Transform trans, GameObject weapon = null, bool doesDropWeapon = false,
        bool doesDropPowerUp = false, bool doesDropCoins = false)
    {
        // for drop rates the number is percent chance it will drop, e.g. 5 is 5% chance to drop
        int coin500DropRate = 10;
        int coin50DropRate = 20;
        int coin25DropRate = 35;
        int coin10DropRate = 60;
        int weaponDropRate = 5;
        int powerUpDropRate = 8;

        int ammoDropRate = 50;

        int coin50Var = coin500DropRate + coin50DropRate;
        int coin25Var = coin50Var + coin25DropRate;
        int coin10Var = coin25Var + coin10DropRate;

        int rand;

        if (doesDropCoins)
        {
            rand = Random.Range(1, 100);
            if (rand <= coin500DropRate) // 10% change for 500 coins
            {
                Instantiate(coin500, trans.position, trans.rotation);
            }
            else if (rand > coin500DropRate && rand <= coin50Var) // 20% chance for 50 coins
            {
                Instantiate(coin50, trans.position, trans.rotation);
            }
            else if (rand > coin50Var && rand <= coin25Var) // 35% chance for 25 coins
            {
                Instantiate(coin25, trans.position, trans.rotation);
            }
            else if (rand > coin25Var && rand <= coin10Var) // 60% chance for 10 coins
            {
                Instantiate(coin10, trans.position, trans.rotation);
            }
            // 60% chance for no coins
        }

        if (doesDropWeapon)
        {
            rand = Random.Range(1, 100);
            if (rand <= weaponDropRate)
            {
                Instantiate(weapon, trans.position, trans.rotation);
            }
        }

        if (doesDropPowerUp)
        {
            rand = Random.Range(1, 100);
            if (rand <= powerUpDropRate)
            {
                rand = Random.Range(1, 4);
                switch (rand)
                {
                    case 1:
                        Instantiate(speedPowerUp, trans.position, trans.rotation);
                        break;
                    case 2:
                        Instantiate(healthPowerUp, trans.position, trans.rotation);
                        break;
                    case 3:
                        Instantiate(oneShotPowerUp, trans.position, trans.rotation);
                        break;
                    case 4:
                        Instantiate(ammoPowerUp, trans.position, trans.rotation);
                        break;
                }
            }
        }

        rand = Random.Range(0, 100);
        if(rand <= ammoDropRate)
            Instantiate(ammoPickup, trans.position, trans.rotation);
    }

    public void updateKey()
    {
        key += 1;
    }

    public IEnumerator reloadScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        while(!operation.isDone)
        {
            LoadBar.fillAmount = operation.progress;
            yield return null;
        }
    }



    public void IconTimer(PowerStat p)
    {
        if(p.shootDmgBonus != 0)
        {
            timer1 = p.effectDuration;
            t1On = true;
            instaKillIcon.SetActive(true);
        }
        else if(p.healthBonus != 0)
        {
            timer2 = p.effectDuration;
            t2On = true;
            healingIcon.SetActive(true);
        }
        else if (p.speedBonus != 0)
        {
            timer3 = p.effectDuration;
            t3On = true;
            speedBoostIcon.SetActive(true);
            playerScript.speedPart.Stop();
            playerScript.speedPart.gameObject.SetActive(false);
        }
    }
    #endregion
}