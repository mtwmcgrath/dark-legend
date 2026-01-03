using UnityEngine;

namespace DarkLegend.UI
{
    /// <summary>
    /// Central UI manager
    /// Quản lý UI trung tâm
    /// </summary>
    public class UIManager : Utils.Singleton<UIManager>
    {
        [Header("UI Panels")]
        public HUDController hudController;
        public SkillBarUI skillBarUI;
        public InventoryUI inventoryUI;
        public CharacterInfoUI characterInfoUI;
        public MinimapUI minimapUI;
        
        [Header("Menu Panels")]
        public GameObject pauseMenu;
        public GameObject mainMenu;
        public GameObject settingsMenu;
        
        private bool isPaused = false;
        
        protected override void Awake()
        {
            base.Awake();
            
            // Find UI components if not assigned
            if (hudController == null)
                hudController = FindObjectOfType<HUDController>();
            
            if (skillBarUI == null)
                skillBarUI = FindObjectOfType<SkillBarUI>();
            
            if (inventoryUI == null)
                inventoryUI = FindObjectOfType<InventoryUI>();
            
            if (characterInfoUI == null)
                characterInfoUI = FindObjectOfType<CharacterInfoUI>();
            
            if (minimapUI == null)
                minimapUI = FindObjectOfType<MinimapUI>();
        }
        
        private void Start()
        {
            // Initialize UI state
            if (pauseMenu != null)
                pauseMenu.SetActive(false);
            
            if (settingsMenu != null)
                settingsMenu.SetActive(false);
        }
        
        private void Update()
        {
            // Handle ESC key for pause menu
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePauseMenu();
            }
            
            // Handle M key for map
            if (Input.GetKeyDown(KeyCode.M))
            {
                ToggleMap();
            }
        }
        
        /// <summary>
        /// Toggle pause menu
        /// Bật/tắt menu tạm dừng
        /// </summary>
        public void TogglePauseMenu()
        {
            isPaused = !isPaused;
            
            if (pauseMenu != null)
            {
                pauseMenu.SetActive(isPaused);
            }
            
            // Pause game time
            Time.timeScale = isPaused ? 0f : 1f;
            
            // Hide other panels when paused
            if (isPaused)
            {
                if (inventoryUI != null && inventoryUI.inventoryPanel != null)
                    inventoryUI.inventoryPanel.SetActive(false);
                
                if (characterInfoUI != null && characterInfoUI.characterPanel != null)
                    characterInfoUI.characterPanel.SetActive(false);
            }
        }
        
        /// <summary>
        /// Resume game
        /// Tiếp tục game
        /// </summary>
        public void ResumeGame()
        {
            isPaused = false;
            
            if (pauseMenu != null)
                pauseMenu.SetActive(false);
            
            Time.timeScale = 1f;
        }
        
        /// <summary>
        /// Open settings menu
        /// Mở menu cài đặt
        /// </summary>
        public void OpenSettings()
        {
            if (settingsMenu != null)
            {
                settingsMenu.SetActive(true);
            }
            
            if (pauseMenu != null)
            {
                pauseMenu.SetActive(false);
            }
        }
        
        /// <summary>
        /// Close settings menu
        /// Đóng menu cài đặt
        /// </summary>
        public void CloseSettings()
        {
            if (settingsMenu != null)
            {
                settingsMenu.SetActive(false);
            }
            
            if (pauseMenu != null)
            {
                pauseMenu.SetActive(true);
            }
        }
        
        /// <summary>
        /// Toggle map display
        /// Bật/tắt hiển thị bản đồ
        /// </summary>
        public void ToggleMap()
        {
            if (minimapUI != null)
            {
                minimapUI.ToggleMinimap();
            }
        }
        
        /// <summary>
        /// Show damage number at position
        /// Hiển thị số sát thương tại vị trí
        /// </summary>
        public void ShowDamageNumber(Vector3 worldPosition, int damage, bool isCritical = false)
        {
            // TODO: Implement damage number popup
            // This would create a floating text that shows damage
            Debug.Log($"Damage: {damage}{(isCritical ? " CRIT!" : "")}");
        }
        
        /// <summary>
        /// Show notification message
        /// Hiển thị thông báo
        /// </summary>
        public void ShowNotification(string message)
        {
            // TODO: Implement notification system
            Debug.Log($"Notification: {message}");
        }
        
        /// <summary>
        /// Quit to main menu
        /// Thoát về menu chính
        /// </summary>
        public void QuitToMainMenu()
        {
            Time.timeScale = 1f;
            // TODO: Save game before quitting
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
        
        /// <summary>
        /// Quit game
        /// Thoát game
        /// </summary>
        public void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}
