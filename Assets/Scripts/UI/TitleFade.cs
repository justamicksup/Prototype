using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleFade : MonoBehaviour
{
    public Image TeamLogo;
    public Image StartScreen;
    public float fadeSpeed = 1f;
    bool isFading;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PlayIntro());
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
            StartScreen.color = new Color(1, 1, 1, startScreenTransparency);
            yield return null;
        }
        

        isFading = false;
        TeamLogo.enabled = false;

    }
}
