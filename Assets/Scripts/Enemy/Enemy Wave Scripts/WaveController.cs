using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Enemy_Wave_Scripts
{
    [CreateAssetMenu(fileName = "Wave Controller", menuName = "Scriptable Objects/Wave/Create Wave")]
    public class WaveController : ScriptableObject
    {
        [SerializeField] WaveStruct[] waves;


        public List<GameObject> CreateWave()
        {
            int index = gameManager.instance.waveCount - 1;
            int enemyPoolSize = waves[index].wave.EnemiesInWave.Count - 1;
        

            var enemyCount = 0;
            List<GameObject> nextWave = new List<GameObject>();

        
            while (enemyCount < waves[index].enemiesInWave)
            {
                nextWave.Add(waves[index].wave.EnemiesInWave[Random.Range(0, enemyPoolSize)]);
                enemyCount += 1;
            }

            return nextWave;
        }
    }

    [System.Serializable]
    public struct WaveStruct
    {
    
        public int enemiesInWave;
        public Wave wave;
    }
}