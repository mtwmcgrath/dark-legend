using UnityEngine;

namespace DarkLegend.Skills
{
    /// <summary>
    /// ScriptableObject chứa data cấu hình cho skill
    /// ScriptableObject containing skill configuration data
    /// </summary>
    [CreateAssetMenu(fileName = "New Skill", menuName = "Dark Legend/Skills/Skill Data")]
    public class SkillData : ScriptableObject
    {
        [Header("Basic Info")]
        public string skillName;
        public string skillNameVN;
        [TextArea(3, 6)]
        public string description;
        [TextArea(3, 6)]
        public string descriptionVN;
        public Sprite icon;
        
        [Header("Skill Type")]
        public SkillType skillType;
        public SkillElement element;
        public SkillTargetType targetType;
        
        [Header("Level Settings")]
        public int maxLevel = 20;
        public int unlockLevel = 1;
        
        [Header("Damage & Stats")]
        public float baseDamage = 100f;
        public float damagePerLevel = 10f;
        public float strRatio = 0f;      // Tỉ lệ STR ảnh hưởng damage
        public float agiRatio = 0f;      // Tỉ lệ AGI ảnh hưởng damage
        public float vitRatio = 0f;      // Tỉ lệ VIT ảnh hưởng
        public float eneRatio = 0f;      // Tỉ lệ ENE ảnh hưởng damage (magic)
        
        [Header("Range & Area")]
        public float castRange = 5f;
        public float aoeRadius = 0f;     // 0 = single target
        
        [Header("Timing")]
        public float castTime = 0f;      // Thời gian cast
        public float duration = 0f;      // Thời gian effect (cho buff/debuff)
        
        [Header("References")]
        public SkillCooldown cooldown;
        public SkillCost cost;
        public SkillRequirement requirement;
        
        [Header("Visual Effects")]
        public GameObject castEffect;
        public GameObject projectilePrefab;
        public GameObject impactEffect;
        public AudioClip castSound;
        public AudioClip impactSound;
        
        [Header("Advanced")]
        public bool canCrit = true;
        public bool pierceArmor = false;
        public int maxTargets = 1;
        public float knockbackForce = 0f;
        
        /// <summary>
        /// Tính damage ở level cụ thể / Calculate damage at specific level
        /// </summary>
        public float GetDamageAtLevel(int level)
        {
            return baseDamage + (damagePerLevel * (level - 1));
        }
        
        /// <summary>
        /// Lấy mô tả skill với level / Get skill description with level info
        /// </summary>
        public string GetDescription(int level)
        {
            string desc = description;
            desc = desc.Replace("{damage}", GetDamageAtLevel(level).ToString("F0"));
            desc = desc.Replace("{level}", level.ToString());
            
            if (cooldown != null)
            {
                desc += $"\nCooldown: {cooldown.GetCooldownTime(level)}s";
            }
            
            if (cost != null)
            {
                desc += $"\nMP Cost: {cost.GetMPCost(level)}";
            }
            
            return desc;
        }
    }
    
    /// <summary>
    /// Loại skill / Skill types
    /// </summary>
    public enum SkillType
    {
        Active,      // Skill chủ động
        Passive,     // Skill bị động
        Buff,        // Buff bản thân/đồng đội
        Debuff,      // Debuff kẻ địch
        Ultimate     // Ultimate skill
    }
    
    /// <summary>
    /// Nguyên tố skill / Skill elements
    /// </summary>
    public enum SkillElement
    {
        Physical,    // Vật lý
        Fire,        // Lửa
        Ice,         // Băng
        Lightning,   // Sét
        Poison,      // Độc
        Holy,        // Thiêng liêng
        Dark         // Bóng tối
    }
    
    /// <summary>
    /// Loại target / Target types
    /// </summary>
    public enum SkillTargetType
    {
        Self,           // Bản thân
        SingleEnemy,    // 1 kẻ địch
        MultipleEnemy,  // Nhiều kẻ địch
        AllEnemy,       // Tất cả kẻ địch trong range
        SingleAlly,     // 1 đồng minh
        AllAlly,        // Tất cả đồng minh
        Area,           // Khu vực chỉ định
        Direction       // Hướng
    }
}
