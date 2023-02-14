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

    [SerializeField] GameObject[] miniBoss;
    [SerializeField] GameObject boss;
    [SerializeField] float waveDuration = 120.0f; // 2 minutes
    [SerializeField] float coolDownDuration = 30.0f;
    [SerializeField] internal float difficultyMultiplier = 1.0f;
    [SerializeField] private BoxCollider[] spawnLocations;
    [SerializeField] private float spawnInterval;


    private float yMin = 0f;
    internal int currentWaveIndex = 0;

    //private bool waveActive;
    private bool isTriggerSet;
    private bool isSpawning;
    internal bool isBossSpawned;
    internal bool isMiniBossSpawned;

    private bool onCoolDown;

    //Current wave enemy tracker
    // private int enemiesToSpawn;
    private int enemiesSpawned = 1;

    //CountDown clock
    private float waveDurationCountDown;
    private float coolDownCountDown;


    private void Update()
    {
        if (isTriggerSet)
        {
            if (!onCoolDown)
            {
                waveDurationCountDown -= Time.deltaTime;
                waveDurationCountDown = Mathf.Max(waveDurationCountDown, 0);
                gameManager.instance.timer.text = waveDurationCountDown.ToString("0:00.00");
            }
            else
            {
                coolDownCountDown -= Time.deltaTime;
                coolDownCountDown = Mathf.Max(coolDownCountDown, 0);
                gameManager.instance.timer.text = coolDownCountDown.ToString("0:00.00 - Rest");
            }
            
            if (waveDurationCountDown > 0 && !onCoolDown && !isSpawning)
            {
                 StartCoroutine(SpawnWaves());
            }

            if (waveDurationCountDown <= 0)
            {
                onCoolDown = true;
                StartCoroutine(CoolDown());
                resetWaveClock();
            }
        }
        
        //After Boss is Spawned, Destroy spawn System, get rid of countdown
        if (isBossSpawned)
        {
            gameManager.instance.timer.text = "  Boss Wave";
            gameManager.instance.waveText.SetActive(false);
            Destroy(gameObject);
        }

        if (isMiniBossSpawned)
        {
            gameManager.instance.timer.text = "  Mini Boss ";
            gameManager.instance.waveText.SetActive(false);
        }
    }

    public void spawnTheWave()
    {
        gameManager.instance.waveText.SetActive(true);
        waveDurationCountDown = waveDuration;
        coolDownCountDown = coolDownDuration;
        isTriggerSet = true;
    }


    IEnumerator SpawnWaves()
    {
        isSpawning = true;
        
        if (spawnLocations != null)
        {

            Vector3 spawnPosition = box();
          
            
            if (currentWaveIndex < waves.Length)
            {
                
                // Check if it's the last wave
                if (currentWaveIndex == waves.Length - 1)
                {
                    Debug.Log("BOSS Spawned");

                    Instantiate(boss, spawnPosition, Quaternion.identity);
                    isBossSpawned = true;
                    
                }
                // Check if it's time for a mini boss wave
                else if ((currentWaveIndex + 1) % 5 == 0 && !isMiniBossSpawned)
                {
                    Debug.Log("MiniBoss Spawned");
                    // Spawn the mini boss
                    for (int i = 0; i < miniBoss.Length; i++)
                    {
                        Instantiate(miniBoss[i], spawnPosition, Quaternion.identity);
                    }
                    
                    isMiniBossSpawned = true;
                }
                else
                {
                    Instantiate(
                        waves[currentWaveIndex]
                            .EnemiesInWave[Random.Range(0, waves[currentWaveIndex].EnemiesInWave.Count)],
                        spawnPosition, Quaternion.identity);
                }
                
                yield return new WaitForSeconds(spawnInterval);

            
            }
        }

        isSpawning = false;
    }

    IEnumerator CoolDown()
    {
       resetCoolDownClock();
       yield return new WaitForSeconds(coolDownDuration);
       onCoolDown = false;
        enemiesSpawned = 0;
        currentWaveIndex++;
        gameManager.instance.updateWave();
        isMiniBossSpawned = false;
    }
    void resetCoolDownClock()
    {
        coolDownCountDown = coolDownDuration;
    }

    void resetWaveClock()
    {
        waveDurationCountDown = waveDuration;
    }

    Vector3 box()
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

        return spawnPosition;
    }
}