using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace DarkLegend.Mobile.Input
{
    /// <summary>
    /// Quản lý touch input
    /// Touch input manager
    /// </summary>
    public class TouchInputManager : MonoBehaviour
    {
        #region Singleton
        private static TouchInputManager instance;
        public static TouchInputManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<TouchInputManager>();
                }
                return instance;
            }
        }
        #endregion

        [Header("Touch Settings")]
        public bool enableTouchInput = true;
        public int maxTouches = 5;
        public float touchSensitivity = 1.0f;

        [Header("Gesture Settings")]
        public float swipeThreshold = 50f;
        public float doubleTapTime = 0.3f;
        public float longPressTime = 0.5f;

        // Touch tracking
        private Touch[] currentTouches;
        private int touchCount = 0;

        // Events
        public event Action<Vector2> OnTouchBegan;
        public event Action<Vector2> OnTouchMoved;
        public event Action<Vector2> OnTouchEnded;
        public event Action<Vector2> OnDoubleTap;
        public event Action<Vector2> OnLongPress;
        public event Action<Vector2, Vector2> OnSwipe;

        // Long press tracking
        private float touchStartTime = 0f;
        private Vector2 touchStartPos = Vector2.zero;
        private bool longPressTriggered = false;

        // Double tap tracking
        private float lastTapTime = 0f;
        private Vector2 lastTapPos = Vector2.zero;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
        }

        private void Update()
        {
            if (!enableTouchInput)
                return;

            ProcessTouchInput();
        }

        /// <summary>
        /// Process touch input
        /// Xử lý touch input
        /// </summary>
        private void ProcessTouchInput()
        {
            touchCount = UnityEngine.Input.touchCount;

            // Handle mouse input in editor
            #if UNITY_EDITOR || UNITY_STANDALONE
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                HandleTouchBegan(UnityEngine.Input.mousePosition);
            }
            else if (UnityEngine.Input.GetMouseButton(0))
            {
                HandleTouchMoved(UnityEngine.Input.mousePosition);
            }
            else if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                HandleTouchEnded(UnityEngine.Input.mousePosition);
            }
            #endif

            // Handle touch input on mobile
            if (touchCount > 0)
            {
                for (int i = 0; i < touchCount && i < maxTouches; i++)
                {
                    Touch touch = UnityEngine.Input.GetTouch(i);

                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            HandleTouchBegan(touch.position);
                            break;

                        case TouchPhase.Moved:
                        case TouchPhase.Stationary:
                            HandleTouchMoved(touch.position);
                            break;

                        case TouchPhase.Ended:
                        case TouchPhase.Canceled:
                            HandleTouchEnded(touch.position);
                            break;
                    }
                }
            }

            // Check for long press
            CheckLongPress();
        }

        /// <summary>
        /// Handle touch began
        /// Xử lý bắt đầu chạm
        /// </summary>
        private void HandleTouchBegan(Vector2 position)
        {
            // Check if touching UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            touchStartPos = position;
            touchStartTime = Time.time;
            longPressTriggered = false;

            OnTouchBegan?.Invoke(position);

            // Check for double tap
            CheckDoubleTap(position);
        }

        /// <summary>
        /// Handle touch moved
        /// Xử lý di chuyển chạm
        /// </summary>
        private void HandleTouchMoved(Vector2 position)
        {
            OnTouchMoved?.Invoke(position);
        }

        /// <summary>
        /// Handle touch ended
        /// Xử lý kết thúc chạm
        /// </summary>
        private void HandleTouchEnded(Vector2 position)
        {
            OnTouchEnded?.Invoke(position);

            // Check for swipe
            CheckSwipe(position);

            touchStartTime = 0f;
        }

        /// <summary>
        /// Check for double tap
        /// Kiểm tra double tap
        /// </summary>
        private void CheckDoubleTap(Vector2 position)
        {
            float timeSinceLastTap = Time.time - lastTapTime;
            float distance = Vector2.Distance(position, lastTapPos);

            if (timeSinceLastTap < doubleTapTime && distance < 50f)
            {
                OnDoubleTap?.Invoke(position);
                lastTapTime = 0f; // Reset to prevent triple tap
            }
            else
            {
                lastTapTime = Time.time;
                lastTapPos = position;
            }
        }

        /// <summary>
        /// Check for long press
        /// Kiểm tra long press
        /// </summary>
        private void CheckLongPress()
        {
            if (touchStartTime > 0 && !longPressTriggered)
            {
                float pressDuration = Time.time - touchStartTime;
                
                if (pressDuration >= longPressTime)
                {
                    longPressTriggered = true;
                    OnLongPress?.Invoke(touchStartPos);
                }
            }
        }

        /// <summary>
        /// Check for swipe
        /// Kiểm tra swipe
        /// </summary>
        private void CheckSwipe(Vector2 endPosition)
        {
            Vector2 swipeDelta = endPosition - touchStartPos;
            
            if (swipeDelta.magnitude >= swipeThreshold)
            {
                OnSwipe?.Invoke(touchStartPos, swipeDelta.normalized);
            }
        }

        /// <summary>
        /// Get touch count
        /// Lấy số lượng touch
        /// </summary>
        public int GetTouchCount()
        {
            return touchCount;
        }

        /// <summary>
        /// Is touching screen
        /// Đang chạm màn hình
        /// </summary>
        public bool IsTouching()
        {
            return touchCount > 0 || UnityEngine.Input.GetMouseButton(0);
        }
    }
}
