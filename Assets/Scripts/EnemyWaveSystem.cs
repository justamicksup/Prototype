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
    [Header("----- Enemy Info -----")] [SerializeField]
    Wave[] waves;

    [SerializeField] GameObject miniBoss;
    [SerializeField] GameObject boss;
    [SerializeField] float waveDuration = 120.0f; // 2 minutes
    [SerializeField] float coolDownDuration = 30.0f;
    [SerializeField] internal float difficultyMultiplier = 1.0f;
    [SerializeField] private BoxCollider[] spawnLocations;
    [SerializeField] private float spawnInterval;


    private float yMin = 0f;
    internal int currentWaveIndex = 0;

    private bool waveActive;
    private bool isTriggerSet;
    private bool isSpawning;
    internal bool isBossSpawned;

    //Current wave enemy tracker
    private int enemiesToSpawn;
    private int enemiesSpawned = 1;

    //CountDown clock
    private float waveDurationCountDown;
    private float coolDownCountDown;


    private void Update()
    {
        //set current wave enemies that need to spawn
        enemiesToSpawn = waves[currentWaveIndex].EnemyCount;

        if (enemiesSpawned < enemiesToSpawn)
        {
            waveActive = true;
        }

        if (isTriggerSet && !isSpawning && waveActive)
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

        //After Boss is Spawned, Destroy spawn System, get rid of countdown
        if (isBossSpawned)
        {
            gameManager.instance.timer.text = "  Boss Wave";
            gameManager.instance.waveText.SetActive(false);
            Destroy(gameObject);
        }
    }

    public void spawnTheWave()
    {
        gameManager.instance.waveText.SetActive(true);
        isTriggerSet = true;
    }


    IEnumerator SpawnWaves()
    {
        isSpawning = true;
        //waveActive = true;

        if (spawnLocations != null)
        {
            BoxCollider box = spawnLocations[Random.Range(0, spawnLocations.Length)];


            Vector3 areaMin = box.bounds.min;
            Vector3 areaMax = box.bounds.max;
            float x = Random.Range(areaMin.x, areaMax.x);
            float z = Random.Range(areaMin.z, areaMax.z);
            float y = box.transform.position.y;
            Vector3 spawnPosition = new Vector3(x, y, z);
            RaycastHit hit;
            if (Physics.Raycast(spawnPosition, Vector3.down, out hit, Mathf.Infinity))
            {
                spawnPosition.y = Mathf.Max(yMin, hit.point.y);
            }


            if (currentWaveIndex < waves.Length)
            {
                //reset clock
                if (waveDurationCountDown <= 0)
                {
                    waveDurationCountDown = waveDuration;
                }

                // Check if it's the last wave
                if (currentWaveIndex == waves.Length - 1)
                {
                    Debug.Log("BOSS Spawned");
                    
                    Instantiate(boss, spawnPosition, Quaternion.LookRotation(gameManager.instance.player.transform.position));
                    isBossSpawned = true;
                    
                    // Wait for the boss to be defeated


                    // Boss defeated, call the ship to come
                    
                }
                // Check if it's time for a mini boss wave
                else if ((currentWaveIndex + 1) % 5 == 0)
                {
                    // Spawn the mini boss
                    Instantiate(miniBoss, spawnPosition, Quaternion.identity);
                    
                }
                else
                {
                    Instantiate(
                        waves[currentWaveIndex]
                            .EnemiesInWave[Random.Range(0, waves[currentWaveIndex].EnemiesInWave.Count)],
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
                    currentWaveIndex++;
                    gameManager.instance.updateWave();
                }
                else
                {
                    enemiesSpawned++;
                    yield return new WaitForSeconds(spawnInterval);
                }
            }
        }

        isSpawning = false;
    }
}