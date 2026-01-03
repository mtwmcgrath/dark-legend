using UnityEngine;

namespace DarkLegend.Skills
{
    /// <summary>
    /// Heal Skill - Skill hồi máu
    /// Heal Skill - Healing abilities
    /// </summary>
    public class HealSkill : ActiveSkill
    {
        [Header("Heal Settings")]
        public HealType healType = HealType.HP;
        public float healAmount = 100f;
        public float healPercentage = 0f;    // 0 = không dùng %, > 0 = % của max HP/MP
        public bool canHealAllies = true;
        public float healRadius = 10f;
        public bool healOverTime = false;
        public float hotDuration = 10f;      // HoT = Heal over Time
        public float hotTickInterval = 1f;
        
        /// <summary>
        /// Execute heal skill / Thực hiện heal skill
        /// </summary>
        protected override void ExecuteSkill(Vector3 targetPosition, GameObject targetObject)
        {
            base.ExecuteSkill(targetPosition, targetObject);
            
            // Tìm targets để heal
            var targets = GetHealTargets(targetPosition, targetObject);
            
            foreach (GameObject target in targets)
            {
                if (healOverTime)
                {
                    ApplyHealOverTime(target);
                }
                else
                {
                    ApplyInstantHeal(target);
                }
            }
            
            Debug.Log($"Heal applied to {targets.Count} targets: {skillData.skillName}");
        }
        
        /// <summary>
        /// Lấy danh sách targets để heal / Get list of targets to heal
        /// </summary>
        protected virtual System.Collections.Generic.List<GameObject> GetHealTargets(Vector3 position, GameObject primaryTarget)
        {
            var targets = new System.Collections.Generic.List<GameObject>();
            
            // Nếu có target cụ thể
            if (primaryTarget != null && IsValidHealTarget(primaryTarget))
            {
                targets.Add(primaryTarget);
                
                // Nếu là AoE heal, tìm thêm allies xung quanh
                if (skillData.targetType == SkillTargetType.AllAlly && canHealAllies)
                {
                    Collider[] colliders = Physics.OverlapSphere(primaryTarget.transform.position, healRadius);
                    foreach (Collider col in colliders)
                    {
                        if (col.gameObject == primaryTarget) continue;
                        if (IsValidHealTarget(col.gameObject) && !targets.Contains(col.gameObject))
                        {
                            targets.Add(col.gameObject);
                        }
                    }
                }
            }
            else
            {
                // Không có target cụ thể, heal bản thân hoặc tất cả allies trong range
                if (skillData.targetType == SkillTargetType.Self)
                {
                    targets.Add(owner);
                }
                else if (canHealAllies)
                {
                    targets.Add(owner); // Luôn heal bản thân
                    
                    Collider[] colliders = Physics.OverlapSphere(position, healRadius);
                    foreach (Collider col in colliders)
                    {
                        if (col.gameObject == owner) continue;
                        if (IsValidHealTarget(col.gameObject))
                        {
                            targets.Add(col.gameObject);
                        }
                    }
                }
            }
            
            return targets;
        }
        
        /// <summary>
        /// Áp dụng heal ngay lập tức / Apply instant heal
        /// </summary>
        protected virtual void ApplyInstantHeal(GameObject target)
        {
            CharacterStats stats = target.GetComponent<CharacterStats>();
            if (stats == null) return;
            
            float healValue = CalculateHealAmount(stats);
            
            if (healType == HealType.HP)
            {
                float oldHP = stats.currentHP;
                stats.currentHP = Mathf.Min(stats.maxHP, stats.currentHP + healValue);
                float actualHeal = stats.currentHP - oldHP;
                
                Debug.Log($"Healed {target.name} for {actualHeal} HP");
            }
            else if (healType == HealType.MP)
            {
                float oldMP = stats.currentMP;
                stats.currentMP = Mathf.Min(stats.maxMP, stats.currentMP + healValue);
                float actualHeal = stats.currentMP - oldMP;
                
                Debug.Log($"Restored {actualHeal} MP to {target.name}");
            }
            else if (healType == HealType.Both)
            {
                stats.currentHP = Mathf.Min(stats.maxHP, stats.currentHP + healValue);
                stats.currentMP = Mathf.Min(stats.maxMP, stats.currentMP + healValue * 0.5f);
            }
            
            // Spawn heal effect
            SpawnHealEffect(target);
        }
        
        /// <summary>
        /// Áp dụng heal theo thời gian / Apply heal over time
        /// </summary>
        protected virtual void ApplyHealOverTime(GameObject target)
        {
            HealOverTimeEffect hotEffect = target.GetComponent<HealOverTimeEffect>();
            if (hotEffect == null)
            {
                hotEffect = target.AddComponent<HealOverTimeEffect>();
            }
            
            float healValue = CalculateHealAmount(target.GetComponent<CharacterStats>());
            
            hotEffect.Initialize(
                healType,
                healValue / (hotDuration / hotTickInterval), // Chia đều heal amount theo số tick
                hotDuration,
                hotTickInterval,
                skillData.icon
            );
            
            SpawnHealEffect(target);
            
            Debug.Log($"HoT applied to {target.name}");
        }
        
        /// <summary>
        /// Tính lượng heal / Calculate heal amount
        /// </summary>
        protected virtual float CalculateHealAmount(CharacterStats targetStats)
        {
            float heal = healAmount * (1f + (currentLevel - 1) * 0.2f);
            
            // Nếu dùng percentage
            if (healPercentage > 0f)
            {
                if (healType == HealType.HP)
                {
                    heal = targetStats.maxHP * healPercentage;
                }
                else if (healType == HealType.MP)
                {
                    heal = targetStats.maxMP * healPercentage;
                }
            }
            
            return heal;
        }
        
        /// <summary>
        /// Spawn heal effect / Tạo hiệu ứng heal
        /// </summary>
        protected virtual void SpawnHealEffect(GameObject target)
        {
            if (skillData.impactEffect != null)
            {
                Vector3 effectPosition = target.transform.position;
                GameObject effect = Instantiate(skillData.impactEffect, effectPosition, Quaternion.identity);
                effect.transform.SetParent(target.transform);
                Destroy(effect, 2f);
            }
        }
        
        /// <summary>
        /// Kiểm tra có phải target hợp lệ để heal không / Check if valid heal target
        /// </summary>
        protected virtual bool IsValidHealTarget(GameObject target)
        {
            CharacterStats stats = target.GetComponent<CharacterStats>();
            if (stats == null) return false;
            
            // Không heal nếu đã full HP/MP
            if (healType == HealType.HP && stats.currentHP >= stats.maxHP) return false;
            if (healType == HealType.MP && stats.currentMP >= stats.maxMP) return false;
            
            // Không heal enemy
            if (target.CompareTag("Enemy") || target.CompareTag("Monster")) return false;
            
            return true;
        }
    }
    
    /// <summary>
    /// Loại heal / Heal types
    /// </summary>
    public enum HealType
    {
        HP,      // Hồi HP
        MP,      // Hồi MP
        Both     // Hồi cả HP và MP
    }
    
    /// <summary>
    /// Component cho Heal over Time effect / HoT effect component
    /// </summary>
    public class HealOverTimeEffect : MonoBehaviour
    {
        private HealType healType;
        private float healPerTick;
        private float duration;
        private float tickInterval;
        private Sprite icon;
        
        private float remainingTime;
        private float nextTickTime;
        
        public void Initialize(HealType type, float healPerTick, float duration, float tickInterval, Sprite icon)
        {
            this.healType = type;
            this.healPerTick = healPerTick;
            this.duration = duration;
            this.tickInterval = tickInterval;
            this.icon = icon;
            
            this.remainingTime = duration;
            this.nextTickTime = tickInterval;
        }
        
        private void Update()
        {
            remainingTime -= Time.deltaTime;
            nextTickTime -= Time.deltaTime;
            
            if (nextTickTime <= 0f)
            {
                ApplyHealTick();
                nextTickTime = tickInterval;
            }
            
            if (remainingTime <= 0f)
            {
                Destroy(this);
            }
        }
        
        private void ApplyHealTick()
        {
            CharacterStats stats = GetComponent<CharacterStats>();
            if (stats == null) return;
            
            if (healType == HealType.HP)
            {
                stats.currentHP = Mathf.Min(stats.maxHP, stats.currentHP + healPerTick);
            }
            else if (healType == HealType.MP)
            {
                stats.currentMP = Mathf.Min(stats.maxMP, stats.currentMP + healPerTick);
            }
            else if (healType == HealType.Both)
            {
                stats.currentHP = Mathf.Min(stats.maxHP, stats.currentHP + healPerTick);
                stats.currentMP = Mathf.Min(stats.maxMP, stats.currentMP + healPerTick * 0.5f);
            }
        }
    }
}
