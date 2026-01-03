using UnityEngine;
using System;
using System.Collections.Generic;

namespace DarkLegend.PvP
{
    /// <summary>
    /// PK System - Player Kill System
    /// Hệ thống giết người chơi
    /// </summary>
    [Serializable]
    public class PKData
    {
        public int pkCount = 0;
        public PKStatus status = PKStatus.Normal;
        public DateTime lastPKTime;
        public float pkDecayTimer = 0f;
        
        public PKData()
        {
            lastPKTime = DateTime.MinValue;
        }
    }

    /// <summary>
    /// PK System Component
    /// </summary>
    public class PKSystem : MonoBehaviour
    {
        [Header("PK Settings")]
        public float pkDecayRate = 1f;            // PK count decay per hour
        public int murdererThreshold = 1;         // Orange name
        public int outlawThreshold = 10;          // Red name
        public float selfDefenseDuration = 300f;  // 5 minutes
        
        // Player PK data
        private Dictionary<string, PKData> playerPKData = new Dictionary<string, PKData>();
        
        // Events
        public event Action<GameObject, PKStatus> OnPKStatusChanged;
        
        private void Update()
        {
            // Process PK decay
            ProcessPKDecay();
        }
        
        /// <summary>
        /// Handle player kill event
        /// Xử lý sự kiện giết người chơi
        /// </summary>
        public void OnPlayerKill(GameObject killer, GameObject victim)
        {
            string killerId = killer.GetInstanceID().ToString();
            string victimId = victim.GetInstanceID().ToString();
            
            PKData killerData = GetPKData(killerId);
            PKData victimData = GetPKData(victimId);
            
            // Check if it's self-defense
            bool isSelfDefense = victimData.status == PKStatus.Murderer || 
                                 victimData.status == PKStatus.Outlaw;
            
            if (isSelfDefense)
            {
                // Killing murderers/outlaws makes you a hero
                if (killerData.status == PKStatus.Normal)
                {
                    SetPKStatus(killerId, killer, PKStatus.Hero);
                }
            }
            else
            {
                // Increase PK count
                killerData.pkCount++;
                killerData.lastPKTime = DateTime.Now;
                
                // Update status
                UpdatePKStatus(killerId, killer, killerData);
            }
            
            // Victim enters self-defense mode temporarily
            if (victimData.status == PKStatus.Normal)
            {
                SetPKStatus(victimId, victim, PKStatus.SelfDefense);
            }
        }
        
        /// <summary>
        /// Get or create PK data for player
        /// Lấy hoặc tạo dữ liệu PK cho người chơi
        /// </summary>
        public PKData GetPKData(string playerId)
        {
            if (!playerPKData.ContainsKey(playerId))
            {
                playerPKData[playerId] = new PKData();
            }
            return playerPKData[playerId];
        }
        
        /// <summary>
        /// Get PK status for player
        /// Lấy trạng thái PK của người chơi
        /// </summary>
        public PKStatus GetPKStatus(string playerId)
        {
            return GetPKData(playerId).status;
        }
        
        /// <summary>
        /// Update PK status based on PK count
        /// Cập nhật trạng thái PK dựa trên số lần giết
        /// </summary>
        private void UpdatePKStatus(string playerId, GameObject player, PKData data)
        {
            PKStatus newStatus;
            
            if (data.pkCount >= outlawThreshold)
            {
                newStatus = PKStatus.Outlaw;
            }
            else if (data.pkCount >= murdererThreshold)
            {
                newStatus = PKStatus.Murderer;
            }
            else
            {
                newStatus = PKStatus.Normal;
            }
            
            if (data.status != newStatus)
            {
                SetPKStatus(playerId, player, newStatus);
            }
        }
        
        /// <summary>
        /// Set PK status
        /// Đặt trạng thái PK
        /// </summary>
        private void SetPKStatus(string playerId, GameObject player, PKStatus status)
        {
            PKData data = GetPKData(playerId);
            data.status = status;
            
            OnPKStatusChanged?.Invoke(player, status);
            Debug.Log($"{player.name} PK status changed to {status}");
        }
        
        /// <summary>
        /// Process PK count decay
        /// Xử lý giảm dần số lần giết
        /// </summary>
        private void ProcessPKDecay()
        {
            float deltaTime = Time.deltaTime;
            
            foreach (var kvp in playerPKData)
            {
                PKData data = kvp.Value;
                
                if (data.pkCount > 0)
                {
                    data.pkDecayTimer += deltaTime;
                    
                    // Decay every hour (3600 seconds)
                    if (data.pkDecayTimer >= 3600f)
                    {
                        data.pkCount = Mathf.Max(0, data.pkCount - 1);
                        data.pkDecayTimer = 0f;
                        
                        // TODO: Update status if needed
                        // UpdatePKStatus(kvp.Key, player, data);
                    }
                }
            }
        }
        
        /// <summary>
        /// Get name color for PK status
        /// Lấy màu tên cho trạng thái PK
        /// </summary>
        public Color GetNameColor(PKStatus status)
        {
            switch (status)
            {
                case PKStatus.Normal:
                    return Color.white;
                case PKStatus.SelfDefense:
                    return Color.yellow;
                case PKStatus.Murderer:
                    return new Color(1f, 0.5f, 0f); // Orange
                case PKStatus.Outlaw:
                    return Color.red;
                case PKStatus.Hero:
                    return Color.cyan;
                default:
                    return Color.white;
            }
        }
    }
}
