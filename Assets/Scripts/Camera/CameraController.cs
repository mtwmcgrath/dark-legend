using UnityEngine;

namespace DarkLegend.CameraSystem
{
    /// <summary>
    /// Third-person camera controller with mouse input
    /// Điều khiển camera góc nhìn thứ ba với chuột
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("Target")]
        public Transform target;
        public Vector3 targetOffset = new Vector3(0, 1.5f, 0);
        
        [Header("Distance Settings")]
        public float distance = 5f;
        public float minDistance = 3f;
        public float maxDistance = 10f;
        public float zoomSpeed = 2f;
        
        [Header("Rotation Settings")]
        public float rotationSpeed = 5f;
        public float minVerticalAngle = -30f;
        public float maxVerticalAngle = 60f;
        
        [Header("Smoothing")]
        public float positionSmoothing = 5f;
        public float rotationSmoothing = 5f;
        
        [Header("Collision")]
        public bool checkCollision = true;
        public LayerMask collisionLayers;
        public float collisionOffset = 0.2f;
        
        // Private variables
        private float currentX = 0f;
        private float currentY = 20f;
        private float currentDistance;
        private Vector3 smoothPosition;
        
        private void Start()
        {
            // Find player if not assigned
            if (target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag(Utils.Constants.TAG_PLAYER);
                if (player != null)
                {
                    target = player.transform;
                }
            }
            
            currentDistance = distance;
            smoothPosition = transform.position;
            
            // Initialize rotation from current camera rotation
            Vector3 angles = transform.eulerAngles;
            currentX = angles.y;
            currentY = angles.x;
        }
        
        private void LateUpdate()
        {
            if (target == null) return;
            
            HandleInput();
            UpdateCamera();
        }
        
        /// <summary>
        /// Handle mouse input for camera control
        /// Xử lý input chuột để điều khiển camera
        /// </summary>
        private void HandleInput()
        {
            // Right mouse button for camera rotation
            if (Input.GetMouseButton(1))
            {
                currentX += Input.GetAxis(Utils.Constants.MOUSE_X) * rotationSpeed;
                currentY -= Input.GetAxis(Utils.Constants.MOUSE_Y) * rotationSpeed;
                
                // Clamp vertical angle
                currentY = Utils.Extensions.ClampAngle(currentY, minVerticalAngle, maxVerticalAngle);
            }
            
            // Mouse scroll for zoom
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                currentDistance -= scroll * zoomSpeed;
                currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
            }
        }
        
        /// <summary>
        /// Update camera position and rotation
        /// Cập nhật vị trí và xoay camera
        /// </summary>
        private void UpdateCamera()
        {
            // Calculate desired position
            Vector3 targetPosition = target.position + targetOffset;
            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
            
            float finalDistance = currentDistance;
            
            // Check for collision
            if (checkCollision)
            {
                Vector3 direction = rotation * -Vector3.forward;
                RaycastHit hit;
                
                if (Physics.Raycast(targetPosition, direction, out hit, currentDistance, collisionLayers))
                {
                    finalDistance = hit.distance - collisionOffset;
                    finalDistance = Mathf.Max(finalDistance, minDistance);
                }
            }
            
            Vector3 desiredPosition = targetPosition + rotation * new Vector3(0, 0, -finalDistance);
            
            // Smooth position
            smoothPosition = Vector3.Lerp(smoothPosition, desiredPosition, positionSmoothing * Time.deltaTime);
            transform.position = smoothPosition;
            
            // Smooth rotation
            Quaternion desiredRotation = Quaternion.LookRotation(targetPosition - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmoothing * Time.deltaTime);
        }
        
        /// <summary>
        /// Set camera target
        /// Đặt mục tiêu camera
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
        
        /// <summary>
        /// Reset camera to default position
        /// Đặt lại camera về vị trí mặc định
        /// </summary>
        public void ResetCamera()
        {
            currentX = 0f;
            currentY = 20f;
            currentDistance = distance;
        }
        
        /// <summary>
        /// Set camera distance
        /// Đặt khoảng cách camera
        /// </summary>
        public void SetDistance(float newDistance)
        {
            currentDistance = Mathf.Clamp(newDistance, minDistance, maxDistance);
        }
    }
}
