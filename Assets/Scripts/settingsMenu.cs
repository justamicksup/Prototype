using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class settingsMenu : MonoBehaviour
{

    [SerializeField] Slider sensitivitySlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider SFXSlider;
    [SerializeField] Slider masterSlider;
    [SerializeField] AudioMixer mixer;
    [SerializeField] Toggle fullscreen;

    float sensitivity;
    float music;
    float SFX;
    float master;

    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.HasKey("sensitivity"))
        {
            sensitivity = PlayerPrefs.GetFloat("sensitivity");
            sensitivitySlider.value = sensitivity;
        }

        if(PlayerPrefs.HasKey("music"))
        {
            music = PlayerPrefs.GetFloat("music");
            musicSlider.value = music;
        }

        if(PlayerPrefs.HasKey("SFX"))
        {
            SFX = PlayerPrefs.GetFloat("SFX");
            SFXSlider.value = SFX;
        }
        if(PlayerPrefs.HasKey("master"))
        {
            master = PlayerPrefs.GetFloat("master");
            masterSlider.value = master;
        }

        if(Screen.fullScreenMode == FullScreenMode.FullScreenWindow)
        {
            fullscreen.enabled = true;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        sensitivity = sensitivitySlider.value;
        music = musicSlider.value;
        SFX = SFXSlider.value;
        master = masterSlider.value;

        mixer.SetFloat("Master", Mathf.Log10(master) * 20);
        mixer.SetFloat("Music", Mathf.Log10(music) * 20);
        mixer.SetFloat("SFX", Mathf.Log10(SFX) * 20);
    }

    public void Save()
    {
        PlayerPrefs.SetFloat("sensitivity", sensitivity);
        PlayerPrefs.SetFloat("music", music);
        PlayerPrefs.SetFloat("sfx", SFX);
        PlayerPrefs.SetFloat("master", master);

        if(gameManager.instance != null)
        {
            gameManager.instance.sensitivity = (int)sensitivity;
        }
    }
}
