using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class destroyObstacle : MonoBehaviour, actionObject
{
    [SerializeField] GameObject obstacleLeft;
    [SerializeField] GameObject obstacleRight;
    [SerializeField] Collider box;
    [SerializeField] NavMeshObstacle obstacle;
    [SerializeField] int obstacleCost;
    private Transform target = null;

    
    bool hasCoins;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            target = other.transform;
        }
        if (gameManager.instance.playerScript.GetCoins() >= obstacleCost)
        {
            hasCoins = true;
        }
        else
            hasCoins = false;
        //UI to display cost and action keys
        Debug.Log("PLAYER");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            target = null;
        }
        //remove UI
        Debug.Log("NO PLAYER");
    }

    public void primaryAction()
    {
        if (hasCoins)
        {
            //This should destroy whatever door or object we use for blockades
            //even if a single object.
            if (obstacleLeft != null)
            {
                Destroy(obstacleLeft.gameObject);
            }
            if (obstacleRight != null)
            {
                Destroy(obstacleRight.gameObject);
            }
            box.enabled = false;
            obstacle.enabled = false;
        }
        else
        {
            //need UI here
            Debug.Log("Not enough coins");
        }
    }
    public void secondaryAction()
    {
        //no secondary action for doors
        primaryAction();
    }
}
