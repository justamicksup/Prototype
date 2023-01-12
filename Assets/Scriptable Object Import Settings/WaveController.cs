using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave Controller", menuName = "Scriptable Objects/Wave/Create Wave")]
public class WaveController : ScriptableObject
{
    //[SerializeField] WaveStruct[] waves;

    public Wave waves;
    
    public List<GameObject> CreateWave()
    {
        int enemyCount = 0;
        List<GameObject> nextWave = new List<GameObject>();

        int max = waves.EnemiesInWave.Count;

        while (enemyCount < max)
        {
            nextWave.Add(waves.EnemiesInWave[Random.Range(0, max)]); 
            enemyCount += 1;
            //     enemyCount += 1;
        }
            // {
            //     nextWave.Add(waves[gameManager.instance.waveCount].wave.EnemiesInWave[Random.Range(0, max)]);
            //     enemyCount += 1;
            // }
        // int max = waves[gameManager.instance.waveCount].wave.EnemiesInWave.Count;
        //
        // while (enemyCount < waves[gameManager.instance.waveCount].enemiesInWave)
        // {
        //     nextWave.Add(waves[gameManager.instance.waveCount].wave.EnemiesInWave[Random.Range(0, max)]);
        //     enemyCount += 1;
        // }

        return nextWave;
    }
}

[System.Serializable]
public struct WaveStruct
{
    public int enemiesInWave;
    public Wave wave;
}