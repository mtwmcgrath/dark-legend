using UnityEngine;

namespace DarkLegend.Maps.Spawning
{
    /// <summary>
    /// Boss spawner với timer và thông báo
    /// Boss spawner with timer and announcements
    /// </summary>
    public class BossSpawner : MonoBehaviour
    {
        [Header("Boss Configuration")]
        [Tooltip("Tên boss / Boss name")]
        [SerializeField] private string bossName = "World Boss";
        
        [Tooltip("Prefab boss / Boss prefab")]
        [SerializeField] private GameObject bossPrefab;
        
        [Tooltip("Level boss / Boss level")]
        [SerializeField] private int bossLevel = 100;
        
        [Tooltip("Vị trí spawn / Spawn position")]
        [SerializeField] private Vector3 spawnPosition;
        
        [Header("Spawn Timing")]
        [Tooltip("Thời gian spawn (giờ) / Spawn interval in hours")]
        [SerializeField] private float spawnIntervalHours = 2f;
        
        [Tooltip("Spawn ngay khi start / Spawn on start")]
        [SerializeField] private bool spawnOnStart = false;
        
        [Tooltip("Thời gian cảnh báo (phút) / Warning time in minutes")]
        [SerializeField] private int[] warningMinutes = { 30, 10, 5, 1 };
        
        [Header("Announcements")]
        [Tooltip("Thông báo toàn server / Server-wide announcement")]
        [SerializeField] private bool announceToServer = true;
        
        [Tooltip("Âm thanh thông báo / Announcement sound")]
        [SerializeField] private AudioClip announcementSound;
        
        [Header("Boss Mechanics")]
        [Tooltip("HP multiplier / HP multiplier")]
        [SerializeField] private float hpMultiplier = 10f;
        
        [Tooltip("Damage multiplier / Damage multiplier")]
        [SerializeField] private float damageMultiplier = 2f;
        
        [Tooltip("Spawn minions / Spawn minions")]
        [SerializeField] private bool spawnMinions = true;
        
        [Tooltip("Minion prefabs / Minion prefabs")]
        [SerializeField] private GameObject[] minionPrefabs;
        
        [Tooltip("Số minions / Minion count")]
        [SerializeField] private int minionCount = 5;
        
        [Header("Rewards")]
        [Tooltip("Guaranteed drop / Guaranteed drop")]
        [SerializeField] private bool guaranteedDrop = true;
        
        [Tooltip("Drop rate multiplier / Drop rate multiplier")]
        [SerializeField] private float dropRateMultiplier = 5f;
        
        private GameObject currentBoss;
        private float nextSpawnTime;
        private bool bossAlive = false;
        private bool[] warningsSent;
        
        private void Start()
        {
            warningsSent = new bool[warningMinutes.Length];
            
            if (spawnOnStart)
            {
                SpawnBoss();
            }
            else
            {
                // Schedule first spawn
                nextSpawnTime = Time.time + (spawnIntervalHours * 3600);
                Debug.Log($"[BossSpawner] {bossName} scheduled to spawn in {spawnIntervalHours} hours");
            }
        }
        
        private void Update()
        {
            if (bossAlive)
            {
                // Check if boss is still alive
                if (currentBoss == null)
                {
                    OnBossDefeated();
                }
            }
            else
            {
                // Check for warnings
                float timeUntilSpawn = nextSpawnTime - Time.time;
                CheckAndSendWarnings(timeUntilSpawn);
                
                // Check if it's time to spawn
                if (Time.time >= nextSpawnTime)
                {
                    SpawnBoss();
                }
            }
        }
        
        /// <summary>
        /// Spawn boss / Spawn the boss
        /// </summary>
        public void SpawnBoss()
        {
            if (bossPrefab == null)
            {
                Debug.LogError($"[BossSpawner] Boss prefab is null!");
                return;
            }
            
            if (bossAlive)
            {
                Debug.LogWarning($"[BossSpawner] {bossName} is already alive!");
                return;
            }
            
            Debug.Log($"[BossSpawner] Spawning {bossName} at {spawnPosition}");
            
            // Spawn boss
            currentBoss = Instantiate(bossPrefab, spawnPosition, Quaternion.identity, transform);
            currentBoss.name = bossName;
            bossAlive = true;
            
            // Apply boss modifiers
            ApplyBossModifiers(currentBoss);
            
            // Spawn minions
            if (spawnMinions && minionPrefabs != null && minionPrefabs.Length > 0)
            {
                SpawnBossMinions();
            }
            
            // Announce spawn
            if (announceToServer)
            {
                AnnounceSpawn();
            }
            
            // Reset warnings
            for (int i = 0; i < warningsSent.Length; i++)
            {
                warningsSent[i] = false;
            }
        }
        
        /// <summary>
        /// Áp dụng modifiers cho boss / Apply boss modifiers
        /// </summary>
        private void ApplyBossModifiers(GameObject boss)
        {
            // TODO: Apply HP and damage multipliers
            Debug.Log($"[BossSpawner] Applied modifiers: HP x{hpMultiplier}, DMG x{damageMultiplier}");
        }
        
        /// <summary>
        /// Spawn minions cho boss / Spawn boss minions
        /// </summary>
        private void SpawnBossMinions()
        {
            for (int i = 0; i < minionCount; i++)
            {
                if (minionPrefabs.Length == 0) break;
                
                GameObject minionPrefab = minionPrefabs[Random.Range(0, minionPrefabs.Length)];
                if (minionPrefab == null) continue;
                
                // Spawn in circle around boss
                float angle = (360f / minionCount) * i;
                float radian = angle * Mathf.Deg2Rad;
                Vector3 offset = new Vector3(Mathf.Cos(radian), 0, Mathf.Sin(radian)) * 10f;
                Vector3 minionPos = spawnPosition + offset;
                
                GameObject minion = Instantiate(minionPrefab, minionPos, Quaternion.identity, transform);
                Debug.Log($"[BossSpawner] Spawned minion {i + 1}/{minionCount}");
            }
        }
        
        /// <summary>
        /// Kiểm tra và gửi cảnh báo / Check and send warnings
        /// </summary>
        private void CheckAndSendWarnings(float timeUntilSpawn)
        {
            for (int i = 0; i < warningMinutes.Length; i++)
            {
                if (!warningsSent[i])
                {
                    float warningThreshold = warningMinutes[i] * 60;
                    if (timeUntilSpawn <= warningThreshold && timeUntilSpawn > warningThreshold - 60)
                    {
                        SendWarning(warningMinutes[i]);
                        warningsSent[i] = true;
                    }
                }
            }
        }
        
        /// <summary>
        /// Gửi cảnh báo / Send warning
        /// </summary>
        private void SendWarning(int minutes)
        {
            string message = $"⚠️ {bossName} sẽ xuất hiện sau {minutes} phút! ⚠️";
            Debug.Log($"[BossSpawner] {message}");
            
            if (announceToServer)
            {
                // TODO: Send server-wide announcement
            }
            
            if (announcementSound != null)
            {
                AudioSource.PlayClipAtPoint(announcementSound, Camera.main.transform.position);
            }
        }
        
        /// <summary>
        /// Thông báo boss spawn / Announce boss spawn
        /// </summary>
        private void AnnounceSpawn()
        {
            string message = $"🔥 {bossName.ToUpper()} ĐÃ XUẤT HIỆN! 🔥\nVị trí: {transform.parent?.name ?? "Unknown"}";
            Debug.Log($"[BossSpawner] {message}");
            
            // TODO: Send server-wide announcement with coordinates
            
            if (announcementSound != null)
            {
                AudioSource.PlayClipAtPoint(announcementSound, Camera.main.transform.position);
            }
        }
        
        /// <summary>
        /// Khi boss bị đánh bại / When boss is defeated
        /// </summary>
        private void OnBossDefeated()
        {
            Debug.Log($"[BossSpawner] {bossName} has been defeated!");
            
            bossAlive = false;
            currentBoss = null;
            
            // Announce defeat
            string message = $"🏆 {bossName} đã bị đánh bại! 🏆";
            Debug.Log($"[BossSpawner] {message}");
            
            if (announceToServer)
            {
                // TODO: Send server-wide announcement
            }
            
            // Drop rewards
            DropRewards();
            
            // Schedule next spawn
            nextSpawnTime = Time.time + (spawnIntervalHours * 3600);
            Debug.Log($"[BossSpawner] Next spawn in {spawnIntervalHours} hours");
        }
        
        /// <summary>
        /// Drop phần thưởng / Drop rewards
        /// </summary>
        private void DropRewards()
        {
            // TODO: Implement reward dropping
            Debug.Log($"[BossSpawner] Dropping rewards with multiplier: {dropRateMultiplier}x");
        }
        
        /// <summary>
        /// Force spawn (GM command) / Force spawn boss
        /// </summary>
        public void ForceSpawn()
        {
            SpawnBoss();
        }
        
        /// <summary>
        /// Lấy thời gian còn lại / Get time until spawn
        /// </summary>
        public float GetTimeUntilSpawn()
        {
            return Mathf.Max(0, nextSpawnTime - Time.time);
        }
        
        /// <summary>
        /// Kiểm tra boss có sống không / Check if boss is alive
        /// </summary>
        public bool IsBossAlive()
        {
            return bossAlive && currentBoss != null;
        }
        
        private void OnDrawGizmos()
        {
            // Draw spawn position
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(spawnPosition, 2f);
            
            // Draw minion spawn circle
            if (spawnMinions)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(spawnPosition, 10f);
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            // Draw boss area
            Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
            Gizmos.DrawSphere(spawnPosition, 20f);
        }
    }
}
