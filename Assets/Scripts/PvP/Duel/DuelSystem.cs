using UnityEngine;
using System;
using System.Collections.Generic;

namespace DarkLegend.PvP
{
    /// <summary>
    /// Duel System - Hệ thống đấu tay đôi
    /// Manages 1v1 duels between players
    /// </summary>
    public class DuelSystem : MonoBehaviour
    {
        // Active duels and pending requests
        private List<ActiveDuel> activeDuels = new List<ActiveDuel>();
        private List<DuelRequest> pendingRequests = new List<DuelRequest>();
        
        // Events
        public event Action<GameObject, GameObject> OnDuelRequestSent;
        public event Action<GameObject, GameObject> OnDuelAccepted;
        public event Action<GameObject, GameObject> OnDuelDeclined;
        public event Action<GameObject, GameObject, GameObject> OnDuelEnd; // winner, loser, or null for draw
        
        private void Update()
        {
            // Clean up expired requests
            pendingRequests.RemoveAll(r => r.IsExpired);
            
            // Check for expired duels
            CheckDuelTimeouts();
        }
        
        /// <summary>
        /// Send duel request to target player
        /// Gửi yêu cầu đấu tay đôi
        /// </summary>
        public void SendDuelRequest(GameObject challenger, GameObject target, DuelSettings settings = null)
        {
            if (challenger == target)
            {
                Debug.LogWarning("Cannot duel yourself");
                return;
            }
            
            // Check if already in duel
            if (IsInDuel(challenger) || IsInDuel(target))
            {
                Debug.LogWarning("One or both players already in duel");
                return;
            }
            
            // Check if request already exists
            if (HasPendingRequest(challenger, target))
            {
                Debug.LogWarning("Duel request already pending");
                return;
            }
            
            // Create request
            if (settings == null)
            {
                settings = new DuelSettings();
            }
            
            DuelRequest request = new DuelRequest(challenger, target, settings);
            pendingRequests.Add(request);
            
            OnDuelRequestSent?.Invoke(challenger, target);
            Debug.Log($"Duel request sent from {challenger.name} to {target.name}");
        }
        
        /// <summary>
        /// Accept duel request
        /// Chấp nhận đấu tay đôi
        /// </summary>
        public void AcceptDuel(GameObject target, GameObject challenger)
        {
            DuelRequest request = pendingRequests.Find(r => 
                r.challenger == challenger && r.target == target);
            
            if (request == null || request.IsExpired)
            {
                Debug.LogWarning("No valid duel request found");
                return;
            }
            
            // Remove request
            pendingRequests.Remove(request);
            
            // Start duel
            StartDuel(challenger, target, request.settings);
            
            OnDuelAccepted?.Invoke(challenger, target);
        }
        
        /// <summary>
        /// Decline duel request
        /// Từ chối đấu tay đôi
        /// </summary>
        public void DeclineDuel(GameObject target, GameObject challenger)
        {
            DuelRequest request = pendingRequests.Find(r => 
                r.challenger == challenger && r.target == target);
            
            if (request == null)
            {
                Debug.LogWarning("No duel request found");
                return;
            }
            
            pendingRequests.Remove(request);
            OnDuelDeclined?.Invoke(challenger, target);
            Debug.Log($"Duel declined by {target.name}");
        }
        
        /// <summary>
        /// Start a duel
        /// Bắt đầu đấu tay đôi
        /// </summary>
        private void StartDuel(GameObject challenger, GameObject target, DuelSettings settings)
        {
            ActiveDuel duel = new ActiveDuel
            {
                challenger = challenger,
                target = target,
                settings = settings,
                challengerOriginalPosition = challenger.transform.position,
                targetOriginalPosition = target.transform.position,
                startTime = Time.time,
                endTime = Time.time + settings.timeLimit
            };
            
            activeDuels.Add(duel);
            
            // TODO: Teleport players to duel arena
            // TODO: Heal players to full HP/MP
            // TODO: Apply duel rules
            
            Debug.Log($"Duel started between {challenger.name} and {target.name}");
        }
        
        /// <summary>
        /// End a duel
        /// Kết thúc đấu tay đôi
        /// </summary>
        public void EndDuel(GameObject player1, GameObject player2, GameObject winner)
        {
            ActiveDuel duel = activeDuels.Find(d => 
                (d.challenger == player1 && d.target == player2) ||
                (d.challenger == player2 && d.target == player1));
            
            if (duel == null)
            {
                Debug.LogWarning("No active duel found");
                return;
            }
            
            activeDuels.Remove(duel);
            
            // Determine loser
            GameObject loser = null;
            if (winner == player1) loser = player2;
            else if (winner == player2) loser = player1;
            
            // Update rankings if ranked
            if (duel.settings.type == DuelType.Ranked && winner != null)
            {
                UpdateRankedDuel(winner, loser);
            }
            
            // Handle bet
            if (duel.settings.type == DuelType.Bet && winner != null)
            {
                HandleDuelBet(winner, loser, duel.settings.betAmount);
            }
            
            // Restore players
            RestoreAfterDuel(duel);
            
            OnDuelEnd?.Invoke(winner, loser, null);
            Debug.Log($"Duel ended. Winner: {(winner != null ? winner.name : "Draw")}");
        }
        
        /// <summary>
        /// Check for duel timeouts
        /// Kiểm tra đấu hết giờ
        /// </summary>
        private void CheckDuelTimeouts()
        {
            for (int i = activeDuels.Count - 1; i >= 0; i--)
            {
                if (activeDuels[i].IsExpired)
                {
                    // Timeout - end as draw
                    EndDuel(activeDuels[i].challenger, activeDuels[i].target, null);
                }
            }
        }
        
        /// <summary>
        /// Restore players after duel
        /// Khôi phục người chơi sau đấu
        /// </summary>
        private void RestoreAfterDuel(ActiveDuel duel)
        {
            // TODO: Heal to full HP/MP
            // TODO: Teleport back to original positions
            // TODO: Remove duel buffs/debuffs
            
            duel.challenger.transform.position = duel.challengerOriginalPosition;
            duel.target.transform.position = duel.targetOriginalPosition;
        }
        
        /// <summary>
        /// Update rankings for ranked duel
        /// Cập nhật xếp hạng cho đấu ranked
        /// </summary>
        private void UpdateRankedDuel(GameObject winner, GameObject loser)
        {
            // TODO: Integrate with ranking system
            var rankingSystem = PvPManager.Instance.GetRankingSystem();
            if (rankingSystem != null)
            {
                // This will be implemented in PvPRankingSystem
            }
        }
        
        /// <summary>
        /// Handle bet for bet duel
        /// Xử lý cược cho đấu bet
        /// </summary>
        private void HandleDuelBet(GameObject winner, GameObject loser, int betAmount)
        {
            // TODO: Transfer bet amount from loser to winner
            Debug.Log($"{winner.name} won {betAmount * 2} Zen from duel bet");
        }
        
        // Helper methods
        public bool IsInDuel(GameObject player)
        {
            return activeDuels.Exists(d => d.challenger == player || d.target == player);
        }
        
        public bool HasPendingRequest(GameObject challenger, GameObject target)
        {
            return pendingRequests.Exists(r => r.challenger == challenger && r.target == target);
        }
        
        public List<DuelRequest> GetPendingRequestsFor(GameObject player)
        {
            return pendingRequests.FindAll(r => r.target == player);
        }
    }
}
