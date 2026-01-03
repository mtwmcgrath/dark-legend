using UnityEngine;

namespace DarkLegend.Maps.Zones
{
    /// <summary>
    /// Boss zone - Khu vực boss đặc biệt
    /// Special boss encounter zone
    /// </summary>
    public class BossZone : ZoneBase
    {
        [Header("Boss Configuration")]
        [Tooltip("Tên boss / Boss name")]
        [SerializeField] private string bossName;
        
        [Tooltip("Prefab boss / Boss prefab")]
        [SerializeField] private GameObject bossPrefab;
        
        [Tooltip("Level boss / Boss level")]
        [SerializeField] private int bossLevel = 100;
        
        [Tooltip("Vị trí spawn boss / Boss spawn position")]
        [SerializeField] private Vector3 bossSpawnPosition;
        
        [Header("Spawn Settings")]
        [Tooltip("Thời gian spawn (giờ) / Spawn interval in hours")]
        [SerializeField] private float spawnIntervalHours = 2f;
        
        [Tooltip("Thông báo toàn server / Server-wide announcement")]
        [SerializeField] private bool announceSpawn = true;
        
        [Tooltip("Thời gian cảnh báo trước (phút) / Warning time before spawn")]
        [SerializeField] private int warningMinutes = 10;
        
        [Header("Boss Mechanics")]
        [Tooltip("Boss có minions / Boss has minions")]
        [SerializeField] private bool hasMinions = true;
        
        [Tooltip("Prefab minions / Minion prefabs")]
        [SerializeField] private GameObject[] minionPrefabs;
        
        [Tooltip("Số minions tối đa / Maximum minions")]
        [SerializeField] private int maxMinions = 10;
        
        [Header("Rewards")]
        [Tooltip("Guaranteed drop / Guaranteed item drop")]
        [SerializeField] private bool guaranteedDrop = true;
        
        [Tooltip("Drop rate cao / High drop rate")]
        [SerializeField] private float dropRateMultiplier = 3f;
        
        [Tooltip("EXP bonus / EXP bonus multiplier")]
        [SerializeField] private float expBonusMultiplier = 5f;
        
        private GameObject currentBoss;
        private float nextSpawnTime;
        private bool bossAlive = false;
        
        public override void InitializeZone()
        {
            base.InitializeZone();
            
            // Tính thời gian spawn tiếp theo
            nextSpawnTime = Time.time + (spawnIntervalHours * 3600);
            
            Debug.Log($"[BossZone] Boss zone initialized: {bossName}");
            Debug.Log($"[BossZone] Next spawn in {spawnIntervalHours} hours");
        }
        
        public override void CleanupZone()
        {
            base.CleanupZone();
            
            // Destroy boss if exists
            if (currentBoss != null)
            {
                Destroy(currentBoss);
                currentBoss = null;
            }
        }
        
        protected override void UpdateZone()
        {
            base.UpdateZone();
            
            // Check spawn time
            if (!bossAlive && Time.time >= nextSpawnTime)
            {
                SpawnBoss();
            }
            
            // Check for warning time
            float timeUntilSpawn = nextSpawnTime - Time.time;
            if (!bossAlive && timeUntilSpawn <= warningMinutes * 60 && timeUntilSpawn > (warningMinutes - 1) * 60)
            {
                AnnounceWarning();
            }
            
            // Check if boss is dead
            if (bossAlive && currentBoss == null)
            {
                OnBossDefeated();
            }
        }
        
        /// <summary>
        /// Spawn boss / Spawn the boss
        /// </summary>
        private void SpawnBoss()
        {
            if (bossPrefab == null)
            {
                Debug.LogError($"[BossZone] Boss prefab is null!");
                return;
            }
            
            // Spawn boss
            currentBoss = Instantiate(bossPrefab, bossSpawnPosition, Quaternion.identity, transform);
            currentBoss.name = bossName;
            bossAlive = true;
            
            // Spawn minions if needed
            if (hasMinions && minionPrefabs != null && minionPrefabs.Length > 0)
            {
                SpawnMinions();
            }
            
            // Announce spawn
            if (announceSpawn)
            {
                AnnounceSpawn();
            }
            
            Debug.Log($"[BossZone] Boss spawned: {bossName} at {bossSpawnPosition}");
        }
        
        /// <summary>
        /// Spawn minions / Spawn boss minions
        /// </summary>
        private void SpawnMinions()
        {
            for (int i = 0; i < maxMinions; i++)
            {
                GameObject minionPrefab = minionPrefabs[Random.Range(0, minionPrefabs.Length)];
                if (minionPrefab != null)
                {
                    Vector3 offset = Random.insideUnitSphere * 10f;
                    offset.y = 0;
                    Vector3 spawnPos = bossSpawnPosition + offset;
                    
                    GameObject minion = Instantiate(minionPrefab, spawnPos, Quaternion.identity, transform);
                    Debug.Log($"[BossZone] Spawned minion at {spawnPos}");
                }
            }
        }
        
        /// <summary>
        /// Thông báo boss spawn / Announce boss spawn
        /// </summary>
        private void AnnounceSpawn()
        {
            string announcement = $"🔥 BOSS {bossName.ToUpper()} ĐÃ XUẤT HIỆN TẠI {zoneName}! 🔥";
            Debug.Log($"[BossZone] {announcement}");
            // TODO: Send server-wide announcement
        }
        
        /// <summary>
        /// Cảnh báo trước khi spawn / Warning before spawn
        /// </summary>
        private void AnnounceWarning()
        {
            string warning = $"⚠️ Boss {bossName} sẽ xuất hiện trong {warningMinutes} phút tại {zoneName}!";
            Debug.Log($"[BossZone] {warning}");
            // TODO: Send server-wide warning
        }
        
        /// <summary>
        /// Khi boss bị đánh bại / When boss is defeated
        /// </summary>
        private void OnBossDefeated()
        {
            bossAlive = false;
            
            // Announce defeat
            string announcement = $"🏆 Boss {bossName} đã bị đánh bại! 🏆";
            Debug.Log($"[BossZone] {announcement}");
            // TODO: Send server-wide announcement
            
            // Schedule next spawn
            nextSpawnTime = Time.time + (spawnIntervalHours * 3600);
            Debug.Log($"[BossZone] Next spawn scheduled in {spawnIntervalHours} hours");
            
            // Drop rewards
            DropBossRewards();
        }
        
        /// <summary>
        /// Drop phần thưởng / Drop boss rewards
        /// </summary>
        private void DropBossRewards()
        {
            // TODO: Implement reward dropping system
            Debug.Log($"[BossZone] Dropping rewards with multiplier: {dropRateMultiplier}x");
        }
        
        /// <summary>
        /// Force spawn boss (GM command) / Force spawn boss
        /// </summary>
        public void ForceSpawnBoss()
        {
            if (bossAlive)
            {
                Debug.Log($"[BossZone] Boss already alive!");
                return;
            }
            
            SpawnBoss();
        }
        
        /// <summary>
        /// Lấy thời gian còn lại đến spawn / Get time until next spawn
        /// </summary>
        public float GetTimeUntilSpawn()
        {
            return Mathf.Max(0, nextSpawnTime - Time.time);
        }
        
        /// <summary>
        /// Kiểm tra boss có còn sống không / Check if boss is alive
        /// </summary>
        public bool IsBossAlive()
        {
            return bossAlive;
        }
        
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            
            // Draw boss spawn position
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(bossSpawnPosition, 2f);
            Gizmos.DrawWireSphere(bossSpawnPosition, 10f);
        }
    }
}
