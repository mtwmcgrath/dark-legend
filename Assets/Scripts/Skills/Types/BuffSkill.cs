using UnityEngine;
using System.Collections.Generic;

namespace DarkLegend.Skills
{
    /// <summary>
    /// Buff skill - Skill tăng cường bản thân hoặc đồng đội
    /// Buff skill - Skills that enhance self or allies
    /// </summary>
    public class BuffSkill : ActiveSkill
    {
        [Header("Buff Settings")]
        public BuffType buffType;
        public float buffValue = 0f;
        public float buffDuration = 30f;
        public bool canBuffAllies = true;
        public float buffRadius = 10f;
        
        private Dictionary<GameObject, BuffInstance> activeBuffs = new Dictionary<GameObject, BuffInstance>();
        
        /// <summary>
        /// Execute buff skill / Thực hiện buff skill
        /// </summary>
        protected override void ExecuteSkill(Vector3 targetPosition, GameObject targetObject)
        {
            base.ExecuteSkill(targetPosition, targetObject);
            
            List<GameObject> targets = GetBuffTargets(targetPosition);
            
            foreach (GameObject target in targets)
            {
                ApplyBuff(target);
            }
            
            Debug.Log($"Buff applied to {targets.Count} targets: {skillData.skillName}");
        }
        
        /// <summary>
        /// Lấy danh sách targets để buff / Get list of targets to buff
        /// </summary>
        protected virtual List<GameObject> GetBuffTargets(Vector3 position)
        {
            List<GameObject> targets = new List<GameObject>();
            
            // Luôn buff bản thân
            targets.Add(owner);
            
            // Nếu có thể buff đồng đội
            if (canBuffAllies && skillData.targetType == SkillTargetType.AllAlly)
            {
                Collider[] colliders = Physics.OverlapSphere(position, buffRadius);
                
                foreach (Collider col in colliders)
                {
                    if (col.gameObject == owner) continue;
                    
                    // TODO: Check if is ally
                    // Cần có system để phân biệt ally/enemy
                    if (IsAlly(col.gameObject))
                    {
                        targets.Add(col.gameObject);
                    }
                }
            }
            
            return targets;
        }
        
        /// <summary>
        /// Áp dụng buff lên target / Apply buff to target
        /// </summary>
        protected virtual void ApplyBuff(GameObject target)
        {
            CharacterStats stats = target.GetComponent<CharacterStats>();
            if (stats == null) return;
            
            // Tính giá trị buff dựa trên level
            float actualBuffValue = buffValue * (1f + (currentLevel - 1) * 0.15f);
            float duration = buffDuration;
            
            // Nếu đã có buff, remove cái cũ trước
            if (activeBuffs.ContainsKey(target))
            {
                RemoveBuff(target);
            }
            
            // Tạo buff instance mới
            BuffInstance buff = new BuffInstance
            {
                buffType = this.buffType,
                value = actualBuffValue,
                duration = duration,
                remainingTime = duration,
                source = owner
            };
            
            // Áp dụng buff effect
            ApplyBuffEffect(stats, buff);
            
            // Lưu buff instance
            activeBuffs[target] = buff;
            
            Debug.Log($"Buff {buffType} applied to {target.name}: +{actualBuffValue} for {duration}s");
        }
        
        /// <summary>
        /// Áp dụng hiệu ứng buff lên stats / Apply buff effect to stats
        /// </summary>
        protected virtual void ApplyBuffEffect(CharacterStats stats, BuffInstance buff)
        {
            switch (buff.buffType)
            {
                case BuffType.AttackPower:
                    stats.attackPower += buff.value;
                    break;
                    
                case BuffType.Defense:
                    stats.defense += buff.value;
                    break;
                    
                case BuffType.AttackSpeed:
                    // TODO: Apply attack speed buff
                    break;
                    
                case BuffType.MovementSpeed:
                    // TODO: Apply movement speed buff
                    break;
                    
                case BuffType.CritRate:
                    stats.critRate += buff.value;
                    break;
                    
                case BuffType.MaxHP:
                    stats.maxHP += buff.value;
                    break;
                    
                case BuffType.MaxMP:
                    stats.maxMP += buff.value;
                    break;
            }
        }
        
        /// <summary>
        /// Loại bỏ buff khỏi target / Remove buff from target
        /// </summary>
        protected virtual void RemoveBuff(GameObject target)
        {
            if (!activeBuffs.ContainsKey(target)) return;
            
            CharacterStats stats = target.GetComponent<CharacterStats>();
            if (stats != null)
            {
                BuffInstance buff = activeBuffs[target];
                RemoveBuffEffect(stats, buff);
            }
            
            activeBuffs.Remove(target);
        }
        
        /// <summary>
        /// Loại bỏ hiệu ứng buff khỏi stats / Remove buff effect from stats
        /// </summary>
        protected virtual void RemoveBuffEffect(CharacterStats stats, BuffInstance buff)
        {
            switch (buff.buffType)
            {
                case BuffType.AttackPower:
                    stats.attackPower -= buff.value;
                    break;
                    
                case BuffType.Defense:
                    stats.defense -= buff.value;
                    break;
                    
                case BuffType.CritRate:
                    stats.critRate -= buff.value;
                    break;
                    
                case BuffType.MaxHP:
                    stats.maxHP -= buff.value;
                    break;
                    
                case BuffType.MaxMP:
                    stats.maxMP -= buff.value;
                    break;
            }
        }
        
        /// <summary>
        /// Update để countdown buff duration / Update to countdown buff duration
        /// </summary>
        protected override void Update()
        {
            base.Update();
            
            // Update tất cả active buffs
            List<GameObject> expiredBuffs = new List<GameObject>();
            
            foreach (var kvp in activeBuffs)
            {
                BuffInstance buff = kvp.Value;
                buff.remainingTime -= Time.deltaTime;
                
                if (buff.remainingTime <= 0f)
                {
                    expiredBuffs.Add(kvp.Key);
                }
            }
            
            // Remove expired buffs
            foreach (GameObject target in expiredBuffs)
            {
                RemoveBuff(target);
                Debug.Log($"Buff expired on {target.name}");
            }
        }
        
        /// <summary>
        /// Kiểm tra có phải ally không / Check if is ally
        /// </summary>
        protected virtual bool IsAlly(GameObject target)
        {
            // TODO: Implement proper ally detection
            // Tạm thời return true nếu cùng layer hoặc cùng tag
            return target.CompareTag("Player") || target.CompareTag("Ally");
        }
    }
    
    /// <summary>
    /// Loại buff / Buff types
    /// </summary>
    public enum BuffType
    {
        AttackPower,      // Tăng sức tấn công
        Defense,          // Tăng phòng thủ
        AttackSpeed,      // Tăng tốc độ tấn công
        MovementSpeed,    // Tăng tốc độ di chuyển
        CritRate,         // Tăng tỉ lệ chí mạng
        MaxHP,            // Tăng HP tối đa
        MaxMP,            // Tăng MP tối đa
        HPRegen,          // Hồi HP liên tục
        MPRegen           // Hồi MP liên tục
    }
    
    /// <summary>
    /// Instance của một buff đang active / Active buff instance
    /// </summary>
    public class BuffInstance
    {
        public BuffType buffType;
        public float value;
        public float duration;
        public float remainingTime;
        public GameObject source;
    }
}
