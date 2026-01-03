using UnityEngine;
using UnityEngine.UI;

namespace DarkLegend.Skills
{
    /// <summary>
    /// UI tooltip hiển thị thông tin skill
    /// UI tooltip showing skill information
    /// </summary>
    public class SkillTooltipUI : MonoBehaviour
    {
        [Header("UI Elements")]
        public GameObject tooltipPanel;
        public Text skillNameText;
        public Text skillDescriptionText;
        public Text skillLevelText;
        public Text skillCostText;
        public Text skillCooldownText;
        public Text skillDamageText;
        public Text skillRangeText;
        public Image skillIcon;
        
        [Header("Requirement Display")]
        public Text requirementText;
        public Color requirementMetColor = Color.green;
        public Color requirementNotMetColor = Color.red;
        
        [Header("Settings")]
        public Vector2 offset = new Vector2(10f, 10f);
        public bool followMouse = true;
        
        private RectTransform rectTransform;
        private bool isVisible = false;
        
        /// <summary>
        /// Initialize / Khởi tạo
        /// </summary>
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            
            if (tooltipPanel != null)
            {
                tooltipPanel.SetActive(false);
            }
        }
        
        /// <summary>
        /// Update để follow mouse / Update to follow mouse
        /// </summary>
        private void Update()
        {
            if (isVisible && followMouse && rectTransform != null)
            {
                Vector2 mousePosition = Input.mousePosition;
                rectTransform.position = mousePosition + offset;
                
                // Keep tooltip on screen
                ClampToScreen();
            }
        }
        
        /// <summary>
        /// Hiển thị tooltip / Show tooltip
        /// </summary>
        public void ShowTooltip(SkillBase skill, Vector3 position)
        {
            if (skill == null || skill.skillData == null) return;
            
            // Skill name
            if (skillNameText != null)
            {
                skillNameText.text = skill.skillData.skillName;
            }
            
            // Skill icon
            if (skillIcon != null && skill.skillData.icon != null)
            {
                skillIcon.sprite = skill.skillData.icon;
                skillIcon.enabled = true;
            }
            
            // Skill level
            if (skillLevelText != null)
            {
                skillLevelText.text = $"Level: {skill.currentLevel}/{skill.skillData.maxLevel}";
            }
            
            // Description
            if (skillDescriptionText != null)
            {
                skillDescriptionText.text = skill.GetDescription();
            }
            
            // Cost
            if (skillCostText != null && skill.skillData.cost != null)
            {
                float mpCost = skill.skillData.cost.GetMPCost(skill.currentLevel);
                skillCostText.text = $"MP Cost: {mpCost}";
            }
            
            // Cooldown
            if (skillCooldownText != null && skill.skillData.cooldown != null)
            {
                float cooldown = skill.skillData.cooldown.GetCooldownTime(skill.currentLevel);
                skillCooldownText.text = $"Cooldown: {cooldown}s";
            }
            
            // Damage
            if (skillDamageText != null)
            {
                float damage = skill.GetDamage();
                if (damage > 0)
                {
                    skillDamageText.text = $"Damage: {damage}";
                    skillDamageText.gameObject.SetActive(true);
                }
                else
                {
                    skillDamageText.gameObject.SetActive(false);
                }
            }
            
            // Range
            if (skillRangeText != null)
            {
                float range = skill.skillData.castRange;
                if (range > 0)
                {
                    skillRangeText.text = $"Range: {range}m";
                    skillRangeText.gameObject.SetActive(true);
                }
                else
                {
                    skillRangeText.gameObject.SetActive(false);
                }
            }
            
            // Requirements
            ShowRequirements(skill);
            
            // Position tooltip
            if (!followMouse && rectTransform != null)
            {
                rectTransform.position = position + (Vector3)offset;
                ClampToScreen();
            }
            
            // Show panel
            if (tooltipPanel != null)
            {
                tooltipPanel.SetActive(true);
            }
            
            isVisible = true;
        }
        
        /// <summary>
        /// Hiển thị requirements / Show requirements
        /// </summary>
        private void ShowRequirements(SkillBase skill)
        {
            if (requirementText == null) return;
            if (skill.skillData.requirement == null)
            {
                requirementText.gameObject.SetActive(false);
                return;
            }
            
            GameObject owner = skill.gameObject;
            var unmetRequirements = skill.skillData.requirement.GetUnmetRequirements(owner);
            
            if (unmetRequirements.Count == 0)
            {
                requirementText.gameObject.SetActive(false);
                return;
            }
            
            // Show unmet requirements
            string reqText = "Requirements:\n";
            foreach (string req in unmetRequirements)
            {
                reqText += $"• {req}\n";
            }
            
            requirementText.text = reqText;
            requirementText.color = requirementNotMetColor;
            requirementText.gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Ẩn tooltip / Hide tooltip
        /// </summary>
        public void HideTooltip()
        {
            if (tooltipPanel != null)
            {
                tooltipPanel.SetActive(false);
            }
            
            isVisible = false;
        }
        
        /// <summary>
        /// Clamp tooltip to screen / Giữ tooltip trong màn hình
        /// </summary>
        private void ClampToScreen()
        {
            if (rectTransform == null) return;
            
            Vector3 pos = rectTransform.position;
            
            // Get screen bounds
            float minX = 0f;
            float maxX = Screen.width;
            float minY = 0f;
            float maxY = Screen.height;
            
            // Get tooltip size
            float width = rectTransform.rect.width;
            float height = rectTransform.rect.height;
            
            // Clamp position
            pos.x = Mathf.Clamp(pos.x, minX, maxX - width);
            pos.y = Mathf.Clamp(pos.y, minY + height, maxY);
            
            rectTransform.position = pos;
        }
    }
}
