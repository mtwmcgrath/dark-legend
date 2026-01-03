using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DarkLegend.Maps.Minimap
{
    /// <summary>
    /// World map system - Bản đồ toàn server
    /// Full world map display with all locations
    /// </summary>
    public class WorldMap : MonoBehaviour
    {
        [Header("World Map Configuration")]
        [Tooltip("World map texture / World map texture")]
        [SerializeField] private Texture2D worldMapTexture;
        
        [Tooltip("RawImage display / World map image")]
        [SerializeField] private RawImage worldMapImage;
        
        [Tooltip("Player marker / Player position marker")]
        [SerializeField] private Image playerMarker;
        
        [Header("Map Markers")]
        [Tooltip("Town marker prefab / Town marker")]
        [SerializeField] private GameObject townMarkerPrefab;
        
        [Tooltip("Dungeon marker prefab / Dungeon marker")]
        [SerializeField] private GameObject dungeonMarkerPrefab;
        
        [Tooltip("Boss marker prefab / Boss marker")]
        [SerializeField] private GameObject bossMarkerPrefab;
        
        [Header("Settings")]
        [Tooltip("Hiển thị tất cả maps / Show all maps")]
        [SerializeField] private bool showAllMaps = false;
        
        [Tooltip("Chỉ hiển thị đã khám phá / Show discovered only")]
        [SerializeField] private bool showDiscoveredOnly = true;
        
        private List<GameObject> mapMarkers = new List<GameObject>();
        private bool isMapOpen = false;
        
        private void Start()
        {
            if (worldMapImage != null && worldMapTexture != null)
            {
                worldMapImage.texture = worldMapTexture;
            }
            
            // Hide by default
            HideWorldMap();
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                ToggleWorldMap();
            }
            
            if (isMapOpen)
            {
                UpdatePlayerMarker();
            }
        }
        
        /// <summary>
        /// Hiển thị world map / Show world map
        /// </summary>
        public void ShowWorldMap()
        {
            isMapOpen = true;
            
            if (worldMapImage != null)
            {
                worldMapImage.gameObject.SetActive(true);
            }
            
            LoadMapMarkers();
            Debug.Log("[WorldMap] World map opened");
        }
        
        /// <summary>
        /// Ẩn world map / Hide world map
        /// </summary>
        public void HideWorldMap()
        {
            isMapOpen = false;
            
            if (worldMapImage != null)
            {
                worldMapImage.gameObject.SetActive(false);
            }
            
            ClearMapMarkers();
            Debug.Log("[WorldMap] World map closed");
        }
        
        /// <summary>
        /// Toggle world map / Toggle world map visibility
        /// </summary>
        public void ToggleWorldMap()
        {
            if (isMapOpen)
            {
                HideWorldMap();
            }
            else
            {
                ShowWorldMap();
            }
        }
        
        /// <summary>
        /// Load map markers / Load all map markers
        /// </summary>
        private void LoadMapMarkers()
        {
            ClearMapMarkers();
            
            // Get all maps from MapManager
            if (Core.MapManager.Instance != null)
            {
                List<Core.MapData> allMaps = Core.MapManager.Instance.GetAllMaps();
                
                foreach (var map in allMaps)
                {
                    if (showAllMaps || (!showDiscoveredOnly || IsMapDiscovered(map)))
                    {
                        CreateMapMarker(map);
                    }
                }
            }
        }
        
        /// <summary>
        /// Tạo marker cho map / Create map marker
        /// </summary>
        private void CreateMapMarker(Core.MapData mapData)
        {
            GameObject markerPrefab = GetMarkerPrefab(mapData.mapType);
            
            if (markerPrefab == null) return;
            
            GameObject marker = Instantiate(markerPrefab, worldMapImage.transform);
            
            // TODO: Position marker based on map coordinates
            // marker.GetComponent<RectTransform>().anchoredPosition = GetMapPosition(mapData);
            
            mapMarkers.Add(marker);
        }
        
        /// <summary>
        /// Lấy marker prefab / Get marker prefab for map type
        /// </summary>
        private GameObject GetMarkerPrefab(Core.MapType mapType)
        {
            switch (mapType)
            {
                case Core.MapType.Town:
                    return townMarkerPrefab;
                case Core.MapType.Dungeon:
                    return dungeonMarkerPrefab;
                case Core.MapType.BossArea:
                    return bossMarkerPrefab;
                default:
                    return dungeonMarkerPrefab;
            }
        }
        
        /// <summary>
        /// Xóa markers / Clear all markers
        /// </summary>
        private void ClearMapMarkers()
        {
            foreach (var marker in mapMarkers)
            {
                if (marker != null)
                {
                    Destroy(marker);
                }
            }
            mapMarkers.Clear();
        }
        
        /// <summary>
        /// Cập nhật player marker / Update player marker position
        /// </summary>
        private void UpdatePlayerMarker()
        {
            if (playerMarker == null) return;
            
            // TODO: Update player marker position based on current map
            // playerMarker.GetComponent<RectTransform>().anchoredPosition = GetPlayerMapPosition();
        }
        
        /// <summary>
        /// Kiểm tra map đã khám phá / Check if map is discovered
        /// </summary>
        private bool IsMapDiscovered(Core.MapData mapData)
        {
            // TODO: Check player's discovered maps
            return true; // For now, show all
        }
        
        /// <summary>
        /// Đánh dấu map đã khám phá / Mark map as discovered
        /// </summary>
        public void DiscoverMap(Core.MapData mapData)
        {
            // TODO: Save to player data
            Debug.Log($"[WorldMap] Discovered map: {mapData.mapName}");
        }
    }
}
