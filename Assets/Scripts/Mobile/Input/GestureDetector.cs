using UnityEngine;
using System;

namespace DarkLegend.Mobile.Input
{
    /// <summary>
    /// Gesture detector (swipe, pinch, tap)
    /// Phát hiện cử chỉ (swipe, pinch, tap)
    /// </summary>
    public class GestureDetector : MonoBehaviour
    {
        [Header("Gesture Settings")]
        public float swipeThreshold = 50f;
        public float pinchThreshold = 10f;
        public float tapThreshold = 0.2f;
        public float maxTapDistance = 20f;

        // Events
        public event Action<Vector2> OnSwipeUp;
        public event Action<Vector2> OnSwipeDown;
        public event Action<Vector2> OnSwipeLeft;
        public event Action<Vector2> OnSwipeRight;
        public event Action<float> OnPinch;
        public event Action<Vector2> OnTap;

        // Touch tracking
        private Vector2 touchStartPos;
        private float touchStartTime;
        private float previousPinchDistance;

        private void Update()
        {
            DetectGestures();
        }

        /// <summary>
        /// Detect all gestures
        /// Phát hiện tất cả cử chỉ
        /// </summary>
        private void DetectGestures()
        {
            // Detect pinch (two finger)
            if (UnityEngine.Input.touchCount == 2)
            {
                DetectPinch();
            }
            // Detect swipe and tap (single finger)
            else if (UnityEngine.Input.touchCount == 1)
            {
                Touch touch = UnityEngine.Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        touchStartPos = touch.position;
                        touchStartTime = Time.time;
                        previousPinchDistance = 0f;
                        break;

                    case TouchPhase.Ended:
                        DetectSwipe(touch.position);
                        DetectTap(touch.position);
                        break;
                }
            }
        }

        /// <summary>
        /// Detect swipe gesture
        /// Phát hiện cử chỉ swipe
        /// </summary>
        private void DetectSwipe(Vector2 endPos)
        {
            Vector2 swipeDelta = endPos - touchStartPos;
            float swipeDistance = swipeDelta.magnitude;

            if (swipeDistance < swipeThreshold)
                return;

            Vector2 swipeDirection = swipeDelta.normalized;

            // Determine swipe direction
            if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
            {
                // Horizontal swipe
                if (swipeDirection.x > 0)
                {
                    OnSwipeRight?.Invoke(touchStartPos);
                    Debug.Log("[GestureDetector] Swipe Right");
                }
                else
                {
                    OnSwipeLeft?.Invoke(touchStartPos);
                    Debug.Log("[GestureDetector] Swipe Left");
                }
            }
            else
            {
                // Vertical swipe
                if (swipeDirection.y > 0)
                {
                    OnSwipeUp?.Invoke(touchStartPos);
                    Debug.Log("[GestureDetector] Swipe Up");
                }
                else
                {
                    OnSwipeDown?.Invoke(touchStartPos);
                    Debug.Log("[GestureDetector] Swipe Down");
                }
            }
        }

        /// <summary>
        /// Detect tap gesture
        /// Phát hiện cử chỉ tap
        /// </summary>
        private void DetectTap(Vector2 endPos)
        {
            float touchDuration = Time.time - touchStartTime;
            float touchDistance = Vector2.Distance(touchStartPos, endPos);

            if (touchDuration < tapThreshold && touchDistance < maxTapDistance)
            {
                OnTap?.Invoke(endPos);
                Debug.Log($"[GestureDetector] Tap at {endPos}");
            }
        }

        /// <summary>
        /// Detect pinch gesture
        /// Phát hiện cử chỉ pinch
        /// </summary>
        private void DetectPinch()
        {
            Touch touch0 = UnityEngine.Input.GetTouch(0);
            Touch touch1 = UnityEngine.Input.GetTouch(1);

            // Calculate distance between touches
            float currentDistance = Vector2.Distance(touch0.position, touch1.position);

            if (previousPinchDistance == 0f)
            {
                previousPinchDistance = currentDistance;
                return;
            }

            // Calculate pinch delta
            float pinchDelta = currentDistance - previousPinchDistance;

            if (Mathf.Abs(pinchDelta) > pinchThreshold)
            {
                // Positive = pinch out (zoom in), Negative = pinch in (zoom out)
                OnPinch?.Invoke(pinchDelta);
                Debug.Log($"[GestureDetector] Pinch: {pinchDelta}");
            }

            previousPinchDistance = currentDistance;
        }

        /// <summary>
        /// Get swipe direction enum
        /// Lấy hướng swipe enum
        /// </summary>
        public enum SwipeDirection
        {
            None,
            Up,
            Down,
            Left,
            Right
        }

        /// <summary>
        /// Get swipe direction from delta
        /// Lấy hướng swipe từ delta
        /// </summary>
        public static SwipeDirection GetSwipeDirection(Vector2 swipeDelta)
        {
            if (swipeDelta.magnitude < 50f)
                return SwipeDirection.None;

            Vector2 direction = swipeDelta.normalized;

            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                return direction.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
            }
            else
            {
                return direction.y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
            }
        }
    }
}
