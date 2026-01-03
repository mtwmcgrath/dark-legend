using UnityEngine;

namespace DarkLegend.Skills
{
    /// <summary>
    /// Điều kiện unlock skill trong tree
    /// Unlock conditions for skills in tree
    /// </summary>
    [CreateAssetMenu(fileName = "New Skill Unlock", menuName = "Dark Legend/Skills/Skill Unlock")]
    public class SkillUnlock : ScriptableObject
    {
        [Header("Unlock Conditions")]
        public int requiredLevel = 1;
        public int requiredSkillPoints = 1;
        
        [Header("Tier Requirements")]
        public int requiredTier = 1;
        public int requiredSkillsInPreviousTier = 0;  // Số skills cần trong tier trước
        
        /// <summary>
        /// Kiểm tra có đủ điều kiện unlock không / Check if unlock conditions are met
        /// </summary>
        public bool CanUnlock(GameObject character, SkillNode node, SkillTree tree)
        {
            CharacterStats stats = character.GetComponent<CharacterStats>();
            if (stats == null) return false;
            
            // Check level
            if (stats.level < requiredLevel)
            {
                return false;
            }
            
            // Check skill points
            SkillManager skillManager = character.GetComponent<SkillManager>();
            if (skillManager == null) return false;
            
            if (!skillManager.HasSkillPoints(requiredSkillPoints))
            {
                return false;
            }
            
            // Check tier requirements
            if (node.tier > 1 && requiredSkillsInPreviousTier > 0)
            {
                int learnedInPreviousTier = CountLearnedSkillsInTier(skillManager, tree, node.tier - 1);
                if (learnedInPreviousTier < requiredSkillsInPreviousTier)
                {
                    return false;
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// Đếm số skills đã học trong tier / Count learned skills in tier
        /// </summary>
        private int CountLearnedSkillsInTier(SkillManager skillManager, SkillTree tree, int tier)
        {
            var tierNodes = tree.GetNodesByTier(tier);
            int count = 0;
            
            foreach (SkillNode node in tierNodes)
            {
                if (node.skillData != null && skillManager.HasSkill(node.skillData.skillName))
                {
                    count++;
                }
            }
            
            return count;
        }
    }
}
