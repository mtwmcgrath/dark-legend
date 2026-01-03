using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

namespace DarkLegend.Networking.UI
{
    /// <summary>
    /// UI cho lobby / UI for lobby
    /// </summary>
    public class LobbyUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform roomListContent;
        [SerializeField] private GameObject roomListItemPrefab;
        [SerializeField] private Button createRoomButton;
        [SerializeField] private Button quickJoinButton;
        [SerializeField] private Button refreshButton;
        [SerializeField] private TextMeshProUGUI playerCountText;

        [Header("Create Room Panel")]
        [SerializeField] private GameObject createRoomPanel;
        [SerializeField] private TMP_InputField roomNameInput;
        [SerializeField] private TMP_Dropdown mapDropdown;
        [SerializeField] private TMP_Dropdown difficultyDropdown;
        [SerializeField] private Toggle pvpToggle;
        [SerializeField] private Button confirmCreateButton;
        [SerializeField] private Button cancelCreateButton;

        private RoomManager roomManager;
        private List<GameObject> roomListItems = new List<GameObject>();

        private void Start()
        {
            // Lấy RoomManager / Get RoomManager
            roomManager = RoomManager.Instance;
            if (roomManager == null)
            {
                GameObject managerObj = new GameObject("RoomManager");
                roomManager = managerObj.AddComponent<RoomManager>();
            }

            // Setup UI / Thiết lập UI
            SetupUI();

            // Subscribe to events / Đăng ký events
            if (roomManager != null)
            {
                roomManager.RoomListUpdated += OnRoomListUpdated;
                roomManager.RoomJoinedEvent += OnRoomJoined;
            }

            // Ẩn create room panel / Hide create room panel
            if (createRoomPanel != null)
            {
                createRoomPanel.SetActive(false);
            }

            // Cập nhật player count / Update player count
            UpdatePlayerCount();
        }

        private void OnDestroy()
        {
            // Unsubscribe events / Hủy đăng ký events
            if (roomManager != null)
            {
                roomManager.RoomListUpdated -= OnRoomListUpdated;
                roomManager.RoomJoinedEvent -= OnRoomJoined;
            }
        }

        private void SetupUI()
        {
            // Setup buttons / Thiết lập buttons
            if (createRoomButton != null)
                createRoomButton.onClick.AddListener(OnCreateRoomButtonClicked);

            if (quickJoinButton != null)
                quickJoinButton.onClick.AddListener(OnQuickJoinButtonClicked);

            if (refreshButton != null)
                refreshButton.onClick.AddListener(OnRefreshButtonClicked);

            if (confirmCreateButton != null)
                confirmCreateButton.onClick.AddListener(OnConfirmCreateRoomClicked);

            if (cancelCreateButton != null)
                cancelCreateButton.onClick.AddListener(OnCancelCreateRoomClicked);

            // Setup dropdowns / Thiết lập dropdowns
            if (mapDropdown != null)
            {
                mapDropdown.ClearOptions();
                mapDropdown.AddOptions(new List<string> { "MainMap", "DarkForest", "CastleRuins", "Arena" });
            }

            if (difficultyDropdown != null)
            {
                difficultyDropdown.ClearOptions();
                difficultyDropdown.AddOptions(new List<string> { "Easy", "Normal", "Hard", "Nightmare" });
            }
        }

        #region Button Handlers

        private void OnCreateRoomButtonClicked()
        {
            if (createRoomPanel != null)
            {
                createRoomPanel.SetActive(true);
            }
        }

        private void OnQuickJoinButtonClicked()
        {
            if (roomManager != null)
            {
                roomManager.JoinRandomRoom();
            }
        }

        private void OnRefreshButtonClicked()
        {
            // Room list tự động cập nhật / Room list auto updates
            Debug.Log("[LobbyUI] Refreshing room list...");
        }

        private void OnConfirmCreateRoomClicked()
        {
            string roomName = roomNameInput != null ? roomNameInput.text : "";
            string mapName = mapDropdown != null ? mapDropdown.options[mapDropdown.value].text : "MainMap";
            int difficulty = difficultyDropdown != null ? difficultyDropdown.value + 1 : 1;
            bool pvpEnabled = pvpToggle != null && pvpToggle.isOn;

            if (string.IsNullOrEmpty(roomName))
            {
                roomName = $"Room_{Random.Range(1000, 9999)}";
            }

            if (roomManager != null)
            {
                roomManager.CreateRoom(roomName, mapName, difficulty, pvpEnabled);
            }

            if (createRoomPanel != null)
            {
                createRoomPanel.SetActive(false);
            }
        }

        private void OnCancelCreateRoomClicked()
        {
            if (createRoomPanel != null)
            {
                createRoomPanel.SetActive(false);
            }
        }

        private void OnRoomListItemClicked(string roomName)
        {
            if (roomManager != null)
            {
                roomManager.JoinRoom(roomName);
            }
        }

        #endregion

        #region Room List

        private void OnRoomListUpdated(List<RoomInfo> roomList)
        {
            // Xóa room list cũ / Clear old room list
            ClearRoomList();

            // Tạo room list items mới / Create new room list items
            foreach (RoomInfo room in roomList)
            {
                if (room.RemovedFromList) continue;

                CreateRoomListItem(room);
            }
        }

        private void CreateRoomListItem(RoomInfo room)
        {
            if (roomListItemPrefab == null || roomListContent == null) return;

            GameObject item = Instantiate(roomListItemPrefab, roomListContent);
            roomListItems.Add(item);

            // Setup item data / Thiết lập dữ liệu item
            TextMeshProUGUI roomNameText = item.transform.Find("RoomName")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI playerCountText = item.transform.Find("PlayerCount")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI mapNameText = item.transform.Find("MapName")?.GetComponent<TextMeshProUGUI>();
            Button joinButton = item.transform.Find("JoinButton")?.GetComponent<Button>();

            if (roomNameText != null)
                roomNameText.text = room.Name;

            if (playerCountText != null)
                playerCountText.text = $"{room.PlayerCount}/{room.MaxPlayers}";

            if (mapNameText != null && room.CustomProperties.ContainsKey("MapName"))
                mapNameText.text = room.CustomProperties["MapName"].ToString();

            if (joinButton != null)
                joinButton.onClick.AddListener(() => OnRoomListItemClicked(room.Name));
        }

        private void ClearRoomList()
        {
            foreach (GameObject item in roomListItems)
            {
                Destroy(item);
            }
            roomListItems.Clear();
        }

        #endregion

        #region Callbacks

        private void OnRoomJoined(Room room)
        {
            Debug.Log($"[LobbyUI] Joined room: {room.Name}");
            // TODO: Load room scene or show room panel
        }

        #endregion

        #region UI Updates

        private void UpdatePlayerCount()
        {
            if (playerCountText != null)
            {
                int onlinePlayers = PhotonNetwork.CountOfPlayers;
                int roomCount = PhotonNetwork.CountOfRooms;
                playerCountText.text = $"Online: {onlinePlayers} | Rooms: {roomCount}";
            }

            // Cập nhật mỗi 5 giây / Update every 5 seconds
            Invoke(nameof(UpdatePlayerCount), 5f);
        }

        #endregion
    }
}
