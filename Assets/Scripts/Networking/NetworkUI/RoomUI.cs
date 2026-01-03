using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

namespace DarkLegend.Networking.UI
{
    /// <summary>
    /// UI cho room / UI for room
    /// </summary>
    public class RoomUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI roomNameText;
        [SerializeField] private TextMeshProUGUI mapNameText;
        [SerializeField] private TextMeshProUGUI playerCountText;
        [SerializeField] private Transform playerListContent;
        [SerializeField] private GameObject playerListItemPrefab;
        [SerializeField] private Button leaveRoomButton;
        [SerializeField] private Button startGameButton;
        [SerializeField] private TextMeshProUGUI statusText;

        private RoomManager roomManager;
        private System.Collections.Generic.List<GameObject> playerListItems = 
            new System.Collections.Generic.List<GameObject>();

        private void Start()
        {
            // Lấy RoomManager / Get RoomManager
            roomManager = RoomManager.Instance;

            // Setup UI / Thiết lập UI
            SetupUI();

            // Cập nhật room info / Update room info
            UpdateRoomInfo();
            UpdatePlayerList();
        }

        private void SetupUI()
        {
            // Setup buttons / Thiết lập buttons
            if (leaveRoomButton != null)
                leaveRoomButton.onClick.AddListener(OnLeaveRoomButtonClicked);

            if (startGameButton != null)
            {
                startGameButton.onClick.AddListener(OnStartGameButtonClicked);
                // Chỉ Master Client mới có thể start game / Only Master Client can start game
                startGameButton.interactable = PhotonNetwork.IsMasterClient;
            }
        }

        private void Update()
        {
            // Cập nhật start button / Update start button
            if (startGameButton != null && PhotonNetwork.InRoom)
            {
                startGameButton.interactable = PhotonNetwork.IsMasterClient;
            }
        }

        #region Button Handlers

        private void OnLeaveRoomButtonClicked()
        {
            if (roomManager != null)
            {
                roomManager.LeaveRoom();
            }
        }

        private void OnStartGameButtonClicked()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            // Load game scene / Tải scene game
            UpdateStatusText("Starting game...");
            
            // TODO: Load game scene
            // PhotonNetwork.LoadLevel("GameScene");
            
            Debug.Log("[RoomUI] Starting game...");
        }

        #endregion

        #region Room Info

        private void UpdateRoomInfo()
        {
            if (!PhotonNetwork.InRoom) return;

            // Room name / Tên room
            if (roomNameText != null)
                roomNameText.text = PhotonNetwork.CurrentRoom.Name;

            // Map name / Tên map
            if (mapNameText != null && roomManager != null)
                mapNameText.text = $"Map: {roomManager.GetRoomMapName()}";

            // Player count / Số lượng người chơi
            if (playerCountText != null)
                playerCountText.text = $"Players: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
        }

        #endregion

        #region Player List

        private void UpdatePlayerList()
        {
            // Xóa player list cũ / Clear old player list
            ClearPlayerList();

            // Tạo player list items mới / Create new player list items
            if (PhotonNetwork.InRoom)
            {
                foreach (var player in PhotonNetwork.PlayerList)
                {
                    CreatePlayerListItem(player);
                }
            }
        }

        private void CreatePlayerListItem(Photon.Realtime.Player player)
        {
            if (playerListItemPrefab == null || playerListContent == null) return;

            GameObject item = Instantiate(playerListItemPrefab, playerListContent);
            playerListItems.Add(item);

            // Setup item data / Thiết lập dữ liệu item
            TextMeshProUGUI playerNameText = item.transform.Find("PlayerName")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI playerLevelText = item.transform.Find("PlayerLevel")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI playerClassText = item.transform.Find("PlayerClass")?.GetComponent<TextMeshProUGUI>();
            Image masterIcon = item.transform.Find("MasterIcon")?.GetComponent<Image>();

            if (playerNameText != null)
                playerNameText.text = player.NickName;

            if (playerLevelText != null)
            {
                int level = RoomManager.GetPlayerLevel(player);
                playerLevelText.text = $"Lv.{level}";
            }

            if (playerClassText != null)
            {
                string charClass = RoomManager.GetPlayerCharacterClass(player);
                playerClassText.text = charClass;
            }

            if (masterIcon != null)
                masterIcon.gameObject.SetActive(player.IsMasterClient);
        }

        private void ClearPlayerList()
        {
            foreach (GameObject item in playerListItems)
            {
                Destroy(item);
            }
            playerListItems.Clear();
        }

        #endregion

        #region Photon Callbacks

        public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            UpdateRoomInfo();
            UpdatePlayerList();
            UpdateStatusText($"{newPlayer.NickName} joined the room");
        }

        public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            UpdateRoomInfo();
            UpdatePlayerList();
            UpdateStatusText($"{otherPlayer.NickName} left the room");
        }

        public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
        {
            UpdatePlayerList();
            UpdateStatusText($"{newMasterClient.NickName} is now the host");
        }

        #endregion

        #region UI Updates

        private void UpdateStatusText(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
            Debug.Log($"[RoomUI] {message}");
        }

        #endregion
    }
}
