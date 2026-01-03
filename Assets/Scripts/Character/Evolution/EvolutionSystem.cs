using UnityEngine;
using System;
using System.Collections.Generic;

namespace DarkLegend.Character
{
    /// <summary>
    /// Evolution requirements / Yêu cầu tiến hóa
    /// </summary>
    [System.Serializable]
    public class EvolutionRequirement
    {
        public int RequiredLevel;
        public string EvolutionQuestId;
        public string[] RequiredItemIds;
        public int RequiredZen;
        
        [Header("Minimum Stats / Chỉ số tối thiểu")]
        public int MinStrength;
        public int MinAgility;
        public int MinVitality;
        public int MinEnergy;
        public int MinCommand;
        
        /// <summary>
        /// Check if requirements are met / Kiểm tra đã đáp ứng yêu cầu
        /// </summary>
        public bool AreMet(CharacterStats stats, bool questCompleted, bool hasItems, int currentZen)
        {
            if (stats.Level < RequiredLevel)
                return false;
                
            if (!string.IsNullOrEmpty(EvolutionQuestId) && !questCompleted)
                return false;
                
            if (RequiredItemIds != null && RequiredItemIds.Length > 0 && !hasItems)
                return false;
                
            if (currentZen < RequiredZen)
                return false;
                
            if (stats.Strength < MinStrength)
                return false;
                
            if (stats.Agility < MinAgility)
                return false;
                
            if (stats.Vitality < MinVitality)
                return false;
                
            if (stats.Energy < MinEnergy)
                return false;
                
            if (stats.Command < MinCommand)
                return false;
                
            return true;
        }
    }
    
    /// <summary>
    /// Evolution bonuses / Phần thưởng tiến hóa
    /// </summary>
    [System.Serializable]
    public class EvolutionBonus
    {
        public int BonusStatPoints;
        public string[] UnlockedSkillIds;
        public float StatMultiplier = 1.0f;
        public string NewTitle;
        
        [Header("Bonus Stats / Chỉ số thưởng")]
        public int BonusStrength;
        public int BonusAgility;
        public int BonusVitality;
        public int BonusEnergy;
        public int BonusCommand;
        
        /// <summary>
        /// Apply bonuses to stats / Áp dụng phần thưởng cho chỉ số
        /// </summary>
        public void ApplyBonuses(CharacterStats stats)
        {
            stats.Strength += BonusStrength;
            stats.Agility += BonusAgility;
            stats.Vitality += BonusVitality;
            stats.Energy += BonusEnergy;
            stats.Command += BonusCommand;
            stats.FreePoints += BonusStatPoints;
            
            // Apply multiplier to base stats / Áp dụng hệ số nhân cho chỉ số cơ bản
            if (StatMultiplier != 1.0f)
            {
                stats.Strength = Mathf.RoundToInt(stats.Strength * StatMultiplier);
                stats.Agility = Mathf.RoundToInt(stats.Agility * StatMultiplier);
                stats.Vitality = Mathf.RoundToInt(stats.Vitality * StatMultiplier);
                stats.Energy = Mathf.RoundToInt(stats.Energy * StatMultiplier);
                stats.Command = Mathf.RoundToInt(stats.Command * StatMultiplier);
            }
        }
    }
    
    /// <summary>
    /// Evolution system / Hệ thống tiến hóa
    /// </summary>
    public class EvolutionSystem : MonoBehaviour
    {
        [Header("Evolution Data / Dữ liệu tiến hóa")]
        [SerializeField] private Dictionary<CharacterClassType, EvolutionData> evolutionDatabase = new Dictionary<CharacterClassType, EvolutionData>();
        
        // Events / Sự kiện
        public event Action<CharacterClassType, CharacterClassType> OnEvolutionComplete;
        public event Action<string> OnEvolutionFailed;
        
        /// <summary>
        /// Evolution data / Dữ liệu tiến hóa
        /// </summary>
        [System.Serializable]
        public class EvolutionData
        {
            public CharacterClassType FromClass;
            public CharacterClassType ToClass;
            public EvolutionRequirement Requirements;
            public EvolutionBonus Bonuses;
        }
        
        /// <summary>
        /// Register evolution data / Đăng ký dữ liệu tiến hóa
        /// </summary>
        public void RegisterEvolution(EvolutionData data)
        {
            if (!evolutionDatabase.ContainsKey(data.FromClass))
            {
                evolutionDatabase[data.FromClass] = data;
            }
        }
        
        /// <summary>
        /// Check if can evolve / Kiểm tra có thể tiến hóa
        /// </summary>
        public bool CanEvolve(CharacterClassType currentClass, CharacterStats stats, bool questCompleted, bool hasItems, int currentZen)
        {
            if (!evolutionDatabase.ContainsKey(currentClass))
                return false;
                
            var data = evolutionDatabase[currentClass];
            return data.Requirements.AreMet(stats, questCompleted, hasItems, currentZen);
        }
        
        /// <summary>
        /// Perform evolution / Thực hiện tiến hóa
        /// </summary>
        public bool Evolve(ref CharacterClassType currentClass, CharacterStats stats, bool questCompleted, bool hasItems, int currentZen)
        {
            if (!CanEvolve(currentClass, stats, questCompleted, hasItems, currentZen))
            {
                OnEvolutionFailed?.Invoke("Requirements not met");
                return false;
            }
            
            var data = evolutionDatabase[currentClass];
            var oldClass = currentClass;
            
            // Apply bonuses / Áp dụng phần thưởng
            data.Bonuses.ApplyBonuses(stats);
            
            // Change class / Đổi class
            currentClass = data.ToClass;
            
            OnEvolutionComplete?.Invoke(oldClass, currentClass);
            Debug.Log($"Evolution complete: {oldClass} -> {currentClass}");
            
            return true;
        }
        
        /// <summary>
        /// Get evolution requirements / Lấy yêu cầu tiến hóa
        /// </summary>
        public EvolutionRequirement GetRequirements(CharacterClassType classType)
        {
            if (evolutionDatabase.ContainsKey(classType))
            {
                return evolutionDatabase[classType].Requirements;
            }
            return null;
        }
        
        /// <summary>
        /// Get evolution bonuses / Lấy phần thưởng tiến hóa
        /// </summary>
        public EvolutionBonus GetBonuses(CharacterClassType classType)
        {
            if (evolutionDatabase.ContainsKey(classType))
            {
                return evolutionDatabase[classType].Bonuses;
            }
            return null;
        }
    }
}
