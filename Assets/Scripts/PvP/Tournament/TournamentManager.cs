using UnityEngine;
using System;
using System.Collections.Generic;

namespace DarkLegend.PvP
{
    /// <summary>
    /// Tournament Manager - Quản lý tournaments
    /// </summary>
    public class TournamentManager : MonoBehaviour
    {
        [Header("Tournament Schedule")]
        public bool enableWeeklyTournament = true;
        public DayOfWeek weeklyTournamentDay = DayOfWeek.Sunday;
        public int weeklyTournamentHour = 20;     // 8 PM
        
        public bool enableMonthlyTournament = true;
        public int monthlyTournamentDay = 1;      // First Saturday
        public int monthlyTournamentHour = 19;    // 7 PM
        
        // Active tournaments
        private List<TournamentBracket> activeTournaments = new List<TournamentBracket>();
        
        // Reward system
        private TournamentRewardSystem rewardSystem;
        
        // Events
        public event Action<TournamentBracket> OnTournamentStart;
        public event Action<TournamentBracket, GameObject> OnTournamentEnd; // bracket, winner
        
        private void Awake()
        {
            rewardSystem = gameObject.AddComponent<TournamentRewardSystem>();
        }
        
        /// <summary>
        /// Create new tournament
        /// Tạo tournament mới
        /// </summary>
        public TournamentBracket CreateTournament(TournamentType type, int maxParticipants = 16)
        {
            GameObject bracketObj = new GameObject($"Tournament_{type}");
            bracketObj.transform.SetParent(transform);
            
            TournamentBracket bracket = bracketObj.AddComponent<TournamentBracket>();
            bracket.tournamentType = type;
            bracket.maxParticipants = maxParticipants;
            bracket.bracketType = BracketType.SingleElimination;
            
            // Set prize pool based on type
            bracket.prizePool = GetPrizePool(type);
            
            // Subscribe to events
            bracket.OnTournamentComplete += (winner) => OnTournamentComplete(bracket, winner);
            
            activeTournaments.Add(bracket);
            
            Debug.Log($"Created tournament: {type}");
            return bracket;
        }
        
        /// <summary>
        /// Register player for tournament
        /// Đăng ký người chơi cho tournament
        /// </summary>
        public bool RegisterPlayer(TournamentBracket bracket, GameObject player)
        {
            return bracket.RegisterParticipant(player);
        }
        
        /// <summary>
        /// Start tournament
        /// Bắt đầu tournament
        /// </summary>
        public void StartTournament(TournamentBracket bracket)
        {
            bracket.StartTournament();
            OnTournamentStart?.Invoke(bracket);
        }
        
        /// <summary>
        /// Handle tournament completion
        /// Xử lý khi tournament kết thúc
        /// </summary>
        private void OnTournamentComplete(TournamentBracket bracket, GameObject winner)
        {
            OnTournamentEnd?.Invoke(bracket, winner);
            
            // Remove from active list
            activeTournaments.Remove(bracket);
            
            // Clean up after some time
            Destroy(bracket.gameObject, 60f);
        }
        
        /// <summary>
        /// Get prize pool for tournament type
        /// Lấy giải thưởng cho loại tournament
        /// </summary>
        private int GetPrizePool(TournamentType type)
        {
            switch (type)
            {
                case TournamentType.Weekly1v1:
                    return 1000000;       // 1M Zen
                case TournamentType.Monthly2v2:
                    return 5000000;       // 5M Zen
                case TournamentType.Seasonal5v5:
                    return 10000000;      // 10M Zen
                case TournamentType.GuildTournament:
                    return 20000000;      // 20M Zen
                case TournamentType.WorldChampionship:
                    return 100000000;     // 100M Zen
                default:
                    return 1000000;
            }
        }
        
        /// <summary>
        /// Check and schedule tournaments
        /// Kiểm tra và lên lịch tournaments
        /// </summary>
        private void CheckScheduledTournaments()
        {
            DateTime now = DateTime.Now;
            
            // Check weekly tournament
            if (enableWeeklyTournament && now.DayOfWeek == weeklyTournamentDay && now.Hour == weeklyTournamentHour)
            {
                // Check if not already created
                if (!HasActiveTournament(TournamentType.Weekly1v1))
                {
                    CreateTournament(TournamentType.Weekly1v1, 16);
                }
            }
            
            // Check monthly tournament
            if (enableMonthlyTournament)
            {
                // First Saturday of the month
                if (now.Day <= 7 && now.DayOfWeek == DayOfWeek.Saturday && now.Hour == monthlyTournamentHour)
                {
                    if (!HasActiveTournament(TournamentType.Monthly2v2))
                    {
                        CreateTournament(TournamentType.Monthly2v2, 32);
                    }
                }
            }
        }
        
        /// <summary>
        /// Check if tournament type is active
        /// Kiểm tra loại tournament có đang hoạt động không
        /// </summary>
        private bool HasActiveTournament(TournamentType type)
        {
            return activeTournaments.Exists(t => t.tournamentType == type);
        }
        
        /// <summary>
        /// Get active tournaments
        /// Lấy tournaments đang hoạt động
        /// </summary>
        public List<TournamentBracket> GetActiveTournaments()
        {
            return new List<TournamentBracket>(activeTournaments);
        }
        
        /// <summary>
        /// Get tournament by type
        /// Lấy tournament theo loại
        /// </summary>
        public TournamentBracket GetTournament(TournamentType type)
        {
            return activeTournaments.Find(t => t.tournamentType == type);
        }
    }
}
