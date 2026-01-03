using UnityEngine;
using System.Collections.Generic;

namespace DarkLegend.Enemy
{
    /// <summary>
    /// Spawns enemies in waves
    /// Sinh quái theo đợt
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        public List<EnemyData> enemyTypes = new List<EnemyData>();
        public int enemiesPerWave = 5;
        public float spawnRadius = 10f;
        public float timeBetweenWaves = 30f;
        public int maxActiveEnemies = 20;
        
        [Header("Wave Settings")]
        public bool autoStart = true;
        public int currentWave = 0;
        public int maxWaves = 10; // 0 = infinite
        
        [Header("Spawn Points")]
        public Transform[] spawnPoints;
        public bool useRandomPositions = true;
        
        // Private variables
        private List<GameObject> activeEnemies = new List<GameObject>();
        private float waveTimer = 0f;
        private bool isSpawning = false;
        
        private void Start()
        {
            if (autoStart)
            {
                StartSpawning();
            }
        }
        
        private void Update()
        {
            if (!isSpawning) return;
            
            // Clean up dead enemies
            activeEnemies.RemoveAll(enemy => enemy == null);
            
            // Check if wave is complete
            if (activeEnemies.Count == 0 && currentWave > 0)
            {
                waveTimer -= Time.deltaTime;
                
                if (waveTimer <= 0f)
                {
                    SpawnNextWave();
                }
            }
        }
        
        /// <summary>
        /// Start spawning enemies
        /// Bắt đầu sinh quái
        /// </summary>
        public void StartSpawning()
        {
            isSpawning = true;
            currentWave = 0;
            SpawnNextWave();
        }
        
        /// <summary>
        /// Stop spawning enemies
        /// Dừng sinh quái
        /// </summary>
        public void StopSpawning()
        {
            isSpawning = false;
        }
        
        /// <summary>
        /// Spawn next wave of enemies
        /// Sinh đợt quái tiếp theo
        /// </summary>
        private void SpawnNextWave()
        {
            if (maxWaves > 0 && currentWave >= maxWaves)
            {
                isSpawning = false;
                Debug.Log("All waves completed!");
                return;
            }
            
            currentWave++;
            waveTimer = timeBetweenWaves;
            
            int enemiesToSpawn = Mathf.Min(enemiesPerWave, maxActiveEnemies - activeEnemies.Count);
            
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                SpawnEnemy();
            }
            
            Debug.Log($"Wave {currentWave} started! Spawned {enemiesToSpawn} enemies.");
        }
        
        /// <summary>
        /// Spawn a single enemy
        /// Sinh một quái
        /// </summary>
        private void SpawnEnemy()
        {
            if (enemyTypes.Count == 0)
            {
                Debug.LogWarning("No enemy types defined in spawner!");
                return;
            }
            
            // Select random enemy type
            EnemyData enemyData = enemyTypes[Random.Range(0, enemyTypes.Count)];
            
            if (enemyData == null || enemyData.enemyPrefab == null)
            {
                Debug.LogWarning("Enemy data or prefab is null!");
                return;
            }
            
            // Get spawn position
            Vector3 spawnPosition = GetSpawnPosition();
            
            // Spawn enemy
            GameObject enemy = Instantiate(enemyData.enemyPrefab, spawnPosition, Quaternion.identity);
            
            // Setup enemy
            EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();
            if (enemyBase != null)
            {
                enemyBase.enemyData = enemyData;
            }
            
            // Scale enemy stats with wave
            EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                enemyStats.level += (currentWave - 1); // Increase level with waves
                enemyStats.InitializeFromData(enemyData);
            }
            
            // Add to active list
            activeEnemies.Add(enemy);
        }
        
        /// <summary>
        /// Get spawn position
        /// Lấy vị trí spawn
        /// </summary>
        private Vector3 GetSpawnPosition()
        {
            if (useRandomPositions || spawnPoints == null || spawnPoints.Length == 0)
            {
                // Random position around spawner
                Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
                return transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);
            }
            else
            {
                // Use defined spawn points
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                return spawnPoint.position;
            }
        }
        
        /// <summary>
        /// Manually spawn a specific enemy type
        /// Sinh thủ công một loại quái cụ thể
        /// </summary>
        public GameObject SpawnSpecificEnemy(EnemyData enemyData, Vector3 position)
        {
            if (enemyData == null || enemyData.enemyPrefab == null)
            {
                Debug.LogWarning("Cannot spawn enemy: data or prefab is null");
                return null;
            }
            
            GameObject enemy = Instantiate(enemyData.enemyPrefab, position, Quaternion.identity);
            
            EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();
            if (enemyBase != null)
            {
                enemyBase.enemyData = enemyData;
            }
            
            activeEnemies.Add(enemy);
            return enemy;
        }
        
        /// <summary>
        /// Clear all active enemies
        /// Xóa tất cả quái đang hoạt động
        /// </summary>
        public void ClearAllEnemies()
        {
            foreach (GameObject enemy in activeEnemies)
            {
                if (enemy != null)
                {
                    Destroy(enemy);
                }
            }
            
            activeEnemies.Clear();
        }
        
        /// <summary>
        /// Get current active enemy count
        /// Lấy số lượng quái đang hoạt động
        /// </summary>
        public int GetActiveEnemyCount()
        {
            activeEnemies.RemoveAll(enemy => enemy == null);
            return activeEnemies.Count;
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
            
            if (spawnPoints != null)
            {
                Gizmos.color = Color.yellow;
                foreach (Transform spawnPoint in spawnPoints)
                {
                    if (spawnPoint != null)
                    {
                        Gizmos.DrawWireSphere(spawnPoint.position, 1f);
                    }
                }
            }
        }
    }
}
