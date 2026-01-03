using UnityEngine;
using System.Collections.Generic;

namespace DarkLegend.Skills
{
    /// <summary>
    /// AoE Skill - Skill tấn công diện rộng
    /// AoE Skill - Area of Effect attack skills
    /// </summary>
    public class AoESkill : ActiveSkill
    {
        [Header("AoE Settings")]
        public LayerMask enemyLayer;
        public bool showAoEIndicator = true;
        public GameObject aoeIndicatorPrefab;
        
        private GameObject currentIndicator;
        
        /// <summary>
        /// Execute AoE skill / Thực hiện AoE skill
        /// </summary>
        protected override void ExecuteSkill(Vector3 targetPosition, GameObject targetObject)
        {
            base.ExecuteSkill(targetPosition, targetObject);
            
            // Tìm tất cả enemies trong AoE radius
            List<GameObject> hitTargets = GetTargetsInArea(targetPosition);
            
            // Apply damage lên từng target
            foreach (GameObject target in hitTargets)
            {
                DealDamageToTarget(target);
            }
            
            // Spawn AoE effect
            if (skillData.impactEffect != null)
            {
                GameObject effect = Instantiate(skillData.impactEffect, targetPosition, Quaternion.identity);
                
                // Scale effect theo AoE radius
                if (skillData.aoeRadius > 0)
                {
                    float scale = skillData.aoeRadius / 5f; // 5 là default radius
                    effect.transform.localScale = Vector3.one * scale;
                }
                
                Destroy(effect, 3f);
            }
            
            Debug.Log($"AoE skill hit {hitTargets.Count} targets: {skillData.skillName}");
        }
        
        /// <summary>
        /// Lấy tất cả targets trong AoE / Get all targets in AoE
        /// </summary>
        protected virtual List<GameObject> GetTargetsInArea(Vector3 center)
        {
            List<GameObject> targets = new List<GameObject>();
            
            Collider[] colliders = Physics.OverlapSphere(center, skillData.aoeRadius, enemyLayer);
            
            int targetCount = 0;
            foreach (Collider col in colliders)
            {
                if (IsValidEnemy(col.gameObject))
                {
                    targets.Add(col.gameObject);
                    targetCount++;
                    
                    // Giới hạn số target nếu có
                    if (skillData.maxTargets > 0 && targetCount >= skillData.maxTargets)
                    {
                        break;
                    }
                }
            }
            
            return targets;
        }
        
        /// <summary>
        /// Gây damage lên một target / Deal damage to a target
        /// </summary>
        protected virtual void DealDamageToTarget(GameObject target)
        {
            CharacterStats targetStats = target.GetComponent<CharacterStats>();
            if (targetStats == null) return;
            
            CharacterStats ownerStats = owner.GetComponent<CharacterStats>();
            if (ownerStats == null) return;
            
            // Tính damage
            float damage = CalculateDamage(ownerStats, targetStats);
            
            // Apply damage
            targetStats.currentHP = Mathf.Max(0, targetStats.currentHP - damage);
            
            // Spawn damage number
            ShowDamageNumber(target.transform.position, damage);
            
            Debug.Log($"Dealt {damage} damage to {target.name}");
        }
        
        /// <summary>
        /// Tính damage dựa trên formula / Calculate damage based on formula
        /// </summary>
        protected virtual float CalculateDamage(CharacterStats attacker, CharacterStats defender)
        {
            // Base damage từ skill
            float baseDamage = GetDamage();
            
            // Stat bonus
            float statBonus = (attacker.STR * skillData.strRatio) + 
                            (attacker.ENE * skillData.eneRatio) +
                            (attacker.AGI * skillData.agiRatio);
            
            float totalDamage = baseDamage + statBonus + attacker.attackPower;
            
            // Critical hit check
            if (skillData.canCrit && Random.value < attacker.critRate)
            {
                totalDamage *= 2f; // Crit = x2 damage
                Debug.Log("Critical Hit!");
            }
            
            // Defense reduction
            if (!skillData.pierceArmor)
            {
                float damageReduction = defender.defense / (defender.defense + 100f);
                totalDamage *= (1f - damageReduction);
            }
            
            return Mathf.Max(1f, totalDamage);
        }
        
        /// <summary>
        /// Hiển thị damage number / Show damage number
        /// </summary>
        protected virtual void ShowDamageNumber(Vector3 position, float damage)
        {
            // TODO: Implement damage number popup
            // Cần có UI system để hiển thị damage numbers
        }
        
        /// <summary>
        /// Kiểm tra có phải enemy hợp lệ không / Check if valid enemy
        /// </summary>
        protected virtual bool IsValidEnemy(GameObject target)
        {
            if (target == owner) return false;
            
            CharacterStats stats = target.GetComponent<CharacterStats>();
            if (stats == null) return false;
            
            if (stats.currentHP <= 0) return false;
            
            return target.CompareTag("Enemy") || target.CompareTag("Monster");
        }
        
        /// <summary>
        /// Hiển thị AoE indicator khi đang chọn target / Show AoE indicator when targeting
        /// </summary>
        public virtual void ShowAoEIndicator(Vector3 position)
        {
            if (!showAoEIndicator || aoeIndicatorPrefab == null) return;
            
            if (currentIndicator == null)
            {
                currentIndicator = Instantiate(aoeIndicatorPrefab);
            }
            
            currentIndicator.transform.position = position;
            
            // Scale indicator theo AoE radius
            float scale = skillData.aoeRadius * 2f;
            currentIndicator.transform.localScale = new Vector3(scale, 0.1f, scale);
            
            currentIndicator.SetActive(true);
        }
        
        /// <summary>
        /// Ẩn AoE indicator / Hide AoE indicator
        /// </summary>
        public virtual void HideAoEIndicator()
        {
            if (currentIndicator != null)
            {
                currentIndicator.SetActive(false);
            }
        }
        
        /// <summary>
        /// Cleanup khi destroy / Cleanup on destroy
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (currentIndicator != null)
            {
                Destroy(currentIndicator);
            }
        }
    }
}
