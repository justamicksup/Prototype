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
using UnityEngine.EventSystems;
using Cursor = UnityEngine.Cursor;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    public static Animation shipAnim;
    public static GameObject sun;
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
    public GameObject instaKillIcon;
    public GameObject speedBoostIcon;
    public GameObject healingIcon;
    public GameObject screenFlash;
    //[SerializeField] private Text[] ammoCountText;
    [SerializeField] Image[] weaponIcons;
    [SerializeField] Text ammoText;
    [SerializeField] private Text coinsText;
    [SerializeField] private Text waveCountText;
    public Text alertText;
    public GameObject TitleScreen;
    public GameObject LoadScreen;
    public Image LoadBar;
    [Header("----- Weapons and Ammo -----")]
    public int ammoRemaining;
    public int weaponsInLevel;
    [Header("----- Background -----")]
    public AudioSource audBackground;
    public AudioClip[] levelMusicBackground;
    [Range(0, 1)] [SerializeField] float levelVolBackground;
    [Header("----- Fight Music -----")]
    public AudioSource aud;
    public AudioClip[] levelMusic;
    [Range(0, 1)] [SerializeField] float levelVol;
    [Header("----- Game Settings -----]")]
    public int sensitivity;


    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        GameObject go = GameObject.FindGameObjectWithTag("Ships");
        if (go != null)
        {
            shipAnim = go.GetComponent<Animation>();
        }
        sun = GameObject.FindGameObjectWithTag("Sun");
        //HUD = transform.parent.gameObject.GetComponent<HUD>();
        playerScript = player.GetComponent<playerController>();

        //****TURN ON FOR TESTING*****\\

        playerScript.addCoins(2000);

        //*******TURN ON FOR TESTING********\\


        playerSpawnPos = GameObject.FindGameObjectWithTag("Player Spawn Pos");
        timeScaleOrig = Time.timeScale;
        waveController = Resources.Load("10 Waves (2Bosses)") as WaveController;

        //ammoCountText[0].text = "";
        //ammoCountText[1].text = "";
        //ammoCountText[2].text = "";
        waveCountText.text = "";
        coinsText.text = playerScript.GetCoins().ToString();

        if (audBackground != null)
        {
            audBackground.volume = levelVolBackground;
            audBackground.clip = levelMusicBackground[Random.Range(0, levelMusicBackground.Length)];
            audBackground.Play();
        }


    }

    void Update()
    {
        UpdateUI();
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

        if (shipAnim != null)
        {
            shipAnim.clip = shipAnim.GetClip("ShipsLanding");
            shipAnim.Play();
        }
        if (sun != null)
        {
            float time = 0;
            Quaternion startValue = transform.rotation;
            Quaternion endValue = Quaternion.Euler(-52, 4, -145);
            while (time < 1.5f)
            {
                sun.transform.rotation = Quaternion.Lerp(startValue, endValue, time / 1.5f);
                time += Time.deltaTime;
                yield return null;
            }
            sun.transform.rotation = endValue;
        }

        if (aud != null)
        {
            audBackground.Stop();
            aud.volume = levelVol;
            aud.clip = levelMusic[Random.Range(0, levelMusic.Length)];
            aud.Play();
        }
        yield return new WaitForSeconds(1.5f);
        instance.updateWave();
        instance.UpdateUI();
    }

    public IEnumerator flash()
    {
        screenFlash.SetActive(true);
        yield return new WaitForSeconds(0.15f);
        screenFlash.SetActive(false);

    }

    public void updateEnemyRemaining(int amount)
    {
        enemiesRemaining += amount;
        //        enemiesRemainingText.text = enemiesRemaining.ToString("F0");

        // check to see if game is over based on enemy count <= 0


        if (enemiesRemaining <= 0)
        {
            if (waveCount >= waveController.numWaves)
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

    public void updateAmmoUI(bool gun = true)
    {
        if (gun)
        {
            ammoText.text = $"{playerScript.weaponList[playerScript.currentWeapon].currentClip} / {playerScript.ammoRemaining}";
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
            if (playerScript.currentWeapon == 0)
            {
                weaponIcons[0].color = new Color(255, 255, 255, 1f);
                weaponIcons[1].color = new Color(255, 255, 255, 0.2f);
                weaponIcons[2].color = new Color(255, 255, 255, 0.2f);
            }
            else if (playerScript.currentWeapon == 1)
            {
                weaponIcons[0].color = new Color(255, 255, 255, 0.2f);
                weaponIcons[1].color = new Color(255, 255, 255, 1f);
                weaponIcons[2].color = new Color(255, 255, 255, 0.2f);
            }
            else if (playerScript.currentWeapon == 2)
            {
                weaponIcons[0].color = new Color(255, 255, 255, 0.2f);
                weaponIcons[1].color = new Color(255, 255, 255, 0.2f);
                weaponIcons[2].color = new Color(255, 255, 255, 1f);
            }
        }
        coinsText.text = playerScript.GetCoins().ToString();
        waveCountText.text = $" {waveCount}";

    }

    public void updateCoinUI()
    {
        coinsText.text = playerScript.GetCoins().ToString();
    }
}