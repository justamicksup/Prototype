using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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

    public float yMin = 0f;
    [SerializeField] private BoxCollider area;
    [SerializeField] private float timer;
    private bool isTriggerSet;
    private bool isSpawning;
    [SerializeField] private int enemiesToSpawn;
    [SerializeField] private int enemiesSpawned = 0;
    public int currentWave = 0;
    public  float waveDurationCountDown;
    public float coolDownCountDown;
    
    
    private void Update()
    {
        enemiesToSpawn = waves[currentWave].EnemyCount ;

        if (isTriggerSet && !isSpawning && enemiesSpawned <= enemiesToSpawn)
        {
            StartCoroutine(SpawnWaves());
        }

        // Wave Count Down doesn't start until all enemies are spawned
        if (waveActive && enemiesSpawned == enemiesToSpawn)
        {
            waveDurationCountDown -= Time.deltaTime;
             waveDurationCountDown = Mathf.Max(waveDurationCountDown, 0);
            gameManager.instance.timer.text = waveDurationCountDown.ToString("0:00.00");

        }
        //Cool Down Timer
        if (!waveActive && isTriggerSet)
        {
            coolDownCountDown -= Time.deltaTime;
            coolDownCountDown = Mathf.Max(coolDownCountDown, 0);
            gameManager.instance.timer.text = coolDownCountDown.ToString("0:00.00");
        }

        if (currentWave == waves.Length - 1)
        {
            gameManager.instance.timer.text = "";
            Destroy(gameObject);
        }
       
    }

    public void spawnTheWave()
    {
        isTriggerSet = true;
    }


    IEnumerator SpawnWaves()
    {
        isSpawning = true;
        waveActive = true;

        Vector3 areaMin = area.bounds.min;
        Vector3 areaMax = area.bounds.max;
        float x = Random.Range(areaMin.x, areaMax.x);
        float z = Random.Range(areaMin.z, areaMax.z);
        float y = area.transform.position.y;
        Vector3 spawnPosition = new Vector3(x, y, z);
        RaycastHit hit;
        if (Physics.Raycast(spawnPosition, Vector3.down, out hit, Mathf.Infinity))
        {
            spawnPosition.y = Mathf.Max(yMin, hit.point.y);
        }

        if (currentWave < waves.Length)
        {

            //reset clock
            if (waveDurationCountDown <= 0)
            {
                waveDurationCountDown = waveDuration;
            }

            // Check if it's the last wave
            if (currentWave == waves.Length - 1)
            {
                // Spawn the boss
                Instantiate(boss, spawnPosition,
                    Quaternion.LookRotation(gameManager.instance.player.transform.position));

                // Wait for the boss to be defeated


                // Boss defeated, call the ship to come

            }
            // Check if it's time for a mini boss wave
            else if ((currentWave + 1) % 5 == 0)
            {
                // Spawn the mini boss
                Instantiate(miniBoss, spawnPosition, Quaternion.identity);

                // Wait for the mini boss to be defeated

                // Mini boss defeated, continue with next wave
            }
            else
            {
                Instantiate(waves[currentWave].EnemiesInWave[Random.Range(0, waves[currentWave].EnemiesInWave.Count)],
                    spawnPosition, Quaternion.LookRotation(gameManager.instance.player.transform.position));
            }

            if (enemiesSpawned == enemiesToSpawn)
            {
                // Wait for the wave duration
                yield return new WaitForSeconds(waveDuration);

                // Wave is over, set waveActive to false
                waveActive = false;

                //reset clock
                if (coolDownCountDown <= 0)
                {
                    coolDownCountDown = coolDownDuration;
                }

                // Wait for the cool down
                yield return new WaitForSeconds(coolDownDuration);
                enemiesSpawned = 0;
                currentWave++;
                gameManager.instance.updateWave();
            }
            else
            {
                enemiesSpawned++;
                yield return new WaitForSeconds(timer);
            }

            isSpawning = false;

        }
    }
}