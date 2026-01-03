using System.Collections.Generic;
using UnityEngine;

namespace DarkLegend.Maps.NPCs
{
    /// <summary>
    /// NPC teleport / Teleporter NPC
    /// </summary>
    public class TeleportNPC : NPCBase
    {
        [Header("Teleport Configuration")]
        [Tooltip("Danh sách điểm đến / Destination list")]
        [SerializeField] private List<TeleportDestination> destinations = new List<TeleportDestination>();
        
        [Tooltip("Chi phí base / Base teleport cost")]
        [SerializeField] private int baseTeleportCost = 1000;
        
        [Tooltip("Tính phí theo khoảng cách / Calculate cost by distance")]
        [SerializeField] private bool costByDistance = true;
        
        [Tooltip("Zen per unit distance / Zen per unit")]
        [SerializeField] private float zenPerUnit = 10f;
        
        [Header("Restrictions")]
        [Tooltip("Yêu cầu combat off / Require out of combat")]
        [SerializeField] private bool requireOutOfCombat = true;
        
        [Tooltip("Thời gian cooldown (giây) / Cooldown time")]
        [SerializeField] private float cooldownTime = 10f;
        
        private Dictionary<int, float> playerCooldowns = new Dictionary<int, float>();
        
        protected override void InitializeNPC()
        {
            base.InitializeNPC();
            
            Debug.Log($"[TeleportNPC] Teleport NPC initialized: {npcName}");
            Debug.Log($"[TeleportNPC] Available destinations: {destinations.Count}");
        }
        
        protected override void OpenNPCUI(GameObject player)
        {
            Debug.Log($"[TeleportNPC] Opening teleport UI");
            
            // Check cooldown
            if (IsOnCooldown(player))
            {
                float remaining = GetCooldownRemaining(player);
                ShowDialog($"Vui lòng chờ {remaining:F1} giây!");
                return;
            }
            
            // Check combat status
            if (requireOutOfCombat && IsInCombat(player))
            {
                ShowDialog("Không thể dịch chuyển trong combat!");
                return;
            }
            
            // Show teleport destinations
            ShowTeleportUI(player);
        }
        
        /// <summary>
        /// Hiển thị UI teleport / Show teleport UI
        /// </summary>
        private void ShowTeleportUI(GameObject player)
        {
            Debug.Log($"[TeleportNPC] Showing {destinations.Count} destinations");
            // TODO: Show teleport UI with destinations and costs
        }
        
        /// <summary>
        /// Teleport player / Teleport player to destination
        /// </summary>
        public bool TeleportPlayer(GameObject player, TeleportDestination destination)
        {
            // Check if destination is valid
            if (destination == null || destination.targetMap == null)
            {
                ShowDialog("Điểm đến không hợp lệ!");
                return false;
            }
            
            // Check level requirement
            if (!CheckLevelRequirement(player, destination))
            {
                ShowDialog($"Cần level {destination.requiredLevel} để đến {destination.destinationName}!");
                return false;
            }
            
            // Calculate cost
            int cost = CalculateTeleportCost(destination);
            
            // Check if player has enough Zen
            if (!HasEnoughZen(player, cost))
            {
                ShowDialog($"Không đủ Zen! Cần {cost} Zen.");
                return false;
            }
            
            // Deduct cost
            DeductZen(player, cost);
            
            // Perform teleport
            PerformTeleport(player, destination);
            
            // Set cooldown
            SetCooldown(player);
            
            Debug.Log($"[TeleportNPC] Teleported player to {destination.destinationName}");
            return true;
        }
        
        /// <summary>
        /// Thực hiện teleport / Perform teleportation
        /// </summary>
        private void PerformTeleport(GameObject player, TeleportDestination destination)
        {
            // Close UI
            EndInteraction();
            
            // Teleport via MapManager
            if (DarkLegend.Maps.Core.MapManager.Instance != null)
            {
                DarkLegend.Maps.Core.MapManager.Instance.TransitionToMap(
                    destination.targetMap,
                    destination.spawnPosition
                );
            }
            
            ShowDialog($"Đã dịch chuyển đến {destination.destinationName}!");
        }
        
        /// <summary>
        /// Tính chi phí teleport / Calculate teleport cost
        /// </summary>
        private int CalculateTeleportCost(TeleportDestination destination)
        {
            if (destination.overrideCost)
            {
                return destination.customCost;
            }
            
            if (!costByDistance)
            {
                return baseTeleportCost;
            }
            
            // Calculate based on distance
            float distance = Vector3.Distance(transform.position, destination.spawnPosition);
            int cost = baseTeleportCost + Mathf.RoundToInt(distance * zenPerUnit);
            
            return cost;
        }
        
        /// <summary>
        /// Kiểm tra level requirement / Check level requirement
        /// </summary>
        private bool CheckLevelRequirement(GameObject player, TeleportDestination destination)
        {
            // TODO: Get player level
            int playerLevel = 100; // Placeholder
            return playerLevel >= destination.requiredLevel;
        }
        
        /// <summary>
        /// Kiểm tra có đủ Zen / Check if has enough Zen
        /// </summary>
        private bool HasEnoughZen(GameObject player, int cost)
        {
            // TODO: Check player's Zen
            return true;
        }
        
        /// <summary>
        /// Trừ Zen / Deduct Zen
        /// </summary>
        private void DeductZen(GameObject player, int amount)
        {
            // TODO: Deduct Zen from player
            Debug.Log($"[TeleportNPC] Deducted {amount} Zen");
        }
        
        /// <summary>
        /// Kiểm tra có trong combat / Check if in combat
        /// </summary>
        private bool IsInCombat(GameObject player)
        {
            // TODO: Check player's combat status
            return false;
        }
        
        /// <summary>
        /// Set cooldown / Set cooldown for player
        /// </summary>
        private void SetCooldown(GameObject player)
        {
            int playerId = player.GetInstanceID();
            playerCooldowns[playerId] = Time.time + cooldownTime;
        }
        
        /// <summary>
        /// Kiểm tra cooldown / Check if on cooldown
        /// </summary>
        private bool IsOnCooldown(GameObject player)
        {
            int playerId = player.GetInstanceID();
            
            if (playerCooldowns.ContainsKey(playerId))
            {
                return Time.time < playerCooldowns[playerId];
            }
            
            return false;
        }
        
        /// <summary>
        /// Lấy thời gian cooldown còn lại / Get cooldown remaining
        /// </summary>
        private float GetCooldownRemaining(GameObject player)
        {
            int playerId = player.GetInstanceID();
            
            if (playerCooldowns.ContainsKey(playerId))
            {
                return Mathf.Max(0, playerCooldowns[playerId] - Time.time);
            }
            
            return 0f;
        }
        
        /// <summary>
        /// Lấy danh sách destinations / Get destinations
        /// </summary>
        public List<TeleportDestination> GetDestinations()
        {
            return new List<TeleportDestination>(destinations);
        }
        
        /// <summary>
        /// Thêm destination / Add destination
        /// </summary>
        public void AddDestination(TeleportDestination destination)
        {
            destinations.Add(destination);
            Debug.Log($"[TeleportNPC] Added destination: {destination.destinationName}");
        }
    }
    
    /// <summary>
    /// Điểm đến teleport / Teleport destination
    /// </summary>
    [System.Serializable]
    public class TeleportDestination
    {
        [Tooltip("Tên điểm đến / Destination name")]
        public string destinationName;
        
        [Tooltip("Map đích / Target map")]
        public DarkLegend.Maps.Core.MapData targetMap;
        
        [Tooltip("Vị trí spawn / Spawn position")]
        public Vector3 spawnPosition;
        
        [Tooltip("Level yêu cầu / Required level")]
        public int requiredLevel = 1;
        
        [Tooltip("Override giá / Override cost")]
        public bool overrideCost = false;
        
        [Tooltip("Giá tùy chỉnh / Custom cost")]
        public int customCost = 0;
        
        [Tooltip("Mô tả / Description")]
        public string description;
        
        [Tooltip("Icon / Destination icon")]
        public Sprite icon;
    }
}
