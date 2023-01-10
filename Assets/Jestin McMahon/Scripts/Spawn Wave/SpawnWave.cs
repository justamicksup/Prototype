using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWave : MonoBehaviour
{
    public WaveController nextWave;
    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private float xPos;
    [SerializeField] private float zPos;


    [SerializeField] private int enemyCount;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemy(.1f));
        gameManager.instance.updateWave(1);
    }

    // Update is called once per frame
    void Update()
    {
    }

    //ReSharper disable Unity.PerformanceAnalysis
    IEnumerator SpawnEnemy(float interval)
    {
        var wave = nextWave.CreateWave();
        foreach (var enemy in wave)
        {
            // Randomize x and z offset
            float xOffset = Random.Range(-1, 1f);
            float zOffset = Random.Range(-1, 1f);

            //Spawn Position
            Vector3 position = spawnPoint.transform.position;

            //Spawn Point Local Scale
            Vector3 scale = spawnPoint.transform.localScale;

            //Max bounds of the Spawn Point
            Vector3 max = spawnPoint.GetComponent<MeshFilter>().mesh.bounds.max;

            // X and Z position offset within the max bounds
            xPos = position.x + max.x * xOffset * scale.x;
            zPos = position.z + max.z * zOffset * scale.z;

            //Generate GameObject
            Instantiate(enemy, new Vector3(xPos, 0, zPos), Quaternion.identity);
            yield return new WaitForSeconds(interval);
        }
    }
}