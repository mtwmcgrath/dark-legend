using UnityEngine;

namespace DarkLegend.Mobile.Camera
{
    /// <summary>
    /// Mobile camera controller
    /// Điều khiển camera cho mobile
    /// </summary>
    public class MobileCameraController : MonoBehaviour
    {
        [Header("Target")]
        public Transform target;
        public Vector3 offset = new Vector3(0, 5, -10);

        [Header("Touch Rotation")]
        public float rotationSpeed = 0.5f;
        public float verticalClamp = 60f;
        public bool invertY = false;

        [Header("Pinch Zoom")]
        public float minZoom = 5f;
        public float maxZoom = 20f;
        public float zoomSpeed = 0.5f;
        public float zoomSmoothTime = 0.1f;

        [Header("Auto Follow")]
        public bool autoFollowInCombat = true;
        public float autoFollowSpeed = 2f;
        public Transform combatTarget;

        [Header("Touch Areas")]
        public RectTransform joystickArea;
        public RectTransform buttonArea;

        private float currentZoom;
        private float targetZoom;
        private float zoomVelocity;
        private float rotationX = 0f;
        private float rotationY = 0f;
        private Vector2 lastTouchPosition;
        private bool isTouchingCamera = false;

        private void Start()
        {
            currentZoom = offset.magnitude;
            targetZoom = currentZoom;

            if (target != null)
            {
                // Calculate initial rotation
                Vector3 angles = transform.eulerAngles;
                rotationX = angles.y;
                rotationY = angles.x;
            }
        }

        private void LateUpdate()
        {
            if (target == null)
                return;

            HandleTouchInput();
            HandlePinchZoom();
            UpdateCameraPosition();
        }

        /// <summary>
        /// Handle touch input cho rotation
        /// Handle touch input for rotation
        /// </summary>
        private void HandleTouchInput()
        {
            // Handle mouse in editor
            #if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButton(0) && !IsPointerOverUI(Input.mousePosition))
            {
                Vector2 delta = (Vector2)Input.mousePosition - lastTouchPosition;
                RotateCamera(delta);
                lastTouchPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonDown(0))
            {
                lastTouchPosition = Input.mousePosition;
            }
            #endif

            // Handle touch on mobile
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                // Check if touch is not in UI areas
                if (!IsPointerOverUI(touch.position))
                {
                    if (touch.phase == TouchPhase.Moved)
                    {
                        RotateCamera(touch.deltaPosition);
                    }
                }
            }
        }

        /// <summary>
        /// Rotate camera
        /// Xoay camera
        /// </summary>
        private void RotateCamera(Vector2 delta)
        {
            rotationX += delta.x * rotationSpeed;
            rotationY += delta.y * rotationSpeed * (invertY ? 1 : -1);

            // Clamp vertical rotation
            rotationY = Mathf.Clamp(rotationY, -verticalClamp, verticalClamp);
        }

        /// <summary>
        /// Handle pinch zoom
        /// Xử lý zoom bằng pinch
        /// </summary>
        private void HandlePinchZoom()
        {
            if (Input.touchCount == 2)
            {
                Touch touch0 = Input.GetTouch(0);
                Touch touch1 = Input.GetTouch(1);

                Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
                Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

                float prevMagnitude = (touch0PrevPos - touch1PrevPos).magnitude;
                float currentMagnitude = (touch0.position - touch1.position).magnitude;

                float difference = currentMagnitude - prevMagnitude;

                Zoom(difference * zoomSpeed * -0.01f);
            }

            // Smooth zoom
            currentZoom = Mathf.SmoothDamp(currentZoom, targetZoom, ref zoomVelocity, zoomSmoothTime);
        }

        /// <summary>
        /// Zoom camera
        /// Zoom camera
        /// </summary>
        private void Zoom(float increment)
        {
            targetZoom = Mathf.Clamp(targetZoom + increment, minZoom, maxZoom);
        }

        /// <summary>
        /// Update camera position
        /// Cập nhật vị trí camera
        /// </summary>
        private void UpdateCameraPosition()
        {
            // Calculate rotation
            Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0);

            // Calculate position with zoom
            Vector3 zoomedOffset = offset.normalized * currentZoom;
            Vector3 position = target.position + rotation * zoomedOffset;

            // Apply position and rotation
            transform.position = position;
            transform.LookAt(target.position + Vector3.up * 1.5f);

            // Auto follow combat target
            if (autoFollowInCombat && combatTarget != null)
            {
                Vector3 directionToTarget = (combatTarget.position - target.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                float targetAngle = targetRotation.eulerAngles.y;
                
                rotationX = Mathf.LerpAngle(rotationX, targetAngle, autoFollowSpeed * Time.deltaTime);
            }
        }

        /// <summary>
        /// Check if pointer is over UI
        /// Kiểm tra con trỏ có trên UI không
        /// </summary>
        private bool IsPointerOverUI(Vector2 position)
        {
            if (joystickArea != null && RectTransformUtility.RectangleContainsScreenPoint(joystickArea, position))
                return true;

            if (buttonArea != null && RectTransformUtility.RectangleContainsScreenPoint(buttonArea, position))
                return true;

            return false;
        }

        /// <summary>
        /// Set target
        /// Đặt mục tiêu
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        /// <summary>
        /// Set combat target
        /// Đặt mục tiêu chiến đấu
        /// </summary>
        public void SetCombatTarget(Transform newTarget)
        {
            combatTarget = newTarget;
        }

        /// <summary>
        /// Reset camera
        /// Reset camera
        /// </summary>
        public void ResetCamera()
        {
            rotationX = 0f;
            rotationY = 20f;
            targetZoom = offset.magnitude;
        }
    }
}
