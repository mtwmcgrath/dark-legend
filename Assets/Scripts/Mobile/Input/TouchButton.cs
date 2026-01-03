using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;

namespace DarkLegend.Mobile.Input
{
    /// <summary>
    /// Base class cho touch buttons
    /// Base class for touch buttons
    /// </summary>
    public class TouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Button Settings")]
        public string buttonName = "Button";
        public KeyCode pcEquivalent = KeyCode.None;
        public bool allowHold = true;
        public float holdDelay = 0.5f;

        [Header("Visual Feedback")]
        public Color normalColor = Color.white;
        public Color pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        public Color disabledColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        public float pressScale = 0.9f;

        [Header("Haptic")]
        public bool useHaptic = true;
        public HapticType hapticType = HapticType.Light;

        [Header("Components")]
        public Image buttonImage;
        public Text buttonText;

        // State
        protected bool isPressed = false;
        protected bool isDisabled = false;
        protected bool isHolding = false;

        // Events
        public event Action OnButtonDown;
        public event Action OnButtonUp;
        public event Action OnButtonHold;

        protected Vector3 originalScale;
        protected Color originalColor;

        protected virtual void Awake()
        {
            if (buttonImage == null)
            {
                buttonImage = GetComponent<Image>();
            }

            if (buttonImage != null)
            {
                originalColor = buttonImage.color;
                originalScale = transform.localScale;
            }
        }

        protected virtual void Update()
        {
            // Check for PC keyboard input
            if (pcEquivalent != KeyCode.None)
            {
                if (UnityEngine.Input.GetKeyDown(pcEquivalent))
                {
                    SimulatePress();
                }
                else if (UnityEngine.Input.GetKeyUp(pcEquivalent))
                {
                    SimulateRelease();
                }
            }
        }

        /// <summary>
        /// Handle pointer down
        /// Xử lý nhấn xuống
        /// </summary>
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (isDisabled)
                return;

            isPressed = true;
            ApplyPressedVisual();

            if (useHaptic)
            {
                TriggerHaptic();
            }

            OnButtonDown?.Invoke();

            if (allowHold)
            {
                StartCoroutine(CheckHold());
            }
        }

        /// <summary>
        /// Handle pointer up
        /// Xử lý thả tay
        /// </summary>
        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (isDisabled)
                return;

            isPressed = false;
            isHolding = false;
            ApplyNormalVisual();

            OnButtonUp?.Invoke();
        }

        /// <summary>
        /// Check for hold
        /// Kiểm tra giữ nút
        /// </summary>
        protected IEnumerator CheckHold()
        {
            float timer = 0f;

            while (isPressed && timer < holdDelay)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            if (isPressed)
            {
                isHolding = true;
                OnButtonHold?.Invoke();

                if (useHaptic)
                {
                    TriggerHaptic();
                }
            }
        }

        /// <summary>
        /// Apply pressed visual
        /// Áp dụng hiển thị khi nhấn
        /// </summary>
        protected virtual void ApplyPressedVisual()
        {
            if (buttonImage != null)
            {
                buttonImage.color = pressedColor;
            }

            transform.localScale = originalScale * pressScale;
        }

        /// <summary>
        /// Apply normal visual
        /// Áp dụng hiển thị bình thường
        /// </summary>
        protected virtual void ApplyNormalVisual()
        {
            if (buttonImage != null)
            {
                buttonImage.color = isDisabled ? disabledColor : normalColor;
            }

            transform.localScale = originalScale;
        }

        /// <summary>
        /// Simulate press (for keyboard input)
        /// Mô phỏng nhấn (cho keyboard)
        /// </summary>
        public void SimulatePress()
        {
            OnPointerDown(null);
        }

        /// <summary>
        /// Simulate release (for keyboard input)
        /// Mô phỏng thả (cho keyboard)
        /// </summary>
        public void SimulateRelease()
        {
            OnPointerUp(null);
        }

        /// <summary>
        /// Set button enabled/disabled
        /// Đặt nút bật/tắt
        /// </summary>
        public virtual void SetEnabled(bool enabled)
        {
            isDisabled = !enabled;
            ApplyNormalVisual();
        }

        /// <summary>
        /// Trigger haptic feedback
        /// Kích hoạt rung phản hồi
        /// </summary>
        protected void TriggerHaptic()
        {
            #if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
            #endif
        }

        /// <summary>
        /// Is button pressed
        /// Nút có đang được nhấn không
        /// </summary>
        public bool IsPressed()
        {
            return isPressed;
        }

        /// <summary>
        /// Is button holding
        /// Nút có đang được giữ không
        /// </summary>
        public bool IsHolding()
        {
            return isHolding;
        }
    }

    public enum HapticType
    {
        Light,      // UI feedback
        Medium,     // Skill cast
        Heavy,      // Damage received
        Success,    // Level up, rare drop
        Error       // Failed action
    }
}
