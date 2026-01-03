using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace DarkLegend.UI
{
    /// <summary>
    /// Skill bar UI with hotkeys 1-6
    /// UI thanh skill với hotkeys 1-6
    /// </summary>
    public class SkillBarUI : MonoBehaviour
    {
        [System.Serializable]
        public class SkillSlotUI
        {
            public Button button;
            public Image iconImage;
            public Image cooldownOverlay;
            public TextMeshProUGUI cooldownText;
            public TextMeshProUGUI hotkeyText;
        }
        
        [Header("Skill Slots")]
        public List<SkillSlotUI> skillSlots = new List<SkillSlotUI>();
        
        [Header("References")]
        public Combat.SkillManager skillManager;
        
        [Header("Colors")]
        public Color availableColor = Color.white;
        public Color cooldownColor = new Color(0.5f, 0.5f, 0.5f, 0.7f);
        public Color noManaColor = new Color(1f, 0.3f, 0.3f, 0.7f);
        
        private void Start()
        {
            // Find player if not assigned
            if (skillManager == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag(Utils.Constants.TAG_PLAYER);
                if (player != null)
                {
                    skillManager = player.GetComponent<Combat.SkillManager>();
                }
            }
            
            // Subscribe to events
            if (skillManager != null)
            {
                skillManager.OnSkillCast += OnSkillCast;
            }
            
            // Setup skill slots
            SetupSkillSlots();
            
            // Initial update
            UpdateAllSlots();
        }
        
        private void Update()
        {
            UpdateCooldowns();
        }
        
        /// <summary>
        /// Setup skill slot UI
        /// Thiết lập UI ô skill
        /// </summary>
        private void SetupSkillSlots()
        {
            for (int i = 0; i < skillSlots.Count; i++)
            {
                int index = i; // Capture for lambda
                
                // Set hotkey text
                if (skillSlots[i].hotkeyText != null)
                {
                    skillSlots[i].hotkeyText.text = (i + 1).ToString();
                }
                
                // Add button click handler
                if (skillSlots[i].button != null)
                {
                    skillSlots[i].button.onClick.AddListener(() => OnSkillButtonClick(index));
                }
            }
        }
        
        /// <summary>
        /// Update all skill slots
        /// Cập nhật tất cả các ô skill
        /// </summary>
        private void UpdateAllSlots()
        {
            if (skillManager == null) return;
            
            for (int i = 0; i < skillSlots.Count; i++)
            {
                UpdateSkillSlot(i);
            }
        }
        
        /// <summary>
        /// Update specific skill slot
        /// Cập nhật ô skill cụ thể
        /// </summary>
        private void UpdateSkillSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= skillSlots.Count) return;
            if (skillManager == null) return;
            
            SkillSlotUI slot = skillSlots[slotIndex];
            
            // Check if skill exists
            if (slotIndex < skillManager.equippedSkills.Count && skillManager.equippedSkills[slotIndex] != null)
            {
                Combat.Skill skill = skillManager.equippedSkills[slotIndex];
                
                // Set icon (would need SkillData reference)
                // For now, just enable the slot
                if (slot.iconImage != null)
                {
                    slot.iconImage.enabled = true;
                }
                
                // Update button interactability
                bool canCast = skillManager.characterStats != null && 
                              skill.CanCast(skillManager.characterStats.currentMP);
                
                if (slot.button != null)
                {
                    slot.button.interactable = canCast;
                }
            }
            else
            {
                // Empty slot
                if (slot.iconImage != null)
                {
                    slot.iconImage.enabled = false;
                }
                
                if (slot.button != null)
                {
                    slot.button.interactable = false;
                }
            }
        }
        
        /// <summary>
        /// Update cooldown displays
        /// Cập nhật hiển thị cooldown
        /// </summary>
        private void UpdateCooldowns()
        {
            if (skillManager == null) return;
            
            for (int i = 0; i < skillSlots.Count; i++)
            {
                if (i >= skillManager.equippedSkills.Count) continue;
                
                Combat.Skill skill = skillManager.equippedSkills[i];
                if (skill == null) continue;
                
                SkillSlotUI slot = skillSlots[i];
                
                // Update cooldown overlay
                if (slot.cooldownOverlay != null)
                {
                    if (skill.IsOnCooldown)
                    {
                        slot.cooldownOverlay.enabled = true;
                        slot.cooldownOverlay.fillAmount = 1f - skill.GetCooldownProgress();
                    }
                    else
                    {
                        slot.cooldownOverlay.enabled = false;
                    }
                }
                
                // Update cooldown text
                if (slot.cooldownText != null)
                {
                    if (skill.IsOnCooldown)
                    {
                        slot.cooldownText.enabled = true;
                        slot.cooldownText.text = Mathf.Ceil(skill.currentCooldown).ToString();
                    }
                    else
                    {
                        slot.cooldownText.enabled = false;
                    }
                }
            }
        }
        
        /// <summary>
        /// Handle skill button click
        /// Xử lý click nút skill
        /// </summary>
        private void OnSkillButtonClick(int slotIndex)
        {
            if (skillManager != null)
            {
                skillManager.TryCastSkill(slotIndex);
            }
        }
        
        /// <summary>
        /// Handle skill cast event
        /// Xử lý sự kiện cast skill
        /// </summary>
        private void OnSkillCast(Combat.Skill skill)
        {
            // Could add visual feedback here
            Debug.Log($"Skill cast: {skill.skillName}");
        }
        
        private void OnDestroy()
        {
            if (skillManager != null)
            {
                skillManager.OnSkillCast -= OnSkillCast;
            }
        }
    }
}
