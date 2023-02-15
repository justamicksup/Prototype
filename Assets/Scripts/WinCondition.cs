using Enemy;
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

    public void SearchAndDestroy()
    {
        if (pirateShips != null)
        {
            foreach (var t in pirateShips)
            {
                t.SinkTheShip();
            }
        }
    }
}
