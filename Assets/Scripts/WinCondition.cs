using UnityEngine;

public class WinCondition : MonoBehaviour
{
    private bool _isPlayer;
   
    public DestroyPirateShip[] pirateShips;
   


    private void Update()
    {
        if (_isPlayer)
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
            _isPlayer = true;
           
        }
    }

    // void SearchAndDestroy()
    // {
    //     if (pirateShips != null)
    //     {
    //         for (int i = 0; i < pirateShips.Length; i++)
    //         {
    //             pirateShips[i].sinkTheShip();
    //         }
    //     }
    // }
}
