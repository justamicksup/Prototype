using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiusGOActivator : MonoBehaviour
{
    public float activationRadius = 10.0f;
    private Transform playerTransform;

    private void Start()
    {
        playerTransform = gameManager.instance.player.transform;
    }

    private void Update()
    {
        foreach (Transform child in transform)
        {
            float distance = Vector3.Distance(playerTransform.position, child.transform.position);
         
            if (distance < activationRadius)
            {
                child.gameObject.SetActive(true);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        
        }
    }
}
