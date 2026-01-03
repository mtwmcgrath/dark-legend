using System.Collections.Generic;
using UnityEngine;

namespace DarkLegend.Maps.Spawning
{
    /// <summary>
    /// Quản lý spawn monsters trong một zone
    /// Manages monster spawning in a zone
    /// </summary>
    public class MonsterSpawner : MonoBehaviour
    {
        [Header("Spawner Configuration")]
        [Tooltip("Tên spawner / Spawner name")]
        [SerializeField] private string spawnerName = "Monster Spawner";
        
        [Tooltip("Prefab monster / Monster prefab")]
        [SerializeField] private GameObject monsterPrefab;
        
        [Tooltip("Số lượng tối đa / Maximum spawn count")]
        [SerializeField] private int maxSpawnCount = 10;
        
        [Tooltip("Thời gian respawn (giây) / Respawn time in seconds")]
        [SerializeField] private float respawnTime = 30f;
        
        [Header("Spawn Area")]
        [Tooltip("Sử dụng spawn points / Use spawn points")]
        [SerializeField] private bool useSpawnPoints = true;
        
        [Tooltip("Danh sách spawn points / Spawn point list")]
        [SerializeField] private List<SpawnPoint> spawnPoints = new List<SpawnPoint>();
        
        [Tooltip("Khu vực spawn (nếu không dùng points) / Spawn area")]
        [SerializeField] private Vector3 spawnAreaCenter;
        
        [Tooltip("Kích thước khu vực / Area size")]
        [SerializeField] private Vector3 spawnAreaSize = new Vector3(50, 0, 50);
        
        [Header("Spawn Rules")]
        [Tooltip("Spawn ngay khi khởi tạo / Spawn on start")]
        [SerializeField] private bool spawnOnStart = true;
        
        [Tooltip("Chỉ spawn ban đêm / Night only")]
        [SerializeField] private bool nightOnly = false;
        
        [Tooltip("Chỉ spawn ban ngày / Day only")]
        [SerializeField] private bool dayOnly = false;
        
        [Tooltip("Spawn khi có player gần / Spawn when player nearby")]
        [SerializeField] private bool spawnWhenPlayerNear = false;
        
        [Tooltip("Khoảng cách phát hiện player / Player detection radius")]
        [SerializeField] private float playerDetectionRadius = 50f;
        
        [Header("Monster Settings")]
        [Tooltip("Level min / Minimum level")]
        [SerializeField] private int minLevel = 1;
        
        [Tooltip("Level max / Maximum level")]
        [SerializeField] private int maxLevel = 10;
        
        [Tooltip("Áp dụng random stats / Apply random stats")]
        [SerializeField] private bool randomizeStats = true;
        
        private List<GameObject> spawnedMonsters = new List<GameObject>();
        private Dictionary<GameObject, float> monsterDeathTimes = new Dictionary<GameObject, float>();
        private bool isActive = true;
        
        private void Start()
        {
            if (spawnOnStart)
            {
                InitializeSpawner();
            }
        }
        
        private void Update()
        {
            if (!isActive) return;
            
            // Kiểm tra điều kiện spawn
            if (!CanSpawn())
            {
                return;
            }
            
            // Kiểm tra và respawn monsters
            CheckAndRespawn();
        }
        
        /// <summary>
        /// Khởi tạo spawner / Initialize spawner
        /// </summary>
        public void InitializeSpawner()
        {
            Debug.Log($"[MonsterSpawner] Initializing: {spawnerName}");
            
            // Spawn initial monsters
            for (int i = 0; i < maxSpawnCount; i++)
            {
                SpawnMonster();
            }
        }
        
        /// <summary>
        /// Kiểm tra có thể spawn không / Check if can spawn
        /// </summary>
        private bool CanSpawn()
        {
            // Kiểm tra night/day
            if (nightOnly && !IsNightTime())
            {
                return false;
            }
            
            if (dayOnly && IsNightTime())
            {
                return false;
            }
            
            // Kiểm tra player nearby
            if (spawnWhenPlayerNear && !IsPlayerNearby())
            {
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Kiểm tra và respawn / Check and respawn monsters
        /// </summary>
        private void CheckAndRespawn()
        {
            // Cleanup dead monsters
            spawnedMonsters.RemoveAll(m => m == null);
            
            // Check for monsters to respawn
            List<GameObject> toRemove = new List<GameObject>();
            foreach (var kvp in monsterDeathTimes)
            {
                if (kvp.Key == null)
                {
                    float timeSinceDeath = Time.time - kvp.Value;
                    if (timeSinceDeath >= respawnTime)
                    {
                        SpawnMonster();
                        toRemove.Add(kvp.Key);
                    }
                }
            }
            
            // Remove processed entries
            foreach (var key in toRemove)
            {
                monsterDeathTimes.Remove(key);
            }
            
            // Spawn thêm nếu thiếu
            int currentCount = spawnedMonsters.Count;
            if (currentCount < maxSpawnCount)
            {
                SpawnMonster();
            }
        }
        
        /// <summary>
        /// Spawn một monster / Spawn a single monster
        /// </summary>
        private void SpawnMonster()
        {
            if (monsterPrefab == null)
            {
                Debug.LogWarning($"[MonsterSpawner] Monster prefab is null on {spawnerName}");
                return;
            }
            
            // Lấy vị trí spawn
            Vector3 spawnPos = GetSpawnPosition();
            
            // Spawn monster
            GameObject monster = Instantiate(monsterPrefab, spawnPos, Quaternion.identity, transform);
            spawnedMonsters.Add(monster);
            
            // Apply settings
            ApplyMonsterSettings(monster);
            
            // Register death callback
            RegisterMonsterCallbacks(monster);
            
            Debug.Log($"[MonsterSpawner] Spawned monster at {spawnPos}");
        }
        
        /// <summary>
        /// Lấy vị trí spawn / Get spawn position
        /// </summary>
        private Vector3 GetSpawnPosition()
        {
            if (useSpawnPoints && spawnPoints.Count > 0)
            {
                // Chọn random spawn point
                SpawnPoint point = spawnPoints[Random.Range(0, spawnPoints.Count)];
                return point.GetSpawnPosition();
            }
            else
            {
                // Random trong khu vực
                float x = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
                float z = Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2);
                return spawnAreaCenter + new Vector3(x, 0, z);
            }
        }
        
        /// <summary>
        /// Áp dụng settings cho monster / Apply settings to monster
        /// </summary>
        private void ApplyMonsterSettings(GameObject monster)
        {
            // Random level
            int level = Random.Range(minLevel, maxLevel + 1);
            
            // TODO: Apply level and stats to monster component
            Debug.Log($"[MonsterSpawner] Applied level {level} to monster");
        }
        
        /// <summary>
        /// Đăng ký callbacks cho monster / Register monster callbacks
        /// </summary>
        private void RegisterMonsterCallbacks(GameObject monster)
        {
            // TODO: Register to monster death event
            // For now, we'll track it manually
        }
        
        /// <summary>
        /// Gọi khi monster chết / Called when monster dies
        /// </summary>
        public void OnMonsterDeath(GameObject monster)
        {
            if (spawnedMonsters.Contains(monster))
            {
                monsterDeathTimes[monster] = Time.time;
                Debug.Log($"[MonsterSpawner] Monster died, will respawn in {respawnTime}s");
            }
        }
        
        /// <summary>
        /// Kiểm tra có phải ban đêm / Check if night time
        /// </summary>
        private bool IsNightTime()
        {
            // TODO: Integrate with DayNightCycle system
            return false;
        }
        
        /// <summary>
        /// Kiểm tra có player gần / Check if player nearby
        /// </summary>
        private bool IsPlayerNearby()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                return distance <= playerDetectionRadius;
            }
            return false;
        }
        
        /// <summary>
        /// Dừng spawner / Stop spawner
        /// </summary>
        public void StopSpawner()
        {
            isActive = false;
        }
        
        /// <summary>
        /// Tiếp tục spawner / Resume spawner
        /// </summary>
        public void ResumeSpawner()
        {
            isActive = true;
        }
        
        /// <summary>
        /// Xóa tất cả monsters / Clear all monsters
        /// </summary>
        public void ClearAllMonsters()
        {
            foreach (var monster in spawnedMonsters)
            {
                if (monster != null)
                {
                    Destroy(monster);
                }
            }
            spawnedMonsters.Clear();
            monsterDeathTimes.Clear();
        }
        
        /// <summary>
        /// Lấy số lượng monster hiện tại / Get current monster count
        /// </summary>
        public int GetCurrentCount()
        {
            return spawnedMonsters.Count;
        }
        
        private void OnDrawGizmos()
        {
            // Draw spawn area
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
            Gizmos.DrawCube(spawnAreaCenter, spawnAreaSize);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
            
            // Draw player detection radius
            if (spawnWhenPlayerNear)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
            }
        }
    }
}
