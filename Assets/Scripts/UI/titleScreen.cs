using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class titleScreen : MonoBehaviour
{
    public Image TeamLogo;
    public Image StartScreen;
    public Button StartButton;
    public GameObject LoadScreen;
    public Image LoadBar;
    public float fadeSpeed = 1f;
    bool isFading = false;



    // Start is called before the first frame update
    void Start()
    {
        StartButton.onClick.AddListener(clicked);
        StartCoroutine(PlayIntro());
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Submit"))
        {
            clicked();
        }
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
            StartScreen.color = new Color(1, 1, 1, startScreenTransparency);
            yield return null;
        }


        isFading = false;
        TeamLogo.enabled = false;

    }

    void clicked()
    {
        LoadScreen.SetActive(true);
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(1);

        while (!asyncOperation.isDone)
        {
            //Output the current progress
            LoadBar.fillAmount = asyncOperation.progress;
            Debug.Log(asyncOperation.progress);
            yield return null;

        }
    }
}
