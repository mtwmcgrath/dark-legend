using System.Collections.Generic;
using UnityEngine;

namespace DarkLegend.Maps.Zones
{
    /// <summary>
    /// Vùng săn quái / Hunting ground zone
    /// Monster spawning and combat area
    /// </summary>
    public class HuntingZone : ZoneBase
    {
        [Header("Monster Configuration")]
        [Tooltip("Danh sách loại quái / Monster types")]
        [SerializeField] private List<MonsterSpawnData> monsterTypes = new List<MonsterSpawnData>();
        
        [Tooltip("Số lượng quái tối đa / Maximum monster count")]
        [SerializeField] private int maxMonsters = 50;
        
        [Tooltip("Thời gian respawn / Respawn time")]
        [SerializeField] private float respawnTime = 30f;
        
        [Header("Difficulty")]
        [Tooltip("Độ khó / Difficulty multiplier")]
        [SerializeField] private float difficultyMultiplier = 1f;
        
        [Tooltip("Spawn nhiều hơn ban đêm / More spawns at night")]
        [SerializeField] private bool increaseSpawnAtNight = true;
        
        [Tooltip("Quái mạnh hơn ban đêm / Stronger at night")]
        [SerializeField] private bool strongerAtNight = true;
        
        private List<GameObject> spawnedMonsters = new List<GameObject>();
        private float nextSpawnCheck = 0f;
        
        public override void InitializeZone()
        {
            base.InitializeZone();
            
            // Start spawning monsters
            StartMonsterSpawning();
            
            Debug.Log($"[HuntingZone] Hunting zone initialized: {zoneName}");
        }
        
        public override void CleanupZone()
        {
            base.CleanupZone();
            
            // Cleanup all monsters
            foreach (var monster in spawnedMonsters)
            {
                if (monster != null)
                {
                    Destroy(monster);
                }
            }
            spawnedMonsters.Clear();
        }
        
        protected override void UpdateZone()
        {
            base.UpdateZone();
            
            // Check và spawn monsters
            if (Time.time >= nextSpawnCheck)
            {
                CheckAndSpawnMonsters();
                nextSpawnCheck = Time.time + 5f; // Check mỗi 5 giây
            }
            
            // Cleanup dead monsters
            CleanupDeadMonsters();
        }
        
        /// <summary>
        /// Bắt đầu spawn quái / Start monster spawning
        /// </summary>
        private void StartMonsterSpawning()
        {
            // Spawn initial monsters
            for (int i = 0; i < maxMonsters / 2; i++)
            {
                SpawnRandomMonster();
            }
        }
        
        /// <summary>
        /// Kiểm tra và spawn thêm quái / Check and spawn more monsters
        /// </summary>
        private void CheckAndSpawnMonsters()
        {
            int currentCount = spawnedMonsters.Count;
            
            if (currentCount < maxMonsters)
            {
                int toSpawn = Mathf.Min(5, maxMonsters - currentCount);
                for (int i = 0; i < toSpawn; i++)
                {
                    SpawnRandomMonster();
                }
            }
        }
        
        /// <summary>
        /// Spawn quái random / Spawn random monster
        /// </summary>
        private void SpawnRandomMonster()
        {
            if (monsterTypes.Count == 0)
            {
                return;
            }
            
            // Chọn random monster type
            MonsterSpawnData monsterData = monsterTypes[Random.Range(0, monsterTypes.Count)];
            
            if (monsterData.prefab == null)
            {
                return;
            }
            
            // Lấy vị trí spawn random
            Vector3 spawnPos = GetRandomPositionInZone();
            
            // Spawn monster
            GameObject monster = Instantiate(monsterData.prefab, spawnPos, Quaternion.identity, transform);
            spawnedMonsters.Add(monster);
            
            // Apply difficulty multiplier
            ApplyDifficultyToMonster(monster);
            
            Debug.Log($"[HuntingZone] Spawned {monsterData.monsterName} at {spawnPos}");
        }
        
        /// <summary>
        /// Áp dụng độ khó cho quái / Apply difficulty to monster
        /// </summary>
        private void ApplyDifficultyToMonster(GameObject monster)
        {
            // TODO: Modify monster stats based on difficulty
            float finalMultiplier = difficultyMultiplier;
            
            // Tăng độ khó ban đêm
            if (strongerAtNight && IsNightTime())
            {
                finalMultiplier *= 1.3f;
            }
            
            Debug.Log($"[HuntingZone] Applied difficulty multiplier: {finalMultiplier}");
        }
        
        /// <summary>
        /// Cleanup quái đã chết / Cleanup dead monsters
        /// </summary>
        private void CleanupDeadMonsters()
        {
            spawnedMonsters.RemoveAll(m => m == null);
        }
        
        /// <summary>
        /// Kiểm tra có phải ban đêm không / Check if night time
        /// </summary>
        private bool IsNightTime()
        {
            // TODO: Check with day/night system
            return false;
        }
        
        /// <summary>
        /// Lấy số quái hiện tại / Get current monster count
        /// </summary>
        public int GetCurrentMonsterCount()
        {
            return spawnedMonsters.Count;
        }
        
        /// <summary>
        /// Lấy tất cả quái trong vùng / Get all monsters in zone
        /// </summary>
        public List<GameObject> GetAllMonsters()
        {
            return new List<GameObject>(spawnedMonsters);
        }
    }
    
    /// <summary>
    /// Dữ liệu spawn monster / Monster spawn data
    /// </summary>
    [System.Serializable]
    public class MonsterSpawnData
    {
        [Tooltip("Tên quái / Monster name")]
        public string monsterName;
        
        [Tooltip("Prefab quái / Monster prefab")]
        public GameObject prefab;
        
        [Tooltip("Level min-max")]
        public Vector2Int levelRange;
        
        [Tooltip("Tỷ lệ spawn / Spawn rate (0-1)")]
        [Range(0f, 1f)]
        public float spawnRate = 0.5f;
        
        [Tooltip("Chỉ spawn ban đêm / Night only")]
        public bool nightOnly = false;
    }
}
