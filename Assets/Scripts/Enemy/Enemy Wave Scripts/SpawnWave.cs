using System.Collections;
using UnityEngine;

namespace Enemy.Enemy_Wave_Scripts
{
    public class SpawnWave : MonoBehaviour
    {
        [SerializeField] private int enemiesToSpawn;
        [SerializeField] private float timer;
        [SerializeField] private Wave wave;
        [SerializeField] private BoxCollider area;

        private bool _isSpawning;
        private bool _triggerSet;
        [SerializeField] int enemiesSpawned;
        public float yMin;

    
        // Update is called once per frame
        void Update()
        {
            if (_triggerSet && !_isSpawning && enemiesSpawned < enemiesToSpawn)
            {
                StartCoroutine(Spawn());
            }

            if (enemiesSpawned == enemiesToSpawn)
            {
                Destroy(gameObject);
            }
       
        
        }

        public void SpawnTheWave()
        {
            _triggerSet = true;
        }
    
        IEnumerator Spawn()
        {
            var enemy = wave.EnemiesInWave[Random.Range(0, wave.EnemiesInWave.Count - 1)];
            _isSpawning = true;
            var bounds = area.bounds;
            Vector3 areaMin = bounds.min;
            Vector3 areaMax = bounds.max;
            float x = Random.Range(areaMin.x, areaMax.x);
            float z = Random.Range(areaMin.z, areaMax.z);
            float y = area.transform.position.y;
            Vector3 spawnPosition = new Vector3(x, y, z);
            if (Physics.Raycast(spawnPosition, Vector3.down, out var hit, Mathf.Infinity))
            {
                spawnPosition.y = Mathf.Max(yMin, hit.point.y);
            }
            Instantiate(enemy, spawnPosition, Quaternion.identity);

            enemiesSpawned++;
            yield return new WaitForSeconds(timer);
            _isSpawning = false;
        }
    }
}