using System.Collections;
using Enemy.Enemy_Wave_Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

public class AreaSpawners : MonoBehaviour
{
    //[SerializeField] int enemiesToSpawn;
    [SerializeField] float interval;
    [SerializeField] private Wave wave;
    [SerializeField] private BoxCollider area;
    [SerializeField] float spawnStopDelay;
    bool isSpawning;
    bool playerInRange;
   // int enemiesSpawned;
    public float yMin = 0f;
    
    // Update is called once per frame
    void Update()
    {
        if(playerInRange && !isSpawning)
        {
            StartCoroutine(spawn());
        }

       
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            StartCoroutine(delay());
        }
    }

    IEnumerator spawn()
    {
        var _enemy = wave.EnemiesInWave[Random.Range(0, wave.EnemiesInWave.Count - 1)];
        isSpawning = true;
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
        Instantiate(_enemy, spawnPosition, Quaternion.identity);

      //  enemiesSpawned++;
        yield return new WaitForSeconds(interval);
        isSpawning = false;
    }

    IEnumerator delay()
    {
        yield return new WaitForSeconds(spawnStopDelay);
        playerInRange = false;
    }
}
