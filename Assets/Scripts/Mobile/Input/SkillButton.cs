using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DarkLegend.Mobile.Input
{
    /// <summary>
    /// Skill button với cooldown
    /// Skill button with cooldown
    /// </summary>
    public class SkillButton : TouchButton, IDragHandler
    {
        [Header("Skill Settings")]
        public int skillSlotIndex = 0;
        public float skillCooldown = 5f;
        public int mpCost = 10;

        [Header("UI Components")]
        public Image cooldownOverlay;
        public Text cooldownText;
        public Image skillIcon;
        public Text mpCostText;

        [Header("Drag to Aim")]
        public bool dragToAim = false;
        public float aimRadius = 200f;
        public LineRenderer aimLine;

        private float cooldownTimer = 0f;
        private bool isOnCooldown = false;
        private Vector2 dragStartPos;
        private Vector2 aimDirection;

        protected override void Awake()
        {
            base.Awake();
            
            if (mpCostText != null)
            {
                mpCostText.text = mpCost.ToString();
            }
        }

        protected override void Update()
        {
            base.Update();

            // Update cooldown
            if (isOnCooldown)
            {
                cooldownTimer -= Time.deltaTime;

                if (cooldownTimer <= 0f)
                {
                    isOnCooldown = false;
                    cooldownTimer = 0f;
                    SetEnabled(true);
                    UpdateCooldownUI();
                }
                else
                {
                    UpdateCooldownUI();
                }
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (isOnCooldown)
                return;

            base.OnPointerDown(eventData);

            if (dragToAim)
            {
                dragStartPos = eventData.position;
                if (aimLine != null)
                {
                    aimLine.enabled = true;
                }
            }
            else
            {
                // Cast skill immediately
                CastSkill(Vector2.zero);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!dragToAim || isOnCooldown)
                return;

            // Calculate aim direction
            Vector2 dragDelta = eventData.position - dragStartPos;
            
            if (dragDelta.magnitude > 10f)
            {
                aimDirection = dragDelta.normalized;
                UpdateAimLine(aimDirection);
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            if (dragToAim && !isOnCooldown)
            {
                // Cast skill with aim direction
                CastSkill(aimDirection);

                if (aimLine != null)
                {
                    aimLine.enabled = false;
                }
            }

            aimDirection = Vector2.zero;
        }

        /// <summary>
        /// Cast skill
        /// Phóng skill
        /// </summary>
        private void CastSkill(Vector2 direction)
        {
            // TODO: Check if player has enough MP
            // if (PlayerStats.CurrentMP < mpCost) return;

            Debug.Log($"[SkillButton] Skill {skillSlotIndex} cast! Direction: {direction}");

            // Start cooldown
            StartCooldown();

            // Trigger haptic feedback
            if (useHaptic)
            {
                TriggerHaptic();
            }

            // TODO: Call player skill system
            // PlayerSkillSystem.CastSkill(skillSlotIndex, direction);
        }

        /// <summary>
        /// Start cooldown
        /// Bắt đầu cooldown
        /// </summary>
        private void StartCooldown()
        {
            isOnCooldown = true;
            cooldownTimer = skillCooldown;
            SetEnabled(false);
            UpdateCooldownUI();
        }

        /// <summary>
        /// Update cooldown UI
        /// Cập nhật UI cooldown
        /// </summary>
        private void UpdateCooldownUI()
        {
            if (cooldownOverlay != null)
            {
                cooldownOverlay.fillAmount = cooldownTimer / skillCooldown;
            }

            if (cooldownText != null)
            {
                if (isOnCooldown)
                {
                    cooldownText.text = Mathf.Ceil(cooldownTimer).ToString();
                    cooldownText.gameObject.SetActive(true);
                }
                else
                {
                    cooldownText.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Update aim line
        /// Cập nhật line ngắm
        /// </summary>
        private void UpdateAimLine(Vector2 direction)
        {
            if (aimLine == null)
                return;

            Vector3 startPos = transform.position;
            Vector3 endPos = startPos + new Vector3(direction.x, 0, direction.y) * aimRadius;

            aimLine.SetPosition(0, startPos);
            aimLine.SetPosition(1, endPos);
        }

        /// <summary>
        /// Set skill icon
        /// Đặt icon skill
        /// </summary>
        public void SetSkillIcon(Sprite icon)
        {
            if (skillIcon != null)
            {
                skillIcon.sprite = icon;
            }
        }

        /// <summary>
        /// Set skill cooldown
        /// Đặt cooldown skill
        /// </summary>
        public void SetSkillCooldown(float cooldown)
        {
            skillCooldown = cooldown;
        }

        /// <summary>
        /// Reset cooldown
        /// Reset cooldown
        /// </summary>
        public void ResetCooldown()
        {
            isOnCooldown = false;
            cooldownTimer = 0f;
            SetEnabled(true);
            UpdateCooldownUI();
        }
    }
}
