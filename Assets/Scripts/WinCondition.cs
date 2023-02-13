using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    private bool isPlayer;
    //[SerializeField] private SphereCollider rescueShip;
    private void Update()
    {
        if (isPlayer)
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.F))
            {
                gameManager.instance.youWin();
            }
        }
       
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayer = true;
           
        }
    }
   
}
