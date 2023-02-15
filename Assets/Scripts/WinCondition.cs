using Enemy;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    private bool _isPlayer;
    private bool goToShip = false;
   
    public DestroyPirateShip[] pirateShips;
   


    private void Update()
    {
        if (goToShip)
        {
            gameManager.instance.alertText.text = "Hurry! Get to your ship!";
        }

        if (_isPlayer)
        {
            gameManager.instance.alertText.text = "Press E or F To board your ship";
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

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player") && _isPlayer)
        {
            _isPlayer = false;
        }
    }

    public void SearchAndDestroy()
    {
        if (pirateShips != null)
        {
            foreach (var t in pirateShips)
            {
                t.SinkTheShip();
            }
        }
        goToShip = true;
    }
}
