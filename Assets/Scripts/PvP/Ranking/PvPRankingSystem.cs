using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace DarkLegend.PvP
{
    /// <summary>
    /// PvP Ranking System - Hệ thống xếp hạng PvP tổng hợp
    /// </summary>
    public class PvPRankingSystem : MonoBehaviour
    {
        [Header("Components")]
        private Leaderboard leaderboard;
        
        [Header("Titles")]
        public List<PvPTitle> availableTitles = new List<PvPTitle>();
        
        // Player data
        private Dictionary<string, Dictionary<ArenaMode, EloRating>> playerRatings = new Dictionary<string, Dictionary<ArenaMode, EloRating>>();
        private Dictionary<string, PlayerPvPStats> playerStats = new Dictionary<string, PlayerPvPStats>();
        private Dictionary<string, List<PvPTitle>> playerTitles = new Dictionary<string, List<PvPTitle>>();
        private Dictionary<string, PvPTitle> activeTitles = new Dictionary<string, PvPTitle>();
        
        // Events
        public event Action<string, PvPTitle> OnTitleUnlocked;
        public event Action<string, RankTier, RankTier> OnRankChanged; // playerId, oldRank, newRank
        
        private void Awake()
        {
            leaderboard = gameObject.AddComponent<Leaderboard>();
        }
        
        /// <summary>
        /// Get player rating for mode
        /// Lấy rating của người chơi cho chế độ
        /// </summary>
        public EloRating GetPlayerRating(string playerId, ArenaMode mode)
        {
            if (!playerRatings.ContainsKey(playerId))
            {
                playerRatings[playerId] = new Dictionary<ArenaMode, EloRating>();
            }
            
            if (!playerRatings[playerId].ContainsKey(mode))
            {
                playerRatings[playerId][mode] = new EloRating();
            }
            
            return playerRatings[playerId][mode];
        }
        
        /// <summary>
        /// Update player rating after match
        /// Cập nhật rating sau trận đấu
        /// </summary>
        public void UpdateRating(string playerId, string playerName, ArenaMode mode, bool won, int ratingChange)
        {
            var rating = GetPlayerRating(playerId, mode);
            RankTier oldRank = rating.GetRankTier();
            
            rating.UpdateAfterMatch(won, ratingChange);
            
            // Update leaderboard
            leaderboard.UpdateEntry(mode, playerId, playerName, rating.rating, rating.wins, rating.losses);
            
            // Check for rank change
            RankTier newRank = rating.GetRankTier();
            if (oldRank != newRank)
            {
                OnRankChanged?.Invoke(playerId, oldRank, newRank);
                
                // Update highest rank
                var stats = GetPlayerStats(playerId);
                if (newRank > stats.highestRank)
                {
                    stats.highestRank = newRank;
                }
            }
            
            // Update stats
            UpdatePlayerStats(playerId, won, mode);
            
            // Check for new titles
            CheckTitles(playerId);
        }
        
        /// <summary>
        /// Get player stats
        /// Lấy thống kê người chơi
        /// </summary>
        public PlayerPvPStats GetPlayerStats(string playerId)
        {
            if (!playerStats.ContainsKey(playerId))
            {
                playerStats[playerId] = new PlayerPvPStats();
            }
            return playerStats[playerId];
        }
        
        /// <summary>
        /// Update player statistics
        /// Cập nhật thống kê người chơi
        /// </summary>
        private void UpdatePlayerStats(string playerId, bool won, ArenaMode mode)
        {
            var stats = GetPlayerStats(playerId);
            
            if (won)
            {
                stats.arenaWins++;
            }
        }
        
        /// <summary>
        /// Add PvP kill to stats
        /// Thêm kill PvP vào thống kê
        /// </summary>
        public void AddPvPKill(string playerId)
        {
            var stats = GetPlayerStats(playerId);
            stats.totalKills++;
            
            CheckTitles(playerId);
        }
        
        /// <summary>
        /// Add duel win to stats
        /// Thêm duel thắng vào thống kê
        /// </summary>
        public void AddDuelWin(string playerId)
        {
            var stats = GetPlayerStats(playerId);
            stats.duelWins++;
            
            CheckTitles(playerId);
        }
        
        /// <summary>
        /// Check and unlock titles
        /// Kiểm tra và mở khóa danh hiệu
        /// </summary>
        private void CheckTitles(string playerId)
        {
            var stats = GetPlayerStats(playerId);
            
            if (!playerTitles.ContainsKey(playerId))
            {
                playerTitles[playerId] = new List<PvPTitle>();
            }
            
            var unlockedTitles = playerTitles[playerId];
            
            foreach (var title in availableTitles)
            {
                if (!unlockedTitles.Contains(title) && title.MeetsRequirement(stats))
                {
                    UnlockTitle(playerId, title);
                }
            }
        }
        
        /// <summary>
        /// Unlock title for player
        /// Mở khóa danh hiệu cho người chơi
        /// </summary>
        private void UnlockTitle(string playerId, PvPTitle title)
        {
            if (!playerTitles.ContainsKey(playerId))
            {
                playerTitles[playerId] = new List<PvPTitle>();
            }
            
            playerTitles[playerId].Add(title);
            OnTitleUnlocked?.Invoke(playerId, title);
            
            Debug.Log($"Player {playerId} unlocked title: {title.titleName}");
        }
        
        /// <summary>
        /// Set active title for player
        /// Đặt danh hiệu hiện tại cho người chơi
        /// </summary>
        public bool SetActiveTitle(string playerId, PvPTitle title)
        {
            // Check if player has this title
            if (!playerTitles.ContainsKey(playerId) || !playerTitles[playerId].Contains(title))
            {
                return false;
            }
            
            activeTitles[playerId] = title;
            Debug.Log($"Player {playerId} equipped title: {title.titleName}");
            return true;
        }
        
        /// <summary>
        /// Get active title for player
        /// Lấy danh hiệu hiện tại của người chơi
        /// </summary>
        public PvPTitle GetActiveTitle(string playerId)
        {
            return activeTitles.ContainsKey(playerId) ? activeTitles[playerId] : null;
        }
        
        /// <summary>
        /// Get unlocked titles for player
        /// Lấy danh hiệu đã mở khóa của người chơi
        /// </summary>
        public List<PvPTitle> GetUnlockedTitles(string playerId)
        {
            return playerTitles.ContainsKey(playerId) ? playerTitles[playerId] : new List<PvPTitle>();
        }
        
        /// <summary>
        /// Get leaderboard
        /// Lấy bảng xếp hạng
        /// </summary>
        public Leaderboard GetLeaderboard()
        {
            return leaderboard;
        }
        
        /// <summary>
        /// Get rank tier name
        /// Lấy tên hạng
        /// </summary>
        public static string GetRankTierName(RankTier tier)
        {
            return tier.ToString().Replace("_", " ");
        }
    }
}
