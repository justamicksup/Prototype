using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TitleFade : MonoBehaviour
    {
        public Image teamLogo;
        public Image startScreen;
        public float fadeSpeed = 1f;
        //bool isFading = false;
    
    
    
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(PlayIntro());
        }

        IEnumerator PlayIntro()
        {
            //isFading = true;
            float teamLogoTransparency = 1;
            float startScreenTransparency = 0;

            while (teamLogoTransparency > 0)
            {
                teamLogoTransparency -= Time.deltaTime * fadeSpeed;
                startScreenTransparency += Time.deltaTime * fadeSpeed;
            
                teamLogo.color = new Color(1, 1, 1, teamLogoTransparency);
                startScreen.color = new Color(1, 1, 1, startScreenTransparency);
                yield return null;
            }
        

            //isFading = false;
            teamLogo.enabled = false;

        }
    }
}
