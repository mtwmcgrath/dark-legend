using UnityEngine;
using System;

namespace DarkLegend.Character
{
    /// <summary>
    /// Level and experience system
    /// Hệ thống cấp độ và kinh nghiệm
    /// </summary>
    public class LevelSystem : MonoBehaviour
    {
        [Header("Level Info")]
        public int currentLevel = 1;
        public long currentExp = 0;
        public long expToNextLevel = 100;
        
        [Header("References")]
        public CharacterStats characterStats;
        
        // Events
        public event Action<int> OnLevelUp;  // new level
        public event Action<long, long> OnExpChanged;  // current exp, required exp
        
        private void Start()
        {
            if (characterStats == null)
            {
                characterStats = GetComponent<CharacterStats>();
            }
            
            CalculateExpRequirement();
        }
        
        /// <summary>
        /// Calculate EXP requirement for next level
        /// Tính toán yêu cầu EXP cho level tiếp theo
        /// </summary>
        private void CalculateExpRequirement()
        {
            // Formula: BaseEXP * (Level ^ Multiplier)
            expToNextLevel = (long)(Utils.Constants.BASE_EXP_REQUIREMENT * 
                            Mathf.Pow(currentLevel, Utils.Constants.EXP_MULTIPLIER));
        }
        
        /// <summary>
        /// Add experience points
        /// Thêm điểm kinh nghiệm
        /// </summary>
        public void AddExp(long amount)
        {
            currentExp += amount;
            OnExpChanged?.Invoke(currentExp, expToNextLevel);
            
            // Check for level up
            while (currentExp >= expToNextLevel && currentLevel < Utils.Constants.MAX_LEVEL)
            {
                LevelUp();
            }
        }
        
        /// <summary>
        /// Level up the character
        /// Tăng cấp độ nhân vật
        /// </summary>
        private void LevelUp()
        {
            currentLevel++;
            currentExp -= expToNextLevel;
            
            // Grant stat points
            if (characterStats != null)
            {
                characterStats.availableStatPoints += Utils.Constants.STAT_POINTS_PER_LEVEL;
                
                // Auto-increase stats based on class growth
                if (characterStats.classData != null)
                {
                    characterStats.strength += Mathf.RoundToInt(characterStats.classData.strengthGrowth);
                    characterStats.agility += Mathf.RoundToInt(characterStats.classData.agilityGrowth);
                    characterStats.vitality += Mathf.RoundToInt(characterStats.classData.vitalityGrowth);
                    characterStats.energy += Mathf.RoundToInt(characterStats.classData.energyGrowth);
                    
                    characterStats.CalculateDerivedStats();
                    
                    // Fully heal on level up
                    characterStats.currentHP = characterStats.maxHP;
                    characterStats.currentMP = characterStats.maxMP;
                }
            }
            
            CalculateExpRequirement();
            OnLevelUp?.Invoke(currentLevel);
            OnExpChanged?.Invoke(currentExp, expToNextLevel);
            
            Debug.Log($"Level Up! Now level {currentLevel}. Next level requires {expToNextLevel} EXP.");
        }
        
        /// <summary>
        /// Get level progress percentage
        /// Lấy phần trăm tiến độ level
        /// </summary>
        public float GetLevelProgress()
        {
            return (float)currentExp / expToNextLevel;
        }
        
        /// <summary>
        /// Set level directly (for testing or loading saved data)
        /// Đặt level trực tiếp (để test hoặc load dữ liệu đã lưu)
        /// </summary>
        public void SetLevel(int level)
        {
            currentLevel = Mathf.Clamp(level, 1, Utils.Constants.MAX_LEVEL);
            currentExp = 0;
            CalculateExpRequirement();
            
            if (characterStats != null && characterStats.classData != null)
            {
                // Recalculate stats based on new level
                characterStats.InitializeStats();
                
                // Apply level-based growth
                int levelsToAdd = currentLevel - 1;
                characterStats.strength += Mathf.RoundToInt(characterStats.classData.strengthGrowth * levelsToAdd);
                characterStats.agility += Mathf.RoundToInt(characterStats.classData.agilityGrowth * levelsToAdd);
                characterStats.vitality += Mathf.RoundToInt(characterStats.classData.vitalityGrowth * levelsToAdd);
                characterStats.energy += Mathf.RoundToInt(characterStats.classData.energyGrowth * levelsToAdd);
                
                characterStats.availableStatPoints = Utils.Constants.STAT_POINTS_PER_LEVEL * levelsToAdd;
                characterStats.CalculateDerivedStats();
            }
        }
        
        /// <summary>
        /// Calculate total EXP earned
        /// Tính tổng EXP đã kiếm được
        /// </summary>
        public long GetTotalExp()
        {
            long totalExp = currentExp;
            for (int i = 1; i < currentLevel; i++)
            {
                totalExp += (long)(Utils.Constants.BASE_EXP_REQUIREMENT * Mathf.Pow(i, Utils.Constants.EXP_MULTIPLIER));
            }
            return totalExp;
        }
    }
}
