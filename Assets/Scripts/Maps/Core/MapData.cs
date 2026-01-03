using System.Collections.Generic;
using UnityEngine;

namespace DarkLegend.Maps.Core
{
    /// <summary>
    /// ScriptableObject chứa dữ liệu cấu hình cho một map
    /// Contains configuration data for a map
    /// </summary>
    [CreateAssetMenu(fileName = "MapData", menuName = "Dark Legend/Maps/Map Data")]
    public class MapData : ScriptableObject
    {
        [Header("Basic Info")]
        [Tooltip("Tên map hiển thị / Display name")]
        public string mapName;
        
        [Tooltip("ID duy nhất của map / Unique map identifier")]
        public int mapId;
        
        [Tooltip("Level tối thiểu để vào map / Minimum level requirement")]
        public int minLevel;
        
        [Tooltip("Level tối đa khuyến nghị / Maximum recommended level")]
        public int maxLevel;
        
        [Header("Map Type")]
        [Tooltip("Loại map / Map category")]
        public MapType mapType;
        
        [Tooltip("Có phải safe zone không? / Is this a safe zone?")]
        public bool isSafeZone;
        
        [Tooltip("Cho phép PvP / PvP enabled")]
        public bool pvpEnabled;
        
        [Header("Scene Info")]
        [Tooltip("Tên scene trong Unity / Unity scene name")]
        public string sceneName;
        
        [Tooltip("Vị trí spawn mặc định / Default spawn position")]
        public Vector3 spawnPosition;
        
        [Tooltip("Hướng spawn mặc định / Default spawn rotation")]
        public Vector3 spawnRotation;
        
        [Header("Environment")]
        [Tooltip("Thời tiết mặc định / Default weather")]
        public WeatherType defaultWeather;
        
        [Tooltip("Có chu kỳ ngày đêm không? / Has day/night cycle?")]
        public bool hasDayNightCycle;
        
        [Tooltip("Âm thanh môi trường / Ambient sound clip")]
        public AudioClip ambientSound;
        
        [Header("Spawning")]
        [Tooltip("Cấu hình spawn monsters / Monster spawn configurations")]
        public List<SpawnConfig> spawnConfigs = new List<SpawnConfig>();
        
        [Tooltip("Boss spawn configuration")]
        public BossSpawnConfig bossSpawnConfig;
        
        [Header("NPCs")]
        [Tooltip("Danh sách NPCs trong map / NPCs in this map")]
        public List<NPCSpawnData> npcSpawns = new List<NPCSpawnData>();
        
        [Header("Portals")]
        [Tooltip("Danh sách portals / Portal list")]
        public List<PortalData> portals = new List<PortalData>();
        
        [Header("Special Features")]
        [Tooltip("Map đặc biệt (event, dungeon) / Special map features")]
        public SpecialMapFeatures specialFeatures;
        
        [Header("UI")]
        [Tooltip("Icon minimap / Minimap icon")]
        public Sprite minimapIcon;
        
        [Tooltip("Mô tả map / Map description")]
        [TextArea(3, 5)]
        public string description;
    }
    
    /// <summary>
    /// Loại map / Map type categories
    /// </summary>
    public enum MapType
    {
        Town,           // Thành phố
        HuntingGround,  // Vùng đánh quái
        Dungeon,        // Hầm ngục
        BossArea,       // Khu vực boss
        PvPZone,        // Vùng PvP
        EventMap,       // Map sự kiện
        Arena           // Đấu trường
    }
    
    /// <summary>
    /// Loại thời tiết / Weather types
    /// </summary>
    public enum WeatherType
    {
        Clear,      // Quang đãng
        Rain,       // Mưa
        Snow,       // Tuyết
        Fog,        // Sương mù
        Sandstorm   // Bão cát
    }
    
    /// <summary>
    /// Cấu hình spawn monsters / Monster spawn configuration
    /// </summary>
    [System.Serializable]
    public class SpawnConfig
    {
        [Tooltip("Tên loại quái / Monster type name")]
        public string monsterName;
        
        [Tooltip("Prefab quái / Monster prefab")]
        public GameObject monsterPrefab;
        
        [Tooltip("Level range")]
        public Vector2Int levelRange;
        
        [Tooltip("Số lượng tối đa / Maximum count")]
        public int maxCount = 10;
        
        [Tooltip("Thời gian respawn (giây) / Respawn time in seconds")]
        public float respawnTime = 30f;
        
        [Tooltip("Khu vực spawn / Spawn area")]
        public Rect spawnArea;
        
        [Tooltip("Chỉ spawn ban đêm / Spawn at night only")]
        public bool nightOnly;
        
        [Tooltip("Chỉ spawn ban ngày / Spawn at day only")]
        public bool dayOnly;
    }
    
    /// <summary>
    /// Cấu hình spawn boss / Boss spawn configuration
    /// </summary>
    [System.Serializable]
    public class BossSpawnConfig
    {
        [Tooltip("Tên boss / Boss name")]
        public string bossName;
        
        [Tooltip("Prefab boss / Boss prefab")]
        public GameObject bossPrefab;
        
        [Tooltip("Level boss / Boss level")]
        public int level;
        
        [Tooltip("Vị trí spawn / Spawn position")]
        public Vector3 spawnPosition;
        
        [Tooltip("Thời gian spawn (giờ) / Spawn interval in hours")]
        public float spawnIntervalHours = 2f;
        
        [Tooltip("Thông báo khi spawn / Announce when spawned")]
        public bool announceSpawn = true;
    }
    
    /// <summary>
    /// Dữ liệu spawn NPC / NPC spawn data
    /// </summary>
    [System.Serializable]
    public class NPCSpawnData
    {
        [Tooltip("Tên NPC / NPC name")]
        public string npcName;
        
        [Tooltip("Loại NPC / NPC type")]
        public NPCType npcType;
        
        [Tooltip("Prefab NPC / NPC prefab")]
        public GameObject npcPrefab;
        
        [Tooltip("Vị trí / Position")]
        public Vector3 position;
        
        [Tooltip("Hướng / Rotation")]
        public Vector3 rotation;
    }
    
    /// <summary>
    /// Loại NPC / NPC types
    /// </summary>
    public enum NPCType
    {
        Shop,       // Cửa hàng
        Quest,      // Nhiệm vụ
        Storage,    // Kho đồ
        Guild,      // Guild
        Teleport,   // Dịch chuyển
        Buff,       // Buff
        Repair      // Sửa đồ
    }
    
    /// <summary>
    /// Dữ liệu portal / Portal data
    /// </summary>
    [System.Serializable]
    public class PortalData
    {
        [Tooltip("Tên portal / Portal name")]
        public string portalName;
        
        [Tooltip("Vị trí portal / Portal position")]
        public Vector3 position;
        
        [Tooltip("Map đích / Destination map")]
        public MapData destinationMap;
        
        [Tooltip("Vị trí spawn ở map đích / Spawn position in destination")]
        public Vector3 destinationSpawnPos;
        
        [Tooltip("Level yêu cầu / Required level")]
        public int requiredLevel;
        
        [Tooltip("Item yêu cầu / Required item name")]
        public string requiredItem;
        
        [Tooltip("Chi phí Zen / Zen cost")]
        public int zenCost;
        
        [Tooltip("Portal cho cả party / Party portal")]
        public bool partyPortal;
    }
    
    /// <summary>
    /// Tính năng đặc biệt của map / Special map features
    /// </summary>
    [System.Serializable]
    public class SpecialMapFeatures
    {
        [Tooltip("Map sự kiện / Event map")]
        public bool isEventMap;
        
        [Tooltip("Thời gian sự kiện (phút) / Event duration in minutes")]
        public int eventDuration;
        
        [Tooltip("Item cần để vào / Entry item required")]
        public string entryTicket;
        
        [Tooltip("Số waves trong event / Wave count")]
        public int waveCount;
        
        [Tooltip("Thời gian giữa các waves (giây) / Time between waves")]
        public float waveCooldown = 30f;
        
        [Tooltip("Phần thưởng đặc biệt / Special rewards")]
        public List<string> specialRewards = new List<string>();
    }
}
