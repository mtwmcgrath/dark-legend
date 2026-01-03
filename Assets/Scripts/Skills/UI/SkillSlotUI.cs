using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DarkLegend.Skills
{
    /// <summary>
    /// UI slot skill với cooldown
    /// UI skill slot with cooldown display
    /// </summary>
    public class SkillSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, 
        IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("References")]
        public SkillBarUI skillBar;
        public int slotIndex;
        public KeyCode keyBinding;
        
        [Header("UI Elements")]
        public Image skillIcon;
        public Image cooldownOverlay;
        public Text cooldownText;
        public Text keyBindingText;
        public Text levelText;
        
        [Header("Colors")]
        public Color normalColor = Color.white;
        public Color canUseColor = Color.green;
        public Color cooldownColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);
        
        public SkillBase currentSkill;
        private SkillTooltipUI tooltip;
        
        /// <summary>
        /// Initialize slot / Khởi tạo slot
        /// </summary>
        public void Initialize(SkillBarUI skillBar, int index, KeyCode key)
        {
            this.skillBar = skillBar;
            this.slotIndex = index;
            this.keyBinding = key;
            
            // Set key binding text
            if (keyBindingText != null)
            {
                keyBindingText.text = GetKeyDisplayName(key);
            }
            
            // Find tooltip
            tooltip = FindObjectOfType<SkillTooltipUI>();
        }
        
        /// <summary>
        /// Set skill cho slot / Set skill for slot
        /// </summary>
        public void SetSkill(SkillBase skill)
        {
            currentSkill = skill;
            UpdateDisplay();
        }
        
        /// <summary>
        /// Update hiển thị / Update display
        /// </summary>
        private void Update()
        {
            if (currentSkill != null)
            {
                UpdateCooldown();
            }
        }
        
        /// <summary>
        /// Update display / Cập nhật hiển thị
        /// </summary>
        private void UpdateDisplay()
        {
            if (currentSkill == null)
            {
                // Empty slot
                if (skillIcon != null) skillIcon.enabled = false;
                if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0f;
                if (cooldownText != null) cooldownText.text = "";
                if (levelText != null) levelText.text = "";
                return;
            }
            
            // Set icon
            if (skillIcon != null && currentSkill.skillData != null)
            {
                skillIcon.enabled = true;
                skillIcon.sprite = currentSkill.skillData.icon;
                
                // Color based on state
                if (currentSkill.CanUse())
                {
                    skillIcon.color = canUseColor;
                }
                else
                {
                    skillIcon.color = normalColor;
                }
            }
            
            // Set level
            if (levelText != null)
            {
                levelText.text = currentSkill.currentLevel.ToString();
            }
        }
        
        /// <summary>
        /// Update cooldown overlay / Cập nhật overlay cooldown
        /// </summary>
        private void UpdateCooldown()
        {
            if (currentSkill == null) return;
            
            if (currentSkill.isOnCooldown)
            {
                // Show cooldown
                if (cooldownOverlay != null)
                {
                    float cooldownPercent = currentSkill.currentCooldown / 
                        currentSkill.skillData.cooldown.GetCooldownTime(currentSkill.currentLevel);
                    cooldownOverlay.fillAmount = cooldownPercent;
                    cooldownOverlay.color = cooldownColor;
                }
                
                if (cooldownText != null)
                {
                    cooldownText.text = currentSkill.currentCooldown.ToString("F1");
                }
                
                if (skillIcon != null)
                {
                    skillIcon.color = Color.gray;
                }
            }
            else
            {
                // Ready to use
                if (cooldownOverlay != null)
                {
                    cooldownOverlay.fillAmount = 0f;
                }
                
                if (cooldownText != null)
                {
                    cooldownText.text = "";
                }
                
                if (skillIcon != null)
                {
                    skillIcon.color = currentSkill.CanUse() ? canUseColor : normalColor;
                }
            }
        }
        
        /// <summary>
        /// Lấy display name cho key / Get display name for key
        /// </summary>
        private string GetKeyDisplayName(KeyCode key)
        {
            string keyName = key.ToString();
            
            if (keyName.StartsWith("Alpha"))
            {
                return keyName.Replace("Alpha", "");
            }
            else if (keyName.StartsWith("F") && keyName.Length <= 3)
            {
                return keyName;
            }
            else if (key == KeyCode.Minus)
            {
                return "-";
            }
            else if (key == KeyCode.Equals)
            {
                return "=";
            }
            
            return keyName;
        }
        
        // Event Handlers
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (currentSkill != null && tooltip != null)
            {
                tooltip.ShowTooltip(currentSkill, transform.position);
            }
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            if (tooltip != null)
            {
                tooltip.HideTooltip();
            }
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                // Use skill
                if (currentSkill != null && currentSkill.CanUse())
                {
                    // TODO: Get target
                    Vector3 target = Vector3.zero;
                    currentSkill.Use(target);
                }
            }
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (skillBar != null)
            {
                skillBar.StartDragSlot(this);
            }
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            // Visual feedback during drag
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            // Find target slot
            SkillSlotUI targetSlot = eventData.pointerCurrentRaycast.gameObject?.GetComponent<SkillSlotUI>();
            
            if (targetSlot != null && skillBar != null)
            {
                skillBar.EndDragSlot(targetSlot);
            }
        }
    }
}
