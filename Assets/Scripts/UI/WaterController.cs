using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterController : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //swimming
        //other.SendMessage("Swim");
    }

    private void OnTriggerExit(Collider other)
    {
        //other.SendMessage("StopSwim");
    }
}
