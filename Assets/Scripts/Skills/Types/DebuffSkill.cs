using UnityEngine;
using System.Collections.Generic;

namespace DarkLegend.Skills
{
    /// <summary>
    /// Debuff skill - Skill làm suy yếu kẻ địch
    /// Debuff skill - Skills that weaken enemies
    /// </summary>
    public class DebuffSkill : ActiveSkill
    {
        [Header("Debuff Settings")]
        public DebuffType debuffType;
        public float debuffValue = 0f;
        public float debuffDuration = 10f;
        public float debuffRadius = 5f;
        
        private Dictionary<GameObject, DebuffInstance> activeDebuffs = new Dictionary<GameObject, DebuffInstance>();
        
        /// <summary>
        /// Execute debuff skill / Thực hiện debuff skill
        /// </summary>
        protected override void ExecuteSkill(Vector3 targetPosition, GameObject targetObject)
        {
            base.ExecuteSkill(targetPosition, targetObject);
            
            List<GameObject> targets = GetDebuffTargets(targetPosition, targetObject);
            
            foreach (GameObject target in targets)
            {
                ApplyDebuff(target);
            }
            
            Debug.Log($"Debuff applied to {targets.Count} targets: {skillData.skillName}");
        }
        
        /// <summary>
        /// Lấy danh sách targets để debuff / Get list of targets to debuff
        /// </summary>
        protected virtual List<GameObject> GetDebuffTargets(Vector3 position, GameObject primaryTarget)
        {
            List<GameObject> targets = new List<GameObject>();
            
            // Nếu có target cụ thể
            if (primaryTarget != null && IsEnemy(primaryTarget))
            {
                targets.Add(primaryTarget);
                
                // Nếu là AoE, tìm thêm enemies xung quanh
                if (skillData.aoeRadius > 0f)
                {
                    Collider[] colliders = Physics.OverlapSphere(primaryTarget.transform.position, debuffRadius);
                    foreach (Collider col in colliders)
                    {
                        if (col.gameObject == primaryTarget) continue;
                        if (IsEnemy(col.gameObject) && !targets.Contains(col.gameObject))
                        {
                            targets.Add(col.gameObject);
                        }
                    }
                }
            }
            else
            {
                // Không có target cụ thể, tìm tất cả enemies trong AoE
                Collider[] colliders = Physics.OverlapSphere(position, debuffRadius);
                foreach (Collider col in colliders)
                {
                    if (IsEnemy(col.gameObject))
                    {
                        targets.Add(col.gameObject);
                    }
                }
            }
            
            return targets;
        }
        
        /// <summary>
        /// Áp dụng debuff lên target / Apply debuff to target
        /// </summary>
        protected virtual void ApplyDebuff(GameObject target)
        {
            CharacterStats stats = target.GetComponent<CharacterStats>();
            if (stats == null) return;
            
            // Tính giá trị debuff dựa trên level
            float actualDebuffValue = debuffValue * (1f + (currentLevel - 1) * 0.15f);
            float duration = debuffDuration;
            
            // Nếu đã có debuff, remove cái cũ trước
            if (activeDebuffs.ContainsKey(target))
            {
                RemoveDebuff(target);
            }
            
            // Tạo debuff instance mới
            DebuffInstance debuff = new DebuffInstance
            {
                debuffType = this.debuffType,
                value = actualDebuffValue,
                duration = duration,
                remainingTime = duration,
                source = owner,
                tickInterval = 1f,  // Tick mỗi giây cho DoT
                nextTickTime = 1f
            };
            
            // Áp dụng debuff effect
            ApplyDebuffEffect(stats, debuff);
            
            // Lưu debuff instance
            activeDebuffs[target] = debuff;
            
            Debug.Log($"Debuff {debuffType} applied to {target.name}: -{actualDebuffValue} for {duration}s");
        }
        
        /// <summary>
        /// Áp dụng hiệu ứng debuff lên stats / Apply debuff effect to stats
        /// </summary>
        protected virtual void ApplyDebuffEffect(CharacterStats stats, DebuffInstance debuff)
        {
            switch (debuff.debuffType)
            {
                case DebuffType.AttackPower:
                    stats.attackPower = Mathf.Max(0, stats.attackPower - debuff.value);
                    break;
                    
                case DebuffType.Defense:
                    stats.defense = Mathf.Max(0, stats.defense - debuff.value);
                    break;
                    
                case DebuffType.AttackSpeed:
                    // TODO: Apply attack speed debuff
                    break;
                    
                case DebuffType.MovementSpeed:
                    // TODO: Apply movement speed debuff
                    break;
                    
                case DebuffType.Stun:
                    // TODO: Apply stun effect
                    break;
                    
                case DebuffType.Silence:
                    // TODO: Prevent skill usage
                    break;
                    
                case DebuffType.Blind:
                    // TODO: Reduce accuracy
                    break;
            }
        }
        
        /// <summary>
        /// Loại bỏ debuff khỏi target / Remove debuff from target
        /// </summary>
        protected virtual void RemoveDebuff(GameObject target)
        {
            if (!activeDebuffs.ContainsKey(target)) return;
            
            CharacterStats stats = target.GetComponent<CharacterStats>();
            if (stats != null)
            {
                DebuffInstance debuff = activeDebuffs[target];
                RemoveDebuffEffect(stats, debuff);
            }
            
            activeDebuffs.Remove(target);
        }
        
        /// <summary>
        /// Loại bỏ hiệu ứng debuff khỏi stats / Remove debuff effect from stats
        /// </summary>
        protected virtual void RemoveDebuffEffect(CharacterStats stats, DebuffInstance debuff)
        {
            switch (debuff.debuffType)
            {
                case DebuffType.AttackPower:
                    stats.attackPower += debuff.value;
                    break;
                    
                case DebuffType.Defense:
                    stats.defense += debuff.value;
                    break;
            }
        }
        
        /// <summary>
        /// Update để countdown debuff duration và xử lý DoT / Update to countdown and handle DoT
        /// </summary>
        protected override void Update()
        {
            base.Update();
            
            // Update tất cả active debuffs
            List<GameObject> expiredDebuffs = new List<GameObject>();
            
            foreach (var kvp in activeDebuffs)
            {
                DebuffInstance debuff = kvp.Value;
                debuff.remainingTime -= Time.deltaTime;
                
                // Xử lý DoT effects
                if (debuff.debuffType == DebuffType.Poison || debuff.debuffType == DebuffType.Burn)
                {
                    debuff.nextTickTime -= Time.deltaTime;
                    if (debuff.nextTickTime <= 0f)
                    {
                        ApplyDoTTick(kvp.Key, debuff);
                        debuff.nextTickTime = debuff.tickInterval;
                    }
                }
                
                if (debuff.remainingTime <= 0f)
                {
                    expiredDebuffs.Add(kvp.Key);
                }
            }
            
            // Remove expired debuffs
            foreach (GameObject target in expiredDebuffs)
            {
                RemoveDebuff(target);
                Debug.Log($"Debuff expired on {target.name}");
            }
        }
        
        /// <summary>
        /// Áp dụng damage từ DoT / Apply damage from DoT
        /// </summary>
        protected virtual void ApplyDoTTick(GameObject target, DebuffInstance debuff)
        {
            CharacterStats stats = target.GetComponent<CharacterStats>();
            if (stats == null) return;
            
            float damage = debuff.value;
            stats.currentHP = Mathf.Max(0, stats.currentHP - damage);
            
            Debug.Log($"DoT damage: {damage} to {target.name} (HP: {stats.currentHP}/{stats.maxHP})");
        }
        
        /// <summary>
        /// Kiểm tra có phải enemy không / Check if is enemy
        /// </summary>
        protected virtual bool IsEnemy(GameObject target)
        {
            // TODO: Implement proper enemy detection
            // Tạm thời return true nếu có tag Enemy hoặc Monster
            return target.CompareTag("Enemy") || target.CompareTag("Monster");
        }
    }
    
    /// <summary>
    /// Loại debuff / Debuff types
    /// </summary>
    public enum DebuffType
    {
        AttackPower,      // Giảm sức tấn công
        Defense,          // Giảm phòng thủ
        AttackSpeed,      // Giảm tốc độ tấn công
        MovementSpeed,    // Giảm tốc độ di chuyển
        Stun,             // Choáng, không thể hành động
        Silence,          // Im lặng, không thể dùng skill
        Blind,            // Mù, giảm độ chính xác
        Poison,           // Độc, mất HP theo thời gian
        Burn              // Cháy, mất HP theo thời gian và giảm defense
    }
    
    /// <summary>
    /// Instance của một debuff đang active / Active debuff instance
    /// </summary>
    public class DebuffInstance
    {
        public DebuffType debuffType;
        public float value;
        public float duration;
        public float remainingTime;
        public GameObject source;
        public float tickInterval;     // Cho DoT effects
        public float nextTickTime;     // Thời gian tick tiếp theo
    }
}
