using UnityEngine;
using DarkLegend.Mobile.Input;

namespace DarkLegend.Mobile.UI
{
    /// <summary>
    /// Skill wheel UI (Diablo Mobile style)
    /// UI skill dạng wheel (kiểu Diablo Mobile)
    /// </summary>
    public class SkillWheelUI : MonoBehaviour
    {
        [Header("Wheel Settings")]
        public int maxSkills = 6;
        public float wheelRadius = 150f;
        public Transform wheelCenter;

        [Header("Skill Buttons")]
        public SkillButton[] skillButtons;

        [Header("Attack Button")]
        public SkillButton attackButton;
        public bool centerAttackButton = true;

        private void Start()
        {
            InitializeWheel();
        }

        /// <summary>
        /// Initialize skill wheel
        /// Khởi tạo skill wheel
        /// </summary>
        private void InitializeWheel()
        {
            if (skillButtons == null || skillButtons.Length == 0)
            {
                skillButtons = GetComponentsInChildren<SkillButton>();
            }

            // Position attack button at center
            if (centerAttackButton && attackButton != null && wheelCenter != null)
            {
                RectTransform attackRect = attackButton.GetComponent<RectTransform>();
                if (attackRect != null)
                {
                    attackRect.anchoredPosition = Vector2.zero;
                }
            }

            // Arrange skill buttons in circle
            ArrangeSkillsInCircle();

            Debug.Log($"[SkillWheelUI] Initialized with {skillButtons.Length} skills");
        }

        /// <summary>
        /// Arrange skills in circle
        /// Sắp xếp skill theo vòng tròn
        /// </summary>
        private void ArrangeSkillsInCircle()
        {
            if (wheelCenter == null || skillButtons == null)
                return;

            // Calculate angle between each skill
            float angleStep = 360f / maxSkills;
            float startAngle = 90f; // Start from top

            for (int i = 0; i < skillButtons.Length && i < maxSkills; i++)
            {
                // Skip attack button if it's in the array
                if (skillButtons[i] == attackButton)
                    continue;

                float angle = startAngle - (angleStep * i);
                float radian = angle * Mathf.Deg2Rad;

                Vector2 position = new Vector2(
                    Mathf.Cos(radian) * wheelRadius,
                    Mathf.Sin(radian) * wheelRadius
                );

                RectTransform rectTransform = skillButtons[i].GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition = position;
                }
            }
        }

        /// <summary>
        /// Set skill in slot
        /// Đặt skill vào slot
        /// </summary>
        public void SetSkill(int slotIndex, Sprite icon, float cooldown, int mpCost)
        {
            if (slotIndex < 0 || slotIndex >= skillButtons.Length)
                return;

            SkillButton button = skillButtons[slotIndex];
            if (button != null)
            {
                button.SetSkillIcon(icon);
                button.SetSkillCooldown(cooldown);
                button.mpCost = mpCost;
            }
        }

        /// <summary>
        /// Enable/disable skill
        /// Bật/tắt skill
        /// </summary>
        public void SetSkillEnabled(int slotIndex, bool enabled)
        {
            if (slotIndex < 0 || slotIndex >= skillButtons.Length)
                return;

            skillButtons[slotIndex]?.SetEnabled(enabled);
        }

        /// <summary>
        /// Change wheel radius
        /// Đổi bán kính wheel
        /// </summary>
        public void SetWheelRadius(float radius)
        {
            wheelRadius = radius;
            ArrangeSkillsInCircle();
        }

        /// <summary>
        /// Highlight skill slot
        /// Làm nổi bật skill slot
        /// </summary>
        public void HighlightSkill(int slotIndex, bool highlight)
        {
            if (slotIndex < 0 || slotIndex >= skillButtons.Length)
                return;

            // TODO: Add highlight effect
            // skillButtons[slotIndex].SetHighlighted(highlight);
        }
    }
}
