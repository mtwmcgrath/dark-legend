using UnityEngine;

namespace DarkLegend.Character
{
    /// <summary>
    /// ScriptableObject for character class configuration
    /// ScriptableObject cho cấu hình class nhân vật
    /// </summary>
    [CreateAssetMenu(fileName = "New Character Class", menuName = "Dark Legend/Character Class Data")]
    public class CharacterClassData : ScriptableObject
    {
        [Header("Class Information")]
        public CharacterClass classType;
        public string className;
        [TextArea(3, 5)]
        public string description;
        public Sprite classIcon;
        
        [Header("Base Stats")]
        public int baseStrength = 20;
        public int baseAgility = 20;
        public int baseVitality = 20;
        public int baseEnergy = 20;
        
        [Header("Stat Growth Per Level")]
        [Range(0.5f, 3.0f)]
        public float strengthGrowth = 1.0f;
        [Range(0.5f, 3.0f)]
        public float agilityGrowth = 1.0f;
        [Range(0.5f, 3.0f)]
        public float vitalityGrowth = 1.0f;
        [Range(0.5f, 3.0f)]
        public float energyGrowth = 1.0f;
        
        [Header("Combat Multipliers")]
        [Range(0.5f, 2.0f)]
        public float physicalDamageMultiplier = 1.0f;
        [Range(0.5f, 2.0f)]
        public float magicDamageMultiplier = 1.0f;
        [Range(0.5f, 2.0f)]
        public float attackSpeedMultiplier = 1.0f;
        [Range(0.5f, 2.0f)]
        public float defenseMultiplier = 1.0f;
        
        [Header("Starting Skills")]
        public string[] startingSkills;
        
        [Header("Visual")]
        public GameObject characterPrefab;
    }
}
