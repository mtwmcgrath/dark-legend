using UnityEngine;
using System.Collections.Generic;

namespace DarkLegend.Skills
{
    /// <summary>
    /// Skill Tree cho mỗi class
    /// Skill Tree for each class
    /// </summary>
    [CreateAssetMenu(fileName = "New Skill Tree", menuName = "Dark Legend/Skills/Skill Tree")]
    public class SkillTree : ScriptableObject
    {
        [Header("Tree Info")]
        public string treeName;
        public CharacterClassType characterClass;
        public Sprite treeIcon;
        
        [Header("Skill Nodes")]
        public List<SkillNode> nodes = new List<SkillNode>();
        
        [Header("Tree Layout")]
        public int maxTiers = 4;             // Số tầng trong tree (Tier 1-4)
        public int nodesPerTier = 5;         // Số nodes mỗi tầng
        
        /// <summary>
        /// Lấy tất cả skills trong tree / Get all skills in tree
        /// </summary>
        public List<SkillData> GetAllSkills()
        {
            List<SkillData> skills = new List<SkillData>();
            
            foreach (SkillNode node in nodes)
            {
                if (node.skillData != null)
                {
                    skills.Add(node.skillData);
                }
            }
            
            return skills;
        }
        
        /// <summary>
        /// Lấy skills theo tier / Get skills by tier
        /// </summary>
        public List<SkillNode> GetNodesByTier(int tier)
        {
            List<SkillNode> tierNodes = new List<SkillNode>();
            
            foreach (SkillNode node in nodes)
            {
                if (node.tier == tier)
                {
                    tierNodes.Add(node);
                }
            }
            
            return tierNodes;
        }
        
        /// <summary>
        /// Tìm node theo skill name / Find node by skill name
        /// </summary>
        public SkillNode FindNode(string skillName)
        {
            foreach (SkillNode node in nodes)
            {
                if (node.skillData != null && node.skillData.skillName == skillName)
                {
                    return node;
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// Kiểm tra node có thể unlock không / Check if node can be unlocked
        /// </summary>
        public bool CanUnlockNode(SkillNode node, GameObject character)
        {
            if (node == null) return false;
            
            // Check prerequisites
            foreach (SkillNode prereqNode in node.prerequisiteNodes)
            {
                if (prereqNode.skillData == null) continue;
                
                SkillManager skillManager = character.GetComponent<SkillManager>();
                if (skillManager == null) return false;
                
                SkillBase skill = skillManager.GetSkill(prereqNode.skillData.skillName);
                if (skill == null || skill.currentLevel < node.prerequisiteMinLevel)
                {
                    return false;
                }
            }
            
            // Check requirements
            if (node.skillData != null && node.skillData.requirement != null)
            {
                return node.skillData.requirement.IsMet(character);
            }
            
            return true;
        }
    }
}
