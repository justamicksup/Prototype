using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    private bool isPlayer;
    //[SerializeField] private SphereCollider rescueShip;
    public DestroyPirateShip[] pirateShips;
    // private void Start()
    // {
    //     if (pirateShips != null)
    //     {
    //         for (int i = 0; i < pirateShips.Length; i++)
    //         {
    //             pirateShips[i].sinkTheShip();
    //         }
    //     }
    // }


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

    void SearchAndDestroy()
    {
        if (pirateShips != null)
        {
            for (int i = 0; i < pirateShips.Length; i++)
            {
                pirateShips[i].sinkTheShip();
            }
        }
    }
}
