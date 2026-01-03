using UnityEngine;

namespace DarkLegend.Reset
{
    /// <summary>
    /// Master Reset handler - Xử lý Master Reset (Cao nhất)
    /// Manages master character resets - the ultimate reset level
    /// </summary>
    public class MasterReset : MonoBehaviour
    {
        [Header("Master Reset Requirements")]
        [Tooltip("Required grand resets - Số Grand Reset yêu cầu")]
        public int requiredGrandResets = 10;

        [Tooltip("Required level - Level yêu cầu")]
        public int requiredLevel = 400;

        [Tooltip("Zen cost - Chi phí Zen")]
        public long requiredZen = 10000000000; // 10 billion

        [Tooltip("Required special item - Item đặc biệt yêu cầu")]
        public Item requiredItem;

        [Header("Master Reset Rewards")]
        [Tooltip("Master bonus stats - Điểm stats bonus Master")]
        public int masterBonusStats = 50000;

        [Tooltip("Master damage bonus - % damage bonus Master")]
        [Range(0f, 1f)]
        public float masterDamageBonus = 0.50f; // 50%

        [Tooltip("Master defense bonus - % defense bonus Master")]
        [Range(0f, 1f)]
        public float masterDefenseBonus = 0.50f; // 50%

        [Tooltip("Master HP bonus - % HP bonus Master")]
        [Range(0f, 1f)]
        public float masterHPBonus = 0.25f; // 25%

        [Tooltip("Master MP bonus - % MP bonus Master")]
        [Range(0f, 1f)]
        public float masterMPBonus = 0.25f; // 25%

        [Tooltip("Master title - Danh hiệu Master")]
        public string masterTitle = "Master";

        [Tooltip("Master reward item - Item thưởng Master")]
        public Item masterRewardItem;

        [Tooltip("Master name color - Màu tên Master")]
        public Color masterNameColor = Color.yellow;

        [Header("Special Abilities")]
        [Tooltip("Unlock master skills - Mở khóa skills Master")]
        public bool unlockMasterSkills = true;

        [Tooltip("Unlock master wings - Mở khóa wings Master")]
        public bool unlockMasterWings = true;

        [Tooltip("Special aura effect - Hiệu ứng aura đặc biệt")]
        public bool enableMasterAura = true;

        [Header("Settings")]
        [Tooltip("Allow only one master reset - Chỉ cho phép 1 Master Reset")]
        public bool allowOnlyOneMasterReset = true;

        /// <summary>
        /// Check if character can perform master reset
        /// Kiểm tra xem nhân vật có thể Master Reset
        /// </summary>
        public bool CanReset(CharacterStats character)
        {
            if (character == null)
                return false;

            // Check if already has master reset (if limited to one)
            if (allowOnlyOneMasterReset && character.hasMasterReset)
                return false;

            // Check grand reset count
            if (character.grandResetCount < requiredGrandResets)
                return false;

            // Check level
            if (character.level < requiredLevel)
                return false;

            // Check zen
            if (character.zen < requiredZen)
                return false;

            // Check required item (if specified)
            if (requiredItem != null)
            {
                // Implement item check based on your inventory system
                // Implement kiểm tra item dựa trên inventory system
            }

            return true;
        }

        /// <summary>
        /// Perform master reset
        /// Thực hiện Master Reset
        /// </summary>
        public bool PerformReset(CharacterStats character)
        {
            if (!CanReset(character))
                return false;

            // Use the main ResetSystem
            bool success = ResetSystem.Instance.PerformMasterReset(character);

            if (success)
            {
                // Apply special abilities
                if (unlockMasterSkills)
                {
                    UnlockMasterSkills(character);
                }

                if (unlockMasterWings)
                {
                    UnlockMasterWings(character);
                }

                if (enableMasterAura)
                {
                    EnableMasterAura(character);
                }

                // Award special item
                if (masterRewardItem != null)
                {
                    AwardMasterItem(character);
                }
            }

            return success;
        }

        /// <summary>
        /// Unlock master skills
        /// Mở khóa skills Master
        /// </summary>
        private void UnlockMasterSkills(CharacterStats character)
        {
            // Implement skill unlocking based on your skill system
            // Implement mở khóa skills dựa trên skill system
            Debug.Log($"Master skills unlocked for {character.name}");
        }

        /// <summary>
        /// Unlock master wings
        /// Mở khóa wings Master
        /// </summary>
        private void UnlockMasterWings(CharacterStats character)
        {
            // Implement wings unlocking based on your item system
            // Implement mở khóa wings dựa trên item system
            Debug.Log($"Master wings unlocked for {character.name}");
        }

        /// <summary>
        /// Enable master aura effect
        /// Bật hiệu ứng aura Master
        /// </summary>
        private void EnableMasterAura(CharacterStats character)
        {
            // Implement aura effect based on your VFX system
            // Implement hiệu ứng aura dựa trên VFX system
            Debug.Log($"Master aura enabled for {character.name}");
        }

        /// <summary>
        /// Award master item
        /// Trao item Master
        /// </summary>
        private void AwardMasterItem(CharacterStats character)
        {
            // Implement item awarding based on your inventory system
            // Implement trao item dựa trên inventory system
            Debug.Log($"Master item awarded to {character.name}");
        }

        /// <summary>
        /// Get master reset information
        /// Lấy thông tin Master Reset
        /// </summary>
        public string GetResetInfo(CharacterStats character)
        {
            if (character == null)
                return "Invalid character";

            string info = "=== MASTER RESET ===\n";
            info += "THE ULTIMATE RESET\n\n";
            info += "Requirements:\n";
            info += $"- Grand Resets: {character.grandResetCount}/{requiredGrandResets} ";
            info += character.grandResetCount >= requiredGrandResets ? "✓\n" : "✗\n";
            info += $"- Level: {character.level}/{requiredLevel} ";
            info += character.level >= requiredLevel ? "✓\n" : "✗\n";
            info += $"- Zen: {character.zen:N0}/{requiredZen:N0} ";
            info += character.zen >= requiredZen ? "✓\n" : "✗\n";

            if (requiredItem != null)
            {
                info += $"- Special Item: {requiredItem.Name}\n";
            }

            if (character.hasMasterReset)
            {
                info += "\n⚠️ Already performed Master Reset!\n";
            }

            info += $"\nRewards:\n";
            info += $"- Bonus Stats: +{masterBonusStats:N0}\n";
            info += $"- Damage Bonus: +{masterDamageBonus * 100:F0}%\n";
            info += $"- Defense Bonus: +{masterDefenseBonus * 100:F0}%\n";
            info += $"- HP Bonus: +{masterHPBonus * 100:F0}%\n";
            info += $"- MP Bonus: +{masterMPBonus * 100:F0}%\n";
            info += $"- Title: \"{masterTitle}\"\n";
            info += $"- Name Color: Golden\n";

            if (unlockMasterSkills)
                info += $"- Unlock Master Skills\n";
            
            if (unlockMasterWings)
                info += $"- Unlock Master Wings\n";

            if (enableMasterAura)
                info += $"- Special Master Aura\n";

            if (masterRewardItem != null)
                info += $"- Special Item: {masterRewardItem.Name}\n";

            info += $"\nEffects:\n";
            info += $"- Level reset to 1\n";
            info += $"- Keep all previous bonuses\n";
            info += $"- Keep all items and skills\n";
            info += $"- Become the ultimate warrior!\n";

            return info;
        }
    }
}
