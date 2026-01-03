using UnityEngine;
using System.Collections.Generic;

namespace DarkLegend.Skills
{
    /// <summary>
    /// Quản lý tất cả skills của player
    /// Manages all player skills
    /// </summary>
    public class SkillManager : MonoBehaviour
    {
        [Header("Skill Points")]
        public int availableSkillPoints = 0;
        public int totalSkillPoints = 0;
        
        [Header("Learned Skills")]
        public List<SkillBase> learnedSkills = new List<SkillBase>();
        
        [Header("Skill Bar")]
        public SkillBase[] mainSkillBar = new SkillBase[12];    // 0-9, -, =
        public SkillBase[] secondarySkillBar = new SkillBase[12]; // F1-F12
        
        private Dictionary<string, SkillBase> skillDictionary = new Dictionary<string, SkillBase>();
        private GameObject owner;
        
        /// <summary>
        /// Khởi tạo / Initialize
        /// </summary>
        private void Awake()
        {
            owner = gameObject;
        }
        
        /// <summary>
        /// Học skill mới / Learn a new skill
        /// </summary>
        public bool LearnSkill(SkillData skillData)
        {
            if (skillData == null) return false;
            
            // Kiểm tra đã học chưa
            if (HasSkill(skillData.skillName))
            {
                Debug.LogWarning($"Skill {skillData.skillName} already learned!");
                return false;
            }
            
            // Kiểm tra requirement
            if (skillData.requirement != null && !skillData.requirement.IsMet(owner))
            {
                Debug.LogWarning($"Requirements not met for skill {skillData.skillName}!");
                return false;
            }
            
            // Tạo skill instance
            SkillBase skillInstance = CreateSkillInstance(skillData);
            if (skillInstance == null) return false;
            
            skillInstance.Initialize(owner, this);
            learnedSkills.Add(skillInstance);
            skillDictionary[skillData.skillName] = skillInstance;
            
            Debug.Log($"Learned skill: {skillData.skillName}");
            return true;
        }
        
        /// <summary>
        /// Tạo instance của skill dựa trên type / Create skill instance based on type
        /// </summary>
        private SkillBase CreateSkillInstance(SkillData skillData)
        {
            SkillBase skill = null;
            
            switch (skillData.skillType)
            {
                case SkillType.Active:
                    // Xác định loại active skill cụ thể
                    if (skillData.aoeRadius > 0)
                        skill = gameObject.AddComponent<AoESkill>();
                    else if (skillData.projectilePrefab != null)
                        skill = gameObject.AddComponent<ProjectileSkill>();
                    else
                        skill = gameObject.AddComponent<MeleeSkill>();
                    break;
                    
                case SkillType.Passive:
                    skill = gameObject.AddComponent<PassiveSkill>();
                    break;
                    
                case SkillType.Buff:
                    skill = gameObject.AddComponent<BuffSkill>();
                    break;
                    
                case SkillType.Debuff:
                    skill = gameObject.AddComponent<DebuffSkill>();
                    break;
                    
                case SkillType.Ultimate:
                    skill = gameObject.AddComponent<UltimateSkill>();
                    break;
            }
            
            if (skill != null)
            {
                skill.skillData = skillData;
            }
            
            return skill;
        }
        
        /// <summary>
        /// Kiểm tra đã học skill chưa / Check if skill is learned
        /// </summary>
        public bool HasSkill(string skillName)
        {
            return skillDictionary.ContainsKey(skillName);
        }
        
        /// <summary>
        /// Lấy skill theo tên / Get skill by name
        /// </summary>
        public SkillBase GetSkill(string skillName)
        {
            if (skillDictionary.TryGetValue(skillName, out SkillBase skill))
            {
                return skill;
            }
            return null;
        }
        
        /// <summary>
        /// Sử dụng skill / Use a skill
        /// </summary>
        public bool UseSkill(string skillName, Vector3 targetPosition, GameObject targetObject = null)
        {
            SkillBase skill = GetSkill(skillName);
            if (skill == null) return false;
            
            return skill.Use(targetPosition, targetObject);
        }
        
        /// <summary>
        /// Gán skill vào skill bar / Assign skill to skill bar
        /// </summary>
        public bool AssignToMainBar(string skillName, int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= mainSkillBar.Length) return false;
            
            SkillBase skill = GetSkill(skillName);
            if (skill == null) return false;
            
            mainSkillBar[slotIndex] = skill;
            return true;
        }
        
        /// <summary>
        /// Gán skill vào secondary bar / Assign skill to secondary bar
        /// </summary>
        public bool AssignToSecondaryBar(string skillName, int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= secondarySkillBar.Length) return false;
            
            SkillBase skill = GetSkill(skillName);
            if (skill == null) return false;
            
            secondarySkillBar[slotIndex] = skill;
            return true;
        }
        
        /// <summary>
        /// Kiểm tra có đủ skill points không / Check if has enough skill points
        /// </summary>
        public bool HasSkillPoints(int amount)
        {
            return availableSkillPoints >= amount;
        }
        
        /// <summary>
        /// Sử dụng skill points / Use skill points
        /// </summary>
        public bool UseSkillPoints(int amount)
        {
            if (!HasSkillPoints(amount)) return false;
            
            availableSkillPoints -= amount;
            return true;
        }
        
        /// <summary>
        /// Thêm skill points / Add skill points
        /// </summary>
        public void AddSkillPoints(int amount)
        {
            availableSkillPoints += amount;
            totalSkillPoints += amount;
        }
        
        /// <summary>
        /// Level up một skill / Level up a skill
        /// </summary>
        public bool LevelUpSkill(string skillName)
        {
            SkillBase skill = GetSkill(skillName);
            if (skill == null) return false;
            
            return skill.LevelUp();
        }
        
        /// <summary>
        /// Lấy danh sách skills có thể học / Get list of learnable skills
        /// </summary>
        public List<SkillData> GetLearnableSkills(SkillData[] allSkills)
        {
            List<SkillData> learnable = new List<SkillData>();
            
            foreach (SkillData skillData in allSkills)
            {
                if (HasSkill(skillData.skillName)) continue;
                
                if (skillData.requirement == null || skillData.requirement.IsMet(owner))
                {
                    learnable.Add(skillData);
                }
            }
            
            return learnable;
        }
    }
}
