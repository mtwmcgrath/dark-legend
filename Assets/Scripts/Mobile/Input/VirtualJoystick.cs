using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace DarkLegend.Mobile.Input
{
    /// <summary>
    /// Virtual joystick cho di chuyển
    /// Virtual joystick for movement
    /// </summary>
    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [Header("Joystick Settings")]
        public float joystickRadius = 100f;
        public float deadZone = 0.1f;
        public bool isDynamic = true;
        public float handleRange = 1f;

        [Header("Visual")]
        public Image backgroundImage;
        public Image handleImage;
        public CanvasGroup canvasGroup;

        [Header("Output")]
        public Vector2 InputDirection { get; private set; }
        public float InputMagnitude { get; private set; }

        // Events
        public event Action<Vector2> OnJoystickMove;
        public event Action OnJoystickRelease;

        private Vector2 joystickPosition;
        private bool isActive = false;
        private RectTransform backgroundRect;
        private RectTransform handleRect;
        private Canvas canvas;

        private void Awake()
        {
            backgroundRect = backgroundImage.GetComponent<RectTransform>();
            handleRect = handleImage.GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();

            if (isDynamic)
            {
                // Hide joystick initially
                SetVisibility(false);
            }
        }

        private void Update()
        {
            // Apply dead zone
            if (InputMagnitude < deadZone)
            {
                InputDirection = Vector2.zero;
                InputMagnitude = 0f;
            }
        }

        /// <summary>
        /// Handle pointer down
        /// Xử lý bắt đầu nhấn
        /// </summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            if (isDynamic)
            {
                // Position joystick at touch position
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    transform.parent as RectTransform,
                    eventData.position,
                    canvas.worldCamera,
                    out joystickPosition
                );

                backgroundRect.anchoredPosition = joystickPosition;
                SetVisibility(true);
            }

            isActive = true;
            OnDrag(eventData);
        }

        /// <summary>
        /// Handle drag
        /// Xử lý kéo
        /// </summary>
        public void OnDrag(PointerEventData eventData)
        {
            if (!isActive)
                return;

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                backgroundRect,
                eventData.position,
                canvas.worldCamera,
                out localPoint
            );

            // Calculate direction and magnitude
            Vector2 direction = localPoint;
            float magnitude = direction.magnitude;
            float radius = joystickRadius * handleRange;

            // Clamp to joystick radius
            if (magnitude > radius)
            {
                direction = direction.normalized * radius;
            }

            // Update handle position
            handleRect.anchoredPosition = direction;

            // Calculate output values
            InputDirection = direction.normalized;
            InputMagnitude = Mathf.Clamp01(magnitude / radius);

            // Trigger event
            OnJoystickMove?.Invoke(InputDirection * InputMagnitude);
        }

        /// <summary>
        /// Handle pointer up
        /// Xử lý thả tay
        /// </summary>
        public void OnPointerUp(PointerEventData eventData)
        {
            isActive = false;

            // Reset handle position
            handleRect.anchoredPosition = Vector2.zero;

            // Reset output values
            InputDirection = Vector2.zero;
            InputMagnitude = 0f;

            if (isDynamic)
            {
                SetVisibility(false);
            }

            // Trigger event
            OnJoystickRelease?.Invoke();
        }

        /// <summary>
        /// Set joystick visibility
        /// Đặt hiển thị joystick
        /// </summary>
        private void SetVisibility(bool visible)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = visible ? 1f : 0f;
            }
            else
            {
                backgroundImage.enabled = visible;
                handleImage.enabled = visible;
            }
        }

        /// <summary>
        /// Get input direction
        /// Lấy hướng input
        /// </summary>
        public Vector2 GetInputDirection()
        {
            return InputDirection;
        }

        /// <summary>
        /// Get input magnitude
        /// Lấy độ mạnh input
        /// </summary>
        public float GetInputMagnitude()
        {
            return InputMagnitude;
        }

        /// <summary>
        /// Is joystick active
        /// Joystick có đang hoạt động không
        /// </summary>
        public bool IsActive()
        {
            return isActive;
        }
    }
}
