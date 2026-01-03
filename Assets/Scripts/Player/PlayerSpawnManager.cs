using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

namespace DarkLegend.Networking
{
    /// <summary>
    /// Quản lý spawn players khi join room / Manages player spawning when joining room
    /// </summary>
    public class PlayerSpawnManager : MonoBehaviour
    {
        public static PlayerSpawnManager Instance { get; private set; }

        [Header("Spawn Settings")]
        [SerializeField] private string playerPrefabName = "NetworkPlayer";
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private float spawnRadius = 2f;

        [Header("Default Spawn Point")]
        [SerializeField] private Vector3 defaultSpawnPosition = Vector3.zero;
        [SerializeField] private Quaternion defaultSpawnRotation = Quaternion.identity;

        private List<GameObject> spawnedPlayers = new List<GameObject>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            // Tự động spawn player khi vào room / Auto spawn player when entering room
            if (PhotonNetwork.InRoom)
            {
                SpawnLocalPlayer();
            }
        }

        #region Spawn Player

        /// <summary>
        /// Spawn local player / Spawn người chơi local
        /// </summary>
        public GameObject SpawnLocalPlayer()
        {
            if (!PhotonNetwork.IsConnectedAndReady)
            {
                Debug.LogError("[PlayerSpawnManager] Not connected to Photon");
                return null;
            }

            Vector3 spawnPosition = GetSpawnPosition();
            Quaternion spawnRotation = GetSpawnRotation();

            Debug.Log($"[PlayerSpawnManager] Spawning player at {spawnPosition}");

            GameObject player = PhotonNetwork.Instantiate(
                playerPrefabName, 
                spawnPosition, 
                spawnRotation
            );

            if (player != null)
            {
                spawnedPlayers.Add(player);
                OnPlayerSpawned(player);
            }

            return player;
        }

        /// <summary>
        /// Spawn player tại vị trí cụ thể / Spawn player at specific position
        /// </summary>
        public GameObject SpawnPlayerAt(Vector3 position, Quaternion rotation)
        {
            if (!PhotonNetwork.IsConnectedAndReady)
            {
                Debug.LogError("[PlayerSpawnManager] Not connected to Photon");
                return null;
            }

            Debug.Log($"[PlayerSpawnManager] Spawning player at custom position: {position}");

            GameObject player = PhotonNetwork.Instantiate(
                playerPrefabName, 
                position, 
                rotation
            );

            if (player != null)
            {
                spawnedPlayers.Add(player);
                OnPlayerSpawned(player);
            }

            return player;
        }

        #endregion

        #region Spawn Position

        /// <summary>
        /// Lấy spawn position / Get spawn position
        /// </summary>
        private Vector3 GetSpawnPosition()
        {
            // Nếu có spawn points, chọn ngẫu nhiên / If has spawn points, choose randomly
            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                int index = Random.Range(0, spawnPoints.Length);
                Transform spawnPoint = spawnPoints[index];

                if (spawnPoint != null)
                {
                    // Thêm random offset / Add random offset
                    Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
                    randomOffset.y = 0; // Giữ cùng độ cao / Keep same height
                    
                    return spawnPoint.position + randomOffset;
                }
            }

            // Nếu không có spawn points, dùng default position / If no spawn points, use default position
            Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
            randomOffset.y = 0;
            
            return defaultSpawnPosition + randomOffset;
        }

        /// <summary>
        /// Lấy spawn rotation / Get spawn rotation
        /// </summary>
        private Quaternion GetSpawnRotation()
        {
            // Nếu có spawn points, dùng rotation của spawn point / If has spawn points, use spawn point rotation
            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                int index = Random.Range(0, spawnPoints.Length);
                Transform spawnPoint = spawnPoints[index];

                if (spawnPoint != null)
                {
                    return spawnPoint.rotation;
                }
            }

            return defaultSpawnRotation;
        }

        #endregion

        #region Spawn Management

        /// <summary>
        /// Callback khi player spawn / Callback when player spawns
        /// </summary>
        private void OnPlayerSpawned(GameObject player)
        {
            Debug.Log($"[PlayerSpawnManager] Player spawned: {player.name}");

            // Setup player components / Thiết lập player components
            PlayerPhotonView playerView = player.GetComponent<PlayerPhotonView>();
            if (playerView != null && playerView.IsLocalPlayer())
            {
                // Setup local player specific things / Thiết lập các thứ cụ thể cho local player
                Debug.Log("[PlayerSpawnManager] Local player spawned");
            }
        }

        /// <summary>
        /// Despawn player / Hủy spawn player
        /// </summary>
        public void DespawnPlayer(GameObject player)
        {
            if (player == null) return;

            PhotonView photonView = player.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                spawnedPlayers.Remove(player);
                PhotonNetwork.Destroy(player);
                
                Debug.Log("[PlayerSpawnManager] Player despawned");
            }
        }

        /// <summary>
        /// Despawn tất cả players / Despawn all players
        /// </summary>
        public void DespawnAllPlayers()
        {
            foreach (GameObject player in spawnedPlayers)
            {
                if (player != null)
                {
                    PhotonView photonView = player.GetComponent<PhotonView>();
                    if (photonView != null && photonView.IsMine)
                    {
                        PhotonNetwork.Destroy(player);
                    }
                }
            }
            
            spawnedPlayers.Clear();
            Debug.Log("[PlayerSpawnManager] All players despawned");
        }

        #endregion

        #region Respawn

        /// <summary>
        /// Respawn player / Hồi sinh player
        /// </summary>
        public void RespawnPlayer(GameObject player)
        {
            if (player == null) return;

            Vector3 spawnPosition = GetSpawnPosition();
            Quaternion spawnRotation = GetSpawnRotation();

            player.transform.position = spawnPosition;
            player.transform.rotation = spawnRotation;

            Debug.Log($"[PlayerSpawnManager] Player respawned at {spawnPosition}");
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Đăng ký spawn point / Register spawn point
        /// </summary>
        public void RegisterSpawnPoint(Transform spawnPoint)
        {
            if (spawnPoint == null) return;

            List<Transform> spawnPointsList = new List<Transform>();
            if (spawnPoints != null)
            {
                spawnPointsList.AddRange(spawnPoints);
            }

            if (!spawnPointsList.Contains(spawnPoint))
            {
                spawnPointsList.Add(spawnPoint);
                spawnPoints = spawnPointsList.ToArray();
                
                Debug.Log($"[PlayerSpawnManager] Registered spawn point: {spawnPoint.name}");
            }
        }

        /// <summary>
        /// Hủy đăng ký spawn point / Unregister spawn point
        /// </summary>
        public void UnregisterSpawnPoint(Transform spawnPoint)
        {
            if (spawnPoint == null || spawnPoints == null) return;

            List<Transform> spawnPointsList = new List<Transform>(spawnPoints);
            if (spawnPointsList.Remove(spawnPoint))
            {
                spawnPoints = spawnPointsList.ToArray();
                Debug.Log($"[PlayerSpawnManager] Unregistered spawn point: {spawnPoint.name}");
            }
        }

        /// <summary>
        /// Đặt player prefab name / Set player prefab name
        /// </summary>
        public void SetPlayerPrefabName(string prefabName)
        {
            playerPrefabName = prefabName;
        }

        /// <summary>
        /// Lấy danh sách spawned players / Get spawned players list
        /// </summary>
        public List<GameObject> GetSpawnedPlayers()
        {
            return new List<GameObject>(spawnedPlayers);
        }

        #endregion
    }
}
