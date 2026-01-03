using System.Collections;
using UnityEngine;

namespace DarkLegend.Maps.Portals
{
    /// <summary>
    /// Town portal - Portal về thành phố
    /// Town portal for returning to town
    /// </summary>
    public class TownPortal : Portal
    {
        [Header("Town Portal Settings")]
        [Tooltip("Town mặc định / Default town")]
        [SerializeField] private string defaultTownName = "Lorencia";
        
        [Tooltip("Có thể sử dụng trong combat / Usable in combat")]
        [SerializeField] private bool usableInCombat = false;
        
        [Tooltip("Thời gian cast (giây) / Cast time")]
        [SerializeField] private float castTime = 3f;
        
        [Tooltip("Có thể bị interrupt / Can be interrupted")]
        [SerializeField] private bool canBeInterrupted = true;
        
        private bool isCasting = false;
        private float castStartTime = 0f;
        
        protected override void InitializePortal()
        {
            base.InitializePortal();
            
            // Town portal có màu xanh lá
            portalColor = Color.green;
            
            Debug.Log($"[TownPortal] Town portal initialized");
        }
        
        public override bool TryUsePortal(GameObject player)
        {
            // Check combat status
            if (!usableInCombat && IsInCombat(player))
            {
                ShowMessage(player, "Không thể sử dụng town portal trong combat!");
                return false;
            }
            
            // Start casting
            StartCasting(player);
            return true;
        }
        
        /// <summary>
        /// Bắt đầu cast town portal / Start casting town portal
        /// </summary>
        private void StartCasting(GameObject player)
        {
            if (isCasting)
            {
                ShowMessage(player, "Đang cast portal...");
                return;
            }
            
            isCasting = true;
            castStartTime = Time.time;
            currentPlayer = player;
            
            ShowMessage(player, $"Đang mở town portal... ({castTime}s)");
            Debug.Log($"[TownPortal] Started casting");
            
            // Start cast coroutine
            StartCoroutine(CastPortalCoroutine(player));
        }
        
        /// <summary>
        /// Coroutine cast portal / Portal casting coroutine
        /// </summary>
        private System.Collections.IEnumerator CastPortalCoroutine(GameObject player)
        {
            float elapsed = 0f;
            
            while (elapsed < castTime)
            {
                // Check for interruption
                if (canBeInterrupted)
                {
                    if (IsInCombat(player) || PlayerMoved(player))
                    {
                        InterruptCast(player);
                        yield break;
                    }
                }
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // Cast complete
            CompleteCast(player);
        }
        
        /// <summary>
        /// Hoàn thành cast / Complete cast
        /// </summary>
        private void CompleteCast(GameObject player)
        {
            isCasting = false;
            
            // Get default town
            Core.MapData townMap = GetDefaultTown();
            
            if (townMap != null)
            {
                destinationMap = townMap;
                destinationSpawnPosition = townMap.spawnPosition;
                
                UsePortal(player);
                ShowMessage(player, $"Đã dịch chuyển về {defaultTownName}!");
            }
            else
            {
                ShowMessage(player, "Không tìm thấy thành phố!");
                Debug.LogError($"[TownPortal] Town not found: {defaultTownName}");
            }
        }
        
        /// <summary>
        /// Interrupt cast / Interrupt casting
        /// </summary>
        private void InterruptCast(GameObject player)
        {
            isCasting = false;
            ShowMessage(player, "Town portal bị hủy!");
            Debug.Log($"[TownPortal] Cast interrupted");
        }
        
        /// <summary>
        /// Lấy town mặc định / Get default town
        /// </summary>
        private Core.MapData GetDefaultTown()
        {
            if (Core.MapManager.Instance != null)
            {
                return Core.MapManager.Instance.GetMapByName(defaultTownName);
            }
            return null;
        }
        
        /// <summary>
        /// Kiểm tra có trong combat / Check if in combat
        /// </summary>
        private bool IsInCombat(GameObject player)
        {
            // TODO: Check player combat status
            return false;
        }
        
        /// <summary>
        /// Kiểm tra player có di chuyển / Check if player moved
        /// </summary>
        private bool PlayerMoved(GameObject player)
        {
            // TODO: Check if player position changed significantly
            return false;
        }
        
        /// <summary>
        /// Sử dụng town portal item / Use town portal item
        /// </summary>
        public static void UseTownPortalItem(GameObject player)
        {
            Debug.Log($"[TownPortal] Using town portal item");
            
            // Create temporary portal
            GameObject portalObj = new GameObject("TempTownPortal");
            TownPortal portal = portalObj.AddComponent<TownPortal>();
            portal.TryUsePortal(player);
            
            // Destroy after use
            Destroy(portalObj, 5f);
        }
    }
}
