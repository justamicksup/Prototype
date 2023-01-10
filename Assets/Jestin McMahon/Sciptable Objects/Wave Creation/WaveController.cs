using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "Scriptable Objects/Wave/Create Wave")]
public class WaveController : ScriptableObject
{
    public int enemiesInWave;
    public Wave wave;

    public List<GameObject> CreateWave()
    {
        int enemyCount = 0;
        List<GameObject> nextWave = new List<GameObject>();

        int max = wave.EnemiesInWave.Count;

        while (enemyCount < enemiesInWave)
        {
            nextWave.Add(wave.EnemiesInWave[Random.Range(0, max)]);
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