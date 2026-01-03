using UnityEngine;

namespace DarkLegend.Managers
{
    /// <summary>
    /// Game states
    /// Trạng thái game
    /// </summary>
    public enum GameState
    {
        MainMenu,
        Loading,
        Playing,
        Paused,
        GameOver
    }
    
    /// <summary>
    /// Main game manager using singleton pattern
    /// Quản lý game chính sử dụng singleton pattern
    /// </summary>
    public class GameManager : Utils.Singleton<GameManager>
    {
        [Header("Game State")]
        public GameState currentState = GameState.MainMenu;
        
        [Header("Player")]
        public GameObject playerPrefab;
        public Transform playerSpawnPoint;
        private GameObject currentPlayer;
        
        [Header("Scene Management")]
        public string mainMenuScene = "MainMenu";
        public string gameScene = "GameScene";
        public string loadingScene = "LoadingScene";
        
        // Events
        public System.Action<GameState> OnGameStateChanged;
        public System.Action OnPlayerSpawned;
        public System.Action OnPlayerDied;
        
        protected override void Awake()
        {
            base.Awake();
        }
        
        private void Start()
        {
            // Initialize game
            Initialize();
        }
        
        /// <summary>
        /// Initialize game
        /// Khởi tạo game
        /// </summary>
        private void Initialize()
        {
            // Set target frame rate for PC
            Application.targetFrameRate = 60;
            
            // Lock cursor for gameplay
            if (currentState == GameState.Playing)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        
        /// <summary>
        /// Start new game
        /// Bắt đầu game mới
        /// </summary>
        public void StartNewGame()
        {
            SetGameState(GameState.Loading);
            
            // Load game scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(gameScene);
            
            SetGameState(GameState.Playing);
            SpawnPlayer();
        }
        
        /// <summary>
        /// Spawn player
        /// Sinh người chơi
        /// </summary>
        public void SpawnPlayer()
        {
            if (playerPrefab == null)
            {
                Debug.LogError("Player prefab not assigned!");
                return;
            }
            
            Vector3 spawnPosition = playerSpawnPoint != null ? playerSpawnPoint.position : Vector3.zero;
            Quaternion spawnRotation = playerSpawnPoint != null ? playerSpawnPoint.rotation : Quaternion.identity;
            
            currentPlayer = Instantiate(playerPrefab, spawnPosition, spawnRotation);
            currentPlayer.tag = Utils.Constants.TAG_PLAYER;
            
            // Subscribe to player death
            Character.CharacterStats playerStats = currentPlayer.GetComponent<Character.CharacterStats>();
            if (playerStats != null)
            {
                playerStats.OnDeath += HandlePlayerDeath;
            }
            
            OnPlayerSpawned?.Invoke();
            Debug.Log("Player spawned!");
        }
        
        /// <summary>
        /// Handle player death
        /// Xử lý khi người chơi chết
        /// </summary>
        private void HandlePlayerDeath()
        {
            OnPlayerDied?.Invoke();
            SetGameState(GameState.GameOver);
            
            // Show game over UI after delay
            Invoke(nameof(ShowGameOverScreen), 3f);
        }
        
        /// <summary>
        /// Show game over screen
        /// Hiển thị màn hình game over
        /// </summary>
        private void ShowGameOverScreen()
        {
            // TODO: Show game over UI
            Debug.Log("Game Over!");
        }
        
        /// <summary>
        /// Set game state
        /// Đặt trạng thái game
        /// </summary>
        public void SetGameState(GameState newState)
        {
            if (currentState == newState) return;
            
            GameState previousState = currentState;
            currentState = newState;
            
            OnGameStateChanged?.Invoke(newState);
            
            // Handle state transitions
            switch (newState)
            {
                case GameState.Playing:
                    Time.timeScale = 1f;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    break;
                    
                case GameState.Paused:
                    Time.timeScale = 0f;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    break;
                    
                case GameState.MainMenu:
                case GameState.GameOver:
                    Time.timeScale = 1f;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    break;
            }
        }
        
        /// <summary>
        /// Pause game
        /// Tạm dừng game
        /// </summary>
        public void PauseGame()
        {
            SetGameState(GameState.Paused);
        }
        
        /// <summary>
        /// Resume game
        /// Tiếp tục game
        /// </summary>
        public void ResumeGame()
        {
            SetGameState(GameState.Playing);
        }
        
        /// <summary>
        /// Restart game
        /// Khởi động lại game
        /// </summary>
        public void RestartGame()
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene(gameScene);
            SetGameState(GameState.Playing);
        }
        
        /// <summary>
        /// Load main menu
        /// Tải menu chính
        /// </summary>
        public void LoadMainMenu()
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuScene);
            SetGameState(GameState.MainMenu);
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
        
        /// <summary>
        /// Get current player
        /// Lấy người chơi hiện tại
        /// </summary>
        public GameObject GetPlayer()
        {
            return currentPlayer;
        }
    }
}
