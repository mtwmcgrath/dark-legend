using UnityEngine;
using System;
using System.Collections.Generic;

namespace DarkLegend.PvP
{
    /// <summary>
    /// Base class for battleground modes
    /// Lớp cơ sở cho các chế độ battleground
    /// </summary>
    public abstract class BattlegroundMode : MonoBehaviour
    {
        [Header("Mode Settings")]
        public string modeName;
        public int teamSize;
        public int timeLimit;                     // Seconds
        
        [Header("Match State")]
        protected MatchState state = MatchState.Waiting;
        protected float startTime;
        protected float endTime;
        
        protected List<GameObject> team1 = new List<GameObject>();
        protected List<GameObject> team2 = new List<GameObject>();
        protected int team1Score = 0;
        protected int team2Score = 0;
        
        // Events
        public event Action OnMatchStart;
        public event Action<int> OnMatchEnd;      // Winning team
        public event Action<int, int> OnScoreUpdate; // team1Score, team2Score
        
        /// <summary>
        /// Initialize the match
        /// Khởi tạo trận đấu
        /// </summary>
        public virtual void InitializeMatch(List<GameObject> team1, List<GameObject> team2)
        {
            this.team1 = team1;
            this.team2 = team2;
            this.team1Score = 0;
            this.team2Score = 0;
            this.state = MatchState.Waiting;
        }
        
        /// <summary>
        /// Start the match
        /// Bắt đầu trận đấu
        /// </summary>
        public virtual void StartMatch()
        {
            startTime = Time.time;
            endTime = startTime + timeLimit;
            state = MatchState.InProgress;
            
            OnMatchStart?.Invoke();
            Debug.Log($"Battleground match started: {modeName}");
        }
        
        /// <summary>
        /// End the match
        /// Kết thúc trận đấu
        /// </summary>
        public virtual void EndMatch(int winningTeam)
        {
            state = MatchState.Ended;
            OnMatchEnd?.Invoke(winningTeam);
            Debug.Log($"Battleground match ended. Winner: Team {winningTeam}");
        }
        
        /// <summary>
        /// Update score
        /// Cập nhật điểm
        /// </summary>
        protected virtual void UpdateScore(int team, int points)
        {
            if (team == 1)
                team1Score += points;
            else if (team == 2)
                team2Score += points;
            
            OnScoreUpdate?.Invoke(team1Score, team2Score);
        }
        
        /// <summary>
        /// Check win conditions
        /// Kiểm tra điều kiện thắng
        /// </summary>
        protected abstract bool CheckWinCondition();
        
        /// <summary>
        /// Get time remaining
        /// Lấy thời gian còn lại
        /// </summary>
        public float GetTimeRemaining()
        {
            return Mathf.Max(0, endTime - Time.time);
        }
        
        protected virtual void Update()
        {
            if (state == MatchState.InProgress)
            {
                // Check time limit
                if (Time.time >= endTime)
                {
                    // End match based on score
                    int winner = team1Score > team2Score ? 1 : (team2Score > team1Score ? 2 : 0);
                    EndMatch(winner);
                }
                
                // Check win condition
                if (CheckWinCondition())
                {
                    int winner = team1Score > team2Score ? 1 : 2;
                    EndMatch(winner);
                }
            }
        }
    }
}
