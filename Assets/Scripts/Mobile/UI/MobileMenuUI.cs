using UnityEngine;
using UnityEngine.UI;

namespace DarkLegend.Mobile.UI
{
    /// <summary>
    /// Mobile menu UI
    /// UI menu cho mobile
    /// </summary>
    public class MobileMenuUI : MonoBehaviour
    {
        [Header("Menu Panels")]
        public GameObject menuPanel;
        public GameObject settingsPanel;
        public GameObject characterPanel;
        public GameObject socialPanel;

        [Header("Menu Buttons")]
        public Button settingsButton;
        public Button characterButton;
        public Button socialButton;
        public Button quitButton;
        public Button closeButton;

        [Header("Settings")]
        public Slider soundSlider;
        public Slider musicSlider;
        public Toggle hapticToggle;
        public Dropdown qualityDropdown;

        private bool isOpen = false;
        private GameObject currentPanel;

        private void Start()
        {
            InitializeMenu();
            Close();
        }

        /// <summary>
        /// Initialize menu
        /// Khởi tạo menu
        /// </summary>
        private void InitializeMenu()
        {
            // Setup button listeners
            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(() => OpenPanel(settingsPanel));
            }

            if (characterButton != null)
            {
                characterButton.onClick.AddListener(() => OpenPanel(characterPanel));
            }

            if (socialButton != null)
            {
                socialButton.onClick.AddListener(() => OpenPanel(socialPanel));
            }

            if (quitButton != null)
            {
                quitButton.onClick.AddListener(OnQuitGame);
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Close);
            }

            // Setup settings listeners
            if (soundSlider != null)
            {
                soundSlider.onValueChanged.AddListener(OnSoundVolumeChanged);
            }

            if (musicSlider != null)
            {
                musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            }

            if (hapticToggle != null)
            {
                hapticToggle.onValueChanged.AddListener(OnHapticToggled);
            }

            if (qualityDropdown != null)
            {
                qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
            }

            Debug.Log("[MobileMenuUI] Menu initialized");
        }

        /// <summary>
        /// Toggle menu
        /// Bật/tắt menu
        /// </summary>
        public void Toggle()
        {
            if (isOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
        }

        /// <summary>
        /// Open menu
        /// Mở menu
        /// </summary>
        public void Open()
        {
            isOpen = true;
            if (menuPanel != null)
            {
                menuPanel.SetActive(true);
            }
            
            // Pause game
            Time.timeScale = 0f;
            
            Debug.Log("[MobileMenuUI] Menu opened");
        }

        /// <summary>
        /// Close menu
        /// Đóng menu
        /// </summary>
        public void Close()
        {
            isOpen = false;
            
            if (menuPanel != null)
            {
                menuPanel.SetActive(false);
            }

            CloseAllPanels();
            
            // Resume game
            Time.timeScale = 1f;
            
            Debug.Log("[MobileMenuUI] Menu closed");
        }

        /// <summary>
        /// Open specific panel
        /// Mở panel cụ thể
        /// </summary>
        private void OpenPanel(GameObject panel)
        {
            CloseAllPanels();

            if (panel != null)
            {
                panel.SetActive(true);
                currentPanel = panel;
            }
        }

        /// <summary>
        /// Close all panels
        /// Đóng tất cả panel
        /// </summary>
        private void CloseAllPanels()
        {
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (characterPanel != null) characterPanel.SetActive(false);
            if (socialPanel != null) socialPanel.SetActive(false);
            currentPanel = null;
        }

        /// <summary>
        /// Sound volume changed
        /// Âm lượng sound thay đổi
        /// </summary>
        private void OnSoundVolumeChanged(float value)
        {
            AudioListener.volume = value;
            Debug.Log($"[MobileMenuUI] Sound volume: {value}");
        }

        /// <summary>
        /// Music volume changed
        /// Âm lượng music thay đổi
        /// </summary>
        private void OnMusicVolumeChanged(float value)
        {
            // TODO: Set music volume separately
            Debug.Log($"[MobileMenuUI] Music volume: {value}");
        }

        /// <summary>
        /// Haptic feedback toggled
        /// Rung phản hồi được bật/tắt
        /// </summary>
        private void OnHapticToggled(bool enabled)
        {
            // TODO: Update haptic settings
            Debug.Log($"[MobileMenuUI] Haptic feedback: {enabled}");
        }

        /// <summary>
        /// Quality setting changed
        /// Cài đặt chất lượng thay đổi
        /// </summary>
        private void OnQualityChanged(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
            Debug.Log($"[MobileMenuUI] Quality level: {qualityIndex}");
        }

        /// <summary>
        /// Quit game button clicked
        /// Nút thoát game được click
        /// </summary>
        private void OnQuitGame()
        {
            Debug.Log("[MobileMenuUI] Quitting game...");
            
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        /// <summary>
        /// Is menu open
        /// Menu có đang mở không
        /// </summary>
        public bool IsOpen()
        {
            return isOpen;
        }
    }
}
