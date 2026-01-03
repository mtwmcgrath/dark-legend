using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace DarkLegend.PvP
{
    /// <summary>
    /// Battle Royale Mode - Chế độ sinh tồn
    /// Last player/team standing wins
    /// </summary>
    public class BattleRoyale : BattlegroundMode
    {
        [Header("BR Settings")]
        public int maxPlayers = 50;
        public bool soloMode = true;              // Or duo/squad
        
        [Header("Circle Mechanics")]
        public float initialCircleRadius = 500f;
        public float finalCircleRadius = 20f;
        public float circleShrinkInterval = 120f; // 2 minutes
        public float damageOutsideCircle = 5f;    // HP per second
        public Transform circleCenter;
        
        [Header("Loot")]
        public bool randomLoot = true;
        public float lootSpawnDensity = 0.5f;
        public GameObject[] lootPrefabs;
        
        private float currentCircleRadius;
        private float nextCircleShrink;
        private List<GameObject> alivePlayers = new List<GameObject>();
        private Dictionary<GameObject, float> playerDamageTimers = new Dictionary<GameObject, float>();
        
        private void Awake()
        {
            modeName = "Battle Royale";
            teamSize = soloMode ? 1 : 2;          // 1 for solo, 2 for duo, etc.
            timeLimit = 1800;                      // 30 minutes
        }
        
        public override void InitializeMatch(List<GameObject> team1, List<GameObject> team2)
        {
            // For BR, combine all players into one list
            alivePlayers.Clear();
            alivePlayers.AddRange(team1);
            alivePlayers.AddRange(team2);
            
            this.team1 = team1;
            this.team2 = team2;
            this.state = MatchState.Waiting;
        }
        
        public override void StartMatch()
        {
            base.StartMatch();
            
            // Initialize circle
            currentCircleRadius = initialCircleRadius;
            nextCircleShrink = Time.time + circleShrinkInterval;
            
            // Spawn loot
            if (randomLoot)
            {
                SpawnLoot();
            }
            
            // Spawn players randomly
            SpawnPlayers();
            
            Debug.Log($"Battle Royale started with {alivePlayers.Count} players");
        }
        
        /// <summary>
        /// Spawn players at random locations
        /// Sinh người chơi ở vị trí ngẫu nhiên
        /// </summary>
        private void SpawnPlayers()
        {
            if (circleCenter == null) return;
            
            foreach (var player in alivePlayers)
            {
                // Random position within initial circle
                Vector2 randomPoint = Random.insideUnitCircle * (initialCircleRadius * 0.8f);
                Vector3 spawnPos = circleCenter.position + new Vector3(randomPoint.x, 0, randomPoint.y);
                
                player.transform.position = spawnPos;
            }
        }
        
        /// <summary>
        /// Spawn loot in the area
        /// Sinh đồ trong khu vực
        /// </summary>
        private void SpawnLoot()
        {
            if (lootPrefabs == null || lootPrefabs.Length == 0) return;
            if (circleCenter == null) return;
            
            int lootCount = Mathf.RoundToInt(maxPlayers * lootSpawnDensity);
            
            for (int i = 0; i < lootCount; i++)
            {
                Vector2 randomPoint = Random.insideUnitCircle * initialCircleRadius;
                Vector3 spawnPos = circleCenter.position + new Vector3(randomPoint.x, 0, randomPoint.y);
                
                GameObject lootPrefab = lootPrefabs[Random.Range(0, lootPrefabs.Length)];
                Instantiate(lootPrefab, spawnPos, Quaternion.identity);
            }
        }
        
        /// <summary>
        /// Player eliminated
        /// Người chơi bị loại
        /// </summary>
        public void OnPlayerEliminated(GameObject player)
        {
            if (!alivePlayers.Contains(player)) return;
            
            alivePlayers.Remove(player);
            Debug.Log($"{player.name} eliminated. {alivePlayers.Count} players remaining");
            
            // Check win condition
            if (alivePlayers.Count <= 1)
            {
                EndMatch(alivePlayers.Count > 0 ? 1 : 0);
            }
        }
        
        /// <summary>
        /// Shrink the circle
        /// Thu nhỏ vòng tròn
        /// </summary>
        private void ShrinkCircle()
        {
            float shrinkAmount = (initialCircleRadius - finalCircleRadius) / (timeLimit / circleShrinkInterval);
            currentCircleRadius = Mathf.Max(finalCircleRadius, currentCircleRadius - shrinkAmount);
            
            nextCircleShrink = Time.time + circleShrinkInterval;
            Debug.Log($"Circle shrinking to radius: {currentCircleRadius}");
        }
        
        /// <summary>
        /// Apply damage to players outside circle
        /// Gây sát thương cho người chơi ngoài vòng tròn
        /// </summary>
        private void ApplyCircleDamage()
        {
            if (circleCenter == null) return;
            
            foreach (var player in alivePlayers)
            {
                float distance = Vector3.Distance(player.transform.position, circleCenter.position);
                
                if (distance > currentCircleRadius)
                {
                    // Player is outside circle
                    if (!playerDamageTimers.ContainsKey(player))
                    {
                        playerDamageTimers[player] = Time.time;
                    }
                    
                    if (Time.time - playerDamageTimers[player] >= 1f)
                    {
                        // Apply damage every second
                        // TODO: Integrate with player health system
                        Debug.Log($"{player.name} taking {damageOutsideCircle} damage outside circle");
                        playerDamageTimers[player] = Time.time;
                    }
                }
                else
                {
                    // Player is inside circle
                    if (playerDamageTimers.ContainsKey(player))
                    {
                        playerDamageTimers.Remove(player);
                    }
                }
            }
        }
        
        protected override bool CheckWinCondition()
        {
            return alivePlayers.Count <= 1;
        }
        
        protected override void Update()
        {
            base.Update();
            
            if (state == MatchState.InProgress)
            {
                // Shrink circle periodically
                if (Time.time >= nextCircleShrink && currentCircleRadius > finalCircleRadius)
                {
                    ShrinkCircle();
                }
                
                // Apply circle damage
                ApplyCircleDamage();
            }
        }
        
        /// <summary>
        /// Get players alive count
        /// Lấy số người chơi còn sống
        /// </summary>
        public int GetAliveCount()
        {
            return alivePlayers.Count;
        }
        
        /// <summary>
        /// Get current circle radius
        /// Lấy bán kính vòng tròn hiện tại
        /// </summary>
        public float GetCurrentCircleRadius()
        {
            return currentCircleRadius;
        }
    }
}
