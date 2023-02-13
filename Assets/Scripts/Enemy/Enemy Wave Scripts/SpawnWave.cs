using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWave : MonoBehaviour
{
    [SerializeField] private int enemiesToSpawn;
    [SerializeField] private float timer;
    [SerializeField] private Wave wave;
    [SerializeField] private BoxCollider area;
    
    bool isSpawing;
    bool triggerSet;
    int enemiesSpawned;
    public float yMin = 0f;

    
    // Update is called once per frame
    void Update()
    {
        if (triggerSet && !isSpawing && enemiesSpawned < enemiesToSpawn)
        {
            StartCoroutine(spawn());
        }

        if (enemiesSpawned == enemiesToSpawn)
        {
            Destroy(gameObject);
        }
       
        
    }

    public void spawnTheWave()
    {
        triggerSet = true;
    }
    
    IEnumerator spawn()
    {
        var _enemy = wave.EnemiesInWave[Random.Range(0, wave.EnemiesInWave.Count - 1)];
        isSpawing = true;
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

        enemiesSpawned++;
        yield return new WaitForSeconds(timer);
        isSpawing = false;
    }
}