using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class settingsMenu : MonoBehaviour
{

    [SerializeField] Slider sensitivitySlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider SFXSlider;
    [SerializeField] Slider masterSlider;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        sensitivity = sensitivitySlider.value;
        music = musicSlider.value;
        SFX = SFXSlider.value;
        master = masterSlider.value;
    }

    public void Save()
    {
        PlayerPrefs.SetFloat("sensitivity", sensitivity);
        PlayerPrefs.SetFloat("music", music);
        PlayerPrefs.SetFloat("sfx", SFX);
        PlayerPrefs.SetFloat("master", master);
    }
}
