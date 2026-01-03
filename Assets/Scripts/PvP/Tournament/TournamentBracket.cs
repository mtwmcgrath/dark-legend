using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DarkLegend.PvP
{
    /// <summary>
    /// Tournament Bracket - Bảng đấu tournament
    /// </summary>
    public class TournamentBracket : MonoBehaviour
    {
        [Header("Settings")]
        public TournamentType tournamentType;
        public BracketType bracketType;
        public int maxParticipants = 16;
        
        [Header("Prize Pool")]
        public int prizePool = 1000000;           // Total Zen
        public float[] prizeDistribution = { 0.5f, 0.25f, 0.125f, 0.125f }; // 1st, 2nd, 3rd-4th
        
        // Bracket data
        private List<GameObject> participants = new List<GameObject>();
        private List<TournamentMatch> matches = new List<TournamentMatch>();
        private int currentRound = 1;
        private bool isComplete = false;
        
        // Events
        public event Action<TournamentMatch> OnMatchComplete;
        public event Action<GameObject> OnTournamentComplete; // Winner
        
        /// <summary>
        /// Register participant
        /// Đăng ký người tham gia
        /// </summary>
        public bool RegisterParticipant(GameObject participant)
        {
            if (participants.Count >= maxParticipants)
            {
                Debug.LogWarning("Tournament is full");
                return false;
            }
            
            if (participants.Contains(participant))
            {
                Debug.LogWarning("Participant already registered");
                return false;
            }
            
            participants.Add(participant);
            Debug.Log($"{participant.name} registered for tournament");
            return true;
        }
        
        /// <summary>
        /// Start tournament
        /// Bắt đầu tournament
        /// </summary>
        public void StartTournament()
        {
            if (participants.Count < 2)
            {
                Debug.LogWarning("Not enough participants");
                return;
            }
            
            // Ensure participant count is power of 2
            int targetCount = GetNextPowerOf2(participants.Count);
            while (participants.Count < targetCount)
            {
                // Add byes or wait for more participants
                Debug.LogWarning($"Need {targetCount - participants.Count} more participants");
                return;
            }
            
            // Seed participants (shuffle for fairness)
            ShuffleParticipants();
            
            // Generate first round matches
            GenerateRound(currentRound);
            
            Debug.Log($"Tournament started with {participants.Count} participants");
        }
        
        /// <summary>
        /// Generate matches for a round
        /// Tạo trận đấu cho một vòng
        /// </summary>
        private void GenerateRound(int round)
        {
            List<GameObject> currentPlayers = GetRemainingPlayers(round);
            int matchesInRound = currentPlayers.Count / 2;
            
            for (int i = 0; i < matchesInRound; i++)
            {
                TournamentMatch match = new TournamentMatch(round, i + 1);
                match.team1.Add(currentPlayers[i * 2]);
                match.team2.Add(currentPlayers[i * 2 + 1]);
                matches.Add(match);
                
                Debug.Log($"Round {round}, Match {i + 1}: {match.team1[0].name} vs {match.team2[0].name}");
            }
        }
        
        /// <summary>
        /// Complete a match
        /// Hoàn thành một trận đấu
        /// </summary>
        public void CompleteMatch(string matchId, int winningTeam)
        {
            TournamentMatch match = matches.FirstOrDefault(m => m.matchId == matchId);
            if (match == null || match.isComplete)
            {
                return;
            }
            
            match.winnerId = winningTeam;
            match.isComplete = true;
            
            OnMatchComplete?.Invoke(match);
            
            // Check if round is complete
            if (IsRoundComplete(currentRound))
            {
                AdvanceToNextRound();
            }
        }
        
        /// <summary>
        /// Check if current round is complete
        /// Kiểm tra vòng hiện tại đã xong chưa
        /// </summary>
        private bool IsRoundComplete(int round)
        {
            var roundMatches = matches.Where(m => m.roundNumber == round);
            return roundMatches.All(m => m.isComplete);
        }
        
        /// <summary>
        /// Advance to next round
        /// Chuyển sang vòng tiếp theo
        /// </summary>
        private void AdvanceToNextRound()
        {
            var winners = GetRoundWinners(currentRound);
            
            if (winners.Count == 1)
            {
                // Tournament complete!
                CompleteTournament(winners[0]);
            }
            else
            {
                // Generate next round
                currentRound++;
                GenerateRound(currentRound);
            }
        }
        
        /// <summary>
        /// Get winners from a round
        /// Lấy người thắng từ một vòng
        /// </summary>
        private List<GameObject> GetRoundWinners(int round)
        {
            List<GameObject> winners = new List<GameObject>();
            var roundMatches = matches.Where(m => m.roundNumber == round && m.isComplete);
            
            foreach (var match in roundMatches)
            {
                var winningTeam = match.winnerId == 1 ? match.team1 : match.team2;
                winners.AddRange(winningTeam);
            }
            
            return winners;
        }
        
        /// <summary>
        /// Get remaining players for a round
        /// Lấy người chơi còn lại cho một vòng
        /// </summary>
        private List<GameObject> GetRemainingPlayers(int round)
        {
            if (round == 1)
            {
                return new List<GameObject>(participants);
            }
            return GetRoundWinners(round - 1);
        }
        
        /// <summary>
        /// Complete tournament
        /// Hoàn thành tournament
        /// </summary>
        private void CompleteTournament(GameObject winner)
        {
            isComplete = true;
            OnTournamentComplete?.Invoke(winner);
            
            Debug.Log($"Tournament complete! Winner: {winner.name}");
            
            // Distribute rewards
            DistributeRewards();
        }
        
        /// <summary>
        /// Distribute prizes
        /// Phân phối giải thưởng
        /// </summary>
        private void DistributeRewards()
        {
            // Get final placements
            var placements = CalculatePlacements();
            
            for (int i = 0; i < Mathf.Min(placements.Count, prizeDistribution.Length); i++)
            {
                int prize = Mathf.RoundToInt(prizePool * prizeDistribution[i]);
                Debug.Log($"Place {i + 1}: {placements[i].name} wins {prize} Zen");
                // TODO: Give prize to player
            }
        }
        
        /// <summary>
        /// Calculate final placements
        /// Tính thứ hạng cuối cùng
        /// </summary>
        private List<GameObject> CalculatePlacements()
        {
            List<GameObject> placements = new List<GameObject>();
            
            // Work backwards through rounds to determine placements
            for (int round = currentRound; round >= 1; round--)
            {
                var roundMatches = matches.Where(m => m.roundNumber == round && m.isComplete);
                foreach (var match in roundMatches)
                {
                    var losingTeam = match.winnerId == 1 ? match.team2 : match.team1;
                    
                    // Add losers (they get eliminated at this round)
                    foreach (var player in losingTeam)
                    {
                        if (!placements.Contains(player))
                        {
                            placements.Add(player);
                        }
                    }
                }
            }
            
            // Winner is first
            placements.Reverse();
            
            return placements;
        }
        
        /// <summary>
        /// Shuffle participants for seeding
        /// Xáo trộn người tham gia
        /// </summary>
        private void ShuffleParticipants()
        {
            for (int i = 0; i < participants.Count; i++)
            {
                int randomIndex = UnityEngine.Random.Range(i, participants.Count);
                var temp = participants[i];
                participants[i] = participants[randomIndex];
                participants[randomIndex] = temp;
            }
        }
        
        /// <summary>
        /// Get next power of 2
        /// Lấy lũy thừa 2 tiếp theo
        /// </summary>
        private int GetNextPowerOf2(int n)
        {
            int power = 2;
            while (power < n)
            {
                power *= 2;
            }
            return power;
        }
        
        /// <summary>
        /// Get current round matches
        /// Lấy trận đấu vòng hiện tại
        /// </summary>
        public List<TournamentMatch> GetCurrentRoundMatches()
        {
            return matches.Where(m => m.roundNumber == currentRound).ToList();
        }
        
        /// <summary>
        /// Check if tournament is complete
        /// Kiểm tra tournament đã xong chưa
        /// </summary>
        public bool IsComplete()
        {
            return isComplete;
        }
    }
}
