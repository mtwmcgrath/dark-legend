using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace DarkLegend.Networking
{
    /// <summary>
    /// Quản lý rooms: tạo, join, leave / Manages rooms: create, join, leave
    /// </summary>
    public class RoomManager : MonoBehaviourPunCallbacks
    {
        public static RoomManager Instance { get; private set; }

        [Header("Room Settings")]
        [SerializeField] private byte maxPlayersPerRoom = 20;
        [SerializeField] private string defaultRoomName = "DarkLegend_Room";

        // Room property keys
        private const string PROP_MAP_NAME = "MapName";
        private const string PROP_DIFFICULTY = "Difficulty";
        private const string PROP_PVP_ENABLED = "PvPEnabled";

        // Player property keys
        private const string PROP_CHARACTER_CLASS = "CharacterClass";
        private const string PROP_LEVEL = "Level";
        private const string PROP_CHARACTER_NAME = "CharacterName";

        public delegate void OnRoomListUpdated(List<RoomInfo> roomList);
        public event OnRoomListUpdated RoomListUpdated;

        public delegate void OnRoomJoined(Room room);
        public event OnRoomJoined RoomJoinedEvent;

        public delegate void OnRoomLeft();
        public event OnRoomLeft RoomLeftEvent;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        #region Create Room

        /// <summary>
        /// Tạo room mới với options / Create new room with options
        /// </summary>
        public void CreateRoom(string roomName, string mapName, int difficulty, bool pvpEnabled)
        {
            if (!PhotonNetwork.IsConnectedAndReady)
            {
                Debug.LogError("[RoomManager] Not connected to Photon");
                return;
            }

            if (string.IsNullOrEmpty(roomName))
            {
                roomName = $"{defaultRoomName}_{Random.Range(1000, 9999)}";
            }

            RoomOptions roomOptions = new RoomOptions
            {
                MaxPlayers = maxPlayersPerRoom,
                IsVisible = true,
                IsOpen = true,
                CustomRoomProperties = new Hashtable
                {
                    { PROP_MAP_NAME, mapName },
                    { PROP_DIFFICULTY, difficulty },
                    { PROP_PVP_ENABLED, pvpEnabled }
                },
                CustomRoomPropertiesForLobby = new string[]
                {
                    PROP_MAP_NAME, PROP_DIFFICULTY, PROP_PVP_ENABLED
                }
            };

            Debug.Log($"[RoomManager] Creating room: {roomName}");
            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }

        /// <summary>
        /// Tạo room nhanh với settings mặc định / Create quick room with default settings
        /// </summary>
        public void CreateQuickRoom()
        {
            CreateRoom(null, "MainMap", 1, false);
        }

        #endregion

        #region Join Room

        /// <summary>
        /// Join room theo tên / Join room by name
        /// </summary>
        public void JoinRoom(string roomName)
        {
            if (!PhotonNetwork.IsConnectedAndReady)
            {
                Debug.LogError("[RoomManager] Not connected to Photon");
                return;
            }

            Debug.Log($"[RoomManager] Joining room: {roomName}");
            PhotonNetwork.JoinRoom(roomName);
        }

        /// <summary>
        /// Join room ngẫu nhiên / Join random room
        /// </summary>
        public void JoinRandomRoom()
        {
            if (!PhotonNetwork.IsConnectedAndReady)
            {
                Debug.LogError("[RoomManager] Not connected to Photon");
                return;
            }

            Debug.Log("[RoomManager] Joining random room...");
            PhotonNetwork.JoinRandomRoom();
        }

        /// <summary>
        /// Join hoặc tạo room / Join or create room
        /// </summary>
        public void JoinOrCreateRoom(string roomName, string mapName, int difficulty, bool pvpEnabled)
        {
            if (!PhotonNetwork.IsConnectedAndReady)
            {
                Debug.LogError("[RoomManager] Not connected to Photon");
                return;
            }

            RoomOptions roomOptions = new RoomOptions
            {
                MaxPlayers = maxPlayersPerRoom,
                IsVisible = true,
                IsOpen = true,
                CustomRoomProperties = new Hashtable
                {
                    { PROP_MAP_NAME, mapName },
                    { PROP_DIFFICULTY, difficulty },
                    { PROP_PVP_ENABLED, pvpEnabled }
                },
                CustomRoomPropertiesForLobby = new string[]
                {
                    PROP_MAP_NAME, PROP_DIFFICULTY, PROP_PVP_ENABLED
                }
            };

            Debug.Log($"[RoomManager] Joining or creating room: {roomName}");
            PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
        }

        #endregion

        #region Leave Room

        /// <summary>
        /// Rời khỏi room hiện tại / Leave current room
        /// </summary>
        public void LeaveRoom()
        {
            if (PhotonNetwork.InRoom)
            {
                Debug.Log("[RoomManager] Leaving room...");
                PhotonNetwork.LeaveRoom();
            }
        }

        #endregion

        #region Room Properties

        /// <summary>
        /// Lấy room properties / Get room properties
        /// </summary>
        public Hashtable GetRoomProperties()
        {
            if (PhotonNetwork.CurrentRoom != null)
            {
                return PhotonNetwork.CurrentRoom.CustomProperties;
            }
            return null;
        }

        /// <summary>
        /// Cập nhật room properties / Update room properties
        /// </summary>
        public void UpdateRoomProperties(Hashtable properties)
        {
            if (PhotonNetwork.CurrentRoom != null)
            {
                PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
            }
        }

        /// <summary>
        /// Lấy tên map của room / Get room map name
        /// </summary>
        public string GetRoomMapName()
        {
            if (PhotonNetwork.CurrentRoom != null &&
                PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(PROP_MAP_NAME))
            {
                return PhotonNetwork.CurrentRoom.CustomProperties[PROP_MAP_NAME].ToString();
            }
            return "Unknown";
        }

        /// <summary>
        /// Kiểm tra PvP có được bật không / Check if PvP is enabled
        /// </summary>
        public bool IsPvPEnabled()
        {
            if (PhotonNetwork.CurrentRoom != null &&
                PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(PROP_PVP_ENABLED))
            {
                return (bool)PhotonNetwork.CurrentRoom.CustomProperties[PROP_PVP_ENABLED];
            }
            return false;
        }

        #endregion

        #region Player Properties

        /// <summary>
        /// Đặt player properties / Set player properties
        /// </summary>
        public void SetPlayerProperties(string characterClass, int level, string characterName)
        {
            Hashtable properties = new Hashtable
            {
                { PROP_CHARACTER_CLASS, characterClass },
                { PROP_LEVEL, level },
                { PROP_CHARACTER_NAME, characterName }
            };

            PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
        }

        /// <summary>
        /// Lấy player properties / Get player properties
        /// </summary>
        public static string GetPlayerCharacterClass(Player player)
        {
            if (player.CustomProperties.ContainsKey(PROP_CHARACTER_CLASS))
            {
                return player.CustomProperties[PROP_CHARACTER_CLASS].ToString();
            }
            return "Unknown";
        }

        public static int GetPlayerLevel(Player player)
        {
            if (player.CustomProperties.ContainsKey(PROP_LEVEL))
            {
                return (int)player.CustomProperties[PROP_LEVEL];
            }
            return 1;
        }

        public static string GetPlayerCharacterName(Player player)
        {
            if (player.CustomProperties.ContainsKey(PROP_CHARACTER_NAME))
            {
                return player.CustomProperties[PROP_CHARACTER_NAME].ToString();
            }
            return player.NickName;
        }

        #endregion

        #region Photon Callbacks

        public override void OnJoinedRoom()
        {
            Debug.Log($"[RoomManager] Joined room: {PhotonNetwork.CurrentRoom.Name}");
            Debug.Log($"[RoomManager] Players in room: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}");
            RoomJoinedEvent?.Invoke(PhotonNetwork.CurrentRoom);
        }

        public override void OnLeftRoom()
        {
            Debug.Log("[RoomManager] Left room");
            RoomLeftEvent?.Invoke();
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogError($"[RoomManager] Create room failed. Code: {returnCode}, Message: {message}");
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.LogError($"[RoomManager] Join room failed. Code: {returnCode}, Message: {message}");
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.LogWarning($"[RoomManager] Join random room failed. Code: {returnCode}, Message: {message}");
            Debug.Log("[RoomManager] Creating new room instead...");
            CreateQuickRoom();
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log($"[RoomManager] Room list updated. Rooms: {roomList.Count}");
            RoomListUpdated?.Invoke(roomList);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log($"[RoomManager] Player joined: {newPlayer.NickName}");
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log($"[RoomManager] Player left: {otherPlayer.NickName}");
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            Debug.Log($"[RoomManager] Master client switched to: {newMasterClient.NickName}");
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Kiểm tra có phải Master Client không / Check if is Master Client
        /// </summary>
        public bool IsMasterClient()
        {
            return PhotonNetwork.IsMasterClient;
        }

        /// <summary>
        /// Lấy danh sách người chơi trong room / Get players in room
        /// </summary>
        public Player[] GetPlayersInRoom()
        {
            if (PhotonNetwork.CurrentRoom != null)
            {
                return PhotonNetwork.CurrentRoom.Players.Values.ToArray();
            }
            return new Player[0];
        }

        /// <summary>
        /// Lấy số lượng người chơi trong room / Get player count in room
        /// </summary>
        public int GetPlayerCount()
        {
            if (PhotonNetwork.CurrentRoom != null)
            {
                return PhotonNetwork.CurrentRoom.PlayerCount;
            }
            return 0;
        }

        #endregion
    }

    /// <summary>
    /// Extension methods cho Player class
    /// </summary>
    public static class PlayerExtensions
    {
        public static T[] ToArray<T>(this Dictionary<int, T>.ValueCollection collection)
        {
            T[] array = new T[collection.Count];
            collection.CopyTo(array, 0);
            return array;
        }
    }
}
