using UnityEngine;

namespace DarkLegend.Mobile.Input
{
    /// <summary>
    /// Dodge/Dash button
    /// Nút né/lướt
    /// </summary>
    public class DodgeButton : TouchButton
    {
        [Header("Dodge Settings")]
        public float dodgeCooldown = 3f;
        public int staminaCost = 20;

        private float cooldownTimer = 0f;
        private bool isOnCooldown = false;

        protected override void Awake()
        {
            base.Awake();
            buttonName = "Dodge";
            pcEquivalent = KeyCode.LeftShift;
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
                }
            }
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (isOnCooldown)
                return;

            base.OnPointerDown(eventData);

            PerformDodge();
        }

        /// <summary>
        /// Perform dodge
        /// Thực hiện né
        /// </summary>
        private void PerformDodge()
        {
            // TODO: Check if player has enough stamina
            // if (PlayerStats.CurrentStamina < staminaCost) return;

            Debug.Log("[DodgeButton] Dodge performed!");

            // Start cooldown
            isOnCooldown = true;
            cooldownTimer = dodgeCooldown;
            SetEnabled(false);

            // Trigger haptic feedback
            if (useHaptic)
            {
                TriggerHaptic();
            }

            // TODO: Call player dodge system
            // PlayerMovement.PerformDodge();
        }

        /// <summary>
        /// Get cooldown progress (0-1)
        /// Lấy tiến trình cooldown (0-1)
        /// </summary>
        public float GetCooldownProgress()
        {
            return isOnCooldown ? (1f - cooldownTimer / dodgeCooldown) : 1f;
        }
    }
}
