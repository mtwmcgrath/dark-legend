using UnityEngine;

namespace DarkLegend.Mobile.Input
{
    /// <summary>
    /// Attack button
    /// Nút tấn công
    /// </summary>
    public class AttackButton : TouchButton
    {
        [Header("Attack Settings")]
        public float attackCooldown = 0.5f;
        public bool autoAttack = false;

        private float lastAttackTime = 0f;
        private bool canAttack = true;

        protected override void Awake()
        {
            base.Awake();
            buttonName = "Attack";
            pcEquivalent = KeyCode.Space;
        }

        protected override void Update()
        {
            base.Update();

            // Update cooldown
            if (!canAttack)
            {
                float timeSinceAttack = Time.time - lastAttackTime;
                if (timeSinceAttack >= attackCooldown)
                {
                    canAttack = true;
                    SetEnabled(true);
                }
            }

            // Auto attack
            if (autoAttack && canAttack && isHolding)
            {
                PerformAttack();
            }
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            if (canAttack)
            {
                PerformAttack();
            }
        }

        /// <summary>
        /// Perform attack
        /// Thực hiện tấn công
        /// </summary>
        private void PerformAttack()
        {
            canAttack = false;
            lastAttackTime = Time.time;
            SetEnabled(false);

            Debug.Log("[AttackButton] Attack performed!");
            
            // Trigger haptic feedback
            if (useHaptic)
            {
                TriggerHaptic();
            }
        }

        /// <summary>
        /// Set auto attack
        /// Đặt tự động tấn công
        /// </summary>
        public void SetAutoAttack(bool enabled)
        {
            autoAttack = enabled;
        }
    }
}
