using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Wave Controller", menuName = "Scriptable Objects/Wave/Create Wave")]
public class WaveController : ScriptableObject
{
    [SerializeField] WaveStruct[] waves;

    public int numWaves
    {
        get { return waves.Length; }
    }
    //public Wave waves;


    public List<GameObject> CreateWave()
    {
        int index = gameManager.instance.waveCount - 1;
        int EnemyPoolSize = waves.Length - 1;

        

        var enemyCount = 0;
        List<GameObject> nextWave = new List<GameObject>();

        
        while (enemyCount < waves[index].enemiesInWave)
        {
            nextWave.Add(waves[index].wave.EnemiesInWave[Random.Range(0, EnemyPoolSize)]);
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