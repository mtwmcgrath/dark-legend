using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace DarkLegend.Networking
{
    /// <summary>
    /// Quản lý kết nối Photon server / Manages Photon server connection
    /// Xử lý callbacks và connection state / Handles callbacks and connection state
    /// </summary>
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        public static NetworkManager Instance { get; private set; }

        [Header("Connection Settings")]
        [SerializeField] private string gameVersion = "1.0";
        [SerializeField] private int maxReconnectAttempts = 3;
        [SerializeField] private float reconnectDelay = 5f;

        private int reconnectAttempts = 0;
        private bool isReconnecting = false;

        public bool IsConnected => PhotonNetwork.IsConnected;
        public bool IsConnectedAndReady => PhotonNetwork.IsConnectedAndReady;
        public string PlayerName
        {
            get => PhotonNetwork.NickName;
            set => PhotonNetwork.NickName = value;
        }

        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Cấu hình Photon / Configure Photon
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = gameVersion;
        }

        /// <summary>
        /// Kết nối đến Photon Master Server / Connect to Photon Master Server
        /// </summary>
        public void ConnectToPhoton()
        {
            if (PhotonNetwork.IsConnected)
            {
                Debug.Log("[NetworkManager] Already connected to Photon");
                return;
            }

            Debug.Log("[NetworkManager] Connecting to Photon...");
            PhotonNetwork.ConnectUsingSettings();
        }

        /// <summary>
        /// Ngắt kết nối khỏi Photon / Disconnect from Photon
        /// </summary>
        public void DisconnectFromPhoton()
        {
            if (PhotonNetwork.IsConnected)
            {
                Debug.Log("[NetworkManager] Disconnecting from Photon...");
                PhotonNetwork.Disconnect();
            }
        }

        #region Photon Callbacks

        public override void OnConnectedToMaster()
        {
            Debug.Log("[NetworkManager] Connected to Master Server");
            reconnectAttempts = 0;
            isReconnecting = false;

            // Tự động join lobby / Auto join lobby
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarning($"[NetworkManager] Disconnected from Photon. Cause: {cause}");

            // Auto reconnect nếu không phải do người dùng ngắt kết nối
            // Auto reconnect if not disconnected by user
            if (cause != DisconnectCause.DisconnectByClientLogic && !isReconnecting)
            {
                TryReconnect();
            }
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("[NetworkManager] Joined Lobby");
        }

        public override void OnLeftLobby()
        {
            Debug.Log("[NetworkManager] Left Lobby");
        }

        public override void OnJoinedRoom()
        {
            Debug.Log($"[NetworkManager] Joined Room: {PhotonNetwork.CurrentRoom.Name}");
        }

        public override void OnLeftRoom()
        {
            Debug.Log("[NetworkManager] Left Room");
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log($"[NetworkManager] Player {newPlayer.NickName} entered room");
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log($"[NetworkManager] Player {otherPlayer.NickName} left room");
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.LogError($"[NetworkManager] Join Room Failed. Code: {returnCode}, Message: {message}");
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogError($"[NetworkManager] Create Room Failed. Code: {returnCode}, Message: {message}");
        }

        #endregion

        #region Auto Reconnect

        /// <summary>
        /// Thử kết nối lại / Try to reconnect
        /// </summary>
        private void TryReconnect()
        {
            if (reconnectAttempts >= maxReconnectAttempts)
            {
                Debug.LogError("[NetworkManager] Max reconnect attempts reached. Please restart the game.");
                isReconnecting = false;
                return;
            }

            reconnectAttempts++;
            isReconnecting = true;
            Debug.Log($"[NetworkManager] Attempting to reconnect... ({reconnectAttempts}/{maxReconnectAttempts})");

            Invoke(nameof(ConnectToPhoton), reconnectDelay);
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Lấy ping hiện tại / Get current ping
        /// </summary>
        public int GetPing()
        {
            return PhotonNetwork.GetPing();
        }

        /// <summary>
        /// Lấy số lượng người chơi online / Get online player count
        /// </summary>
        public int GetOnlinePlayerCount()
        {
            return PhotonNetwork.CountOfPlayers;
        }

        /// <summary>
        /// Lấy số lượng room hiện có / Get current room count
        /// </summary>
        public int GetRoomCount()
        {
            return PhotonNetwork.CountOfRooms;
        }

        #endregion
    }
}
