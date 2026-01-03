using UnityEngine;

namespace DarkLegend.Maps.Core
{
    /// <summary>
    /// Vùng an toàn - không PvP, không combat
    /// Safe zone - no PvP, no combat
    /// </summary>
    public class SafeZone : MonoBehaviour
    {
        [Header("Safe Zone Settings")]
        [Tooltip("Tên vùng an toàn / Safe zone name")]
        [SerializeField] private string zoneName = "Safe Zone";
        
        [Tooltip("Bán kính vùng / Zone radius")]
        [SerializeField] private float radius = 10f;
        
        [Tooltip("Hiển thị gizmo / Show gizmo in editor")]
        [SerializeField] private bool showGizmo = true;
        
        [Header("Rules")]
        [Tooltip("Vô hiệu hóa combat / Disable combat")]
        [SerializeField] private bool disableCombat = true;
        
        [Tooltip("Vô hiệu hóa PvP / Disable PvP")]
        [SerializeField] private bool disablePvP = true;
        
        [Tooltip("Hồi HP/MP / Regenerate HP/MP")]
        [SerializeField] private bool enableRegen = true;
        
        [Tooltip("Tốc độ hồi HP/giây / HP regen per second")]
        [SerializeField] private float hpRegenPerSecond = 5f;
        
        [Tooltip("Tốc độ hồi MP/giây / MP regen per second")]
        [SerializeField] private float mpRegenPerSecond = 5f;
        
        [Header("Visual")]
        [Tooltip("Màu vùng an toàn / Safe zone color")]
        [SerializeField] private Color zoneColor = new Color(0f, 1f, 0f, 0.3f);
        
        [Tooltip("Hiệu ứng vào vùng / Entry effect")]
        [SerializeField] private GameObject entryEffectPrefab;
        
        [Tooltip("Hiệu ứng ra vùng / Exit effect")]
        [SerializeField] private GameObject exitEffectPrefab;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                OnPlayerEnterSafeZone(other.gameObject);
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                OnPlayerExitSafeZone(other.gameObject);
            }
        }
        
        /// <summary>
        /// Khi player vào safe zone / When player enters safe zone
        /// </summary>
        private void OnPlayerEnterSafeZone(GameObject player)
        {
            Debug.Log($"[SafeZone] Player entered safe zone: {zoneName}");
            
            // Show entry effect
            if (entryEffectPrefab != null)
            {
                GameObject effect = Instantiate(entryEffectPrefab, player.transform.position, Quaternion.identity);
                Destroy(effect, 2f);
            }
            
            // Apply safe zone buffs
            ApplySafeZoneEffects(player, true);
            
            // Show message
            ShowSafeZoneMessage(player, true);
        }
        
        /// <summary>
        /// Khi player rời safe zone / When player exits safe zone
        /// </summary>
        private void OnPlayerExitSafeZone(GameObject player)
        {
            Debug.Log($"[SafeZone] Player exited safe zone: {zoneName}");
            
            // Show exit effect
            if (exitEffectPrefab != null)
            {
                GameObject effect = Instantiate(exitEffectPrefab, player.transform.position, Quaternion.identity);
                Destroy(effect, 2f);
            }
            
            // Remove safe zone buffs
            ApplySafeZoneEffects(player, false);
            
            // Show message
            ShowSafeZoneMessage(player, false);
        }
        
        /// <summary>
        /// Áp dụng hiệu ứng safe zone / Apply safe zone effects
        /// </summary>
        private void ApplySafeZoneEffects(GameObject player, bool enable)
        {
            // TODO: Implement player component interaction
            // - Disable/enable combat
            // - Disable/enable PvP
            // - Enable/disable regen
            
            if (enable)
            {
                Debug.Log($"[SafeZone] Applying safe zone effects to player");
                // Start regen if enabled
                if (enableRegen)
                {
                    StartPlayerRegen(player);
                }
            }
            else
            {
                Debug.Log($"[SafeZone] Removing safe zone effects from player");
                // Stop regen
                StopPlayerRegen(player);
            }
        }
        
        /// <summary>
        /// Bắt đầu hồi HP/MP / Start HP/MP regeneration
        /// </summary>
        private void StartPlayerRegen(GameObject player)
        {
            // TODO: Implement regeneration system
            Debug.Log($"[SafeZone] Starting regen: HP {hpRegenPerSecond}/s, MP {mpRegenPerSecond}/s");
        }
        
        /// <summary>
        /// Dừng hồi HP/MP / Stop HP/MP regeneration
        /// </summary>
        private void StopPlayerRegen(GameObject player)
        {
            // TODO: Stop regeneration
            Debug.Log($"[SafeZone] Stopping regen");
        }
        
        /// <summary>
        /// Hiển thị thông báo safe zone / Show safe zone message
        /// </summary>
        private void ShowSafeZoneMessage(GameObject player, bool entering)
        {
            string message = entering 
                ? $"Đã vào vùng an toàn: {zoneName}" 
                : $"Đã rời vùng an toàn: {zoneName}";
            
            // TODO: Show UI message
            Debug.Log($"[SafeZone] {message}");
        }
        
        /// <summary>
        /// Kiểm tra vị trí có trong safe zone không / Check if position is in safe zone
        /// </summary>
        public bool IsPositionInSafeZone(Vector3 position)
        {
            float distance = Vector3.Distance(transform.position, position);
            return distance <= radius;
        }
        
        private void OnDrawGizmos()
        {
            if (showGizmo)
            {
                Gizmos.color = zoneColor;
                Gizmos.DrawSphere(transform.position, radius);
                
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, radius);
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            // Draw zone info
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
