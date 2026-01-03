using UnityEngine;

namespace DarkLegend.Character
{
    /// <summary>
    /// Character class types
    /// Các loại class nhân vật
    /// </summary>
    public enum CharacterClass
    {
        DarkKnight,     // Hiệp Sĩ Bóng Tối - High STR, VIT - Melee
        DarkWizard,     // Pháp Sư Bóng Tối - High ENE - Magic
        Elf             // Tiên Nữ - High AGI - Ranged
    }

    /// <summary>
    /// Character class bonuses and multipliers
    /// Bonus và hệ số của các class
    /// </summary>
    [System.Serializable]
    public class ClassStats
    {
        public CharacterClass classType;
        public string className;
        public string description;
        
        // Starting stats
        public int baseStrength = 20;
        public int baseAgility = 20;
        public int baseVitality = 20;
        public int baseEnergy = 20;
        
        // Stat growth per level
        public float strengthGrowth = 1.0f;
        public float agilityGrowth = 1.0f;
        public float vitalityGrowth = 1.0f;
        public float energyGrowth = 1.0f;
        
        // Combat modifiers
        public float physicalDamageMultiplier = 1.0f;
        public float magicDamageMultiplier = 1.0f;
        public float attackSpeedMultiplier = 1.0f;
        public float defenseMultiplier = 1.0f;
    }
}
