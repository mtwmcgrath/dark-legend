using UnityEngine;
using System.Collections.Generic;
using System;

namespace DarkLegend.Combat
{
    /// <summary>
    /// Manages character skills and skill casting
    /// Quản lý các kỹ năng và việc cast skill
    /// </summary>
    public class SkillManager : MonoBehaviour
    {
        [Header("Skills")]
        public List<SkillData> availableSkills = new List<SkillData>();
        public List<Skill> equippedSkills = new List<Skill>();
        public int maxEquippedSkills = 6; // Hotkeys 1-6
        
        [Header("References")]
        public Character.CharacterStats characterStats;
        public Animator animator;
        public Transform castPoint;
        
        // Events
        public event Action<Skill> OnSkillCast;
        public event Action<int> OnSkillCooldownChanged; // skill index
        
        private Skill currentlyCastingSkill;
        private float castTimer = 0f;
        private GameObject currentTarget;
        
        private void Start()
        {
            if (characterStats == null)
            {
                characterStats = GetComponent<Character.CharacterStats>();
            }
            
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }
            
            if (castPoint == null)
            {
                castPoint = transform;
            }
            
            InitializeSkills();
        }
        
        private void Update()
        {
            UpdateSkillCooldowns();
            UpdateCasting();
            HandleSkillInput();
        }
        
        /// <summary>
        /// Initialize skills from skill data
        /// Khởi tạo skills từ dữ liệu skill
        /// </summary>
        private void InitializeSkills()
        {
            equippedSkills.Clear();
            
            foreach (var skillData in availableSkills)
            {
                if (equippedSkills.Count >= maxEquippedSkills)
                    break;
                
                if (skillData != null)
                {
                    equippedSkills.Add(skillData.CreateSkillInstance());
                }
            }
        }
        
        /// <summary>
        /// Update all skill cooldowns
        /// Cập nhật tất cả cooldown của skills
        /// </summary>
        private void UpdateSkillCooldowns()
        {
            for (int i = 0; i < equippedSkills.Count; i++)
            {
                if (equippedSkills[i].IsOnCooldown)
                {
                    equippedSkills[i].UpdateCooldown(Time.deltaTime);
                    OnSkillCooldownChanged?.Invoke(i);
                }
            }
        }
        
        /// <summary>
        /// Update casting state
        /// Cập nhật trạng thái casting
        /// </summary>
        private void UpdateCasting()
        {
            if (currentlyCastingSkill != null)
            {
                castTimer -= Time.deltaTime;
                
                if (castTimer <= 0f)
                {
                    ExecuteSkill(currentlyCastingSkill);
                    currentlyCastingSkill = null;
                }
            }
        }
        
        /// <summary>
        /// Handle keyboard input for skills (1-6)
        /// Xử lý input bàn phím cho skills (1-6)
        /// </summary>
        private void HandleSkillInput()
        {
            if (characterStats != null && characterStats.IsDead)
                return;
            
            for (int i = 0; i < maxEquippedSkills; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    TryCastSkill(i);
                }
            }
        }
        
        /// <summary>
        /// Try to cast a skill by index
        /// Thử cast một skill theo index
        /// </summary>
        public bool TryCastSkill(int skillIndex, GameObject target = null)
        {
            if (skillIndex < 0 || skillIndex >= equippedSkills.Count)
                return false;
            
            Skill skill = equippedSkills[skillIndex];
            
            if (skill == null)
                return false;
            
            // Check if can cast
            if (characterStats == null || !skill.CanCast(characterStats.currentMP))
                return false;
            
            if (currentlyCastingSkill != null)
                return false;
            
            // Consume mana
            if (!characterStats.ConsumeMP(skill.manaCost))
                return false;
            
            // Set target
            currentTarget = target;
            
            // Start casting
            if (skill.castTime > 0f)
            {
                currentlyCastingSkill = skill;
                castTimer = skill.castTime;
            }
            else
            {
                ExecuteSkill(skill);
            }
            
            // Trigger animation
            if (animator != null)
            {
                animator.SetTrigger(Utils.Constants.ANIM_SKILL_TRIGGER);
            }
            
            return true;
        }
        
        /// <summary>
        /// Execute skill effect
        /// Thực thi hiệu ứng skill
        /// </summary>
        private void ExecuteSkill(Skill skill)
        {
            if (skill == null) return;
            
            // Play sound
            if (skill.castSound != null)
            {
                AudioSource.PlayClipAtPoint(skill.castSound, transform.position);
            }
            
            // Spawn effect
            if (skill.effectPrefab != null)
            {
                Vector3 spawnPos = castPoint != null ? castPoint.position : transform.position;
                Quaternion spawnRot = castPoint != null ? castPoint.rotation : transform.rotation;
                
                GameObject effect = Instantiate(skill.effectPrefab, spawnPos, spawnRot);
                Destroy(effect, 5f); // Clean up after 5 seconds
            }
            
            // Apply skill effect based on type
            switch (skill.skillType)
            {
                case SkillType.Physical:
                case SkillType.Magic:
                    ApplyDamageSkill(skill);
                    break;
                case SkillType.Heal:
                    ApplyHealSkill(skill);
                    break;
                case SkillType.Buff:
                case SkillType.Debuff:
                    // TODO: Implement buff/debuff system
                    break;
            }
            
            // Start cooldown
            skill.StartCooldown();
            OnSkillCast?.Invoke(skill);
        }
        
        /// <summary>
        /// Apply damage skill to targets
        /// Áp dụng skill sát thương lên mục tiêu
        /// </summary>
        private void ApplyDamageSkill(Skill skill)
        {
            List<GameObject> targets = FindTargets(skill);
            
            foreach (GameObject target in targets)
            {
                Character.CharacterStats targetStats = target.GetComponent<Character.CharacterStats>();
                if (targetStats != null && !targetStats.IsDead)
                {
                    bool isCritical = DamageCalculator.RollCritical(characterStats.criticalChance);
                    
                    float baseDamage = skill.skillType == SkillType.Magic ? 
                                      characterStats.magicDamage : 
                                      characterStats.physicalDamage;
                    
                    int damage = DamageCalculator.CalculateSkillDamage(
                        baseDamage,
                        skill.damageMultiplier,
                        targetStats.defense,
                        skill.skillType == SkillType.Magic,
                        isCritical
                    );
                    
                    targetStats.TakeDamage(damage);
                }
            }
        }
        
        /// <summary>
        /// Apply healing skill
        /// Áp dụng skill hồi máu
        /// </summary>
        private void ApplyHealSkill(Skill skill)
        {
            if (characterStats != null)
            {
                int healAmount = Mathf.RoundToInt(characterStats.maxHP * skill.damageMultiplier);
                characterStats.Heal(healAmount);
            }
        }
        
        /// <summary>
        /// Find targets for skill
        /// Tìm mục tiêu cho skill
        /// </summary>
        private List<GameObject> FindTargets(Skill skill)
        {
            List<GameObject> targets = new List<GameObject>();
            
            switch (skill.targetType)
            {
                case SkillTargetType.Self:
                    targets.Add(gameObject);
                    break;
                    
                case SkillTargetType.SingleEnemy:
                    if (currentTarget != null)
                    {
                        targets.Add(currentTarget);
                    }
                    break;
                    
                case SkillTargetType.AOE:
                case SkillTargetType.AllEnemies:
                    Collider[] hitColliders = Physics.OverlapSphere(transform.position, skill.range);
                    foreach (var hitCollider in hitColliders)
                    {
                        if (hitCollider.CompareTag(Utils.Constants.TAG_ENEMY))
                        {
                            targets.Add(hitCollider.gameObject);
                            if (targets.Count >= skill.maxTargets)
                                break;
                        }
                    }
                    break;
            }
            
            return targets;
        }
        
        /// <summary>
        /// Learn a new skill
        /// Học một skill mới
        /// </summary>
        public bool LearnSkill(SkillData skillData)
        {
            if (skillData == null || availableSkills.Contains(skillData))
                return false;
            
            availableSkills.Add(skillData);
            
            // Auto-equip if there's space
            if (equippedSkills.Count < maxEquippedSkills)
            {
                equippedSkills.Add(skillData.CreateSkillInstance());
            }
            
            return true;
        }
        
        /// <summary>
        /// Equip skill to hotbar slot
        /// Trang bị skill vào ô hotbar
        /// </summary>
        public bool EquipSkill(SkillData skillData, int slotIndex)
        {
            if (skillData == null || !availableSkills.Contains(skillData))
                return false;
            
            if (slotIndex < 0 || slotIndex >= maxEquippedSkills)
                return false;
            
            while (equippedSkills.Count <= slotIndex)
            {
                equippedSkills.Add(null);
            }
            
            equippedSkills[slotIndex] = skillData.CreateSkillInstance();
            return true;
        }
    }
}
