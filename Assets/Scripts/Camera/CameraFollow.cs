using UnityEngine;

namespace DarkLegend.CameraSystem
{
    /// <summary>
    /// Simple smooth camera follow
    /// Camera theo dõi mượt mà đơn giản
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        [Header("Target")]
        public Transform target;
        public Vector3 offset = new Vector3(0, 5f, -8f);
        
        [Header("Smoothing")]
        public float smoothSpeed = 5f;
        public bool useFixedUpdate = true;
        
        [Header("Look At")]
        public bool lookAtTarget = true;
        public Vector3 lookAtOffset = Vector3.zero;
        
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
        }
        
        private void LateUpdate()
        {
            if (!useFixedUpdate)
            {
                UpdateCameraPosition();
            }
        }
        
        private void FixedUpdate()
        {
            if (useFixedUpdate)
            {
                UpdateCameraPosition();
            }
        }
        
        /// <summary>
        /// Update camera position to follow target
        /// Cập nhật vị trí camera để theo dõi mục tiêu
        /// </summary>
        private void UpdateCameraPosition()
        {
            if (target == null) return;
            
            // Calculate desired position
            Vector3 desiredPosition = target.position + offset;
            
            // Smoothly move camera
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
            
            // Look at target
            if (lookAtTarget)
            {
                Vector3 lookTarget = target.position + lookAtOffset;
                transform.LookAt(lookTarget);
            }
        }
        
        /// <summary>
        /// Set follow target
        /// Đặt mục tiêu theo dõi
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
        
        /// <summary>
        /// Set camera offset
        /// Đặt khoảng cách camera
        /// </summary>
        public void SetOffset(Vector3 newOffset)
        {
            offset = newOffset;
        }
    }
}
