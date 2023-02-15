using Player;
using UnityEngine;
using UnityEngine.AI;

public class destroyObstacle : MonoBehaviour, IActionObject
{
    [SerializeField] GameObject obstacleLeft;
    [SerializeField] GameObject obstacleRight;
    [SerializeField] Collider box;
    [SerializeField] NavMeshObstacle obstacle;
    [SerializeField] int obstacleCost;
    [SerializeField] string obstacleText;
    private Transform target = null;

    
    bool hasCoins;
    bool playerInRange;

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && obstacleLeft != null && obstacleRight != null)
        {
            gameManager.instance.alertText.text = $"F: {obstacleText} ({obstacleCost})";
            target = gameManager.instance.player.transform;
            playerInRange= true;
            if (obstacleCost <= gameManager.instance.playerScript.GetCoins())
            {
                hasCoins = true;
            }
            if (playerInRange && Input.GetButton("Submit") && hasCoins)
            {
                PrimaryAction();
                gameManager.instance.playerScript.addCoins(-obstacleCost);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            target = null;
            hasCoins = false;
            playerInRange= false;
        }
        gameManager.instance.alertText.text = "";
        // Debug.Log("NO PLAYER");
    }

    public void PrimaryAction()
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
            gameManager.instance.alertText.text = "";
            box.enabled = false;
            obstacle.enabled = false;
        }
        else
        {
            //need UI here
            // Debug.Log("Not enough coins");
        }
    }
    public void SecondaryAction()
    {
        //no secondary action for doors
        PrimaryAction();
    }
}
