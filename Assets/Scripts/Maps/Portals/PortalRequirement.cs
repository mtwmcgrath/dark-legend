using UnityEngine;

namespace DarkLegend.Maps.Portals
{
    /// <summary>
    /// Yêu cầu để sử dụng portal
    /// Portal entry requirements checker
    /// </summary>
    public class PortalRequirement : MonoBehaviour
    {
        [Header("Level Requirements")]
        [Tooltip("Level tối thiểu / Minimum level")]
        public int minLevel = 1;
        
        [Tooltip("Level tối đa / Maximum level")]
        public int maxLevel = 999;
        
        [Header("Item Requirements")]
        [Tooltip("Danh sách items yêu cầu / Required items")]
        public RequiredItem[] requiredItems;
        
        [Header("Quest Requirements")]
        [Tooltip("Quests phải hoàn thành / Required quests")]
        public string[] requiredQuests;
        
        [Header("Class Requirements")]
        [Tooltip("Classes được phép / Allowed classes")]
        public CharacterClass[] allowedClasses;
        
        [Tooltip("Tất cả classes / Allow all classes")]
        public bool allowAllClasses = true;
        
        [Header("Party Requirements")]
        [Tooltip("Yêu cầu party / Require party")]
        public bool requireParty = false;
        
        [Tooltip("Party size min / Minimum party size")]
        public int minPartySize = 2;
        
        [Tooltip("Party size max / Maximum party size")]
        public int maxPartySize = 5;
        
        [Header("Other Requirements")]
        [Tooltip("Zen yêu cầu / Zen requirement")]
        public int zenRequired = 0;
        
        [Tooltip("Guild yêu cầu / Require guild")]
        public bool requireGuild = false;
        
        [Tooltip("PvP mode yêu cầu / Require PvP mode")]
        public bool requirePvPMode = false;
        
        /// <summary>
        /// Kiểm tra tất cả yêu cầu / Check all requirements
        /// </summary>
        public bool CheckAllRequirements(GameObject player, out string failureReason)
        {
            failureReason = "";
            
            // Check level
            if (!CheckLevelRequirement(player, out failureReason))
            {
                return false;
            }
            
            // Check items
            if (!CheckItemRequirements(player, out failureReason))
            {
                return false;
            }
            
            // Check quests
            if (!CheckQuestRequirements(player, out failureReason))
            {
                return false;
            }
            
            // Check class
            if (!CheckClassRequirement(player, out failureReason))
            {
                return false;
            }
            
            // Check party
            if (!CheckPartyRequirement(player, out failureReason))
            {
                return false;
            }
            
            // Check Zen
            if (!CheckZenRequirement(player, out failureReason))
            {
                return false;
            }
            
            // Check guild
            if (!CheckGuildRequirement(player, out failureReason))
            {
                return false;
            }
            
            // Check PvP mode
            if (!CheckPvPRequirement(player, out failureReason))
            {
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Kiểm tra level / Check level requirement
        /// </summary>
        private bool CheckLevelRequirement(GameObject player, out string reason)
        {
            reason = "";
            int playerLevel = GetPlayerLevel(player);
            
            if (playerLevel < minLevel)
            {
                reason = $"Cần level {minLevel} trở lên!";
                return false;
            }
            
            if (playerLevel > maxLevel)
            {
                reason = $"Level tối đa: {maxLevel}!";
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Kiểm tra items / Check item requirements
        /// </summary>
        private bool CheckItemRequirements(GameObject player, out string reason)
        {
            reason = "";
            
            if (requiredItems == null || requiredItems.Length == 0)
            {
                return true;
            }
            
            foreach (var reqItem in requiredItems)
            {
                if (!HasItem(player, reqItem.itemName, reqItem.quantity))
                {
                    reason = $"Cần {reqItem.quantity}x {reqItem.itemName}!";
                    return false;
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// Kiểm tra quests / Check quest requirements
        /// </summary>
        private bool CheckQuestRequirements(GameObject player, out string reason)
        {
            reason = "";
            
            if (requiredQuests == null || requiredQuests.Length == 0)
            {
                return true;
            }
            
            foreach (var questName in requiredQuests)
            {
                if (!HasCompletedQuest(player, questName))
                {
                    reason = $"Phải hoàn thành quest: {questName}!";
                    return false;
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// Kiểm tra class / Check class requirement
        /// </summary>
        private bool CheckClassRequirement(GameObject player, out string reason)
        {
            reason = "";
            
            if (allowAllClasses)
            {
                return true;
            }
            
            if (allowedClasses == null || allowedClasses.Length == 0)
            {
                return true;
            }
            
            CharacterClass playerClass = GetPlayerClass(player);
            
            foreach (var allowedClass in allowedClasses)
            {
                if (playerClass == allowedClass)
                {
                    return true;
                }
            }
            
            reason = "Class của bạn không được phép vào đây!";
            return false;
        }
        
        /// <summary>
        /// Kiểm tra party / Check party requirement
        /// </summary>
        private bool CheckPartyRequirement(GameObject player, out string reason)
        {
            reason = "";
            
            if (!requireParty)
            {
                return true;
            }
            
            if (!IsInParty(player))
            {
                reason = "Cần có party để vào!";
                return false;
            }
            
            int partySize = GetPartySize(player);
            
            if (partySize < minPartySize)
            {
                reason = $"Party cần ít nhất {minPartySize} người!";
                return false;
            }
            
            if (partySize > maxPartySize)
            {
                reason = $"Party không được quá {maxPartySize} người!";
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Kiểm tra Zen / Check Zen requirement
        /// </summary>
        private bool CheckZenRequirement(GameObject player, out string reason)
        {
            reason = "";
            
            if (zenRequired <= 0)
            {
                return true;
            }
            
            if (!HasEnoughZen(player, zenRequired))
            {
                reason = $"Cần {zenRequired} Zen!";
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Kiểm tra guild / Check guild requirement
        /// </summary>
        private bool CheckGuildRequirement(GameObject player, out string reason)
        {
            reason = "";
            
            if (!requireGuild)
            {
                return true;
            }
            
            if (!IsInGuild(player))
            {
                reason = "Cần tham gia guild!";
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Kiểm tra PvP mode / Check PvP requirement
        /// </summary>
        private bool CheckPvPRequirement(GameObject player, out string reason)
        {
            reason = "";
            
            if (!requirePvPMode)
            {
                return true;
            }
            
            if (!IsPvPModeEnabled(player))
            {
                reason = "Phải bật PvP mode!";
                return false;
            }
            
            return true;
        }
        
        // Helper methods (TODO: Implement with actual game systems)
        private int GetPlayerLevel(GameObject player) => 100;
        private bool HasItem(GameObject player, string itemName, int quantity) => true;
        private bool HasCompletedQuest(GameObject player, string questName) => true;
        private CharacterClass GetPlayerClass(GameObject player) => CharacterClass.DarkKnight;
        private bool IsInParty(GameObject player) => false;
        private int GetPartySize(GameObject player) => 1;
        private bool HasEnoughZen(GameObject player, int amount) => true;
        private bool IsInGuild(GameObject player) => false;
        private bool IsPvPModeEnabled(GameObject player) => false;
    }
    
    /// <summary>
    /// Item yêu cầu / Required item
    /// </summary>
    [System.Serializable]
    public class RequiredItem
    {
        public string itemName;
        public int quantity = 1;
        public bool consumeOnUse = true;
    }
    
    /// <summary>
    /// Classes trong game / Character classes
    /// </summary>
    public enum CharacterClass
    {
        DarkKnight,     // Chiến binh
        DarkWizard,     // Pháp sư
        Elf,            // Cung thủ
        MagicGladiator, // Chiến thần
        DarkLord,       // Ám kỵ sĩ
        Summoner        // Triệu hồi sư
    }
}
