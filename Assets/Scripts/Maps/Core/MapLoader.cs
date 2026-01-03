using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DarkLegend.Maps.Core
{
    /// <summary>
    /// Xử lý việc load và unload maps
    /// Handles loading and unloading of maps
    /// Sử dụng async scene loading
    /// </summary>
    public class MapLoader : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Thời gian fade giữa maps / Fade time between maps")]
        [SerializeField] private float fadeTime = 0.5f;
        
        [Tooltip("Minimum loading time (giây) / Minimum loading time in seconds")]
        [SerializeField] private float minLoadingTime = 1f;
        
        private AsyncOperation currentLoadOperation;
        
        /// <summary>
        /// Load map async / Asynchronously load a map
        /// </summary>
        public IEnumerator LoadMap(MapData mapData, System.Action<float> onProgress = null)
        {
            if (mapData == null)
            {
                Debug.LogError("[MapLoader] Cannot load null map!");
                yield break;
            }
            
            float startTime = Time.time;
            
            Debug.Log($"[MapLoader] Loading map: {mapData.mapName} (Scene: {mapData.sceneName})");
            
            // Load scene async
            currentLoadOperation = SceneManager.LoadSceneAsync(mapData.sceneName, LoadSceneMode.Single);
            currentLoadOperation.allowSceneActivation = false;
            
            // Chờ load xong
            while (!currentLoadOperation.isDone)
            {
                float progress = Mathf.Clamp01(currentLoadOperation.progress / 0.9f);
                onProgress?.Invoke(progress);
                
                // Khi load xong 90%, chờ thêm để đảm bảo minimum loading time
                if (currentLoadOperation.progress >= 0.9f)
                {
                    float elapsedTime = Time.time - startTime;
                    if (elapsedTime >= minLoadingTime)
                    {
                        currentLoadOperation.allowSceneActivation = true;
                    }
                }
                
                yield return null;
            }
            
            // Khởi tạo môi trường map
            yield return InitializeMapEnvironment(mapData);
            
            onProgress?.Invoke(1f);
            Debug.Log($"[MapLoader] Map loaded successfully: {mapData.mapName}");
        }
        
        /// <summary>
        /// Unload map hiện tại / Unload current map
        /// </summary>
        public IEnumerator UnloadMap(MapData mapData)
        {
            if (mapData == null)
            {
                yield break;
            }
            
            Debug.Log($"[MapLoader] Unloading map: {mapData.mapName}");
            
            // Cleanup map resources
            CleanupMapResources(mapData);
            
            // Unload scene nếu cần
            Scene scene = SceneManager.GetSceneByName(mapData.sceneName);
            if (scene.isLoaded)
            {
                AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(scene);
                if (unloadOp != null)
                {
                    yield return unloadOp;
                }
            }
            
            // Force garbage collection
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
            
            Debug.Log($"[MapLoader] Map unloaded: {mapData.mapName}");
        }
        
        /// <summary>
        /// Khởi tạo môi trường map / Initialize map environment
        /// </summary>
        private IEnumerator InitializeMapEnvironment(MapData mapData)
        {
            // Spawn NPCs
            foreach (var npcSpawn in mapData.npcSpawns)
            {
                SpawnNPC(npcSpawn);
            }
            
            // Setup portals
            foreach (var portalData in mapData.portals)
            {
                SetupPortal(portalData);
            }
            
            // Setup monster spawners
            SetupMonsterSpawners(mapData);
            
            // Setup boss spawner
            if (mapData.bossSpawnConfig != null && mapData.bossSpawnConfig.bossPrefab != null)
            {
                SetupBossSpawner(mapData.bossSpawnConfig);
            }
            
            // Setup environment
            SetupEnvironment(mapData);
            
            yield return null;
        }
        
        /// <summary>
        /// Spawn NPC / Spawn an NPC
        /// </summary>
        private void SpawnNPC(NPCSpawnData npcData)
        {
            if (npcData.npcPrefab == null)
            {
                Debug.LogWarning($"[MapLoader] NPC prefab is null for {npcData.npcName}");
                return;
            }
            
            GameObject npc = Instantiate(npcData.npcPrefab, npcData.position, Quaternion.Euler(npcData.rotation));
            npc.name = npcData.npcName;
            
            Debug.Log($"[MapLoader] Spawned NPC: {npcData.npcName} at {npcData.position}");
        }
        
        /// <summary>
        /// Setup portal / Setup a portal
        /// </summary>
        private void SetupPortal(PortalData portalData)
        {
            // TODO: Instantiate portal prefab
            Debug.Log($"[MapLoader] Setting up portal: {portalData.portalName}");
        }
        
        /// <summary>
        /// Setup monster spawners / Setup monster spawning system
        /// </summary>
        private void SetupMonsterSpawners(MapData mapData)
        {
            // Tạo spawner manager cho map này
            GameObject spawnerManager = new GameObject("MonsterSpawnerManager");
            
            foreach (var spawnConfig in mapData.spawnConfigs)
            {
                if (spawnConfig.monsterPrefab == null)
                {
                    Debug.LogWarning($"[MapLoader] Monster prefab is null for {spawnConfig.monsterName}");
                    continue;
                }
                
                // TODO: Create individual spawners
                Debug.Log($"[MapLoader] Setting up spawner for: {spawnConfig.monsterName}");
            }
        }
        
        /// <summary>
        /// Setup boss spawner / Setup boss spawning
        /// </summary>
        private void SetupBossSpawner(BossSpawnConfig bossConfig)
        {
            GameObject bossSpawnerObj = new GameObject($"BossSpawner_{bossConfig.bossName}");
            // TODO: Add BossSpawner component
            
            Debug.Log($"[MapLoader] Setting up boss spawner: {bossConfig.bossName}");
        }
        
        /// <summary>
        /// Setup environment / Setup environmental effects
        /// </summary>
        private void SetupEnvironment(MapData mapData)
        {
            // Setup weather
            if (mapData.defaultWeather != WeatherType.Clear)
            {
                // TODO: Activate weather system
                Debug.Log($"[MapLoader] Setting weather: {mapData.defaultWeather}");
            }
            
            // Setup day/night cycle
            if (mapData.hasDayNightCycle)
            {
                // TODO: Activate day/night system
                Debug.Log($"[MapLoader] Enabling day/night cycle");
            }
            
            // Setup ambient sound
            if (mapData.ambientSound != null)
            {
                // TODO: Play ambient sound
                Debug.Log($"[MapLoader] Playing ambient sound");
            }
        }
        
        /// <summary>
        /// Cleanup resources khi unload / Cleanup resources when unloading
        /// </summary>
        private void CleanupMapResources(MapData mapData)
        {
            // Destroy tất cả NPCs
            GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
            foreach (var npc in npcs)
            {
                Destroy(npc);
            }
            
            // Destroy tất cả monsters
            GameObject[] monsters = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var monster in monsters)
            {
                Destroy(monster);
            }
            
            // Destroy portals
            GameObject[] portals = GameObject.FindGameObjectsWithTag("Portal");
            foreach (var portal in portals)
            {
                Destroy(portal);
            }
            
            Debug.Log($"[MapLoader] Cleaned up resources for: {mapData.mapName}");
        }
    }
}
