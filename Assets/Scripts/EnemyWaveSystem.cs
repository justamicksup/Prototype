using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveSystem : MonoBehaviour
{
    
   
   // public int enemiesInWave;
    public Wave[] waves;
    public GameObject miniBoss;
    public GameObject boss;
    public float waveDuration = 10.0f; // 2 minutes
    public float coolDownDuration = 5.0f;
    public float difficultyMultiplier = 1.0f;

    public int currentWaveIndex = 0;
    private bool waveActive;
    private int wallet;

    void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
       
        for (int i = 0; i < waves.Length; i++)
        {
            currentWaveIndex = i;
            waveActive = true;

            // Check if it's the last wave
            if (i == waves.Length - 1)
            {
                // Spawn the boss
                Instantiate(boss, transform.position, Quaternion.identity);

                // Wait for the boss to be defeated
                

                // Boss defeated, call the ship to come
               

                break;
            }
           
            // Spawn the enemies in the current wave
            for (int j = 0; j < waves[i].EnemyCount; j++)
            {
                Instantiate(waves[i].EnemiesInWave[Random.Range(0,waves[i].EnemiesInWave.Count)], transform.position, Quaternion.identity);
            }
            
            // Wait for the wave duration
            yield return new WaitForSeconds(waveDuration);

            // Wave is over, set waveActive to false
            waveActive = false;

            // Wait for the cool down
            yield return new WaitForSeconds(coolDownDuration);

            // Check if it's time for a mini boss wave
            if ((i + 1) % 5 == 0)
            {
                // Spawn the mini boss
                Instantiate(miniBoss, transform.position, Quaternion.identity);

                // Wait for the mini boss to be defeated
                
                // Mini boss defeated, continue with next wave
            }
        }
    }
    
}
