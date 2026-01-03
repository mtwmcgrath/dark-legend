using UnityEngine;
using System;

namespace DarkLegend.PvP
{
    /// <summary>
    /// PK Penalty System - Hệ thống hình phạt PK
    /// Penalties for murderers and outlaws
    /// </summary>
    public class PKPenalty : MonoBehaviour
    {
        [Header("Murderer Penalties (Orange Name)")]
        public float murdererExpLoss = 0.05f;        // 5% EXP loss on death
        public float murdererItemDropChance = 0.03f;  // 3% chance to drop equipped item
        public bool murdererCanEnterTown = true;
        public bool murdererNPCsAttack = false;
        
        [Header("Outlaw Penalties (Red Name)")]
        public float outlawExpLoss = 0.10f;          // 10% EXP loss on death
        public float outlawItemDropChance = 0.10f;    // 10% chance to drop equipped item
        public bool outlawCanEnterTown = false;       // Guards attack
        public bool outlawNPCsAttack = true;          // All NPCs hostile
        public float outlawStatPenalty = 0.10f;       // -10% all stats
        
        [Header("Other Penalties")]
        public int outlawTeleportCostMultiplier = 5;  // 5x teleport cost
        public bool outlawCanUseShop = false;
        
        /// <summary>
        /// Get experience loss on death
        /// Lấy % kinh nghiệm mất khi chết
        /// </summary>
        public float GetExpLoss(PKStatus status)
        {
            switch (status)
            {
                case PKStatus.Murderer:
                    return murdererExpLoss;
                case PKStatus.Outlaw:
                    return outlawExpLoss;
                default:
                    return 0f;
            }
        }
        
        /// <summary>
        /// Get item drop chance on death
        /// Lấy tỷ lệ rơi đồ khi chết
        /// </summary>
        public float GetItemDropChance(PKStatus status)
        {
            switch (status)
            {
                case PKStatus.Murderer:
                    return murdererItemDropChance;
                case PKStatus.Outlaw:
                    return outlawItemDropChance;
                default:
                    return 0f;
            }
        }
        
        /// <summary>
        /// Check if player can enter town
        /// Kiểm tra người chơi có thể vào thành không
        /// </summary>
        public bool CanEnterTown(PKStatus status)
        {
            switch (status)
            {
                case PKStatus.Outlaw:
                    return outlawCanEnterTown;
                case PKStatus.Murderer:
                    return murdererCanEnterTown;
                default:
                    return true;
            }
        }
        
        /// <summary>
        /// Check if NPCs should attack player
        /// Kiểm tra NPCs có tấn công người chơi không
        /// </summary>
        public bool NPCsAttack(PKStatus status)
        {
            switch (status)
            {
                case PKStatus.Outlaw:
                    return outlawNPCsAttack;
                case PKStatus.Murderer:
                    return murdererNPCsAttack;
                default:
                    return false;
            }
        }
        
        /// <summary>
        /// Get stat penalty multiplier
        /// Lấy hệ số phạt chỉ số
        /// </summary>
        public float GetStatPenalty(PKStatus status)
        {
            if (status == PKStatus.Outlaw)
            {
                return outlawStatPenalty;
            }
            return 0f;
        }
        
        /// <summary>
        /// Get teleport cost multiplier
        /// Lấy hệ số phí dịch chuyển
        /// </summary>
        public int GetTeleportCostMultiplier(PKStatus status)
        {
            if (status == PKStatus.Outlaw)
            {
                return outlawTeleportCostMultiplier;
            }
            return 1;
        }
        
        /// <summary>
        /// Check if player can use shops
        /// Kiểm tra người chơi có thể dùng shop không
        /// </summary>
        public bool CanUseShop(PKStatus status)
        {
            if (status == PKStatus.Outlaw)
            {
                return outlawCanUseShop;
            }
            return true;
        }
        
        /// <summary>
        /// Apply death penalty to player
        /// Áp dụng hình phạt chết cho người chơi
        /// </summary>
        public void ApplyDeathPenalty(GameObject player, PKStatus status)
        {
            // Apply EXP loss
            float expLoss = GetExpLoss(status);
            if (expLoss > 0)
            {
                // TODO: Integrate with player experience system
                Debug.Log($"{player.name} lost {expLoss * 100}% EXP due to PK penalty");
            }
            
            // Check for item drop
            float dropChance = GetItemDropChance(status);
            if (dropChance > 0 && UnityEngine.Random.value < dropChance)
            {
                // TODO: Drop random equipped item
                Debug.Log($"{player.name} dropped an equipped item due to PK penalty");
            }
        }
    }
}
