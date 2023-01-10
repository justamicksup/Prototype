using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "Scriptable Objects/Wave/Create Wave")]
public class WaveController : ScriptableObject
{
    public WaveStruct[] waves;

    public List<GameObject> CreateWave()
    {
        int enemyCount = 0;
        List<GameObject> nextWave = new List<GameObject>();

        int max = waves[gameManager.instance.waveCount].wave.EnemiesInWave.Count;

        while (enemyCount < waves[gameManager.instance.waveCount].enemiesInWave)
        {
            nextWave.Add(waves[gameManager.instance.waveCount].wave.EnemiesInWave[Random.Range(0, max)]);
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