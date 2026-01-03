using UnityEngine;
using System;
using System.Collections.Generic;

namespace DarkLegend.PvP
{
    /// <summary>
    /// Arena Manager - Quản lý hệ thống đấu trường
    /// Main controller for arena system
    /// </summary>
    public class ArenaManager : MonoBehaviour
    {
        [Header("Systems")]
        private ArenaQueue arenaQueue;
        private ArenaRanking arenaRanking;
        private ArenaRewardSystem arenaReward;
        
        [Header("Current Season")]
        private ArenaSeason currentSeason;
        
        // Active matches
        private List<ArenaMatch> activeMatches = new List<ArenaMatch>();
        
        // Player ratings per mode
        private Dictionary<string, Dictionary<ArenaMode, EloRating>> playerRatings = new Dictionary<string, Dictionary<ArenaMode, EloRating>>();
        
        // Events
        public event Action<ArenaMatch> OnMatchStart;
        public event Action<ArenaMatch, int> OnMatchEnd; // match, winning team
        
        private void Awake()
        {
            InitializeSystems();
            StartNewSeason();
        }
        
        private void InitializeSystems()
        {
            arenaQueue = gameObject.AddComponent<ArenaQueue>();
            arenaRanking = gameObject.AddComponent<ArenaRanking>();
            arenaReward = gameObject.AddComponent<ArenaRewardSystem>();
            
            // Subscribe to queue events
            arenaQueue.OnMatchFound += OnMatchFound;
            
            Debug.Log("Arena Manager initialized");
        }
        
        private void Update()
        {
            // Update active matches
            UpdateMatches();
        }
        
        /// <summary>
        /// Start a new season
        /// Bắt đầu mùa giải mới
        /// </summary>
        private void StartNewSeason()
        {
            int seasonNumber = currentSeason != null ? currentSeason.seasonNumber + 1 : 1;
            currentSeason = new ArenaSeason(seasonNumber);
            Debug.Log($"Arena Season {seasonNumber} started");
        }
        
        /// <summary>
        /// Join arena queue
        /// Tham gia hàng đợi đấu trường
        /// </summary>
        public void JoinQueue(GameObject player, ArenaMode mode)
        {
            string playerId = player.GetInstanceID().ToString();
            int rating = GetPlayerRating(playerId, mode).rating;
            arenaQueue.JoinQueue(player, mode, rating);
        }
        
        /// <summary>
        /// Leave arena queue
        /// Rời khỏi hàng đợi
        /// </summary>
        public void LeaveQueue(GameObject player)
        {
            arenaQueue.LeaveQueue(player);
        }
        
        /// <summary>
        /// Get player rating for mode
        /// Lấy rating của người chơi
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
        /// Handle match found event
        /// Xử lý khi tìm thấy trận đấu
        /// </summary>
        private void OnMatchFound(ArenaMatch match)
        {
            activeMatches.Add(match);
            match.StartMatch();
            
            // Subscribe to match events
            match.OnMatchEnd += (winningTeam) => OnMatchComplete(match, winningTeam);
            
            OnMatchStart?.Invoke(match);
            Debug.Log($"Arena match started: {match.matchId}");
        }
        
        /// <summary>
        /// Handle match completion
        /// Xử lý khi trận đấu kết thúc
        /// </summary>
        private void OnMatchComplete(ArenaMatch match, int winningTeam)
        {
            activeMatches.Remove(match);
            
            // Update ratings and rankings
            UpdateRatingsAfterMatch(match, winningTeam);
            
            // Give rewards
            GiveMatchRewards(match, winningTeam);
            
            OnMatchEnd?.Invoke(match, winningTeam);
        }
        
        /// <summary>
        /// Update ratings after match
        /// Cập nhật rating sau trận đấu
        /// </summary>
        private void UpdateRatingsAfterMatch(ArenaMatch match, int winningTeam)
        {
            // Calculate average ratings for each team
            List<GameObject> winners = winningTeam == 1 ? match.team1 : match.team2;
            List<GameObject> losers = winningTeam == 1 ? match.team2 : match.team1;
            
            float avgWinnerRating = 0;
            float avgLoserRating = 0;
            
            foreach (var player in winners)
            {
                string playerId = player.GetInstanceID().ToString();
                avgWinnerRating += GetPlayerRating(playerId, match.mode).rating;
            }
            avgWinnerRating /= winners.Count;
            
            foreach (var player in losers)
            {
                string playerId = player.GetInstanceID().ToString();
                avgLoserRating += GetPlayerRating(playerId, match.mode).rating;
            }
            avgLoserRating /= losers.Count;
            
            // Calculate rating changes (simplified - use average ratings)
            EloRating tempWinner = new EloRating { rating = (int)avgWinnerRating };
            EloRating tempLoser = new EloRating { rating = (int)avgLoserRating };
            var (newWinnerRating, newLoserRating) = EloRating.CalculateNewRatings(tempWinner, tempLoser, 1.0f);
            
            int ratingChange = newWinnerRating - (int)avgWinnerRating;
            
            // Apply rating changes to all players
            foreach (var player in winners)
            {
                string playerId = player.GetInstanceID().ToString();
                var rating = GetPlayerRating(playerId, match.mode);
                rating.UpdateAfterMatch(true, ratingChange);
                
                // Update ranking
                arenaRanking.UpdateRanking(match.mode, playerId, player.name, rating.rating, rating.wins, rating.losses);
            }
            
            foreach (var player in losers)
            {
                string playerId = player.GetInstanceID().ToString();
                var rating = GetPlayerRating(playerId, match.mode);
                rating.UpdateAfterMatch(false, -ratingChange);
                
                // Update ranking
                arenaRanking.UpdateRanking(match.mode, playerId, player.name, rating.rating, rating.wins, rating.losses);
            }
        }
        
        /// <summary>
        /// Give rewards to players
        /// Trao phần thưởng cho người chơi
        /// </summary>
        private void GiveMatchRewards(ArenaMatch match, int winningTeam)
        {
            List<GameObject> winners = winningTeam == 1 ? match.team1 : match.team2;
            List<GameObject> losers = winningTeam == 1 ? match.team2 : match.team1;
            
            foreach (var player in winners)
            {
                var (kills, deaths) = match.GetPlayerStats(player);
                var reward = arenaReward.GetMatchReward(true, 20, kills);
                // TODO: Give reward to player
            }
            
            foreach (var player in losers)
            {
                var (kills, deaths) = match.GetPlayerStats(player);
                var reward = arenaReward.GetMatchReward(false, 0, kills);
                // TODO: Give reward to player
            }
        }
        
        /// <summary>
        /// Update all active matches
        /// Cập nhật tất cả trận đấu đang hoạt động
        /// </summary>
        private void UpdateMatches()
        {
            for (int i = activeMatches.Count - 1; i >= 0; i--)
            {
                var match = activeMatches[i];
                
                // Check if match should end due to time limit
                if (match.state == MatchState.InProgress && match.GetTimeRemaining() <= 0)
                {
                    // End match, winner is team with higher score
                    int winningTeam = match.team1Score > match.team2Score ? 1 : 
                                     (match.team2Score > match.team1Score ? 2 : 0);
                    match.EndMatch(winningTeam);
                }
            }
        }
        
        /// <summary>
        /// Get current season
        /// Lấy mùa giải hiện tại
        /// </summary>
        public ArenaSeason GetCurrentSeason()
        {
            return currentSeason;
        }
        
        /// <summary>
        /// Get arena ranking system
        /// Lấy hệ thống xếp hạng
        /// </summary>
        public ArenaRanking GetRanking()
        {
            return arenaRanking;
        }
    }
}
