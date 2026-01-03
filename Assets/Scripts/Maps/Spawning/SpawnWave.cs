using System.Collections.Generic;
using UnityEngine;

namespace DarkLegend.Maps.Spawning
{
    /// <summary>
    /// Spawn monsters theo waves / sóng
    /// Wave-based monster spawning for events
    /// </summary>
    public class SpawnWave : MonoBehaviour
    {
        [Header("Wave Configuration")]
        [Tooltip("Số wave / Wave number")]
        [SerializeField] private int waveNumber = 1;
        
        [Tooltip("Danh sách monsters trong wave / Monster list")]
        [SerializeField] private List<WaveMonsterData> monsters = new List<WaveMonsterData>();
        
        [Tooltip("Thời gian chờ trước wave (giây) / Delay before wave")]
        [SerializeField] private float waveDelay = 5f;
        
        [Tooltip("Thời gian spawn giữa các monsters (giây) / Spawn interval")]
        [SerializeField] private float spawnInterval = 1f;
        
        [Header("Spawn Settings")]
        [Tooltip("Spawn tất cả cùng lúc / Spawn all at once")]
        [SerializeField] private bool spawnAllAtOnce = false;
        
        [Tooltip("Khu vực spawn / Spawn area center")]
        [SerializeField] private Vector3 spawnAreaCenter;
        
        [Tooltip("Kích thước khu vực / Spawn area size")]
        [SerializeField] private Vector3 spawnAreaSize = new Vector3(30, 0, 30);
        
        [Tooltip("Danh sách spawn points / Spawn points")]
        [SerializeField] private List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
        
        [Header("Wave Events")]
        [Tooltip("Thông báo khi bắt đầu / Announce on start")]
        [SerializeField] private bool announceStart = true;
        
        [Tooltip("Thông báo khi hoàn thành / Announce on complete")]
        [SerializeField] private bool announceComplete = true;
        
        private List<GameObject> spawnedMonsters = new List<GameObject>();
        private bool waveStarted = false;
        private bool waveCompleted = false;
        private int currentSpawnIndex = 0;
        private float nextSpawnTime = 0f;
        
        // Events
        public delegate void WaveEventHandler(int waveNumber);
        public event WaveEventHandler OnWaveStart;
        public event WaveEventHandler OnWaveComplete;
        
        private void Update()
        {
            if (!waveStarted || waveCompleted) return;
            
            // Spawn monsters theo interval
            if (!spawnAllAtOnce && Time.time >= nextSpawnTime)
            {
                SpawnNextMonster();
            }
            
            // Kiểm tra wave hoàn thành
            CheckWaveCompletion();
        }
        
        /// <summary>
        /// Bắt đầu wave / Start wave
        /// </summary>
        public void StartWave()
        {
            if (waveStarted)
            {
                Debug.LogWarning($"[SpawnWave] Wave {waveNumber} already started!");
                return;
            }
            
            Debug.Log($"[SpawnWave] Starting wave {waveNumber}");
            
            waveStarted = true;
            currentSpawnIndex = 0;
            
            // Announce wave start
            if (announceStart)
            {
                AnnounceWaveStart();
            }
            
            // Trigger event
            OnWaveStart?.Invoke(waveNumber);
            
            // Start spawning
            if (spawnAllAtOnce)
            {
                SpawnAllMonsters();
            }
            else
            {
                nextSpawnTime = Time.time + waveDelay;
            }
        }
        
        /// <summary>
        /// Spawn tất cả monsters cùng lúc / Spawn all monsters at once
        /// </summary>
        private void SpawnAllMonsters()
        {
            foreach (var monsterData in monsters)
            {
                for (int i = 0; i < monsterData.count; i++)
                {
                    SpawnMonster(monsterData);
                }
            }
        }
        
        /// <summary>
        /// Spawn monster tiếp theo / Spawn next monster
        /// </summary>
        private void SpawnNextMonster()
        {
            if (currentSpawnIndex >= GetTotalMonsterCount())
            {
                return;
            }
            
            // Tìm monster data và index
            int remainingIndex = currentSpawnIndex;
            WaveMonsterData monsterData = null;
            
            foreach (var data in monsters)
            {
                if (remainingIndex < data.count)
                {
                    monsterData = data;
                    break;
                }
                remainingIndex -= data.count;
            }
            
            if (monsterData != null)
            {
                SpawnMonster(monsterData);
            }
            
            currentSpawnIndex++;
            nextSpawnTime = Time.time + spawnInterval;
        }
        
        /// <summary>
        /// Spawn một monster / Spawn a single monster
        /// </summary>
        private void SpawnMonster(WaveMonsterData monsterData)
        {
            if (monsterData.prefab == null)
            {
                Debug.LogWarning($"[SpawnWave] Monster prefab is null in wave {waveNumber}");
                return;
            }
            
            // Lấy vị trí spawn
            Vector3 spawnPos = GetRandomSpawnPosition();
            
            // Spawn monster
            GameObject monster = Instantiate(monsterData.prefab, spawnPos, Quaternion.identity, transform);
            spawnedMonsters.Add(monster);
            
            Debug.Log($"[SpawnWave] Spawned {monsterData.prefab.name} at {spawnPos}");
        }
        
        /// <summary>
        /// Lấy vị trí spawn random / Get random spawn position
        /// </summary>
        private Vector3 GetRandomSpawnPosition()
        {
            // Ưu tiên dùng spawn points
            if (spawnPoints.Count > 0)
            {
                SpawnPoint point = spawnPoints[Random.Range(0, spawnPoints.Count)];
                return point.GetSpawnPosition();
            }
            
            // Fallback: random trong area
            float x = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
            float z = Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2);
            return spawnAreaCenter + new Vector3(x, 0, z);
        }
        
        /// <summary>
        /// Kiểm tra wave hoàn thành / Check if wave completed
        /// </summary>
        private void CheckWaveCompletion()
        {
            if (waveCompleted) return;
            
            // Cleanup dead monsters
            spawnedMonsters.RemoveAll(m => m == null);
            
            // Check nếu đã spawn hết và tất cả đã chết
            if (currentSpawnIndex >= GetTotalMonsterCount() && spawnedMonsters.Count == 0)
            {
                CompleteWave();
            }
        }
        
        /// <summary>
        /// Hoàn thành wave / Complete wave
        /// </summary>
        private void CompleteWave()
        {
            waveCompleted = true;
            
            Debug.Log($"[SpawnWave] Wave {waveNumber} completed!");
            
            // Announce completion
            if (announceComplete)
            {
                AnnounceWaveComplete();
            }
            
            // Trigger event
            OnWaveComplete?.Invoke(waveNumber);
        }
        
        /// <summary>
        /// Thông báo bắt đầu wave / Announce wave start
        /// </summary>
        private void AnnounceWaveStart()
        {
            string message = $"⚔️ Wave {waveNumber} đã bắt đầu!";
            Debug.Log($"[SpawnWave] {message}");
            // TODO: Show UI message
        }
        
        /// <summary>
        /// Thông báo hoàn thành wave / Announce wave complete
        /// </summary>
        private void AnnounceWaveComplete()
        {
            string message = $"✓ Wave {waveNumber} hoàn thành!";
            Debug.Log($"[SpawnWave] {message}");
            // TODO: Show UI message
        }
        
        /// <summary>
        /// Lấy tổng số monsters / Get total monster count
        /// </summary>
        private int GetTotalMonsterCount()
        {
            int total = 0;
            foreach (var data in monsters)
            {
                total += data.count;
            }
            return total;
        }
        
        /// <summary>
        /// Reset wave / Reset wave state
        /// </summary>
        public void ResetWave()
        {
            // Clear all spawned monsters
            foreach (var monster in spawnedMonsters)
            {
                if (monster != null)
                {
                    Destroy(monster);
                }
            }
            
            spawnedMonsters.Clear();
            waveStarted = false;
            waveCompleted = false;
            currentSpawnIndex = 0;
            
            Debug.Log($"[SpawnWave] Wave {waveNumber} reset");
        }
        
        /// <summary>
        /// Kiểm tra wave đã hoàn thành / Check if wave is completed
        /// </summary>
        public bool IsCompleted()
        {
            return waveCompleted;
        }
        
        /// <summary>
        /// Lấy số monsters còn lại / Get remaining monster count
        /// </summary>
        public int GetRemainingCount()
        {
            return spawnedMonsters.Count;
        }
        
        private void OnDrawGizmos()
        {
            // Draw spawn area
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
            Gizmos.DrawCube(spawnAreaCenter, spawnAreaSize);
            
            Gizmos.color = Color.orange;
            Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
        }
    }
    
    /// <summary>
    /// Dữ liệu monster trong wave / Monster data for wave
    /// </summary>
    [System.Serializable]
    public class WaveMonsterData
    {
        [Tooltip("Prefab monster / Monster prefab")]
        public GameObject prefab;
        
        [Tooltip("Số lượng / Count")]
        public int count = 5;
        
        [Tooltip("Level / Monster level")]
        public int level = 1;
    }
}
