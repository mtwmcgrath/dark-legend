using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DarkLegend.PvP
{
    /// <summary>
    /// Leaderboard Entry - Mục xếp hạng
    /// </summary>
    [Serializable]
    public class LeaderboardEntry
    {
        public int rank;
        public string playerId;
        public string playerName;
        public int rating;
        public int wins;
        public int losses;
        public RankTier tier;
        
        public float WinRate => wins + losses > 0 ? (float)wins / (wins + losses) * 100f : 0f;
    }

    /// <summary>
    /// Leaderboard - Bảng xếp hạng
    /// </summary>
    public class Leaderboard : MonoBehaviour
    {
        [Header("Settings")]
        public int entriesPerPage = 30;
        public int challengerTopCount = 100;      // Top 100 for Challenger
        
        private Dictionary<ArenaMode, List<LeaderboardEntry>> leaderboards = new Dictionary<ArenaMode, List<LeaderboardEntry>>();
        private List<LeaderboardEntry> overallLeaderboard = new List<LeaderboardEntry>();
        
        // Events
        public event Action<ArenaMode> OnLeaderboardUpdated;
        
        private void Awake()
        {
            // Initialize leaderboards for each mode
            foreach (ArenaMode mode in Enum.GetValues(typeof(ArenaMode)))
            {
                leaderboards[mode] = new List<LeaderboardEntry>();
            }
        }
        
        /// <summary>
        /// Update player entry in leaderboard
        /// Cập nhật mục người chơi trong bảng xếp hạng
        /// </summary>
        public void UpdateEntry(ArenaMode mode, string playerId, string playerName, int rating, int wins, int losses)
        {
            var leaderboard = leaderboards[mode];
            var entry = leaderboard.FirstOrDefault(e => e.playerId == playerId);
            
            if (entry != null)
            {
                // Update existing entry
                entry.rating = rating;
                entry.wins = wins;
                entry.losses = losses;
            }
            else
            {
                // Add new entry
                entry = new LeaderboardEntry
                {
                    playerId = playerId,
                    playerName = playerName,
                    rating = rating,
                    wins = wins,
                    losses = losses
                };
                leaderboard.Add(entry);
            }
            
            // Update tier
            EloRating tempRating = new EloRating { rating = rating };
            entry.tier = tempRating.GetRankTier();
            
            // Re-sort and update ranks
            RefreshLeaderboard(mode);
        }
        
        /// <summary>
        /// Refresh leaderboard rankings
        /// Làm mới thứ tự xếp hạng
        /// </summary>
        private void RefreshLeaderboard(ArenaMode mode)
        {
            var leaderboard = leaderboards[mode];
            
            // Sort by rating (descending)
            leaderboard.Sort((a, b) => b.rating.CompareTo(a.rating));
            
            // Update rank numbers and check Challenger threshold
            for (int i = 0; i < leaderboard.Count; i++)
            {
                leaderboard[i].rank = i + 1;
                
                // Update tier for top players (Challenger check)
                if (i < challengerTopCount && leaderboard[i].rating >= 2900)
                {
                    leaderboard[i].tier = RankTier.Challenger;
                }
            }
            
            OnLeaderboardUpdated?.Invoke(mode);
        }
        
        /// <summary>
        /// Get top N entries
        /// Lấy top N mục
        /// </summary>
        public List<LeaderboardEntry> GetTopEntries(ArenaMode mode, int count)
        {
            return leaderboards[mode].Take(count).ToList();
        }
        
        /// <summary>
        /// Get entries by page
        /// Lấy mục theo trang
        /// </summary>
        public List<LeaderboardEntry> GetEntriesByPage(ArenaMode mode, int page)
        {
            var leaderboard = leaderboards[mode];
            int skip = (page - 1) * entriesPerPage;
            return leaderboard.Skip(skip).Take(entriesPerPage).ToList();
        }
        
        /// <summary>
        /// Get player entry
        /// Lấy mục của người chơi
        /// </summary>
        public LeaderboardEntry GetPlayerEntry(ArenaMode mode, string playerId)
        {
            return leaderboards[mode].FirstOrDefault(e => e.playerId == playerId);
        }
        
        /// <summary>
        /// Get player rank
        /// Lấy hạng của người chơi
        /// </summary>
        public int GetPlayerRank(ArenaMode mode, string playerId)
        {
            var entry = GetPlayerEntry(mode, playerId);
            return entry?.rank ?? -1;
        }
        
        /// <summary>
        /// Get total entries
        /// Lấy tổng số mục
        /// </summary>
        public int GetTotalEntries(ArenaMode mode)
        {
            return leaderboards[mode].Count;
        }
        
        /// <summary>
        /// Get total pages
        /// Lấy tổng số trang
        /// </summary>
        public int GetTotalPages(ArenaMode mode)
        {
            return Mathf.CeilToInt((float)GetTotalEntries(mode) / entriesPerPage);
        }
        
        /// <summary>
        /// Search players by name
        /// Tìm người chơi theo tên
        /// </summary>
        public List<LeaderboardEntry> SearchPlayers(ArenaMode mode, string searchTerm)
        {
            return leaderboards[mode]
                .Where(e => e.playerName.ToLower().Contains(searchTerm.ToLower()))
                .ToList();
        }
    }
}
