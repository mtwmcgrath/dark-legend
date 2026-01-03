using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DarkLegend.PvP
{
    /// <summary>
    /// Arena Ranking - Xếp hạng đấu trường
    /// </summary>
    [Serializable]
    public class ArenaRankingEntry
    {
        public string playerId;
        public string playerName;
        public int rating;
        public int wins;
        public int losses;
        public int rank;
        
        public ArenaRankingEntry(string playerId, string playerName, int rating, int wins, int losses)
        {
            this.playerId = playerId;
            this.playerName = playerName;
            this.rating = rating;
            this.wins = wins;
            this.losses = losses;
        }
    }

    /// <summary>
    /// Arena Ranking System - Hệ thống xếp hạng đấu trường
    /// </summary>
    public class ArenaRanking : MonoBehaviour
    {
        [Header("Rankings by Mode")]
        private Dictionary<ArenaMode, List<ArenaRankingEntry>> rankings = new Dictionary<ArenaMode, List<ArenaRankingEntry>>();
        
        // Overall ranking (combined all modes)
        private List<ArenaRankingEntry> overallRanking = new List<ArenaRankingEntry>();
        
        private void Awake()
        {
            // Initialize rankings for each mode
            foreach (ArenaMode mode in Enum.GetValues(typeof(ArenaMode)))
            {
                rankings[mode] = new List<ArenaRankingEntry>();
            }
        }
        
        /// <summary>
        /// Update player ranking
        /// Cập nhật xếp hạng người chơi
        /// </summary>
        public void UpdateRanking(ArenaMode mode, string playerId, string playerName, int rating, int wins, int losses)
        {
            var ranking = rankings[mode];
            var entry = ranking.FirstOrDefault(e => e.playerId == playerId);
            
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
                entry = new ArenaRankingEntry(playerId, playerName, rating, wins, losses);
                ranking.Add(entry);
            }
            
            // Re-sort and update ranks
            RefreshRanking(mode);
        }
        
        /// <summary>
        /// Refresh ranking order
        /// Làm mới thứ tự xếp hạng
        /// </summary>
        private void RefreshRanking(ArenaMode mode)
        {
            var ranking = rankings[mode];
            
            // Sort by rating (descending)
            ranking.Sort((a, b) => b.rating.CompareTo(a.rating));
            
            // Update rank numbers
            for (int i = 0; i < ranking.Count; i++)
            {
                ranking[i].rank = i + 1;
            }
        }
        
        /// <summary>
        /// Get top N players for mode
        /// Lấy top N người chơi
        /// </summary>
        public List<ArenaRankingEntry> GetTopPlayers(ArenaMode mode, int count)
        {
            var ranking = rankings[mode];
            return ranking.Take(count).ToList();
        }
        
        /// <summary>
        /// Get player rank
        /// Lấy hạng của người chơi
        /// </summary>
        public int GetPlayerRank(ArenaMode mode, string playerId)
        {
            var entry = rankings[mode].FirstOrDefault(e => e.playerId == playerId);
            return entry?.rank ?? -1;
        }
        
        /// <summary>
        /// Get player ranking entry
        /// Lấy thông tin xếp hạng người chơi
        /// </summary>
        public ArenaRankingEntry GetPlayerRanking(ArenaMode mode, string playerId)
        {
            return rankings[mode].FirstOrDefault(e => e.playerId == playerId);
        }
        
        /// <summary>
        /// Get total players in mode
        /// Lấy tổng số người chơi trong chế độ
        /// </summary>
        public int GetTotalPlayers(ArenaMode mode)
        {
            return rankings[mode].Count;
        }
    }
}
