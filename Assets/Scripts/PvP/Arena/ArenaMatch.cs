using UnityEngine;
using System;
using System.Collections.Generic;

namespace DarkLegend.PvP
{
    /// <summary>
    /// Arena Match - Trận đấu trong đấu trường
    /// </summary>
    public class ArenaMatch
    {
        public string matchId;
        public ArenaMode mode;
        public List<GameObject> team1 = new List<GameObject>();
        public List<GameObject> team2 = new List<GameObject>();
        public int timeLimit;                     // Seconds
        public int team1Score = 0;
        public int team2Score = 0;
        public int killsToWin;
        public float startTime;
        public float endTime;
        public MatchState state = MatchState.Waiting;
        
        // Kill tracking
        private Dictionary<GameObject, int> playerKills = new Dictionary<GameObject, int>();
        private Dictionary<GameObject, int> playerDeaths = new Dictionary<GameObject, int>();
        private GameObject lastKiller;
        private float lastKillTime;
        
        // Events
        public event Action<GameObject, GameObject> OnKill;
        public event Action<int> OnTeamScore;
        public event Action<int> OnMatchEnd;      // Winning team (1 or 2)
        
        public ArenaMatch(ArenaMode mode)
        {
            this.mode = mode;
            this.matchId = Guid.NewGuid().ToString();
            
            // Set kills to win based on mode
            switch (mode)
            {
                case ArenaMode.Solo1v1:
                    killsToWin = 10;
                    break;
                case ArenaMode.Solo2v2:
                case ArenaMode.Solo3v3:
                    killsToWin = 20;
                    break;
                case ArenaMode.Team5v5:
                    killsToWin = 50;
                    break;
                default:
                    killsToWin = 10;
                    break;
            }
        }
        
        /// <summary>
        /// Start the match
        /// Bắt đầu trận đấu
        /// </summary>
        public void StartMatch(int timeLimitSeconds = 900)
        {
            this.timeLimit = timeLimitSeconds;
            this.startTime = Time.time;
            this.endTime = startTime + timeLimit;
            this.state = MatchState.InProgress;
            
            Debug.Log($"Arena match {matchId} started: {mode}");
        }
        
        /// <summary>
        /// Record a kill
        /// Ghi nhận một kill
        /// </summary>
        public void RecordKill(GameObject killer, GameObject victim)
        {
            if (state != MatchState.InProgress) return;
            
            // Update kill/death counts
            if (!playerKills.ContainsKey(killer))
                playerKills[killer] = 0;
            playerKills[killer]++;
            
            if (!playerDeaths.ContainsKey(victim))
                playerDeaths[victim] = 0;
            playerDeaths[victim]++;
            
            // Calculate score bonuses
            int score = 100; // Base kill score
            
            // First blood bonus
            if (team1Score == 0 && team2Score == 0)
            {
                score += 150;
            }
            
            // Killing spree bonus
            int killerStreak = GetPlayerKillStreak(killer);
            if (killerStreak >= 3)
            {
                score += 50 * (killerStreak - 2);
            }
            
            // Multi-kill bonus (within 5 seconds)
            if (lastKiller == killer && (Time.time - lastKillTime) < 5f)
            {
                score += 50; // Double kill, etc.
            }
            
            lastKiller = killer;
            lastKillTime = Time.time;
            
            // Update team score
            if (team1.Contains(killer))
            {
                team1Score++;
                OnTeamScore?.Invoke(1);
            }
            else if (team2.Contains(killer))
            {
                team2Score++;
                OnTeamScore?.Invoke(2);
            }
            
            OnKill?.Invoke(killer, victim);
            
            // Check win condition
            CheckWinCondition();
        }
        
        /// <summary>
        /// Get player kill streak
        /// Lấy chuỗi kill của người chơi
        /// </summary>
        public int GetPlayerKillStreak(GameObject player)
        {
            return playerKills.ContainsKey(player) ? playerKills[player] : 0;
        }
        
        /// <summary>
        /// Check if match should end
        /// Kiểm tra nếu trận đấu kết thúc
        /// </summary>
        private void CheckWinCondition()
        {
            // Check kill limit
            if (team1Score >= killsToWin)
            {
                EndMatch(1);
            }
            else if (team2Score >= killsToWin)
            {
                EndMatch(2);
            }
            // Check time limit
            else if (Time.time >= endTime)
            {
                EndMatch(team1Score > team2Score ? 1 : (team2Score > team1Score ? 2 : 0));
            }
        }
        
        /// <summary>
        /// End the match
        /// Kết thúc trận đấu
        /// </summary>
        public void EndMatch(int winningTeam)
        {
            state = MatchState.Ended;
            OnMatchEnd?.Invoke(winningTeam);
            
            Debug.Log($"Arena match {matchId} ended. Winner: Team {winningTeam}");
        }
        
        /// <summary>
        /// Get time remaining
        /// Lấy thời gian còn lại
        /// </summary>
        public float GetTimeRemaining()
        {
            return Mathf.Max(0, endTime - Time.time);
        }
        
        /// <summary>
        /// Get player statistics
        /// Lấy thống kê người chơi
        /// </summary>
        public (int kills, int deaths) GetPlayerStats(GameObject player)
        {
            int kills = playerKills.ContainsKey(player) ? playerKills[player] : 0;
            int deaths = playerDeaths.ContainsKey(player) ? playerDeaths[player] : 0;
            return (kills, deaths);
        }
    }
}
