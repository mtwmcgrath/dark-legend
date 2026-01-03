using UnityEngine;

namespace DarkLegend.Mobile.Camera
{
    /// <summary>
    /// Auto follow camera
    /// Camera tự động theo dõi
    /// </summary>
    public class CameraAutoFollow : MonoBehaviour
    {
        [Header("Target")]
        public Transform target;
        public Vector3 offset = new Vector3(0, 5, -10);

        [Header("Follow Settings")]
        public bool followPosition = true;
        public bool followRotation = false;
        public float followSpeed = 5f;
        public float rotationSpeed = 3f;

        [Header("Height Settings")]
        public bool maintainHeight = false;
        public float fixedHeight = 5f;

        [Header("Look At")]
        public bool lookAtTarget = true;
        public Vector3 lookAtOffset = new Vector3(0, 1.5f, 0);

        [Header("Collision Detection")]
        public bool avoidCollisions = true;
        public LayerMask collisionLayers;
        public float collisionCheckRadius = 0.3f;

        private Vector3 currentVelocity;

        private void LateUpdate()
        {
            if (target == null)
                return;

            UpdateCameraPosition();
            UpdateCameraRotation();
        }

        /// <summary>
        /// Update camera position
        /// Cập nhật vị trí camera
        /// </summary>
        private void UpdateCameraPosition()
        {
            if (!followPosition)
                return;

            // Calculate target position
            Vector3 targetPosition = target.position + offset;

            // Maintain fixed height if enabled
            if (maintainHeight)
            {
                targetPosition.y = fixedHeight;
            }

            // Check for collisions
            if (avoidCollisions)
            {
                Vector3 direction = targetPosition - target.position;
                float distance = direction.magnitude;
                direction.Normalize();

                RaycastHit hit;
                if (Physics.SphereCast(target.position, collisionCheckRadius, direction, out hit, distance, collisionLayers))
                {
                    // Move camera to collision point
                    targetPosition = hit.point - direction * collisionCheckRadius;
                }
            }

            // Smooth follow
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, 1f / followSpeed);
        }

        /// <summary>
        /// Update camera rotation
        /// Cập nhật xoay camera
        /// </summary>
        private void UpdateCameraRotation()
        {
            if (lookAtTarget)
            {
                // Look at target with offset
                Vector3 lookPosition = target.position + lookAtOffset;
                Quaternion targetRotation = Quaternion.LookRotation(lookPosition - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            else if (followRotation)
            {
                // Follow target rotation
                Quaternion targetRotation = target.rotation;
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
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
        /// Set offset
        /// Đặt offset
        /// </summary>
        public void SetOffset(Vector3 newOffset)
        {
            offset = newOffset;
        }

        /// <summary>
        /// Teleport camera to target
        /// Dịch chuyển camera đến mục tiêu ngay lập tức
        /// </summary>
        public void TeleportToTarget()
        {
            if (target == null)
                return;

            transform.position = target.position + offset;
            
            if (lookAtTarget)
            {
                transform.LookAt(target.position + lookAtOffset);
            }
        }
    }
}
