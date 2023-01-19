using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    Text fpstext;

    // Start is called before the first frame update
    void Start()
    {
        fpstext = GetComponent<Text>();   
    }

    // Update is called once per frame
    void Update()
    {
        float fps = Mathf.Ceil(1 / Time.unscaledDeltaTime);

        if (fps >= 50)
        {
            fpstext.color = Color.green;
        }
        else if (fps >= 20 && fps <= 49)
        {
            fpstext.color = Color.yellow;
        }
        else if (fps < 20)
        {
            fpstext.color = Color.red;
        }
        fpstext.text = fps + " FPS";
    }
}
