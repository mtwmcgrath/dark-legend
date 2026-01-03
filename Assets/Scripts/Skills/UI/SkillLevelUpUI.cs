using UnityEngine;
using UnityEngine.UI;

namespace DarkLegend.Skills
{
    /// <summary>
    /// UI nâng cấp skill
    /// UI for skill leveling up
    /// </summary>
    public class SkillLevelUpUI : MonoBehaviour
    {
        [Header("References")]
        public SkillManager skillManager;
        
        [Header("UI Elements")]
        public GameObject levelUpPanel;
        public Text skillNameText;
        public Text currentLevelText;
        public Text nextLevelText;
        public Text currentStatsText;
        public Text nextStatsText;
        public Text skillPointCostText;
        public Button levelUpButton;
        public Button closeButton;
        
        [Header("Visual")]
        public Image skillIcon;
        public GameObject levelUpEffect;
        
        private SkillBase selectedSkill;
        
        /// <summary>
        /// Initialize / Khởi tạo
        /// </summary>
        private void Start()
        {
            if (levelUpPanel != null)
            {
                levelUpPanel.SetActive(false);
            }
            
            if (levelUpButton != null)
            {
                levelUpButton.onClick.AddListener(OnLevelUpClicked);
            }
            
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(OnCloseClicked);
            }
        }
        
        /// <summary>
        /// Hiển thị UI nâng cấp / Show level up UI
        /// </summary>
        public void ShowLevelUpUI(SkillBase skill)
        {
            if (skill == null || skill.skillData == null) return;
            
            selectedSkill = skill;
            
            // Skill name
            if (skillNameText != null)
            {
                skillNameText.text = skill.skillData.skillName;
            }
            
            // Skill icon
            if (skillIcon != null && skill.skillData.icon != null)
            {
                skillIcon.sprite = skill.skillData.icon;
            }
            
            // Current level
            if (currentLevelText != null)
            {
                currentLevelText.text = $"Level {skill.currentLevel}";
            }
            
            // Next level
            if (nextLevelText != null)
            {
                int nextLevel = skill.currentLevel + 1;
                nextLevelText.text = $"Level {nextLevel}";
            }
            
            // Current stats
            if (currentStatsText != null)
            {
                currentStatsText.text = GetSkillStats(skill, skill.currentLevel);
            }
            
            // Next level stats
            if (nextStatsText != null)
            {
                nextStatsText.text = GetSkillStats(skill, skill.currentLevel + 1);
            }
            
            // Skill point cost
            if (skillPointCostText != null)
            {
                skillPointCostText.text = "Cost: 1 Skill Point";
            }
            
            // Level up button state
            UpdateLevelUpButton();
            
            // Show panel
            if (levelUpPanel != null)
            {
                levelUpPanel.SetActive(true);
            }
        }
        
        /// <summary>
        /// Lấy stats của skill ở level cụ thể / Get skill stats at specific level
        /// </summary>
        private string GetSkillStats(SkillBase skill, int level)
        {
            SkillData data = skill.skillData;
            string stats = "";
            
            // Damage
            if (data.baseDamage > 0)
            {
                float damage = data.GetDamageAtLevel(level);
                stats += $"Damage: {damage}\n";
            }
            
            // MP Cost
            if (data.cost != null)
            {
                float mpCost = data.cost.GetMPCost(level);
                stats += $"MP Cost: {mpCost}\n";
            }
            
            // Cooldown
            if (data.cooldown != null)
            {
                float cooldown = data.cooldown.GetCooldownTime(level);
                stats += $"Cooldown: {cooldown}s\n";
            }
            
            // Range
            if (data.castRange > 0)
            {
                stats += $"Range: {data.castRange}m\n";
            }
            
            // AoE
            if (data.aoeRadius > 0)
            {
                stats += $"AoE Radius: {data.aoeRadius}m\n";
            }
            
            // Duration
            if (data.duration > 0)
            {
                stats += $"Duration: {data.duration}s\n";
            }
            
            return stats;
        }
        
        /// <summary>
        /// Update level up button state / Cập nhật trạng thái nút level up
        /// </summary>
        private void UpdateLevelUpButton()
        {
            if (levelUpButton == null || selectedSkill == null) return;
            
            bool canLevelUp = selectedSkill.currentLevel < selectedSkill.skillData.maxLevel &&
                            skillManager != null &&
                            skillManager.HasSkillPoints(1);
            
            levelUpButton.interactable = canLevelUp;
            
            // Update button text
            Text buttonText = levelUpButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                if (selectedSkill.currentLevel >= selectedSkill.skillData.maxLevel)
                {
                    buttonText.text = "MAX LEVEL";
                }
                else if (!skillManager.HasSkillPoints(1))
                {
                    buttonText.text = "NO SKILL POINTS";
                }
                else
                {
                    buttonText.text = "LEVEL UP";
                }
            }
        }
        
        /// <summary>
        /// Xử lý khi click level up / Handle level up click
        /// </summary>
        private void OnLevelUpClicked()
        {
            if (selectedSkill == null || skillManager == null) return;
            
            bool success = selectedSkill.LevelUp();
            
            if (success)
            {
                // Spawn level up effect
                SpawnLevelUpEffect();
                
                // Refresh UI
                ShowLevelUpUI(selectedSkill);
                
                Debug.Log($"Skill leveled up: {selectedSkill.skillData.skillName} -> Level {selectedSkill.currentLevel}");
            }
        }
        
        /// <summary>
        /// Spawn level up effect / Tạo hiệu ứng level up
        /// </summary>
        private void SpawnLevelUpEffect()
        {
            if (levelUpEffect != null && selectedSkill != null)
            {
                GameObject effect = Instantiate(levelUpEffect, 
                    selectedSkill.transform.position, 
                    Quaternion.identity);
                
                Destroy(effect, 2f);
            }
        }
        
        /// <summary>
        /// Xử lý khi click close / Handle close click
        /// </summary>
        private void OnCloseClicked()
        {
            HideLevelUpUI();
        }
        
        /// <summary>
        /// Ẩn UI / Hide UI
        /// </summary>
        public void HideLevelUpUI()
        {
            if (levelUpPanel != null)
            {
                levelUpPanel.SetActive(false);
            }
            
            selectedSkill = null;
        }
    }
}
