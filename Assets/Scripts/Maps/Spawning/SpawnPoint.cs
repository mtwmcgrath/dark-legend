using UnityEngine;

namespace DarkLegend.Maps.Spawning
{
    /// <summary>
    /// Điểm spawn cho monsters / NPCs
    /// Spawn point for monsters/NPCs
    /// </summary>
    public class SpawnPoint : MonoBehaviour
    {
        [Header("Spawn Point Settings")]
        [Tooltip("Tên spawn point / Spawn point name")]
        [SerializeField] private string pointName = "Spawn Point";
        
        [Tooltip("Bán kính spawn / Spawn radius")]
        [SerializeField] private float spawnRadius = 5f;
        
        [Tooltip("Hướng spawn / Spawn direction")]
        [SerializeField] private Vector3 spawnDirection = Vector3.forward;
        
        [Header("Spawn Rules")]
        [Tooltip("Kích hoạt / Is active")]
        [SerializeField] private bool isActive = true;
        
        [Tooltip("Chỉ spawn một lần / Spawn once only")]
        [SerializeField] private bool spawnOnce = false;
        
        [Tooltip("Đã spawn / Has spawned")]
        [SerializeField] private bool hasSpawned = false;
        
        [Header("Visual")]
        [Tooltip("Hiển thị gizmo / Show gizmo")]
        [SerializeField] private bool showGizmo = true;
        
        [Tooltip("Màu gizmo / Gizmo color")]
        [SerializeField] private Color gizmoColor = Color.green;
        
        /// <summary>
        /// Lấy vị trí spawn / Get spawn position
        /// </summary>
        public Vector3 GetSpawnPosition()
        {
            if (!isActive)
            {
                return transform.position;
            }
            
            if (spawnOnce && hasSpawned)
            {
                Debug.LogWarning($"[SpawnPoint] {pointName} has already spawned once!");
                return transform.position;
            }
            
            // Random position trong radius
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 offset = new Vector3(randomCircle.x, 0, randomCircle.y);
            Vector3 spawnPos = transform.position + offset;
            
            // Mark as spawned
            if (spawnOnce)
            {
                hasSpawned = true;
            }
            
            return spawnPos;
        }
        
        /// <summary>
        /// Lấy hướng spawn / Get spawn rotation
        /// </summary>
        public Quaternion GetSpawnRotation()
        {
            return Quaternion.LookRotation(spawnDirection);
        }
        
        /// <summary>
        /// Reset spawn point / Reset spawn point
        /// </summary>
        public void ResetSpawnPoint()
        {
            hasSpawned = false;
        }
        
        /// <summary>
        /// Kích hoạt spawn point / Activate spawn point
        /// </summary>
        public void Activate()
        {
            isActive = true;
        }
        
        /// <summary>
        /// Vô hiệu hóa spawn point / Deactivate spawn point
        /// </summary>
        public void Deactivate()
        {
            isActive = false;
        }
        
        /// <summary>
        /// Kiểm tra có thể spawn / Check if can spawn
        /// </summary>
        public bool CanSpawn()
        {
            return isActive && (!spawnOnce || !hasSpawned);
        }
        
        private void OnDrawGizmos()
        {
            if (!showGizmo) return;
            
            // Draw spawn radius
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
            
            // Draw spawn direction
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, spawnDirection.normalized * 2f);
            
            // Draw different color if inactive or already spawned
            if (!isActive || (spawnOnce && hasSpawned))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.position, 0.5f);
            }
            else
            {
                Gizmos.color = gizmoColor;
                Gizmos.DrawSphere(transform.position, 0.5f);
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            // Draw spawn area when selected
            Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.3f);
            
            // Draw cylinder for spawn area
            for (int i = 0; i < 360; i += 10)
            {
                float angle1 = i * Mathf.Deg2Rad;
                float angle2 = (i + 10) * Mathf.Deg2Rad;
                
                Vector3 p1 = transform.position + new Vector3(Mathf.Cos(angle1), 0, Mathf.Sin(angle1)) * spawnRadius;
                Vector3 p2 = transform.position + new Vector3(Mathf.Cos(angle2), 0, Mathf.Sin(angle2)) * spawnRadius;
                
                Gizmos.DrawLine(p1, p2);
            }
        }
    }
}
