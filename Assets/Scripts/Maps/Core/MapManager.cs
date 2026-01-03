using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DarkLegend.Maps.Core
{
    /// <summary>
    /// Quản lý tất cả các maps trong game
    /// Manages all maps in the game
    /// Singleton pattern để truy cập toàn cục
    /// </summary>
    public class MapManager : MonoBehaviour
    {
        #region Singleton
        private static MapManager _instance;
        public static MapManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<MapManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("MapManager");
                        _instance = go.AddComponent<MapManager>();
                    }
                }
                return _instance;
            }
        }
        #endregion

        [Header("Map Database")]
        [Tooltip("Danh sách tất cả maps / All available maps")]
        [SerializeField] private List<MapData> allMaps = new List<MapData>();
        
        [Header("Current State")]
        [Tooltip("Map hiện tại / Current active map")]
        [SerializeField] private MapData currentMap;
        
        [Tooltip("Map trước đó / Previous map")]
        [SerializeField] private MapData previousMap;
        
        [Header("Loading")]
        [Tooltip("Màn hình loading / Loading screen prefab")]
        [SerializeField] private GameObject loadingScreenPrefab;
        
        private GameObject currentLoadingScreen;
        private MapLoader mapLoader;
        
        // Events
        public delegate void MapChangedHandler(MapData newMap, MapData oldMap);
        public event MapChangedHandler OnMapChanged;
        
        public delegate void MapLoadingHandler(MapData map, float progress);
        public event MapLoadingHandler OnMapLoading;
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            mapLoader = gameObject.AddComponent<MapLoader>();
            InitializeMapDatabase();
        }
        
        /// <summary>
        /// Khởi tạo database maps / Initialize map database
        /// </summary>
        private void InitializeMapDatabase()
        {
            // Load all MapData from Resources
            MapData[] loadedMaps = Resources.LoadAll<MapData>("Maps");
            foreach (var map in loadedMaps)
            {
                if (!allMaps.Contains(map))
                {
                    allMaps.Add(map);
                }
            }
            
            Debug.Log($"[MapManager] Loaded {allMaps.Count} maps");
        }
        
        /// <summary>
        /// Lấy map theo ID / Get map by ID
        /// </summary>
        public MapData GetMapById(int mapId)
        {
            return allMaps.Find(m => m.mapId == mapId);
        }
        
        /// <summary>
        /// Lấy map theo tên / Get map by name
        /// </summary>
        public MapData GetMapByName(string mapName)
        {
            return allMaps.Find(m => m.mapName == mapName);
        }
        
        /// <summary>
        /// Lấy map hiện tại / Get current map
        /// </summary>
        public MapData GetCurrentMap()
        {
            return currentMap;
        }
        
        /// <summary>
        /// Kiểm tra player có thể vào map không / Check if player can enter map
        /// </summary>
        public bool CanEnterMap(MapData map, int playerLevel, string itemName = null)
        {
            if (map == null)
            {
                Debug.LogWarning("[MapManager] Map is null!");
                return false;
            }
            
            // Kiểm tra level
            if (playerLevel < map.minLevel)
            {
                Debug.Log($"[MapManager] Player level {playerLevel} too low for {map.mapName} (requires {map.minLevel})");
                return false;
            }
            
            // Kiểm tra item nếu là event map
            if (map.specialFeatures != null && map.specialFeatures.isEventMap)
            {
                if (!string.IsNullOrEmpty(map.specialFeatures.entryTicket))
                {
                    // TODO: Kiểm tra trong inventory
                    Debug.Log($"[MapManager] Checking for entry ticket: {map.specialFeatures.entryTicket}");
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// Chuyển sang map mới / Transition to new map
        /// </summary>
        public void TransitionToMap(MapData targetMap, Vector3? spawnPosition = null)
        {
            if (targetMap == null)
            {
                Debug.LogError("[MapManager] Cannot transition to null map!");
                return;
            }
            
            StartCoroutine(TransitionCoroutine(targetMap, spawnPosition));
        }
        
        private System.Collections.IEnumerator TransitionCoroutine(MapData targetMap, Vector3? spawnPosition)
        {
            // Hiển thị loading screen
            ShowLoadingScreen();
            
            // Lưu map cũ
            previousMap = currentMap;
            
            // Unload map cũ
            if (currentMap != null)
            {
                yield return mapLoader.UnloadMap(currentMap);
            }
            
            // Load map mới
            yield return mapLoader.LoadMap(targetMap, (progress) =>
            {
                OnMapLoading?.Invoke(targetMap, progress);
            });
            
            // Cập nhật current map
            currentMap = targetMap;
            
            // Spawn player tại vị trí mới
            Vector3 finalSpawnPos = spawnPosition ?? targetMap.spawnPosition;
            SpawnPlayerAtPosition(finalSpawnPos, targetMap.spawnRotation);
            
            // Ẩn loading screen
            HideLoadingScreen();
            
            // Trigger event
            OnMapChanged?.Invoke(currentMap, previousMap);
            
            Debug.Log($"[MapManager] Transitioned to map: {targetMap.mapName}");
        }
        
        /// <summary>
        /// Spawn player tại vị trí / Spawn player at position
        /// </summary>
        private void SpawnPlayerAtPosition(Vector3 position, Vector3 rotation)
        {
            // TODO: Implement player spawning
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = position;
                player.transform.eulerAngles = rotation;
            }
            
            Debug.Log($"[MapManager] Player spawned at {position}");
        }
        
        /// <summary>
        /// Hiển thị loading screen / Show loading screen
        /// </summary>
        private void ShowLoadingScreen()
        {
            if (loadingScreenPrefab != null && currentLoadingScreen == null)
            {
                currentLoadingScreen = Instantiate(loadingScreenPrefab);
                DontDestroyOnLoad(currentLoadingScreen);
            }
        }
        
        /// <summary>
        /// Ẩn loading screen / Hide loading screen
        /// </summary>
        private void HideLoadingScreen()
        {
            if (currentLoadingScreen != null)
            {
                Destroy(currentLoadingScreen);
                currentLoadingScreen = null;
            }
        }
        
        /// <summary>
        /// Lấy danh sách maps theo loại / Get maps by type
        /// </summary>
        public List<MapData> GetMapsByType(MapType type)
        {
            return allMaps.FindAll(m => m.mapType == type);
        }
        
        /// <summary>
        /// Lấy danh sách towns / Get all town maps
        /// </summary>
        public List<MapData> GetAllTowns()
        {
            return GetMapsByType(MapType.Town);
        }
        
        /// <summary>
        /// Quay về town gần nhất / Return to nearest town
        /// </summary>
        public void ReturnToTown()
        {
            // Mặc định về Lorencia
            MapData lorencia = GetMapByName("Lorencia");
            if (lorencia != null)
            {
                TransitionToMap(lorencia);
            }
            else
            {
                Debug.LogError("[MapManager] Lorencia map not found!");
            }
        }
        
        /// <summary>
        /// Lấy tất cả maps / Get all maps
        /// </summary>
        public List<MapData> GetAllMaps()
        {
            return new List<MapData>(allMaps);
        }
        
        /// <summary>
        /// Kiểm tra có phải safe zone không / Check if current zone is safe
        /// </summary>
        public bool IsInSafeZone()
        {
            return currentMap != null && currentMap.isSafeZone;
        }
        
        /// <summary>
        /// Kiểm tra có thể PvP không / Check if PvP is allowed
        /// </summary>
        public bool IsPvPEnabled()
        {
            return currentMap != null && currentMap.pvpEnabled;
        }
    }
}
