using System.Collections.Generic;
using UnityEngine;

namespace DarkLegend.Maps.Zones
{
    /// <summary>
    /// Dungeon zone - Hầm ngục với nhiều tầng
    /// Dungeon zone with multiple floors
    /// </summary>
    public class DungeonZone : ZoneBase
    {
        [Header("Dungeon Configuration")]
        [Tooltip("Số tầng / Number of floors")]
        [SerializeField] private int floorCount = 3;
        
        [Tooltip("Tầng hiện tại / Current floor")]
        [SerializeField] private int currentFloor = 1;
        
        [Tooltip("Danh sách cấu hình tầng / Floor configurations")]
        [SerializeField] private List<DungeonFloorData> floors = new List<DungeonFloorData>();
        
        [Header("Entry Requirements")]
        [Tooltip("Yêu cầu party / Require party")]
        [SerializeField] private bool requireParty = false;
        
        [Tooltip("Số người tối thiểu / Minimum party size")]
        [SerializeField] private int minPartySize = 1;
        
        [Tooltip("Số người tối đa / Maximum party size")]
        [SerializeField] private int maxPartySize = 5;
        
        [Header("Dungeon Features")]
        [Tooltip("Thời gian giới hạn (phút) / Time limit in minutes")]
        [SerializeField] private int timeLimit = 60;
        
        [Tooltip("Reset khi party wipe / Reset on party wipe")]
        [SerializeField] private bool resetOnWipe = true;
        
        [Tooltip("Drop rate bonus / Drop rate bonus")]
        [SerializeField] private float dropRateBonus = 1.5f;
        
        private float dungeonStartTime;
        private bool dungeonActive = false;
        
        public override void InitializeZone()
        {
            base.InitializeZone();
            
            dungeonStartTime = Time.time;
            dungeonActive = true;
            
            // Initialize first floor
            LoadFloor(1);
            
            Debug.Log($"[DungeonZone] Dungeon initialized: {zoneName} with {floorCount} floors");
        }
        
        public override void CleanupZone()
        {
            base.CleanupZone();
            
            dungeonActive = false;
            
            // Clear current floor
            ClearCurrentFloor();
        }
        
        protected override void UpdateZone()
        {
            base.UpdateZone();
            
            if (!dungeonActive) return;
            
            // Check time limit
            if (timeLimit > 0)
            {
                float elapsed = Time.time - dungeonStartTime;
                float remaining = (timeLimit * 60) - elapsed;
                
                if (remaining <= 0)
                {
                    OnTimeLimitExpired();
                }
            }
        }
        
        public override bool CanPlayerEnter(int playerLevel)
        {
            if (!base.CanPlayerEnter(playerLevel))
            {
                return false;
            }
            
            // Check party requirement
            if (requireParty)
            {
                // TODO: Check if player is in party
                Debug.Log($"[DungeonZone] Party required, min size: {minPartySize}");
            }
            
            return true;
        }
        
        /// <summary>
        /// Load tầng dungeon / Load dungeon floor
        /// </summary>
        public void LoadFloor(int floorNumber)
        {
            if (floorNumber < 1 || floorNumber > floorCount)
            {
                Debug.LogError($"[DungeonZone] Invalid floor number: {floorNumber}");
                return;
            }
            
            // Clear current floor
            ClearCurrentFloor();
            
            currentFloor = floorNumber;
            DungeonFloorData floorData = GetFloorData(floorNumber);
            
            if (floorData != null)
            {
                // Spawn monsters for this floor
                SpawnFloorMonsters(floorData);
                
                // Setup floor portals
                SetupFloorPortals(floorData);
                
                Debug.Log($"[DungeonZone] Loaded floor {floorNumber}: {floorData.floorName}");
            }
        }
        
        /// <summary>
        /// Lấy dữ liệu tầng / Get floor data
        /// </summary>
        private DungeonFloorData GetFloorData(int floorNumber)
        {
            return floors.Find(f => f.floorNumber == floorNumber);
        }
        
        /// <summary>
        /// Spawn quái cho tầng / Spawn floor monsters
        /// </summary>
        private void SpawnFloorMonsters(DungeonFloorData floorData)
        {
            // TODO: Implement monster spawning for floor
            Debug.Log($"[DungeonZone] Spawning monsters for floor {floorData.floorNumber}");
        }
        
        /// <summary>
        /// Setup portal giữa các tầng / Setup floor portals
        /// </summary>
        private void SetupFloorPortals(DungeonFloorData floorData)
        {
            // TODO: Create portals to next/previous floors
            Debug.Log($"[DungeonZone] Setting up portals for floor {floorData.floorNumber}");
        }
        
        /// <summary>
        /// Clear tầng hiện tại / Clear current floor
        /// </summary>
        private void ClearCurrentFloor()
        {
            // TODO: Destroy all monsters and objects
            Debug.Log($"[DungeonZone] Clearing floor {currentFloor}");
        }
        
        /// <summary>
        /// Chuyển lên tầng trên / Move to next floor
        /// </summary>
        public void GoToNextFloor()
        {
            if (currentFloor < floorCount)
            {
                LoadFloor(currentFloor + 1);
            }
            else
            {
                Debug.Log($"[DungeonZone] Already at top floor!");
            }
        }
        
        /// <summary>
        /// Chuyển xuống tầng dưới / Move to previous floor
        /// </summary>
        public void GoToPreviousFloor()
        {
            if (currentFloor > 1)
            {
                LoadFloor(currentFloor - 1);
            }
            else
            {
                Debug.Log($"[DungeonZone] Already at bottom floor!");
            }
        }
        
        /// <summary>
        /// Khi hết thời gian / When time limit expires
        /// </summary>
        private void OnTimeLimitExpired()
        {
            Debug.Log($"[DungeonZone] Time limit expired! Ejecting players...");
            
            // TODO: Eject all players from dungeon
            dungeonActive = false;
        }
        
        /// <summary>
        /// Reset dungeon / Reset dungeon state
        /// </summary>
        public void ResetDungeon()
        {
            Debug.Log($"[DungeonZone] Resetting dungeon...");
            
            currentFloor = 1;
            dungeonStartTime = Time.time;
            dungeonActive = true;
            
            LoadFloor(1);
        }
        
        /// <summary>
        /// Lấy thời gian còn lại / Get remaining time
        /// </summary>
        public float GetRemainingTime()
        {
            if (timeLimit <= 0) return -1;
            
            float elapsed = Time.time - dungeonStartTime;
            float remaining = (timeLimit * 60) - elapsed;
            
            return Mathf.Max(0, remaining);
        }
        
        /// <summary>
        /// Lấy tầng hiện tại / Get current floor
        /// </summary>
        public int GetCurrentFloor()
        {
            return currentFloor;
        }
    }
    
    /// <summary>
    /// Dữ liệu tầng dungeon / Dungeon floor data
    /// </summary>
    [System.Serializable]
    public class DungeonFloorData
    {
        [Tooltip("Số tầng / Floor number")]
        public int floorNumber;
        
        [Tooltip("Tên tầng / Floor name")]
        public string floorName;
        
        [Tooltip("Level range")]
        public Vector2Int levelRange;
        
        [Tooltip("Danh sách quái / Monster list")]
        public List<MonsterSpawnData> monsters = new List<MonsterSpawnData>();
        
        [Tooltip("Boss tầng / Floor boss")]
        public GameObject bossPrefab;
        
        [Tooltip("Vị trí portal lên tầng / Next floor portal position")]
        public Vector3 nextFloorPortalPos;
        
        [Tooltip("Vị trí portal xuống tầng / Previous floor portal position")]
        public Vector3 previousFloorPortalPos;
    }
}
