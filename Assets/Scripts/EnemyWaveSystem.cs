using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyWaveSystem : MonoBehaviour
{
    [Header("----- Enemy Info -----")] [SerializeField]
    private Wave[] waves;

    [SerializeField] private GameObject[] miniBoss;
    [SerializeField] private GameObject boss;
    [SerializeField] private float waveDuration = 120.0f; // 2 minutes
    [SerializeField] private float coolDownDuration = 30.0f;
    [SerializeField] internal float difficultyMultiplier = 1.0f;
    [SerializeField] private BoxCollider[] spawnLocations;
    [SerializeField] private float spawnInterval;


    private const float YMin = 0f;
    internal int currentWaveIndex;

    //private bool waveActive;
    private bool _isTriggerSet;
    private bool _isSpawning;
    internal bool isBossSpawned;
    internal bool isMiniBossSpawned;

    private bool _onCoolDown;

    //Current wave enemy tracker
    // private int enemiesToSpawn;
   // [SerializeField] private int _enemiesSpawned = 1;

    //CountDown clock
    private float _waveDurationCountDown;
    private float _coolDownCountDown;


    private void Update()
    {
        if (_isTriggerSet)
        {
            if (!_onCoolDown)
            {
                _waveDurationCountDown -= Time.deltaTime;
                _waveDurationCountDown = Mathf.Max(_waveDurationCountDown, 0);
                gameManager.instance.timer.text = _waveDurationCountDown.ToString("0:00.00");
            }
            else
            {
                _coolDownCountDown -= Time.deltaTime;
                _coolDownCountDown = Mathf.Max(_coolDownCountDown, 0);
                gameManager.instance.timer.text = _coolDownCountDown.ToString("0:00.00 - Rest");
            }
            
            if (_waveDurationCountDown > 0 && !_onCoolDown && !_isSpawning)
            {
                 StartCoroutine(SpawnWaves());
            }

            if (_waveDurationCountDown <= 0)
            {
                _onCoolDown = true;
                StartCoroutine(CoolDown());
                ResetWaveClock();
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

    public void SpawnTheWave()
    {
        gameManager.instance.waveText.SetActive(true);
        _waveDurationCountDown = waveDuration;
        _coolDownCountDown = coolDownDuration;
        _isTriggerSet = true;
    }


    private IEnumerator SpawnWaves()
    {
        _isSpawning = true;
        
        if (spawnLocations != null)
        {

            Vector3 spawnPosition = Box();
          
            
            if (currentWaveIndex < waves.Length)
            {
                
                // Check if it's the last wave
                if (currentWaveIndex == waves.Length - 1)
                {
                    // Debug.Log("BOSS Spawned");

                    Instantiate(boss, spawnPosition, Quaternion.identity);
                    isBossSpawned = true;
                    
                }
                // Check if it's time for a mini boss wave
                else if ((currentWaveIndex + 1) % 5 == 0 && !isMiniBossSpawned)
                {
                    // Debug.Log("MiniBoss Spawned");
                    // Spawn the mini boss
                    foreach (var mini in miniBoss)
                    {
                        Instantiate(mini, spawnPosition, Quaternion.identity);
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

        _isSpawning = false;
    }

    private IEnumerator CoolDown()
    {
       ResetCoolDownClock();
       yield return new WaitForSeconds(coolDownDuration);
       _onCoolDown = false;
        //_enemiesSpawned = 0;
        currentWaveIndex++;
        gameManager.instance.updateWave();
        isMiniBossSpawned = false;
    }
    void ResetCoolDownClock()
    {
        _coolDownCountDown = coolDownDuration;
    }

    void ResetWaveClock()
    {
        _waveDurationCountDown = waveDuration;
    }

    Vector3 Box()
    {
        BoxCollider box = spawnLocations[Random.Range(0, spawnLocations.Length)];


        var bounds = box.bounds;
        Vector3 areaMin = bounds.min;
        Vector3 areaMax = bounds.max;
        float x = Random.Range(areaMin.x, areaMax.x);
        float z = Random.Range(areaMin.z, areaMax.z);
        float y = box.transform.position.y;
        Vector3 spawnPosition = new Vector3(x, y, z);
        if (Physics.Raycast(spawnPosition, Vector3.down, out var hit, Mathf.Infinity))
        {
            spawnPosition.y = Mathf.Max(YMin, hit.point.y);
        }

        return spawnPosition;
    }
}