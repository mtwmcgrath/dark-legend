using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DarkLegend.Networking.UI
{
    /// <summary>
    /// UI cho màn hình đăng nhập / UI for login screen
    /// </summary>
    public class LoginUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_InputField playerNameInput;
        [SerializeField] private TMP_Dropdown regionDropdown;
        [SerializeField] private Button connectButton;
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private GameObject loadingPanel;

        [Header("Settings")]
        [SerializeField] private string defaultPlayerName = "Player";
        [SerializeField] private string[] regions = { "asia", "us", "eu", "jp", "au" };

        private PhotonLauncher photonLauncher;

        private void Start()
        {
            // Lấy PhotonLauncher / Get PhotonLauncher
            photonLauncher = FindObjectOfType<PhotonLauncher>();
            if (photonLauncher == null)
            {
                GameObject launcherObj = new GameObject("PhotonLauncher");
                photonLauncher = launcherObj.AddComponent<PhotonLauncher>();
            }

            // Setup UI / Thiết lập UI
            SetupUI();

            // Subscribe to events / Đăng ký events
            if (photonLauncher != null)
            {
                photonLauncher.ConnectionStatusChanged += OnConnectionStatusChanged;
                photonLauncher.LobbyJoined += OnLobbyJoined;
            }

            // Load saved player name / Tải tên người chơi đã lưu
            LoadPlayerName();
        }

        private void OnDestroy()
        {
            // Unsubscribe events / Hủy đăng ký events
            if (photonLauncher != null)
            {
                photonLauncher.ConnectionStatusChanged -= OnConnectionStatusChanged;
                photonLauncher.LobbyJoined -= OnLobbyJoined;
            }
        }

        private void SetupUI()
        {
            // Setup region dropdown / Thiết lập dropdown region
            if (regionDropdown != null)
            {
                regionDropdown.ClearOptions();
                regionDropdown.AddOptions(new System.Collections.Generic.List<string>(regions));
            }

            // Setup connect button / Thiết lập nút connect
            if (connectButton != null)
            {
                connectButton.onClick.AddListener(OnConnectButtonClicked);
            }

            // Ẩn loading panel / Hide loading panel
            if (loadingPanel != null)
            {
                loadingPanel.SetActive(false);
            }

            UpdateStatusText("Enter your name and connect");
        }

        #region Button Handlers

        private void OnConnectButtonClicked()
        {
            string playerName = playerNameInput != null ? playerNameInput.text : defaultPlayerName;
            
            if (string.IsNullOrEmpty(playerName))
            {
                UpdateStatusText("Please enter a player name");
                return;
            }

            // Lưu player name / Save player name
            SavePlayerName(playerName);

            // Lấy region / Get region
            string selectedRegion = regions[regionDropdown != null ? regionDropdown.value : 0];

            // Kết nối / Connect
            UpdateStatusText($"Connecting to {selectedRegion}...");
            ShowLoading(true);

            if (photonLauncher != null)
            {
                photonLauncher.Login(playerName);
                photonLauncher.ConnectToRegion(selectedRegion);
            }
        }

        #endregion

        #region Photon Callbacks

        private void OnConnectionStatusChanged(bool connected)
        {
            if (connected)
            {
                UpdateStatusText("Connected! Joining lobby...");
            }
            else
            {
                UpdateStatusText("Disconnected. Please try again.");
                ShowLoading(false);
            }
        }

        private void OnLobbyJoined()
        {
            UpdateStatusText("Successfully joined lobby!");
            ShowLoading(false);

            // Chuyển sang lobby UI / Switch to lobby UI
            // TODO: Load lobby scene or show lobby panel
            Debug.Log("[LoginUI] Joined lobby, switching to lobby UI");
        }

        #endregion

        #region UI Updates

        private void UpdateStatusText(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
            Debug.Log($"[LoginUI] {message}");
        }

        private void ShowLoading(bool show)
        {
            if (loadingPanel != null)
            {
                loadingPanel.SetActive(show);
            }

            if (connectButton != null)
            {
                connectButton.interactable = !show;
            }
        }

        #endregion

        #region Save/Load

        private void SavePlayerName(string playerName)
        {
            PlayerPrefs.SetString("PlayerName", playerName);
            PlayerPrefs.Save();
        }

        private void LoadPlayerName()
        {
            if (PlayerPrefs.HasKey("PlayerName"))
            {
                string savedName = PlayerPrefs.GetString("PlayerName");
                if (playerNameInput != null)
                {
                    playerNameInput.text = savedName;
                }
            }
        }

        #endregion
    }
}
