using UnityEngine;

namespace DarkLegend.Maps.Zones
{
    /// <summary>
    /// Base class cho tất cả zones
    /// Base class for all zone types
    /// </summary>
    public abstract class ZoneBase : MonoBehaviour
    {
        [Header("Zone Info")]
        [Tooltip("Tên zone / Zone name")]
        [SerializeField] protected string zoneName;
        
        [Tooltip("Mô tả zone / Zone description")]
        [TextArea(2, 4)]
        [SerializeField] protected string description;
        
        [Tooltip("Level tối thiểu / Minimum level")]
        [SerializeField] protected int minLevel = 1;
        
        [Tooltip("Level tối đa / Maximum level")]
        [SerializeField] protected int maxLevel = 999;
        
        [Header("Zone Bounds")]
        [Tooltip("Trung tâm zone / Zone center")]
        [SerializeField] protected Vector3 zoneCenter;
        
        [Tooltip("Kích thước zone / Zone size")]
        [SerializeField] protected Vector3 zoneSize = new Vector3(100, 50, 100);
        
        [Tooltip("Hiển thị bounds / Show bounds")]
        [SerializeField] protected bool showBounds = true;
        
        protected bool isActive = false;
        
        /// <summary>
        /// Khởi tạo zone / Initialize zone
        /// </summary>
        protected virtual void Awake()
        {
            zoneCenter = transform.position;
        }
        
        protected virtual void Start()
        {
            InitializeZone();
        }
        
        /// <summary>
        /// Khởi tạo zone / Initialize zone logic
        /// </summary>
        public virtual void InitializeZone()
        {
            Debug.Log($"[ZoneBase] Initializing zone: {zoneName}");
            isActive = true;
        }
        
        /// <summary>
        /// Cleanup zone / Cleanup zone when unloading
        /// </summary>
        public virtual void CleanupZone()
        {
            Debug.Log($"[ZoneBase] Cleaning up zone: {zoneName}");
            isActive = false;
        }
        
        /// <summary>
        /// Kiểm tra player có thể vào zone không / Check if player can enter
        /// </summary>
        public virtual bool CanPlayerEnter(int playerLevel)
        {
            return playerLevel >= minLevel && playerLevel <= maxLevel;
        }
        
        /// <summary>
        /// Khi player vào zone / When player enters zone
        /// </summary>
        public virtual void OnPlayerEnter(GameObject player)
        {
            Debug.Log($"[{zoneName}] Player entered zone");
        }
        
        /// <summary>
        /// Khi player rời zone / When player exits zone
        /// </summary>
        public virtual void OnPlayerExit(GameObject player)
        {
            Debug.Log($"[{zoneName}] Player exited zone");
        }
        
        /// <summary>
        /// Update zone mỗi frame / Update zone each frame
        /// </summary>
        protected virtual void Update()
        {
            if (!isActive) return;
            
            UpdateZone();
        }
        
        /// <summary>
        /// Override để implement zone logic / Override for zone-specific logic
        /// </summary>
        protected virtual void UpdateZone()
        {
            // Override in derived classes
        }
        
        /// <summary>
        /// Kiểm tra vị trí có trong zone không / Check if position is in zone
        /// </summary>
        public virtual bool IsPositionInZone(Vector3 position)
        {
            Bounds bounds = new Bounds(zoneCenter, zoneSize);
            return bounds.Contains(position);
        }
        
        /// <summary>
        /// Lấy vị trí random trong zone / Get random position in zone
        /// </summary>
        public virtual Vector3 GetRandomPositionInZone()
        {
            float x = Random.Range(zoneCenter.x - zoneSize.x / 2, zoneCenter.x + zoneSize.x / 2);
            float z = Random.Range(zoneCenter.z - zoneSize.z / 2, zoneCenter.z + zoneSize.z / 2);
            float y = zoneCenter.y;
            
            return new Vector3(x, y, z);
        }
        
        protected virtual void OnDrawGizmos()
        {
            if (showBounds)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(zoneCenter, zoneSize);
            }
        }
        
        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(zoneCenter, zoneSize);
        }
    }
}
