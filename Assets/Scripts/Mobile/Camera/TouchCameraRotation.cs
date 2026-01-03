using UnityEngine;

namespace DarkLegend.Mobile.Camera
{
    /// <summary>
    /// Touch camera rotation
    /// Xoay camera bằng touch
    /// </summary>
    public class TouchCameraRotation : MonoBehaviour
    {
        [Header("Rotation Settings")]
        public float rotationSpeed = 2f;
        public bool invertX = false;
        public bool invertY = false;

        [Header("Limits")]
        public float minVerticalAngle = -30f;
        public float maxVerticalAngle = 60f;
        public bool limitHorizontal = false;
        public float minHorizontalAngle = -180f;
        public float maxHorizontalAngle = 180f;

        [Header("Smoothing")]
        public bool smoothRotation = true;
        public float smoothSpeed = 10f;

        private float currentRotationX = 0f;
        private float currentRotationY = 0f;
        private float targetRotationX = 0f;
        private float targetRotationY = 0f;
        private Vector2 lastTouchPosition;

        private void Start()
        {
            Vector3 angles = transform.eulerAngles;
            currentRotationX = angles.y;
            currentRotationY = angles.x;
            targetRotationX = currentRotationX;
            targetRotationY = currentRotationY;
        }

        private void Update()
        {
            HandleTouchRotation();
            ApplyRotation();
        }

        /// <summary>
        /// Handle touch rotation
        /// Xử lý xoay bằng touch
        /// </summary>
        private void HandleTouchRotation()
        {
            // Handle mouse in editor
            #if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButton(1)) // Right mouse button
            {
                Vector2 delta = (Vector2)Input.mousePosition - lastTouchPosition;
                UpdateRotation(delta);
            }
            lastTouchPosition = Input.mousePosition;
            #endif

            // Handle touch on mobile
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Moved)
                {
                    UpdateRotation(touch.deltaPosition);
                }
            }
        }

        /// <summary>
        /// Update rotation values
        /// Cập nhật giá trị xoay
        /// </summary>
        private void UpdateRotation(Vector2 delta)
        {
            float horizontalInput = delta.x * rotationSpeed * (invertX ? -1 : 1);
            float verticalInput = delta.y * rotationSpeed * (invertY ? 1 : -1);

            targetRotationX += horizontalInput;
            targetRotationY += verticalInput;

            // Apply limits
            targetRotationY = Mathf.Clamp(targetRotationY, minVerticalAngle, maxVerticalAngle);

            if (limitHorizontal)
            {
                targetRotationX = Mathf.Clamp(targetRotationX, minHorizontalAngle, maxHorizontalAngle);
            }
        }

        /// <summary>
        /// Apply rotation to camera
        /// Áp dụng xoay cho camera
        /// </summary>
        private void ApplyRotation()
        {
            if (smoothRotation)
            {
                currentRotationX = Mathf.LerpAngle(currentRotationX, targetRotationX, smoothSpeed * Time.deltaTime);
                currentRotationY = Mathf.LerpAngle(currentRotationY, targetRotationY, smoothSpeed * Time.deltaTime);
            }
            else
            {
                currentRotationX = targetRotationX;
                currentRotationY = targetRotationY;
            }

            transform.rotation = Quaternion.Euler(currentRotationY, currentRotationX, 0f);
        }

        /// <summary>
        /// Reset rotation
        /// Reset xoay
        /// </summary>
        public void ResetRotation()
        {
            currentRotationX = 0f;
            currentRotationY = 0f;
            targetRotationX = 0f;
            targetRotationY = 0f;
        }

        /// <summary>
        /// Set rotation
        /// Đặt xoay
        /// </summary>
        public void SetRotation(float x, float y)
        {
            targetRotationX = x;
            targetRotationY = y;
            currentRotationX = x;
            currentRotationY = y;
        }
    }
}
