using UnityEngine;
using System.Collections.Generic;

namespace DarkLegend.Skills
{
    /// <summary>
    /// Node trong skill tree
    /// Node in skill tree
    /// </summary>
    [System.Serializable]
    public class SkillNode
    {
        [Header("Node Info")]
        public string nodeName;
        public SkillData skillData;
        
        [Header("Tree Position")]
        public int tier;                     // Tầng trong tree (1-4)
        public int positionInTier;           // Vị trí trong tầng (0-4)
        public Vector2 treePosition;         // Vị trí UI trong tree
        
        [Header("Prerequisites")]
        public List<SkillNode> prerequisiteNodes = new List<SkillNode>();
        public int prerequisiteMinLevel = 1; // Level tối thiểu của prerequisite skills
        
        [Header("Connections")]
        public List<SkillNode> connectedNodes = new List<SkillNode>();
        
        [Header("State")]
        public bool isUnlocked = false;
        public bool isLearned = false;
        
        /// <summary>
        /// Kiểm tra có thể unlock không / Check if can unlock
        /// </summary>
        public bool CanUnlock(SkillManager skillManager)
        {
            if (isUnlocked) return false;
            if (skillData == null) return false;
            
            // Check prerequisites
            foreach (SkillNode prereqNode in prerequisiteNodes)
            {
                if (prereqNode.skillData == null) continue;
                
                SkillBase skill = skillManager.GetSkill(prereqNode.skillData.skillName);
                if (skill == null || skill.currentLevel < prerequisiteMinLevel)
                {
                    return false;
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// Unlock node / Mở khóa node
        /// </summary>
        public void Unlock()
        {
            isUnlocked = true;
        }
        
        /// <summary>
        /// Learn skill từ node / Learn skill from node
        /// </summary>
        public bool Learn(SkillManager skillManager)
        {
            if (!isUnlocked) return false;
            if (isLearned) return false;
            if (skillData == null) return false;
            
            bool learned = skillManager.LearnSkill(skillData);
            if (learned)
            {
                isLearned = true;
            }
            
            return learned;
        }
    }
}
