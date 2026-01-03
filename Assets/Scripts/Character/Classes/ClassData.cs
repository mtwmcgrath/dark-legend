using UnityEngine;

namespace DarkLegend.Character
{
    /// <summary>
    /// ScriptableObject for class configuration / ScriptableObject cho cấu hình class
    /// </summary>
    [CreateAssetMenu(fileName = "ClassData", menuName = "Dark Legend/Character/Class Data")]
    public class ClassData : ScriptableObject
    {
        [Header("Class Identity")]
        public CharacterClassType ClassType;
        public string ClassName;
        [TextArea(3, 10)]
        public string Description;
        public Sprite ClassIcon;
        
        [Header("Base Stats / Chỉ số cơ bản")]
        public int BaseStrength;
        public int BaseAgility;
        public int BaseVitality;
        public int BaseEnergy;
        public int BaseCommand; // Only for Dark Lord / Chỉ dành cho Dark Lord
        
        [Header("Stat Growth Per Level / Tăng chỉ số mỗi level")]
        public int StrengthPerLevel;
        public int AgilityPerLevel;
        public int VitalityPerLevel;
        public int EnergyPerLevel;
        public int CommandPerLevel; // Only for Dark Lord
        
        [Header("Equipment / Trang bị")]
        public WeaponType[] AllowedWeapons;
        public ArmorType AllowedArmor;
        
        [Header("Role / Vai trò")]
        public CharacterRole[] Roles;
        
        [Header("Evolution / Tiến hóa")]
        public int EvolutionLevel; // 0 = base, 1 = 2nd evolution, 2 = 3rd evolution
        public ClassData NextEvolution;
        
        [Header("Requirements / Yêu cầu")]
        public int UnlockLevel; // Level required on any character / Level cần thiết trên nhân vật bất kỳ
        public bool RequiresQuest;
        
        [Header("Special Abilities / Kỹ năng đặc biệt")]
        [TextArea(3, 10)]
        public string SpecialAbilities;
        
        [Header("Visual / Hiển thị")]
        public GameObject CharacterModel;
        public Color PrimaryColor = Color.white;
        public Color SecondaryColor = Color.gray;
        
        /// <summary>
        /// Get total stat at a given level / Lấy tổng chỉ số tại level cho trước
        /// </summary>
        public int GetStatAtLevel(string statName, int level)
        {
            return statName switch
            {
                "Strength" => BaseStrength + (StrengthPerLevel * (level - 1)),
                "Agility" => BaseAgility + (AgilityPerLevel * (level - 1)),
                "Vitality" => BaseVitality + (VitalityPerLevel * (level - 1)),
                "Energy" => BaseEnergy + (EnergyPerLevel * (level - 1)),
                "Command" => BaseCommand + (CommandPerLevel * (level - 1)),
                _ => 0
            };
        }
        
        /// <summary>
        /// Check if can use weapon type / Kiểm tra có thể dùng loại vũ khí
        /// </summary>
        public bool CanUseWeapon(WeaponType weaponType)
        {
            foreach (var allowed in AllowedWeapons)
            {
                if (allowed == weaponType)
                    return true;
            }
            return false;
        }
        
        /// <summary>
        /// Check if this is a base class / Kiểm tra có phải class cơ bản
        /// </summary>
        public bool IsBaseClass()
        {
            return ClassType == CharacterClassType.DarkKnight ||
                   ClassType == CharacterClassType.DarkWizard ||
                   ClassType == CharacterClassType.FairyElf ||
                   ClassType == CharacterClassType.MagicGladiator ||
                   ClassType == CharacterClassType.DarkLord ||
                   ClassType == CharacterClassType.Summoner ||
                   ClassType == CharacterClassType.RageFighter;
        }
        
        /// <summary>
        /// Check if this is an unlockable class / Kiểm tra có phải class cần mở khóa
        /// </summary>
        public bool IsUnlockableClass()
        {
            return ClassType == CharacterClassType.MagicGladiator ||
                   ClassType == CharacterClassType.DarkLord ||
                   ClassType == CharacterClassType.Summoner ||
                   ClassType == CharacterClassType.RageFighter;
        }
    }
}
