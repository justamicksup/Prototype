using UnityEngine;

namespace Enemy.Enemy_Wave_Scripts
{
    public class TriggerEnemyWave : MonoBehaviour
    {
        private bool _playerInRange;
        [SerializeField] GameObject chestTrigger;
       // [SerializeField] private EnemyWaveSystem enemySpawnPoint;
    
        // Update is called once per frame
        void Update()
        {
            if (_playerInRange && !gameManager.instance.isPaused)
            {

                if (Input.GetButtonDown("Action"))
                {
                    gameManager.instance.alertText.text = "";
                    Destroy(chestTrigger); 
                    gameManager.instance.StartGame();
               
                }
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInRange = true;
                gameManager.instance.alertText.text = "E: Take Blackbeard's Gold";
                gameManager.instance.updateKey();
            }
        }
        public void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInRange = false;
                gameManager.instance.alertText.text = "";
            }
        }


    }
}
