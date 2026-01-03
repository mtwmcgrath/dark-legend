using UnityEngine;
using System.Collections.Generic;

namespace DarkLegend.Skills
{
    /// <summary>
    /// Data cho combo sequences
    /// Data for combo sequences
    /// </summary>
    [CreateAssetMenu(fileName = "New Combo", menuName = "Dark Legend/Skills/Combo Data")]
    public class ComboData : ScriptableObject
    {
        [Header("Combo Info")]
        public string comboName;
        public string comboNameVN;
        [TextArea(2, 4)]
        public string description;
        public Sprite comboIcon;
        
        [Header("Sequence")]
        [Tooltip("Danh sách tên skills theo thứ tự / List of skill names in order")]
        public List<string> skillSequence = new List<string>();
        
        [Tooltip("Có cần chính xác thứ tự không / Requires exact order")]
        public bool requiresExactOrder = true;
        
        [Tooltip("Có cho phép skills khác chen giữa không / Allows other skills in between")]
        public bool allowsIntermediateSkills = false;
        
        [Header("Bonuses")]
        public float damageMultiplier = 2f;      // Damage multiplier cho combo
        public float damageBonus = 100f;         // Flat damage bonus
        public float critRateBonus = 0.3f;       // Tăng crit rate
        
        [Header("Finisher")]
        [Tooltip("Skill đặc biệt khi hoàn thành combo / Special skill when combo completes")]
        public SkillData finisherSkill;
        public bool autoExecuteFinisher = true;
        
        [Header("Visual & Audio")]
        public GameObject comboEffect;
        public AudioClip comboSound;
        
        /// <summary>
        /// Kiểm tra sequence có match combo không / Check if sequence matches combo
        /// </summary>
        public bool IsSequenceMatch(List<string> currentSequence)
        {
            if (currentSequence.Count < skillSequence.Count)
            {
                return false;
            }
            
            if (requiresExactOrder && !allowsIntermediateSkills)
            {
                // Exact match với số lượng và thứ tự
                if (currentSequence.Count != skillSequence.Count)
                {
                    return false;
                }
                
                for (int i = 0; i < skillSequence.Count; i++)
                {
                    if (currentSequence[i] != skillSequence[i])
                    {
                        return false;
                    }
                }
                
                return true;
            }
            else if (requiresExactOrder && allowsIntermediateSkills)
            {
                // Đúng thứ tự nhưng cho phép skills khác chen giữa
                int comboIndex = 0;
                
                for (int i = 0; i < currentSequence.Count && comboIndex < skillSequence.Count; i++)
                {
                    if (currentSequence[i] == skillSequence[comboIndex])
                    {
                        comboIndex++;
                    }
                }
                
                return comboIndex == skillSequence.Count;
            }
            else
            {
                // Không cần thứ tự, chỉ cần có đủ skills
                List<string> requiredSkills = new List<string>(skillSequence);
                
                foreach (string skill in currentSequence)
                {
                    if (requiredSkills.Contains(skill))
                    {
                        requiredSkills.Remove(skill);
                    }
                }
                
                return requiredSkills.Count == 0;
            }
        }
        
        /// <summary>
        /// Lấy progress của combo (0-1) / Get combo progress (0-1)
        /// </summary>
        public float GetProgress(List<string> currentSequence)
        {
            if (skillSequence.Count == 0) return 0f;
            
            int matchedCount = 0;
            
            if (requiresExactOrder)
            {
                for (int i = 0; i < Mathf.Min(currentSequence.Count, skillSequence.Count); i++)
                {
                    if (currentSequence[i] == skillSequence[i])
                    {
                        matchedCount++;
                    }
                    else if (!allowsIntermediateSkills)
                    {
                        break; // Sequence broken
                    }
                }
            }
            else
            {
                List<string> remaining = new List<string>(skillSequence);
                
                foreach (string skill in currentSequence)
                {
                    if (remaining.Contains(skill))
                    {
                        remaining.Remove(skill);
                        matchedCount++;
                    }
                }
            }
            
            return (float)matchedCount / skillSequence.Count;
        }
        
        /// <summary>
        /// Lấy skill tiếp theo cần cho combo / Get next required skill for combo
        /// </summary>
        public string GetNextRequiredSkill(List<string> currentSequence)
        {
            if (!requiresExactOrder) return "Any";
            
            int currentIndex = 0;
            
            if (allowsIntermediateSkills)
            {
                // Find last matched skill
                for (int i = 0; i < currentSequence.Count && currentIndex < skillSequence.Count; i++)
                {
                    if (currentSequence[i] == skillSequence[currentIndex])
                    {
                        currentIndex++;
                    }
                }
            }
            else
            {
                currentIndex = currentSequence.Count;
            }
            
            if (currentIndex < skillSequence.Count)
            {
                return skillSequence[currentIndex];
            }
            
            return "";
        }
    }
}
