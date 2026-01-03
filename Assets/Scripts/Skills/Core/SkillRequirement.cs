using UnityEngine;
using System.Collections.Generic;

namespace DarkLegend.Skills
{
    /// <summary>
    /// Yêu cầu để học và sử dụng skill
    /// Requirements to learn and use skills
    /// </summary>
    [CreateAssetMenu(fileName = "New Requirement", menuName = "Dark Legend/Skills/Requirement")]
    public class SkillRequirement : ScriptableObject
    {
        [Header("Level Requirement")]
        [Tooltip("Level tối thiểu / Minimum level")]
        public int requiredLevel = 1;
        
        [Header("Stat Requirements")]
        [Tooltip("STR tối thiểu / Minimum STR")]
        public int requiredSTR = 0;
        
        [Tooltip("AGI tối thiểu / Minimum AGI")]
        public int requiredAGI = 0;
        
        [Tooltip("VIT tối thiểu / Minimum VIT")]
        public int requiredVIT = 0;
        
        [Tooltip("ENE tối thiểu / Minimum ENE")]
        public int requiredENE = 0;
        
        [Header("Class Requirement")]
        [Tooltip("Class được phép dùng / Allowed classes")]
        public List<CharacterClass> allowedClasses = new List<CharacterClass>();
        
        [Header("Prerequisite Skills")]
        [Tooltip("Skills cần học trước / Skills that must be learned first")]
        public List<SkillData> prerequisiteSkills = new List<SkillData>();
        
        [Tooltip("Level tối thiểu của prerequisite skills / Minimum level of prerequisite skills")]
        public int prerequisiteSkillLevel = 1;
        
        [Header("Other Requirements")]
        [Tooltip("Cần weapon cụ thể / Requires specific weapon")]
        public bool requiresWeapon = false;
        
        [Tooltip("Loại weapon cần / Required weapon type")]
        public WeaponType requiredWeaponType;
        
        /// <summary>
        /// Kiểm tra có đủ điều kiện không / Check if requirements are met
        /// </summary>
        public bool IsMet(GameObject owner)
        {
            CharacterStats stats = owner.GetComponent<CharacterStats>();
            if (stats == null)
            {
                Debug.LogWarning("Character has no CharacterStats component!");
                return false;
            }
            
            // Kiểm tra level
            if (stats.level < requiredLevel)
            {
                return false;
            }
            
            // Kiểm tra stats
            if (stats.STR < requiredSTR) return false;
            if (stats.AGI < requiredAGI) return false;
            if (stats.VIT < requiredVIT) return false;
            if (stats.ENE < requiredENE) return false;
            
            // Kiểm tra class
            CharacterClass charClass = owner.GetComponent<CharacterClass>();
            if (allowedClasses.Count > 0)
            {
                if (charClass == null || !allowedClasses.Contains(charClass.classType))
                {
                    return false;
                }
            }
            
            // Kiểm tra prerequisite skills
            if (prerequisiteSkills.Count > 0)
            {
                SkillManager skillManager = owner.GetComponent<SkillManager>();
                if (skillManager == null) return false;
                
                foreach (SkillData prereqSkill in prerequisiteSkills)
                {
                    SkillBase skill = skillManager.GetSkill(prereqSkill.skillName);
                    if (skill == null || skill.currentLevel < prerequisiteSkillLevel)
                    {
                        return false;
                    }
                }
            }
            
            // Kiểm tra weapon
            if (requiresWeapon)
            {
                // TODO: Implement weapon check with equipment system
                // Cần equipment system để check weapon type
            }
            
            return true;
        }
        
        /// <summary>
        /// Lấy danh sách điều kiện chưa đạt / Get list of unmet requirements
        /// </summary>
        public List<string> GetUnmetRequirements(GameObject owner)
        {
            List<string> unmet = new List<string>();
            
            CharacterStats stats = owner.GetComponent<CharacterStats>();
            if (stats == null) return unmet;
            
            if (stats.level < requiredLevel)
            {
                unmet.Add($"Level {requiredLevel} required (current: {stats.level})");
            }
            
            if (stats.STR < requiredSTR)
            {
                unmet.Add($"STR {requiredSTR} required (current: {stats.STR})");
            }
            
            if (stats.AGI < requiredAGI)
            {
                unmet.Add($"AGI {requiredAGI} required (current: {stats.AGI})");
            }
            
            if (stats.VIT < requiredVIT)
            {
                unmet.Add($"VIT {requiredVIT} required (current: {stats.VIT})");
            }
            
            if (stats.ENE < requiredENE)
            {
                unmet.Add($"ENE {requiredENE} required (current: {stats.ENE})");
            }
            
            // Kiểm tra prerequisite skills
            if (prerequisiteSkills.Count > 0)
            {
                SkillManager skillManager = owner.GetComponent<SkillManager>();
                if (skillManager != null)
                {
                    foreach (SkillData prereqSkill in prerequisiteSkills)
                    {
                        SkillBase skill = skillManager.GetSkill(prereqSkill.skillName);
                        if (skill == null)
                        {
                            unmet.Add($"Must learn {prereqSkill.skillName} first");
                        }
                        else if (skill.currentLevel < prerequisiteSkillLevel)
                        {
                            unmet.Add($"{prereqSkill.skillName} must be level {prerequisiteSkillLevel}");
                        }
                    }
                }
            }
            
            return unmet;
        }
    }
    
    /// <summary>
    /// Loại class nhân vật / Character class types
    /// </summary>
    public enum CharacterClassType
    {
        DarkKnight,
        DarkWizard,
        Elf
    }
    
    /// <summary>
    /// Component định nghĩa class của nhân vật
    /// Component defining character class
    /// </summary>
    public class CharacterClass : MonoBehaviour
    {
        public CharacterClassType classType = CharacterClassType.DarkKnight;
    }
    
    /// <summary>
    /// Loại weapon / Weapon types
    /// </summary>
    public enum WeaponType
    {
        Sword,
        TwoHandedSword,
        Axe,
        Mace,
        Spear,
        Staff,
        Scepter,
        Bow,
        Crossbow
    }
}
