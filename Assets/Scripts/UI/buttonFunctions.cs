using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    private CharacterController controller;
    public void resume()
    {
        gameManager.instance.unpauseGame();
        gameManager.instance.isPaused = !gameManager.instance.isPaused;
    }

    public void restart()
    {
        gameManager.instance.unpauseGame();
        gameManager.instance.LoadScreen.SetActive(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void quit()
    {
        Application.Quit();
    }

    public void fullscreen(Toggle fs)
    {
        if(fs.isOn)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            Debug.Log("fullscreen");
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            Debug.Log("windowed");
        }
    }

    public void sensitivity(Slider slider)
    {
        gameManager.instance.sensitivity = (int)slider.value;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void open(GameObject gameObject)
    {
        gameObject.SetActive(true);
    }

    public void close(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }    
}
