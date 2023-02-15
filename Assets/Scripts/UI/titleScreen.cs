using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using UnityEngine.Audio;

public class titleScreen : MonoBehaviour
{
    public Image TeamLogo;
    public GameObject StartScreen;
    public Image StartMenu;
    public Button StartButton;
    public GameObject LevelSelect;
    public Button ScaryScene;
    public Button CaveScene;
    public GameObject LoadScreen;
    public Image LoadBar;
    public float fadeSpeed = 1f;
    [SerializeField] AudioMixer mixer;
    bool isFading = false;
    int sceneNdx;



    // Start is called before the first frame update
    void Start()
    {
        StartButton.onClick.AddListener(clickedStart);
        StartCoroutine(PlayIntro());

        mixer.SetFloat("Master", Mathf.Log10(PlayerPrefs.GetFloat("master")) * 20);
        mixer.SetFloat("Music", Mathf.Log10(PlayerPrefs.GetFloat("music")) * 20);
        mixer.SetFloat("SFX", Mathf.Log10(PlayerPrefs.GetFloat("SFX")) * 20);
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator PlayIntro()
    {
        isFading = true;
        float teamLogoTransparency = 1;
        float startScreenTransparency = 0;

        while (teamLogoTransparency > 0)
        {
            teamLogoTransparency -= Time.deltaTime * fadeSpeed;
            startScreenTransparency += Time.deltaTime * fadeSpeed;

            TeamLogo.color = new Color(1, 1, 1, teamLogoTransparency);
            StartMenu.color = new Color(1, 1, 1, startScreenTransparency);
            yield return null;
        }


        isFading = false;
        TeamLogo.enabled = false;

    }

    void clickedStart()
    {
        LoadScreen.SetActive(true);
        StartCoroutine(StartGame(1));
    }

    IEnumerator StartGame(int index)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(index);

        while (!asyncOperation.isDone)
        {
            //Output the current progress
            LoadBar.fillAmount = asyncOperation.progress;
            Debug.Log(asyncOperation.progress);
            yield return null;

        }
    }
}
