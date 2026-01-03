using UnityEngine;

namespace DarkLegend.Maps.Portals
{
    /// <summary>
    /// Portal cơ bản / Basic portal
    /// Base portal for map transitions
    /// </summary>
    public class Portal : MonoBehaviour
    {
        [Header("Portal Configuration")]
        [Tooltip("Tên portal / Portal name")]
        [SerializeField] protected string portalName = "Portal";
        
        [Tooltip("Map đích / Destination map")]
        [SerializeField] protected Core.MapData destinationMap;
        
        [Tooltip("Vị trí spawn đích / Destination spawn position")]
        [SerializeField] protected Vector3 destinationSpawnPosition;
        
        [Tooltip("Hướng spawn đích / Destination spawn rotation")]
        [SerializeField] protected Vector3 destinationRotation;
        
        [Header("Requirements")]
        [Tooltip("Level tối thiểu / Minimum level")]
        [SerializeField] protected int requiredLevel = 1;
        
        [Tooltip("Item yêu cầu / Required item")]
        [SerializeField] protected string requiredItem = "";
        
        [Tooltip("Chi phí Zen / Zen cost")]
        [SerializeField] protected int zenCost = 0;
        
        [Tooltip("Party portal / Party portal")]
        [SerializeField] protected bool partyPortal = false;
        
        [Header("Portal Settings")]
        [Tooltip("Kích hoạt / Is active")]
        [SerializeField] protected bool isActive = true;
        
        [Tooltip("Thời gian cooldown (giây) / Cooldown time")]
        [SerializeField] protected float cooldownTime = 5f;
        
        [Tooltip("Tự động teleport / Auto teleport")]
        [SerializeField] protected bool autoTeleport = true;
        
        [Header("Visual Effects")]
        [Tooltip("Hiệu ứng portal / Portal effect")]
        [SerializeField] protected GameObject portalEffect;
        
        [Tooltip("Màu portal / Portal color")]
        [SerializeField] protected Color portalColor = Color.blue;
        
        [Tooltip("Âm thanh / Portal sound")]
        [SerializeField] protected AudioClip portalSound;
        
        protected float lastUseTime = 0f;
        protected bool playerInRange = false;
        protected GameObject currentPlayer;
        
        protected virtual void Start()
        {
            InitializePortal();
        }
        
        protected virtual void Update()
        {
            if (!isActive) return;
            
            // Check for player in trigger
            if (playerInRange && currentPlayer != null)
            {
                ShowPortalPrompt();
            }
        }
        
        /// <summary>
        /// Khởi tạo portal / Initialize portal
        /// </summary>
        protected virtual void InitializePortal()
        {
            Debug.Log($"[Portal] Initializing portal: {portalName}");
            
            // Setup visual effects
            if (portalEffect != null)
            {
                Instantiate(portalEffect, transform.position, Quaternion.identity, transform);
            }
        }
        
        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = true;
                currentPlayer = other.gameObject;
                
                if (autoTeleport)
                {
                    TryUsePortal(other.gameObject);
                }
            }
        }
        
        protected virtual void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInRange = false;
                currentPlayer = null;
                HidePortalPrompt();
            }
        }
        
        /// <summary>
        /// Hiển thị prompt portal / Show portal prompt
        /// </summary>
        protected virtual void ShowPortalPrompt()
        {
            if (!autoTeleport)
            {
                // TODO: Show "Press E to use portal" UI
            }
        }
        
        /// <summary>
        /// Ẩn prompt portal / Hide portal prompt
        /// </summary>
        protected virtual void HidePortalPrompt()
        {
            // TODO: Hide portal prompt UI
        }
        
        /// <summary>
        /// Thử sử dụng portal / Try to use portal
        /// </summary>
        public virtual bool TryUsePortal(GameObject player)
        {
            if (!isActive)
            {
                ShowMessage(player, "Portal không hoạt động!");
                return false;
            }
            
            // Check cooldown
            if (Time.time < lastUseTime + cooldownTime)
            {
                float remaining = (lastUseTime + cooldownTime) - Time.time;
                ShowMessage(player, $"Vui lòng chờ {remaining:F1} giây!");
                return false;
            }
            
            // Check requirements
            if (!CheckRequirements(player))
            {
                return false;
            }
            
            // Use portal
            UsePortal(player);
            return true;
        }
        
        /// <summary>
        /// Kiểm tra yêu cầu / Check requirements
        /// </summary>
        protected virtual bool CheckRequirements(GameObject player)
        {
            // Check level
            int playerLevel = GetPlayerLevel(player);
            if (playerLevel < requiredLevel)
            {
                ShowMessage(player, $"Cần level {requiredLevel} để sử dụng portal này!");
                return false;
            }
            
            // Check required item
            if (!string.IsNullOrEmpty(requiredItem))
            {
                if (!HasRequiredItem(player, requiredItem))
                {
                    ShowMessage(player, $"Cần {requiredItem} để sử dụng portal!");
                    return false;
                }
            }
            
            // Check Zen cost
            if (zenCost > 0)
            {
                if (!HasEnoughZen(player, zenCost))
                {
                    ShowMessage(player, $"Cần {zenCost} Zen!");
                    return false;
                }
            }
            
            // Check party requirement
            if (partyPortal && !IsInParty(player))
            {
                ShowMessage(player, "Chỉ party mới có thể sử dụng portal này!");
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Sử dụng portal / Use portal
        /// </summary>
        protected virtual void UsePortal(GameObject player)
        {
            // Deduct costs
            if (zenCost > 0)
            {
                DeductZen(player, zenCost);
            }
            
            if (!string.IsNullOrEmpty(requiredItem))
            {
                ConsumeItem(player, requiredItem);
            }
            
            // Play effects
            PlayPortalEffects();
            
            // Teleport player
            TeleportPlayer(player);
            
            // Set cooldown
            lastUseTime = Time.time;
            
            Debug.Log($"[Portal] Player used portal: {portalName}");
        }
        
        /// <summary>
        /// Teleport player / Teleport player to destination
        /// </summary>
        protected virtual void TeleportPlayer(GameObject player)
        {
            if (destinationMap == null)
            {
                Debug.LogError($"[Portal] Destination map is null for {portalName}!");
                return;
            }
            
            // Use MapManager to transition
            if (Core.MapManager.Instance != null)
            {
                Core.MapManager.Instance.TransitionToMap(
                    destinationMap,
                    destinationSpawnPosition
                );
            }
        }
        
        /// <summary>
        /// Play hiệu ứng portal / Play portal effects
        /// </summary>
        protected virtual void PlayPortalEffects()
        {
            if (portalSound != null)
            {
                AudioSource.PlayClipAtPoint(portalSound, transform.position);
            }
            
            // TODO: Play visual effects
        }
        
        /// <summary>
        /// Lấy level player / Get player level
        /// </summary>
        protected virtual int GetPlayerLevel(GameObject player)
        {
            // TODO: Get actual player level
            return 100; // Placeholder
        }
        
        /// <summary>
        /// Kiểm tra có item / Check if has item
        /// </summary>
        protected virtual bool HasRequiredItem(GameObject player, string itemName)
        {
            // TODO: Check player inventory
            return true;
        }
        
        /// <summary>
        /// Kiểm tra có đủ Zen / Check if has enough Zen
        /// </summary>
        protected virtual bool HasEnoughZen(GameObject player, int amount)
        {
            // TODO: Check player Zen
            return true;
        }
        
        /// <summary>
        /// Trừ Zen / Deduct Zen
        /// </summary>
        protected virtual void DeductZen(GameObject player, int amount)
        {
            // TODO: Deduct Zen from player
            Debug.Log($"[Portal] Deducted {amount} Zen");
        }
        
        /// <summary>
        /// Tiêu thụ item / Consume item
        /// </summary>
        protected virtual void ConsumeItem(GameObject player, string itemName)
        {
            // TODO: Remove item from inventory
            Debug.Log($"[Portal] Consumed {itemName}");
        }
        
        /// <summary>
        /// Kiểm tra trong party / Check if in party
        /// </summary>
        protected virtual bool IsInParty(GameObject player)
        {
            // TODO: Check party status
            return false;
        }
        
        /// <summary>
        /// Hiển thị thông báo / Show message
        /// </summary>
        protected virtual void ShowMessage(GameObject player, string message)
        {
            Debug.Log($"[Portal] {message}");
            // TODO: Show UI message
        }
        
        /// <summary>
        /// Kích hoạt/vô hiệu hóa portal / Activate/deactivate portal
        /// </summary>
        public virtual void SetActive(bool active)
        {
            isActive = active;
            Debug.Log($"[Portal] {portalName} {(active ? "activated" : "deactivated")}");
        }
        
        protected virtual void OnDrawGizmos()
        {
            // Draw portal
            Gizmos.color = portalColor;
            Gizmos.DrawWireSphere(transform.position, 2f);
            
            // Draw destination indicator
            if (destinationMap != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 5f);
            }
        }
        
        protected virtual void OnDrawGizmosSelected()
        {
            // Draw portal area
            Gizmos.color = new Color(portalColor.r, portalColor.g, portalColor.b, 0.3f);
            Gizmos.DrawSphere(transform.position, 2f);
        }
    }
}
