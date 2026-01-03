using UnityEngine;

namespace DarkLegend.Maps.Portals
{
    /// <summary>
    /// Dungeon portal - Portal vào dungeon
    /// Dungeon entry portal with special rules
    /// </summary>
    public class DungeonPortal : Portal
    {
        [Header("Dungeon Portal Settings")]
        [Tooltip("Loại dungeon / Dungeon type")]
        [SerializeField] private DungeonType dungeonType = DungeonType.Normal;
        
        [Tooltip("Yêu cầu party / Require party")]
        [SerializeField] private bool requireParty = false;
        
        [Tooltip("Số người min / Minimum party size")]
        [SerializeField] private int minPartySize = 1;
        
        [Tooltip("Số người max / Maximum party size")]
        [SerializeField] private int maxPartySize = 5;
        
        [Tooltip("Tạo instance riêng / Create instance")]
        [SerializeField] private bool createInstance = true;
        
        [Tooltip("Thời gian giới hạn (phút) / Time limit")]
        [SerializeField] private int timeLimit = 60;
        
        [Header("Entry Restrictions")]
        [Tooltip("Số lần vào/ngày / Daily entry limit")]
        [SerializeField] private int dailyEntryLimit = 3;
        
        [Tooltip("Thời gian cooldown (giờ) / Cooldown hours")]
        [SerializeField] private int cooldownHours = 24;
        
        protected override void InitializePortal()
        {
            base.InitializePortal();
            
            // Dungeon portal có màu đỏ
            portalColor = Color.red;
            
            Debug.Log($"[DungeonPortal] Dungeon portal initialized: {dungeonType}");
        }
        
        protected override bool CheckRequirements(GameObject player)
        {
            // Base requirements
            if (!base.CheckRequirements(player))
            {
                return false;
            }
            
            // Check party requirement
            if (requireParty)
            {
                if (!IsInParty(player))
                {
                    ShowMessage(player, "Cần party để vào dungeon này!");
                    return false;
                }
                
                int partySize = GetPartySize(player);
                if (partySize < minPartySize)
                {
                    ShowMessage(player, $"Party cần ít nhất {minPartySize} người!");
                    return false;
                }
                
                if (partySize > maxPartySize)
                {
                    ShowMessage(player, $"Party không được quá {maxPartySize} người!");
                    return false;
                }
            }
            
            // Check daily entry limit
            if (!CanEnterToday(player))
            {
                ShowMessage(player, $"Bạn đã hết lượt vào dungeon hôm nay! ({dailyEntryLimit} lượt/ngày)");
                return false;
            }
            
            // Check cooldown
            if (IsOnCooldown(player))
            {
                float remaining = GetCooldownRemaining(player);
                ShowMessage(player, $"Vui lòng chờ {remaining:F0} giờ!");
                return false;
            }
            
            return true;
        }
        
        protected override void UsePortal(GameObject player)
        {
            // Log entry
            LogDungeonEntry(player);
            
            // If party portal, teleport whole party
            if (partyPortal && IsInParty(player))
            {
                TeleportParty(player);
            }
            else
            {
                base.UsePortal(player);
            }
            
            // Show dungeon info
            ShowDungeonInfo(player);
        }
        
        /// <summary>
        /// Teleport cả party / Teleport entire party
        /// </summary>
        private void TeleportParty(GameObject player)
        {
            // TODO: Get party members
            Debug.Log($"[DungeonPortal] Teleporting party to dungeon");
            
            // Teleport leader first
            base.UsePortal(player);
            
            // TODO: Teleport party members
        }
        
        /// <summary>
        /// Hiển thị thông tin dungeon / Show dungeon info
        /// </summary>
        private void ShowDungeonInfo(GameObject player)
        {
            string info = $"Dungeon: {portalName}\n";
            info += $"Level: {requiredLevel}+\n";
            
            if (timeLimit > 0)
            {
                info += $"Thời gian: {timeLimit} phút\n";
            }
            
            info += $"Loại: {dungeonType}";
            
            ShowMessage(player, info);
        }
        
        /// <summary>
        /// Lấy party size / Get party size
        /// </summary>
        private int GetPartySize(GameObject player)
        {
            // TODO: Get actual party size
            return 1;
        }
        
        /// <summary>
        /// Kiểm tra có thể vào hôm nay / Check if can enter today
        /// </summary>
        private bool CanEnterToday(GameObject player)
        {
            // TODO: Check daily entry count
            return true;
        }
        
        /// <summary>
        /// Kiểm tra cooldown / Check if on cooldown
        /// </summary>
        private bool IsOnCooldown(GameObject player)
        {
            // TODO: Check cooldown timestamp
            return false;
        }
        
        /// <summary>
        /// Lấy thời gian cooldown còn lại / Get cooldown remaining
        /// </summary>
        private float GetCooldownRemaining(GameObject player)
        {
            // TODO: Calculate remaining cooldown
            return 0f;
        }
        
        /// <summary>
        /// Log lần vào dungeon / Log dungeon entry
        /// </summary>
        private void LogDungeonEntry(GameObject player)
        {
            // TODO: Log to database
            Debug.Log($"[DungeonPortal] Player entered {portalName}");
        }
        
        /// <summary>
        /// Reset daily entries / Reset daily entry count
        /// </summary>
        public void ResetDailyEntries()
        {
            // TODO: Reset entry counts for all players
            Debug.Log($"[DungeonPortal] Daily entries reset");
        }
    }
    
    /// <summary>
    /// Loại dungeon / Dungeon types
    /// </summary>
    public enum DungeonType
    {
        Normal,     // Dungeon thường
        Heroic,     // Dungeon anh hùng
        Epic,       // Dungeon sử thi
        Raid        // Raid dungeon
    }
}
