using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    int selection = 0;
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] Toggle fstoggle;
    buttonFunctions funcs;
    [SerializeField] Text[] options;
    // Start is called before the first frame update
    void Start()
    {
        funcs = GetComponent<buttonFunctions>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            if(selection == 0)
            {
                selection = 4;
            }
            else
            {
                selection--;
            }
            Debug.Log(selection);
        }
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (selection == 4)
            {
                selection = 0;
            }
            else
            {
                selection++;
            }
            Debug.Log(selection);
        }
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            if(selection == 2)
            {
                gameManager.instance.sensitivity = gameManager.instance.sensitivity - 10;
                sensitivitySlider.value = sensitivitySlider.value - 10;
            }
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (selection == 2)
            {
                gameManager.instance.sensitivity = gameManager.instance.sensitivity + 10;
                sensitivitySlider.value = sensitivitySlider.value + 10;
            }
        }
        if(Input.GetButtonDown("Submit"))
        {
            switch(selection)
            {
                case 0:
                    funcs.resume();
                    return;
                case 1:
                    funcs.restart();
                    return;
                case 2:
                    return;
                case 3:
                    fstoggle.isOn = !fstoggle.isOn;
                    return;
                case 4:
                    funcs.quit();
                    return;

            }
        }
    }

}
