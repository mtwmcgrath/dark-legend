using UnityEngine;

namespace DarkLegend.Mobile.Camera
{
    /// <summary>
    /// Pinch to zoom camera
    /// Zoom camera bằng pinch
    /// </summary>
    public class PinchToZoom : MonoBehaviour
    {
        [Header("Zoom Settings")]
        public float minZoom = 5f;
        public float maxZoom = 20f;
        public float zoomSpeed = 0.5f;
        public float zoomSmoothTime = 0.1f;

        [Header("Camera Settings")]
        public UnityEngine.Camera targetCamera;
        public bool useFieldOfView = false; // Use FOV or distance

        private float currentZoom;
        private float targetZoom;
        private float zoomVelocity;
        private float initialDistance;

        private void Start()
        {
            if (targetCamera == null)
            {
                targetCamera = UnityEngine.Camera.main;
            }

            if (useFieldOfView)
            {
                currentZoom = targetCamera.fieldOfView;
                targetZoom = currentZoom;
            }
            else
            {
                // Assume camera is child of parent object that moves
                initialDistance = Vector3.Distance(transform.position, transform.parent.position);
                currentZoom = initialDistance;
                targetZoom = currentZoom;
            }
        }

        private void Update()
        {
            HandlePinchZoom();
            ApplyZoom();
        }

        /// <summary>
        /// Handle pinch zoom
        /// Xử lý zoom bằng pinch
        /// </summary>
        private void HandlePinchZoom()
        {
            // Handle mouse scroll in editor
            #if UNITY_EDITOR || UNITY_STANDALONE
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                Zoom(-scroll * 10f * zoomSpeed);
            }
            #endif

            // Handle pinch on mobile
            if (Input.touchCount == 2)
            {
                Touch touch0 = Input.GetTouch(0);
                Touch touch1 = Input.GetTouch(1);

                Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
                Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

                float prevMagnitude = (touch0PrevPos - touch1PrevPos).magnitude;
                float currentMagnitude = (touch0.position - touch1.position).magnitude;

                float difference = currentMagnitude - prevMagnitude;

                // Invert for zoom (pinch out = zoom in)
                Zoom(-difference * zoomSpeed * 0.1f);
            }
        }

        /// <summary>
        /// Zoom camera
        /// Zoom camera
        /// </summary>
        private void Zoom(float increment)
        {
            if (useFieldOfView)
            {
                // Zoom by changing FOV (inverted - smaller FOV = closer)
                targetZoom = Mathf.Clamp(targetZoom + increment, minZoom, maxZoom);
            }
            else
            {
                // Zoom by changing distance
                targetZoom = Mathf.Clamp(targetZoom + increment, minZoom, maxZoom);
            }
        }

        /// <summary>
        /// Apply zoom to camera
        /// Áp dụng zoom cho camera
        /// </summary>
        private void ApplyZoom()
        {
            currentZoom = Mathf.SmoothDamp(currentZoom, targetZoom, ref zoomVelocity, zoomSmoothTime);

            if (useFieldOfView)
            {
                targetCamera.fieldOfView = currentZoom;
            }
            else
            {
                // Adjust camera position (local Z)
                Vector3 localPos = transform.localPosition;
                localPos.z = -currentZoom;
                transform.localPosition = localPos;
            }
        }

        /// <summary>
        /// Reset zoom
        /// Reset zoom
        /// </summary>
        public void ResetZoom()
        {
            if (useFieldOfView)
            {
                targetZoom = 60f;
            }
            else
            {
                targetZoom = initialDistance;
            }
        }

        /// <summary>
        /// Set zoom
        /// Đặt zoom
        /// </summary>
        public void SetZoom(float zoom)
        {
            targetZoom = Mathf.Clamp(zoom, minZoom, maxZoom);
            currentZoom = targetZoom;
        }

        /// <summary>
        /// Get current zoom
        /// Lấy zoom hiện tại
        /// </summary>
        public float GetCurrentZoom()
        {
            return currentZoom;
        }
    }
}
