using UnityEngine;

namespace DarkLegend.Combat
{
    /// <summary>
    /// ScriptableObject for skill data configuration
    /// ScriptableObject cho cấu hình dữ liệu skill
    /// </summary>
    [CreateAssetMenu(fileName = "New Skill", menuName = "Dark Legend/Skill Data")]
    public class SkillData : ScriptableObject
    {
        [Header("Basic Info")]
        public string skillName;
        [TextArea(2, 4)]
        public string description;
        public Sprite icon;
        
        [Header("Skill Properties")]
        public SkillType skillType;
        public SkillTargetType targetType;
        
        [Header("Costs & Cooldown")]
        public int manaCost = 10;
        public float cooldown = 5f;
        public float castTime = 0f;
        
        [Header("Range & Targeting")]
        public float range = 10f;
        public float aoeRadius = 0f;
        public int maxTargets = 1;
        
        [Header("Damage/Healing")]
        public float damageMultiplier = 1.5f;
        public float healAmount = 0f;
        
        [Header("Visual & Audio")]
        public GameObject effectPrefab;
        public GameObject impactEffectPrefab;
        public AudioClip castSound;
        public AudioClip impactSound;
        
        [Header("Animation")]
        public string animationTrigger = "Skill";
        
        [Header("Requirements")]
        public int requiredLevel = 1;
        public Character.CharacterClass[] allowedClasses;
        
        /// <summary>
        /// Create a skill instance from this data
        /// Tạo một instance skill từ dữ liệu này
        /// </summary>
        public Skill CreateSkillInstance()
        {
            return new Skill
            {
                skillName = this.skillName,
                description = this.description,
                skillType = this.skillType,
                targetType = this.targetType,
                manaCost = this.manaCost,
                cooldown = this.cooldown,
                castTime = this.castTime,
                range = this.range,
                damageMultiplier = this.damageMultiplier,
                aoeRadius = this.aoeRadius,
                maxTargets = this.maxTargets,
                effectPrefab = this.effectPrefab,
                castSound = this.castSound
            };
        }
        
        /// <summary>
        /// Check if character class can use this skill
        /// Kiểm tra class nhân vật có thể dùng skill này không
        /// </summary>
        public bool CanClassUse(Character.CharacterClass characterClass)
        {
            if (allowedClasses == null || allowedClasses.Length == 0)
            {
                return true; // No class restriction
            }
            
            foreach (var allowedClass in allowedClasses)
            {
                if (allowedClass == characterClass)
                {
                    return true;
                }
            }
            
            return false;
        }
    }
}
