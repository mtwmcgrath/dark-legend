using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace DarkLegend.Networking
{
    /// <summary>
    /// Launcher để connect, login và join lobby / Launcher for connecting, login and joining lobby
    /// </summary>
    public class PhotonLauncher : MonoBehaviourPunCallbacks
    {
        [Header("Connection Settings")]
        [SerializeField] private string defaultPlayerName = "Player";
        [SerializeField] private byte maxPlayersPerRoom = 20;

        [Header("Region Settings")]
        [SerializeField] private string preferredRegion = "asia"; // asia, us, eu, etc.

        private bool isConnecting = false;

        public delegate void OnConnectionStatusChanged(bool connected);
        public event OnConnectionStatusChanged ConnectionStatusChanged;

        public delegate void OnLobbyJoined();
        public event OnLobbyJoined LobbyJoined;

        private void Start()
        {
            // Đặt tên người chơi mặc định / Set default player name
            if (string.IsNullOrEmpty(PhotonNetwork.NickName))
            {
                PhotonNetwork.NickName = defaultPlayerName;
            }
        }

        /// <summary>
        /// Kết nối đến Photon với settings / Connect to Photon with settings
        /// </summary>
        public void Connect()
        {
            if (isConnecting || PhotonNetwork.IsConnected)
            {
                Debug.Log("[PhotonLauncher] Already connecting or connected");
                return;
            }

            isConnecting = true;
            Debug.Log("[PhotonLauncher] Connecting to Photon Cloud...");

            // Cấu hình settings / Configure settings
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = Application.version;

            // Kết nối / Connect
            PhotonNetwork.ConnectUsingSettings();
        }

        /// <summary>
        /// Kết nối đến region cụ thể / Connect to specific region
        /// </summary>
        public void ConnectToRegion(string region)
        {
            if (isConnecting || PhotonNetwork.IsConnected)
            {
                Debug.Log("[PhotonLauncher] Already connecting or connected");
                return;
            }

            preferredRegion = region;
            PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = region;
            Connect();
        }

        /// <summary>
        /// Đăng nhập với tên người chơi / Login with player name
        /// </summary>
        public void Login(string playerName)
        {
            if (string.IsNullOrEmpty(playerName))
            {
                Debug.LogWarning("[PhotonLauncher] Player name is empty, using default");
                playerName = defaultPlayerName;
            }

            PhotonNetwork.NickName = playerName;
            Debug.Log($"[PhotonLauncher] Logged in as: {playerName}");

            Connect();
        }

        /// <summary>
        /// Ngắt kết nối / Disconnect
        /// </summary>
        public void Disconnect()
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();
            }
            isConnecting = false;
        }

        #region Photon Callbacks

        public override void OnConnectedToMaster()
        {
            Debug.Log("[PhotonLauncher] Connected to Master Server");
            isConnecting = false;
            ConnectionStatusChanged?.Invoke(true);

            // Tự động join lobby / Auto join lobby
            PhotonNetwork.JoinLobby();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarning($"[PhotonLauncher] Disconnected. Cause: {cause}");
            isConnecting = false;
            ConnectionStatusChanged?.Invoke(false);
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("[PhotonLauncher] Joined Lobby");
            LobbyJoined?.Invoke();
        }

        public override void OnConnectFailed(DisconnectCause cause)
        {
            Debug.LogError($"[PhotonLauncher] Connection Failed. Cause: {cause}");
            isConnecting = false;
            ConnectionStatusChanged?.Invoke(false);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Kiểm tra trạng thái kết nối / Check connection status
        /// </summary>
        public bool IsConnectedToPhoton()
        {
            return PhotonNetwork.IsConnectedAndReady;
        }

        /// <summary>
        /// Lấy tên người chơi hiện tại / Get current player name
        /// </summary>
        public string GetPlayerName()
        {
            return PhotonNetwork.NickName;
        }

        /// <summary>
        /// Đặt tên người chơi / Set player name
        /// </summary>
        public void SetPlayerName(string name)
        {
            PhotonNetwork.NickName = name;
        }

        #endregion
    }
}
