using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DarkLegend.PvP
{
    /// <summary>
    /// Bounty entry - Truy nã
    /// </summary>
    [Serializable]
    public class Bounty
    {
        public string targetId;
        public GameObject target;
        public int amount;                        // Zen
        public string placedById;
        public string placedByName;
        public DateTime expiryTime;               // 24 hours
        
        public bool IsExpired => DateTime.Now > expiryTime;
        
        public Bounty(GameObject target, int amount, string placedById, string placedByName)
        {
            this.targetId = target.GetInstanceID().ToString();
            this.target = target;
            this.amount = amount;
            this.placedById = placedById;
            this.placedByName = placedByName;
            this.expiryTime = DateTime.Now.AddHours(24);
        }
    }

    /// <summary>
    /// Bounty System - Hệ thống truy nã
    /// </summary>
    public class BountySystem : MonoBehaviour
    {
        [Header("Settings")]
        public int minBountyAmount = 10000;           // Minimum Zen for bounty
        public int autoBountyPerPK = 10000;           // Auto bounty for outlaws
        public float bountyDurationHours = 24f;
        
        // Active bounties
        private List<Bounty> activeBounties = new List<Bounty>();
        
        // Events
        public event Action<Bounty> OnBountyPlaced;
        public event Action<Bounty, GameObject> OnBountyClaimed; // bounty, killer
        public event Action<Bounty> OnBountyExpired;
        
        private void Update()
        {
            // Clean up expired bounties
            CleanExpiredBounties();
        }
        
        /// <summary>
        /// Place bounty on target
        /// Đặt truy nã lên mục tiêu
        /// </summary>
        public bool PlaceBounty(GameObject target, int amount, string placerId, string placerName)
        {
            if (amount < minBountyAmount)
            {
                Debug.LogWarning($"Bounty amount too low. Minimum: {minBountyAmount}");
                return false;
            }
            
            // Check if bounty already exists
            Bounty existingBounty = activeBounties.FirstOrDefault(b => b.targetId == target.GetInstanceID().ToString());
            
            if (existingBounty != null)
            {
                // Add to existing bounty
                existingBounty.amount += amount;
                Debug.Log($"Added {amount} to existing bounty on {target.name}. Total: {existingBounty.amount}");
            }
            else
            {
                // Create new bounty
                Bounty bounty = new Bounty(target, amount, placerId, placerName);
                activeBounties.Add(bounty);
                
                OnBountyPlaced?.Invoke(bounty);
                Debug.Log($"Bounty of {amount} Zen placed on {target.name} by {placerName}");
            }
            
            return true;
        }
        
        /// <summary>
        /// Place automatic bounty for outlaw
        /// Đặt truy nã tự động cho kẻ sát nhân
        /// </summary>
        public void PlaceAutoBounty(GameObject target, int pkCount)
        {
            int amount = pkCount * autoBountyPerPK;
            PlaceBounty(target, amount, "System", "System");
        }
        
        /// <summary>
        /// Claim bounty when target is killed
        /// Nhận thưởng khi giết mục tiêu
        /// </summary>
        public int ClaimBounty(GameObject killer, GameObject target)
        {
            string targetId = target.GetInstanceID().ToString();
            Bounty bounty = activeBounties.FirstOrDefault(b => b.targetId == targetId);
            
            if (bounty == null || bounty.IsExpired)
            {
                return 0;
            }
            
            int reward = bounty.amount;
            activeBounties.Remove(bounty);
            
            OnBountyClaimed?.Invoke(bounty, killer);
            Debug.Log($"{killer.name} claimed bounty of {reward} Zen for killing {target.name}");
            
            // TODO: Give Zen to killer
            return reward;
        }
        
        /// <summary>
        /// Get bounty on target
        /// Lấy truy nã trên mục tiêu
        /// </summary>
        public Bounty GetBounty(GameObject target)
        {
            string targetId = target.GetInstanceID().ToString();
            return activeBounties.FirstOrDefault(b => b.targetId == targetId && !b.IsExpired);
        }
        
        /// <summary>
        /// Get bounty amount on target
        /// Lấy số tiền truy nã trên mục tiêu
        /// </summary>
        public int GetBountyAmount(GameObject target)
        {
            Bounty bounty = GetBounty(target);
            return bounty?.amount ?? 0;
        }
        
        /// <summary>
        /// Get top bounties
        /// Lấy danh sách truy nã cao nhất
        /// </summary>
        public List<Bounty> GetTopBounties(int count)
        {
            return activeBounties
                .Where(b => !b.IsExpired)
                .OrderByDescending(b => b.amount)
                .Take(count)
                .ToList();
        }
        
        /// <summary>
        /// Get all active bounties
        /// Lấy tất cả truy nã đang hoạt động
        /// </summary>
        public List<Bounty> GetAllBounties()
        {
            return activeBounties.Where(b => !b.IsExpired).ToList();
        }
        
        /// <summary>
        /// Clean up expired bounties
        /// Dọn dẹp truy nã hết hạn
        /// </summary>
        private void CleanExpiredBounties()
        {
            for (int i = activeBounties.Count - 1; i >= 0; i--)
            {
                if (activeBounties[i].IsExpired)
                {
                    OnBountyExpired?.Invoke(activeBounties[i]);
                    activeBounties.RemoveAt(i);
                }
            }
        }
        
        /// <summary>
        /// Check if target has bounty
        /// Kiểm tra mục tiêu có truy nã không
        /// </summary>
        public bool HasBounty(GameObject target)
        {
            return GetBounty(target) != null;
        }
    }
}
