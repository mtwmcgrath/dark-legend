using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace DarkLegend.Skills
{
    /// <summary>
    /// UI thanh skill (1-9, 0, -, =, F1-F12)
    /// UI for skill bar (1-9, 0, -, =, F1-F12)
    /// </summary>
    public class SkillBarUI : MonoBehaviour
    {
        [Header("References")]
        public SkillManager skillManager;
        public GameObject character;
        
        [Header("Main Bar (1-9, 0, -, =)")]
        public Transform mainBarContainer;
        public List<SkillSlotUI> mainSlots = new List<SkillSlotUI>();
        
        [Header("Secondary Bar (F1-F12)")]
        public Transform secondaryBarContainer;
        public List<SkillSlotUI> secondarySlots = new List<SkillSlotUI>();
        
        [Header("Settings")]
        public GameObject slotPrefab;
        public bool showSecondaryBar = false;
        
        [Header("Key Bindings")]
        public KeyCode[] mainBarKeys = new KeyCode[] {
            KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5,
            KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0,
            KeyCode.Minus, KeyCode.Equals
        };
        
        public KeyCode[] secondaryBarKeys = new KeyCode[] {
            KeyCode.F1, KeyCode.F2, KeyCode.F3, KeyCode.F4, KeyCode.F5, KeyCode.F6,
            KeyCode.F7, KeyCode.F8, KeyCode.F9, KeyCode.F10, KeyCode.F11, KeyCode.F12
        };
        
        public KeyCode toggleBarKey = KeyCode.Tab;
        
        private SkillSlotUI draggedSlot = null;
        
        /// <summary>
        /// Initialize / Khởi tạo
        /// </summary>
        private void Start()
        {
            if (character == null)
            {
                character = GameObject.FindGameObjectWithTag("Player");
            }
            
            if (skillManager == null && character != null)
            {
                skillManager = character.GetComponent<SkillManager>();
            }
            
            InitializeSlots();
            UpdateAllSlots();
        }
        
        /// <summary>
        /// Initialize slots / Khởi tạo slots
        /// </summary>
        private void InitializeSlots()
        {
            // Main bar slots
            if (mainBarContainer != null && slotPrefab != null)
            {
                for (int i = 0; i < mainBarKeys.Length; i++)
                {
                    CreateSlot(mainBarContainer, mainSlots, i, mainBarKeys[i]);
                }
            }
            
            // Secondary bar slots
            if (secondaryBarContainer != null && slotPrefab != null)
            {
                for (int i = 0; i < secondaryBarKeys.Length; i++)
                {
                    CreateSlot(secondaryBarContainer, secondarySlots, i, secondaryBarKeys[i]);
                }
                
                secondaryBarContainer.gameObject.SetActive(showSecondaryBar);
            }
        }
        
        /// <summary>
        /// Tạo một slot / Create a slot
        /// </summary>
        private void CreateSlot(Transform container, List<SkillSlotUI> slotList, int index, KeyCode key)
        {
            GameObject slotObj = Instantiate(slotPrefab, container);
            SkillSlotUI slot = slotObj.GetComponent<SkillSlotUI>();
            
            if (slot != null)
            {
                slot.Initialize(this, index, key);
                slotList.Add(slot);
            }
        }
        
        /// <summary>
        /// Update / Cập nhật mỗi frame
        /// </summary>
        private void Update()
        {
            if (skillManager == null) return;
            
            HandleInput();
            UpdateAllSlots();
        }
        
        /// <summary>
        /// Xử lý input / Handle input
        /// </summary>
        private void HandleInput()
        {
            // Toggle secondary bar
            if (Input.GetKeyDown(toggleBarKey))
            {
                ToggleSecondaryBar();
            }
            
            // Main bar keys
            for (int i = 0; i < mainBarKeys.Length && i < mainSlots.Count; i++)
            {
                if (Input.GetKeyDown(mainBarKeys[i]))
                {
                    UseSkillFromSlot(skillManager.mainSkillBar[i]);
                }
            }
            
            // Secondary bar keys
            if (showSecondaryBar)
            {
                for (int i = 0; i < secondaryBarKeys.Length && i < secondarySlots.Count; i++)
                {
                    if (Input.GetKeyDown(secondaryBarKeys[i]))
                    {
                        UseSkillFromSlot(skillManager.secondarySkillBar[i]);
                    }
                }
            }
        }
        
        /// <summary>
        /// Sử dụng skill từ slot / Use skill from slot
        /// </summary>
        private void UseSkillFromSlot(SkillBase skill)
        {
            if (skill == null) return;
            
            // TODO: Get target position (mouse position, enemy, etc.)
            Vector3 targetPosition = character.transform.position + character.transform.forward * 5f;
            
            skill.Use(targetPosition);
        }
        
        /// <summary>
        /// Toggle secondary bar / Chuyển đổi secondary bar
        /// </summary>
        private void ToggleSecondaryBar()
        {
            showSecondaryBar = !showSecondaryBar;
            
            if (secondaryBarContainer != null)
            {
                secondaryBarContainer.gameObject.SetActive(showSecondaryBar);
            }
        }
        
        /// <summary>
        /// Update tất cả slots / Update all slots
        /// </summary>
        private void UpdateAllSlots()
        {
            // Update main bar
            for (int i = 0; i < mainSlots.Count && i < skillManager.mainSkillBar.Length; i++)
            {
                mainSlots[i].SetSkill(skillManager.mainSkillBar[i]);
            }
            
            // Update secondary bar
            for (int i = 0; i < secondarySlots.Count && i < skillManager.secondarySkillBar.Length; i++)
            {
                secondarySlots[i].SetSkill(skillManager.secondarySkillBar[i]);
            }
        }
        
        /// <summary>
        /// Assign skill vào slot / Assign skill to slot
        /// </summary>
        public void AssignSkillToSlot(SkillBase skill, int slotIndex, bool isSecondaryBar)
        {
            if (skill == null) return;
            
            if (isSecondaryBar)
            {
                if (slotIndex >= 0 && slotIndex < skillManager.secondarySkillBar.Length)
                {
                    skillManager.secondarySkillBar[slotIndex] = skill;
                }
            }
            else
            {
                if (slotIndex >= 0 && slotIndex < skillManager.mainSkillBar.Length)
                {
                    skillManager.mainSkillBar[slotIndex] = skill;
                }
            }
            
            UpdateAllSlots();
        }
        
        /// <summary>
        /// Bắt đầu drag slot / Start dragging slot
        /// </summary>
        public void StartDragSlot(SkillSlotUI slot)
        {
            draggedSlot = slot;
        }
        
        /// <summary>
        /// Kết thúc drag slot / End dragging slot
        /// </summary>
        public void EndDragSlot(SkillSlotUI targetSlot)
        {
            if (draggedSlot == null || targetSlot == null) return;
            
            // Swap skills
            SkillBase draggedSkill = draggedSlot.currentSkill;
            SkillBase targetSkill = targetSlot.currentSkill;
            
            // TODO: Implement slot swapping logic
            
            draggedSlot = null;
        }
    }
}
