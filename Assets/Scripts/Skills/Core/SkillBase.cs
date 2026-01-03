using UnityEngine;
using System.Collections.Generic;

namespace DarkLegend.Skills
{
    /// <summary>
    /// Base class cho tất cả các loại skill trong game
    /// Base class for all skill types in the game
    /// </summary>
    public abstract class SkillBase : MonoBehaviour
    {
        [Header("Skill Configuration")]
        public SkillData skillData;
        
        [Header("Current State")]
        public int currentLevel = 1;
        public float currentCooldown = 0f;
        public bool isOnCooldown = false;
        
        protected SkillManager skillManager;
        protected GameObject owner;
        
        /// <summary>
        /// Khởi tạo skill / Initialize skill
        /// </summary>
        public virtual void Initialize(GameObject owner, SkillManager manager)
        {
            this.owner = owner;
            this.skillManager = manager;
            currentLevel = 1;
        }
        
        /// <summary>
        /// Kiểm tra có thể sử dụng skill không / Check if skill can be used
        /// </summary>
        public virtual bool CanUse()
        {
            if (isOnCooldown) return false;
            if (!CheckRequirements()) return false;
            if (!CheckCost()) return false;
            return true;
        }
        
        /// <summary>
        /// Sử dụng skill / Use the skill
        /// </summary>
        public virtual bool Use(Vector3 targetPosition, GameObject targetObject = null)
        {
            if (!CanUse()) return false;
            
            // Trừ cost
            ConsumeCost();
            
            // Bắt đầu cooldown
            StartCooldown();
            
            // Thực hiện skill effect
            ExecuteSkill(targetPosition, targetObject);
            
            return true;
        }
        
        /// <summary>
        /// Thực hiện hiệu ứng skill / Execute skill effects
        /// </summary>
        protected abstract void ExecuteSkill(Vector3 targetPosition, GameObject targetObject);
        
        /// <summary>
        /// Kiểm tra yêu cầu sử dụng skill / Check skill requirements
        /// </summary>
        protected virtual bool CheckRequirements()
        {
            if (skillData == null || skillData.requirement == null) return true;
            return skillData.requirement.IsMet(owner);
        }
        
        /// <summary>
        /// Kiểm tra đủ cost để sử dụng / Check if cost can be paid
        /// </summary>
        protected virtual bool CheckCost()
        {
            if (skillData == null || skillData.cost == null) return true;
            return skillData.cost.CanPay(owner, currentLevel);
        }
        
        /// <summary>
        /// Trừ cost khi sử dụng skill / Consume skill cost
        /// </summary>
        protected virtual void ConsumeCost()
        {
            if (skillData != null && skillData.cost != null)
            {
                skillData.cost.Pay(owner, currentLevel);
            }
        }
        
        /// <summary>
        /// Bắt đầu cooldown / Start cooldown timer
        /// </summary>
        protected virtual void StartCooldown()
        {
            if (skillData != null && skillData.cooldown != null)
            {
                currentCooldown = skillData.cooldown.GetCooldownTime(currentLevel);
                isOnCooldown = true;
            }
        }
        
        /// <summary>
        /// Update cooldown mỗi frame / Update cooldown each frame
        /// </summary>
        protected virtual void Update()
        {
            if (isOnCooldown)
            {
                currentCooldown -= Time.deltaTime;
                if (currentCooldown <= 0f)
                {
                    currentCooldown = 0f;
                    isOnCooldown = false;
                }
            }
        }
        
        /// <summary>
        /// Nâng cấp skill lên level mới / Level up the skill
        /// </summary>
        public virtual bool LevelUp()
        {
            if (currentLevel >= skillData.maxLevel) return false;
            if (!skillManager.HasSkillPoints(1)) return false;
            
            skillManager.UseSkillPoints(1);
            currentLevel++;
            return true;
        }
        
        /// <summary>
        /// Lấy damage của skill ở level hiện tại / Get skill damage at current level
        /// </summary>
        public virtual float GetDamage()
        {
            if (skillData == null) return 0f;
            return skillData.GetDamageAtLevel(currentLevel);
        }
        
        /// <summary>
        /// Lấy thông tin skill để hiển thị / Get skill info for display
        /// </summary>
        public virtual string GetDescription()
        {
            if (skillData == null) return "";
            return skillData.GetDescription(currentLevel);
        }
    }
}
