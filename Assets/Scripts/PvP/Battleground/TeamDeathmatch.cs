using UnityEngine;
using System.Collections.Generic;

namespace DarkLegend.PvP
{
    /// <summary>
    /// Team Deathmatch Mode - Chế độ tử chiến theo đội
    /// First team to reach kill limit wins
    /// </summary>
    public class TeamDeathmatch : BattlegroundMode
    {
        [Header("TDM Settings")]
        public int killsToWin = 100;
        public int pointsPerKill = 1;
        public int pointsPerAssist = 0;
        public float respawnTime = 5f;
        
        [Header("Spawn Points")]
        public Transform[] team1Spawns;
        public Transform[] team2Spawns;
        
        private Dictionary<GameObject, float> respawnTimers = new Dictionary<GameObject, float>();
        
        private void Awake()
        {
            modeName = "Team Deathmatch";
            teamSize = 10;
            timeLimit = 900; // 15 minutes
        }
        
        public override void InitializeMatch(List<GameObject> team1, List<GameObject> team2)
        {
            base.InitializeMatch(team1, team2);
            respawnTimers.Clear();
        }
        
        public override void StartMatch()
        {
            base.StartMatch();
            
            // Spawn all players
            SpawnTeam(team1, team1Spawns);
            SpawnTeam(team2, team2Spawns);
        }
        
        /// <summary>
        /// Handle player kill
        /// Xử lý khi người chơi giết
        /// </summary>
        public void OnPlayerKill(GameObject killer, GameObject victim)
        {
            if (state != MatchState.InProgress) return;
            
            // Update score
            int killerTeam = team1.Contains(killer) ? 1 : 2;
            UpdateScore(killerTeam, pointsPerKill);
            
            // Queue respawn for victim
            respawnTimers[victim] = Time.time + respawnTime;
            
            Debug.Log($"{killer.name} killed {victim.name}. Score: {team1Score}-{team2Score}");
        }
        
        /// <summary>
        /// Handle player assist
        /// Xử lý khi người chơi hỗ trợ
        /// </summary>
        public void OnPlayerAssist(GameObject assister, GameObject victim)
        {
            if (state != MatchState.InProgress) return;
            if (pointsPerAssist == 0) return;
            
            int assisterTeam = team1.Contains(assister) ? 1 : 2;
            UpdateScore(assisterTeam, pointsPerAssist);
        }
        
        protected override bool CheckWinCondition()
        {
            return team1Score >= killsToWin || team2Score >= killsToWin;
        }
        
        protected override void Update()
        {
            base.Update();
            
            // Handle respawns
            ProcessRespawns();
        }
        
        /// <summary>
        /// Process player respawns
        /// Xử lý hồi sinh người chơi
        /// </summary>
        private void ProcessRespawns()
        {
            List<GameObject> toRespawn = new List<GameObject>();
            
            foreach (var kvp in respawnTimers)
            {
                if (Time.time >= kvp.Value)
                {
                    toRespawn.Add(kvp.Key);
                }
            }
            
            foreach (var player in toRespawn)
            {
                RespawnPlayer(player);
                respawnTimers.Remove(player);
            }
        }
        
        /// <summary>
        /// Respawn player
        /// Hồi sinh người chơi
        /// </summary>
        private void RespawnPlayer(GameObject player)
        {
            Transform[] spawns = team1.Contains(player) ? team1Spawns : team2Spawns;
            if (spawns != null && spawns.Length > 0)
            {
                Transform spawnPoint = spawns[Random.Range(0, spawns.Length)];
                player.transform.position = spawnPoint.position;
                player.transform.rotation = spawnPoint.rotation;
                
                // TODO: Reset player health/mana
                Debug.Log($"{player.name} respawned");
            }
        }
        
        /// <summary>
        /// Spawn team at spawn points
        /// Sinh đội tại điểm spawn
        /// </summary>
        private void SpawnTeam(List<GameObject> team, Transform[] spawns)
        {
            if (spawns == null || spawns.Length == 0) return;
            
            for (int i = 0; i < team.Count; i++)
            {
                Transform spawnPoint = spawns[i % spawns.Length];
                team[i].transform.position = spawnPoint.position;
                team[i].transform.rotation = spawnPoint.rotation;
            }
        }
    }
}
