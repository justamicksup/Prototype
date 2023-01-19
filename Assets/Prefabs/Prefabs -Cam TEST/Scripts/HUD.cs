using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] Image HP;
    [SerializeField] Text ammo1;
    [SerializeField] Text ammo2;
    [SerializeField] Text ammo3;
    [SerializeField] Image stamina;
    [SerializeField] Text coins;
    [SerializeField] Text wave;

    [Header("Pause")]
    public GameObject pauseMenu;
    [SerializeField] Button Resume;
    [SerializeField] Button Settings;
    [SerializeField] Button Quit;
    [SerializeField] GameObject settingsMenu;

    //[Header("Settings")] (for later)

    // Start is called before the first frame update
    void Start()
    {
        // Resume.onClick.AddListener(resume);
        // Resume.onClick.AddListener(settings);
        // Resume.onClick.AddListener(quit);
    }

    // Update is called once per frame
    void Update()
    {
        //hp and stamina bars
        // HP.fillAmount = Mathf.Clamp(gameManager.instance.playerScript.getHP() / 100f, 0, 1f);
        // stamina.fillAmount = Mathf.Clamp(gameManager.instance.playerScript.getStamina() / 1f, 0, 1f);


        //display ammo of weapons;
        // if(gameManager.instance.playerScript.weapons[0] != null)
        // {
        //     ammo1.text = gameManager.instance.playerScript.weapons[0].ammoRemaining.ToString();
        // }
        // if (gameManager.instance.playerScript.weapons[1] != null)
        // {
        //     ammo2.text = gameManager.instance.playerScript.weapons[1].ammoRemaining.ToString();
        // }
        // if (gameManager.instance.playerScript.weapons[2] != null)
        // {
        //     ammo3.text = gameManager.instance.playerScript.weapons[2].ammoRemaining.ToString();
        // }
        //
        // if (HP.fillAmount == 0)
        // {
        //     gameManager.instance.youLose();
        // }
        // coins.text = gameManager.instance.playerScript.GetCoins().ToString();
        // wave.text = $"Wave {gameManager.instance.waveCount}";
    }

    // public void resume()
    // {
    //     gameManager.instance.unpauseGame();
    // }
    //
    //
    // public void quit()
    // {
    //     Application.Quit();
    // }
    //
    // public void restart()
    // {
    //     gameManager.instance.unpauseGame();
    //     SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    //     gameManager.instance.playerScript.respawnPlayer();
    //     
    // }
    // public void respawn()
    // {
    //     gameManager.instance.playerScript.respawnPlayer();
    //     gameManager.instance.unpauseGame();
    // }
}
