using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DarkLegend.PvP
{
    /// <summary>
    /// Arena Queue Entry - Người chơi trong hàng đợi
    /// </summary>
    public class ArenaQueueEntry
    {
        public GameObject player;
        public ArenaMode mode;
        public int rating;
        public float queueTime;
        
        public ArenaQueueEntry(GameObject player, ArenaMode mode, int rating)
        {
            this.player = player;
            this.mode = mode;
            this.rating = rating;
            this.queueTime = Time.time;
        }
        
        public float GetWaitTime()
        {
            return Time.time - queueTime;
        }
    }

    /// <summary>
    /// Arena Queue - Hàng đợi matchmaking
    /// ELO-based matchmaking system
    /// </summary>
    public class ArenaQueue : MonoBehaviour
    {
        [Header("Matchmaking Settings")]
        public int eloRange = 200;                // +/- ELO range
        public int maxWaitTime = 120;             // Max wait time in seconds
        public float matchCheckInterval = 2f;     // Check for matches every X seconds
        
        private Dictionary<ArenaMode, List<ArenaQueueEntry>> queues = new Dictionary<ArenaMode, List<ArenaQueueEntry>>();
        private float lastMatchCheck = 0f;
        
        // Events
        public event Action<ArenaMatch> OnMatchFound;
        public event Action<GameObject, ArenaMode> OnPlayerJoinedQueue;
        public event Action<GameObject, ArenaMode> OnPlayerLeftQueue;
        
        private void Awake()
        {
            // Initialize queues for each mode
            foreach (ArenaMode mode in Enum.GetValues(typeof(ArenaMode)))
            {
                queues[mode] = new List<ArenaQueueEntry>();
            }
        }
        
        private void Update()
        {
            // Periodically check for matches
            if (Time.time - lastMatchCheck >= matchCheckInterval)
            {
                lastMatchCheck = Time.time;
                CheckForMatches();
            }
        }
        
        /// <summary>
        /// Join queue for arena mode
        /// Tham gia hàng đợi đấu trường
        /// </summary>
        public void JoinQueue(GameObject player, ArenaMode mode, int playerRating)
        {
            // Check if already in queue
            if (IsInQueue(player))
            {
                Debug.LogWarning($"{player.name} is already in queue");
                return;
            }
            
            ArenaQueueEntry entry = new ArenaQueueEntry(player, mode, playerRating);
            queues[mode].Add(entry);
            
            OnPlayerJoinedQueue?.Invoke(player, mode);
            Debug.Log($"{player.name} joined {mode} queue (Rating: {playerRating})");
        }
        
        /// <summary>
        /// Leave queue
        /// Rời khỏi hàng đợi
        /// </summary>
        public void LeaveQueue(GameObject player)
        {
            foreach (var mode in queues.Keys)
            {
                var entry = queues[mode].FirstOrDefault(e => e.player == player);
                if (entry != null)
                {
                    queues[mode].Remove(entry);
                    OnPlayerLeftQueue?.Invoke(player, mode);
                    Debug.Log($"{player.name} left {mode} queue");
                    return;
                }
            }
        }
        
        /// <summary>
        /// Check if player is in any queue
        /// Kiểm tra người chơi có trong hàng đợi không
        /// </summary>
        public bool IsInQueue(GameObject player)
        {
            foreach (var queue in queues.Values)
            {
                if (queue.Any(e => e.player == player))
                    return true;
            }
            return false;
        }
        
        /// <summary>
        /// Get queue position for player
        /// Lấy vị trí trong hàng đợi
        /// </summary>
        public int GetQueuePosition(GameObject player, ArenaMode mode)
        {
            var queue = queues[mode];
            for (int i = 0; i < queue.Count; i++)
            {
                if (queue[i].player == player)
                    return i + 1;
            }
            return -1;
        }
        
        /// <summary>
        /// Get number of players in queue for mode
        /// Lấy số người trong hàng đợi
        /// </summary>
        public int GetQueueCount(ArenaMode mode)
        {
            return queues[mode].Count;
        }
        
        /// <summary>
        /// Check for possible matches
        /// Kiểm tra khả năng ghép trận
        /// </summary>
        private void CheckForMatches()
        {
            foreach (var mode in queues.Keys)
            {
                var queue = queues[mode];
                if (queue.Count < GetRequiredPlayers(mode))
                    continue;
                
                ArenaMatch match = FindMatch(mode);
                if (match != null)
                {
                    // Remove matched players from queue
                    foreach (var player in match.team1.Concat(match.team2))
                    {
                        queue.RemoveAll(e => e.player == player);
                    }
                    
                    OnMatchFound?.Invoke(match);
                }
            }
        }
        
        /// <summary>
        /// Find a match for the given mode
        /// Tìm trận đấu cho chế độ
        /// </summary>
        public ArenaMatch FindMatch(ArenaMode mode)
        {
            var queue = queues[mode];
            if (queue.Count < GetRequiredPlayers(mode))
                return null;
            
            // Sort by rating for better matchmaking
            queue.Sort((a, b) => a.rating.CompareTo(b.rating));
            
            int playersPerTeam = GetPlayersPerTeam(mode);
            List<ArenaQueueEntry> potentialMatch = new List<ArenaQueueEntry>();
            
            // Try to find balanced match
            for (int i = 0; i < queue.Count && potentialMatch.Count < playersPerTeam * 2; i++)
            {
                var entry = queue[i];
                
                if (potentialMatch.Count == 0)
                {
                    potentialMatch.Add(entry);
                }
                else
                {
                    // Check ELO range
                    int avgRating = (int)potentialMatch.Average(e => e.rating);
                    int currentRange = GetEffectiveELORange(entry);
                    
                    if (Mathf.Abs(entry.rating - avgRating) <= currentRange)
                    {
                        potentialMatch.Add(entry);
                    }
                }
            }
            
            // Check if we have enough players
            if (potentialMatch.Count < playersPerTeam * 2)
                return null;
            
            // Create match
            ArenaMatch match = new ArenaMatch(mode);
            
            // Balance teams by rating
            potentialMatch = potentialMatch.OrderBy(e => e.rating).ToList();
            for (int i = 0; i < potentialMatch.Count; i++)
            {
                if (i % 2 == 0)
                    match.team1.Add(potentialMatch[i].player);
                else
                    match.team2.Add(potentialMatch[i].player);
            }
            
            return match;
        }
        
        /// <summary>
        /// Get effective ELO range (increases with wait time)
        /// Lấy khoảng ELO hiệu quả (tăng theo thời gian chờ)
        /// </summary>
        private int GetEffectiveELORange(ArenaQueueEntry entry)
        {
            float waitTime = entry.GetWaitTime();
            float multiplier = 1f + (waitTime / maxWaitTime);
            return Mathf.RoundToInt(eloRange * multiplier);
        }
        
        /// <summary>
        /// Get required number of players for mode
        /// Lấy số người chơi cần cho chế độ
        /// </summary>
        private int GetRequiredPlayers(ArenaMode mode)
        {
            return GetPlayersPerTeam(mode) * 2;
        }
        
        /// <summary>
        /// Get players per team for mode
        /// Lấy số người mỗi đội
        /// </summary>
        private int GetPlayersPerTeam(ArenaMode mode)
        {
            switch (mode)
            {
                case ArenaMode.Solo1v1: return 1;
                case ArenaMode.Solo2v2: return 2;
                case ArenaMode.Solo3v3: return 3;
                case ArenaMode.Team5v5: return 5;
                case ArenaMode.FreeForAll: return 1; // Special case
                default: return 1;
            }
        }
    }
}
