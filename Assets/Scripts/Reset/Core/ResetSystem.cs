using System;
using UnityEngine;

namespace DarkLegend.Reset
{
    /// <summary>
    /// Main reset system - Hệ thống reset chính
    /// Handles character reset operations
    /// </summary>
    public class ResetSystem : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Reset configuration data - Dữ liệu cấu hình reset")]
        public ResetData resetData;

        [Header("Events")]
        public event Action<ResetType, CharacterStats> OnResetPerformed;
        public event Action<string> OnResetFailed;

        private static ResetSystem _instance;
        public static ResetSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<ResetSystem>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("ResetSystem");
                        _instance = go.AddComponent<ResetSystem>();
                    }
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Check if character can perform normal reset
        /// Kiểm tra xem nhân vật có thể reset thường không
        /// </summary>
        public bool CanPerformNormalReset(CharacterStats character, out string reason)
        {
            reason = "";

            if (character == null)
            {
                reason = "Invalid character";
                return false;
            }

            // Check max reset limit
            if (character.normalResetCount >= resetData.maxNormalResets)
            {
                reason = $"Maximum normal resets ({resetData.maxNormalResets}) reached";
                return false;
            }

            // Check requirements
            if (!resetData.normalResetRequirement.CheckRequirements(
                character.level, character.zen, character.normalResetCount))
            {
                reason = "Requirements not met";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Perform normal reset - Thực hiện reset thường
        /// </summary>
        public bool PerformNormalReset(CharacterStats character)
        {
            if (!CanPerformNormalReset(character, out string reason))
            {
                OnResetFailed?.Invoke(reason);
                return false;
            }

            // Deduct zen cost
            long zenCost = resetData.normalResetRequirement.CalculateZenCost(character.normalResetCount);
            character.zen -= zenCost;

            // Calculate and apply rewards
            int nextResetCount = character.normalResetCount + 1;
            ResetReward reward = ResetReward.CalculateReward(nextResetCount);
            reward.ApplyToCharacter(character);

            // Reset character level and stats
            int oldLevel = character.level;
            character.level = resetData.levelAfterReset;

            if (!resetData.keepStats)
            {
                character.ResetStatPoints();
            }

            // Update reset count
            character.normalResetCount++;

            // Add to history
            if (character.resetHistory == null)
                character.resetHistory = new ResetHistory();

            character.resetHistory.AddEntry(
                ResetType.Normal,
                character.normalResetCount,
                oldLevel,
                reward.BonusStatPoints
            );

            // Trigger event
            OnResetPerformed?.Invoke(ResetType.Normal, character);

            return true;
        }

        /// <summary>
        /// Check if character can perform grand reset
        /// Kiểm tra xem nhân vật có thể Grand Reset không
        /// </summary>
        public bool CanPerformGrandReset(CharacterStats character, out string reason)
        {
            reason = "";

            if (character == null)
            {
                reason = "Invalid character";
                return false;
            }

            // Check if has required normal resets
            if (character.normalResetCount < resetData.grandResetRequirement.MinResetCount)
            {
                reason = $"Need {resetData.grandResetRequirement.MinResetCount} normal resets";
                return false;
            }

            // Check max grand reset limit
            if (character.grandResetCount >= resetData.maxGrandResets)
            {
                reason = $"Maximum grand resets ({resetData.maxGrandResets}) reached";
                return false;
            }

            // Check requirements
            if (!resetData.grandResetRequirement.CheckRequirements(
                character.level, character.zen, character.normalResetCount))
            {
                reason = "Requirements not met";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Perform grand reset - Thực hiện Grand Reset
        /// </summary>
        public bool PerformGrandReset(CharacterStats character)
        {
            if (!CanPerformGrandReset(character, out string reason))
            {
                OnResetFailed?.Invoke(reason);
                return false;
            }

            // Deduct zen cost
            character.zen -= resetData.grandResetRequirement.ZenCost;

            // Apply grand reset bonuses
            character.resetBonusStats += resetData.grandResetBonusStats;
            character.resetDamageMultiplier += resetData.grandDamageBonus;
            character.resetDefenseMultiplier += resetData.grandDefenseBonus;
            character.resetHPMultiplier += resetData.grandHPBonus;

            // Reset character
            int oldLevel = character.level;
            character.level = resetData.levelAfterReset;
            character.normalResetCount = 0; // Reset normal reset count

            if (!resetData.keepStats)
            {
                character.ResetStatPoints();
            }

            // Update grand reset count
            character.grandResetCount++;

            // Add to history
            if (character.resetHistory == null)
                character.resetHistory = new ResetHistory();

            character.resetHistory.AddEntry(
                ResetType.Grand,
                character.grandResetCount,
                oldLevel,
                resetData.grandResetBonusStats
            );

            // Trigger event
            OnResetPerformed?.Invoke(ResetType.Grand, character);

            return true;
        }

        /// <summary>
        /// Check if character can perform master reset
        /// Kiểm tra xem nhân vật có thể Master Reset không
        /// </summary>
        public bool CanPerformMasterReset(CharacterStats character, out string reason)
        {
            reason = "";

            if (character == null)
            {
                reason = "Invalid character";
                return false;
            }

            // Check if already has master reset
            if (resetData.allowOnlyOneMasterReset && character.hasMasterReset)
            {
                reason = "Already performed master reset";
                return false;
            }

            // Check if has required grand resets
            if (character.grandResetCount < resetData.masterResetRequirement.MinResetCount)
            {
                reason = $"Need {resetData.masterResetRequirement.MinResetCount} grand resets";
                return false;
            }

            // Check requirements
            if (!resetData.masterResetRequirement.CheckRequirements(
                character.level, character.zen, character.grandResetCount))
            {
                reason = "Requirements not met";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Perform master reset - Thực hiện Master Reset
        /// </summary>
        public bool PerformMasterReset(CharacterStats character)
        {
            if (!CanPerformMasterReset(character, out string reason))
            {
                OnResetFailed?.Invoke(reason);
                return false;
            }

            // Deduct zen cost
            character.zen -= resetData.masterResetRequirement.ZenCost;

            // Apply master reset bonuses
            character.resetBonusStats += resetData.masterBonusStats;
            character.resetDamageMultiplier += resetData.masterDamageBonus;
            character.resetDefenseMultiplier += resetData.masterDefenseBonus;

            // Reset character
            int oldLevel = character.level;
            character.level = resetData.levelAfterReset;

            if (!resetData.keepStats)
            {
                character.ResetStatPoints();
            }

            // Set master reset flag
            character.hasMasterReset = true;

            // Add to history
            if (character.resetHistory == null)
                character.resetHistory = new ResetHistory();

            character.resetHistory.AddEntry(
                ResetType.Master,
                1,
                oldLevel,
                resetData.masterBonusStats
            );

            // Trigger event
            OnResetPerformed?.Invoke(ResetType.Master, character);

            return true;
        }

        /// <summary>
        /// Get reset info for display
        /// Lấy thông tin reset để hiển thị
        /// </summary>
        public string GetResetInfo(CharacterStats character, ResetType type)
        {
            if (character == null)
                return "Invalid character";

            string info = "";
            switch (type)
            {
                case ResetType.Normal:
                    info += $"Normal Reset #{character.normalResetCount + 1}\n";
                    info += $"Level Required: {resetData.normalResetRequirement.MinLevel}\n";
                    info += $"Zen Cost: {resetData.normalResetRequirement.CalculateZenCost(character.normalResetCount):N0}\n";
                    ResetReward normalReward = ResetReward.CalculateReward(character.normalResetCount + 1);
                    info += $"Bonus Stats: +{normalReward.BonusStatPoints}\n";
                    info += $"Damage Bonus: +{normalReward.DamageBonus * 100:F1}%\n";
                    break;

                case ResetType.Grand:
                    info += $"Grand Reset #{character.grandResetCount + 1}\n";
                    info += $"Requires: {resetData.grandResetRequirement.MinResetCount} Normal Resets\n";
                    info += $"Zen Cost: {resetData.grandResetRequirement.ZenCost:N0}\n";
                    info += $"Bonus Stats: +{resetData.grandResetBonusStats}\n";
                    info += $"Damage Bonus: +{resetData.grandDamageBonus * 100:F0}%\n";
                    break;

                case ResetType.Master:
                    info += "Master Reset\n";
                    info += $"Requires: {resetData.masterResetRequirement.MinResetCount} Grand Resets\n";
                    info += $"Zen Cost: {resetData.masterResetRequirement.ZenCost:N0}\n";
                    info += $"Bonus Stats: +{resetData.masterBonusStats}\n";
                    info += $"Damage Bonus: +{resetData.masterDamageBonus * 100:F0}%\n";
                    break;
            }

            return info;
        }
    }

    /// <summary>
    /// Extended CharacterStats with reset properties
    /// CharacterStats mở rộng với thuộc tính reset
    /// </summary>
    public partial class CharacterStats : MonoBehaviour
    {
        [Header("Character Info")]
        public int level = 1;
        public long zen = 0;

        [Header("Reset Counts")]
        public int normalResetCount = 0;
        public int grandResetCount = 0;
        public bool hasMasterReset = false;

        [Header("Reset Bonuses")]
        public int resetBonusStats = 0;
        public float resetDamageMultiplier = 1f;
        public float resetDefenseMultiplier = 1f;
        public float resetHPMultiplier = 1f;
        public float resetMPMultiplier = 1f;

        [Header("Reset History")]
        public ResetHistory resetHistory;

        /// <summary>
        /// Calculate final damage with reset bonuses
        /// Tính damage cuối cùng với bonus reset
        /// </summary>
        public int CalculateFinalDamage(int baseDamage)
        {
            return Mathf.RoundToInt(baseDamage * resetDamageMultiplier);
        }

        /// <summary>
        /// Calculate final defense with reset bonuses
        /// Tính defense cuối cùng với bonus reset
        /// </summary>
        public int CalculateFinalDefense(int baseDefense)
        {
            return Mathf.RoundToInt(baseDefense * resetDefenseMultiplier);
        }

        /// <summary>
        /// Calculate max HP with reset bonuses
        /// Tính HP tối đa với bonus reset
        /// </summary>
        public int CalculateMaxHP(int baseHP)
        {
            return Mathf.RoundToInt(baseHP * resetHPMultiplier);
        }

        /// <summary>
        /// Calculate max MP with reset bonuses
        /// Tính MP tối đa với bonus reset
        /// </summary>
        public int CalculateMaxMP(int baseMP)
        {
            return Mathf.RoundToInt(baseMP * resetMPMultiplier);
        }

        /// <summary>
        /// Reset stat points (placeholder)
        /// Reset điểm stats (tạm thời)
        /// </summary>
        public void ResetStatPoints()
        {
            // Implement based on your actual stat system
            // Implement dựa trên stat system thực tế
        }
    }
}
