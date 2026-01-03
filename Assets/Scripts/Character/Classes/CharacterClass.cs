using UnityEngine;

namespace DarkLegend.Character
{
    /// <summary>
    /// Character class types / Các loại class nhân vật
    /// </summary>
    public enum CharacterClassType
    {
        // Base Classes / Classes cơ bản
        DarkKnight,
        DarkWizard,
        FairyElf,
        
        // Advanced Classes (Unlockable) / Classes nâng cao (cần mở khóa)
        MagicGladiator,
        DarkLord,
        Summoner,
        RageFighter,
        
        // Dark Knight Evolutions
        BladeKnight,
        BladeMaster,
        
        // Dark Wizard Evolutions
        SoulMaster,
        GrandMaster,
        
        // Fairy Elf Evolutions
        MuseElf,
        HighElf,
        
        // Magic Gladiator Evolution
        DuelMaster,
        
        // Dark Lord Evolution
        LordEmperor,
        
        // Summoner Evolutions
        BloodySummoner,
        DimensionMaster,
        
        // Rage Fighter Evolution
        FistMaster
    }

    /// <summary>
    /// Weapon types / Loại vũ khí
    /// </summary>
    public enum WeaponType
    {
        Sword,
        Blade,
        TwoHandedSword,
        Staff,
        Wand,
        Bow,
        Crossbow,
        Scepter,
        Shield,
        Book,
        Stick,
        Fist,
        Claw
    }

    /// <summary>
    /// Armor types / Loại giáp
    /// </summary>
    public enum ArmorType
    {
        Heavy,
        Medium,
        Light,
        Robe
    }

    /// <summary>
    /// Character role types / Vai trò nhân vật
    /// </summary>
    public enum CharacterRole
    {
        MeleeDPS,
        Tank,
        MagicDPS,
        RangedDPS,
        Support,
        Healer,
        Hybrid,
        Commander,
        Summoner
    }

    /// <summary>
    /// Base character class / Class nhân vật cơ bản
    /// </summary>
    public abstract class CharacterClass : MonoBehaviour
    {
        [Header("Class Identity")]
        public CharacterClassType ClassType;
        public string ClassName;
        public string Description;
        
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
        public CharacterClassType? NextEvolution;
        
        [Header("Requirements / Yêu cầu")]
        public int UnlockLevel; // Level required on any character / Level cần thiết trên nhân vật bất kỳ
        public bool RequiresQuest;
        
        /// <summary>
        /// Initialize the class / Khởi tạo class
        /// </summary>
        public virtual void Initialize()
        {
            Debug.Log($"Initialized {ClassName}");
        }
        
        /// <summary>
        /// Get total stat at a given level / Lấy tổng chỉ số tại level cho trước
        /// </summary>
        public virtual int GetStatAtLevel(string statName, int level)
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
        /// Check if can evolve / Kiểm tra có thể tiến hóa
        /// </summary>
        public bool CanEvolve(int currentLevel)
        {
            return NextEvolution.HasValue && currentLevel >= UnlockLevel;
        }
        
        /// <summary>
        /// Get class special abilities description / Lấy mô tả kỹ năng đặc biệt
        /// </summary>
        public abstract string GetSpecialAbilities();
    }
}
