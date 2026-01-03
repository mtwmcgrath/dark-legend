using UnityEngine;
using System.Collections.Generic;

namespace DarkLegend.PvP
{
    /// <summary>
    /// Open World PvP Manager - Quản lý PvP ngoài đồng
    /// </summary>
    public class OpenWorldPvP : MonoBehaviour
    {
        [Header("Settings")]
        public bool allowOpenWorldPvP = true;
        public bool requireBothPlayersFlag = false; // Both players must have PvP enabled
        
        // Safe zones
        private List<SafeZone> safeZones = new List<SafeZone>();
        
        private PKSystem pkSystem;
        private PKPenalty pkPenalty;
        private BountySystem bountySystem;
        
        private void Awake()
        {
            pkSystem = GetComponent<PKSystem>();
            pkPenalty = GetComponent<PKPenalty>();
            bountySystem = GetComponent<BountySystem>();
            
            if (pkSystem == null)
                pkSystem = gameObject.AddComponent<PKSystem>();
            if (pkPenalty == null)
                pkPenalty = gameObject.AddComponent<PKPenalty>();
            if (bountySystem == null)
                bountySystem = gameObject.AddComponent<BountySystem>();
        }
        
        /// <summary>
        /// Check if PvP is allowed between two players
        /// Kiểm tra PvP có được phép giữa hai người chơi không
        /// </summary>
        public bool CanPvP(GameObject player1, GameObject player2)
        {
            if (!allowOpenWorldPvP) return false;
            if (player1 == player2) return false;
            
            // Check if either player is in a safe zone
            if (IsInSafeZone(player1) || IsInSafeZone(player2))
                return false;
            
            // Check PvP toggle if required
            if (requireBothPlayersFlag)
            {
                var toggle1 = player1.GetComponent<PvPToggle>();
                var toggle2 = player2.GetComponent<PvPToggle>();
                
                if (toggle1 != null && !toggle1.pvpEnabled) return false;
                if (toggle2 != null && !toggle2.pvpEnabled) return false;
            }
            
            // TODO: Check party/guild membership
            
            return true;
        }
        
        /// <summary>
        /// Handle player death in open world PvP
        /// Xử lý cái chết trong PvP ngoài đồng
        /// </summary>
        public void OnPlayerDeath(GameObject killer, GameObject victim)
        {
            // Apply PK penalties
            string victimId = victim.GetInstanceID().ToString();
            PKStatus victimStatus = pkSystem.GetPKStatus(victimId);
            pkPenalty.ApplyDeathPenalty(victim, victimStatus);
            
            // Check and claim bounty
            int bountyReward = bountySystem.ClaimBounty(killer, victim);
            if (bountyReward > 0)
            {
                Debug.Log($"{killer.name} claimed {bountyReward} Zen bounty");
            }
        }
        
        /// <summary>
        /// Register safe zone
        /// Đăng ký vùng an toàn
        /// </summary>
        public void RegisterSafeZone(SafeZone zone)
        {
            if (!safeZones.Contains(zone))
            {
                safeZones.Add(zone);
            }
        }
        
        /// <summary>
        /// Unregister safe zone
        /// Hủy đăng ký vùng an toàn
        /// </summary>
        public void UnregisterSafeZone(SafeZone zone)
        {
            safeZones.Remove(zone);
        }
        
        /// <summary>
        /// Check if player is in safe zone
        /// Kiểm tra người chơi có trong vùng an toàn không
        /// </summary>
        private bool IsInSafeZone(GameObject player)
        {
            foreach (var zone in safeZones)
            {
                if (zone.GetComponent<Collider>().bounds.Contains(player.transform.position))
                {
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// Get PK system
        /// Lấy hệ thống PK
        /// </summary>
        public PKSystem GetPKSystem()
        {
            return pkSystem;
        }
        
        /// <summary>
        /// Get bounty system
        /// Lấy hệ thống truy nã
        /// </summary>
        public BountySystem GetBountySystem()
        {
            return bountySystem;
        }
    }
}
