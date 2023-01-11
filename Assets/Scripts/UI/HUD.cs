using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    [Header("Pause")]
    public GameObject pauseMenu;
    [SerializeField] Button Resume;
    [SerializeField] Button Settings;
    [SerializeField] Button Quit;

    //[Header("Settings")] (for later)

    // Start is called before the first frame update
    void Start()
    {
        Resume.onClick.AddListener(resume);
        Resume.onClick.AddListener(settings);
        Resume.onClick.AddListener(quit);
    }

    // Update is called once per frame
    void Update()
    {
        HP.fillAmount = Mathf.Clamp(gameManager.instance.playerScript.getHP() / 100f, 0, 1f);
        stamina.fillAmount = Mathf.Clamp(gameManager.instance.playerScript.getStamina() / 1f, 0, 1f);
        //ammo.text = gameManager.instance.playerScript.getAmmo().ToString();
        coins.text = gameManager.instance.playerScript.GetCoins().ToString();
    }

    void resume()
    {
        gameManager.instance.unpauseGame();
    }

    void settings()
    {

    }

    void quit()
    {
        Application.Quit();
    }
}
