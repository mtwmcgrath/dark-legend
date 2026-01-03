using UnityEngine;
using System;

namespace DarkLegend.Mobile.UI
{
    /// <summary>
    /// Quản lý UI cho mobile
    /// Mobile UI manager
    /// </summary>
    public class MobileUIManager : MonoBehaviour
    {
        #region Singleton
        private static MobileUIManager instance;
        public static MobileUIManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<MobileUIManager>();
                }
                return instance;
            }
        }
        #endregion

        [Header("UI Panels")]
        public MobileHUD mobileHUD;
        public MobileSkillBar mobileSkillBar;
        public MobileInventoryUI mobileInventory;
        public MobileMenuUI mobileMenu;

        [Header("UI Settings")]
        public float uiScale = 1.0f;
        public bool autoHideUI = false;
        public float autoHideDelay = 3f;

        [Header("Safe Area")]
        public RectTransform safeAreaPanel;

        // State
        private bool isUIVisible = true;
        private float idleTimer = 0f;

        // Events
        public event Action OnUIShown;
        public event Action OnUIHidden;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;

            ApplySafeArea();
        }

        private void Start()
        {
            InitializeUI();
        }

        private void Update()
        {
            // Auto hide UI logic
            if (autoHideUI && isUIVisible)
            {
                if (Input.touchCount == 0 && !Input.GetMouseButton(0))
                {
                    idleTimer += Time.deltaTime;
                    if (idleTimer >= autoHideDelay)
                    {
                        HideUI();
                    }
                }
                else
                {
                    idleTimer = 0f;
                    if (!isUIVisible)
                    {
                        ShowUI();
                    }
                }
            }
        }

        /// <summary>
        /// Initialize UI components
        /// Khởi tạo các thành phần UI
        /// </summary>
        private void InitializeUI()
        {
            // Find UI components if not assigned
            if (mobileHUD == null)
            {
                mobileHUD = FindObjectOfType<MobileHUD>();
            }

            if (mobileSkillBar == null)
            {
                mobileSkillBar = FindObjectOfType<MobileSkillBar>();
            }

            if (mobileInventory == null)
            {
                mobileInventory = FindObjectOfType<MobileInventoryUI>();
            }

            if (mobileMenu == null)
            {
                mobileMenu = FindObjectOfType<MobileMenuUI>();
            }

            // Apply UI scale
            ApplyUIScale();

            Debug.Log("[MobileUIManager] UI initialized");
        }

        /// <summary>
        /// Apply safe area for notch/rounded corners
        /// Áp dụng safe area cho notch/góc bo
        /// </summary>
        private void ApplySafeArea()
        {
            if (safeAreaPanel == null)
                return;

            Rect safeArea = Screen.safeArea;
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            safeAreaPanel.anchorMin = anchorMin;
            safeAreaPanel.anchorMax = anchorMax;

            Debug.Log($"[MobileUIManager] Safe area applied: {safeArea}");
        }

        /// <summary>
        /// Apply UI scale
        /// Áp dụng scale UI
        /// </summary>
        private void ApplyUIScale()
        {
            Canvas[] canvases = FindObjectsOfType<Canvas>();
            foreach (Canvas canvas in canvases)
            {
                UnityEngine.UI.CanvasScaler scaler = canvas.GetComponent<UnityEngine.UI.CanvasScaler>();
                if (scaler != null)
                {
                    scaler.scaleFactor = uiScale;
                }
            }
        }

        /// <summary>
        /// Show UI
        /// Hiển thị UI
        /// </summary>
        public void ShowUI()
        {
            isUIVisible = true;
            SetUIVisibility(true);
            OnUIShown?.Invoke();
        }

        /// <summary>
        /// Hide UI
        /// Ẩn UI
        /// </summary>
        public void HideUI()
        {
            isUIVisible = false;
            SetUIVisibility(false);
            OnUIHidden?.Invoke();
        }

        /// <summary>
        /// Set UI visibility
        /// Đặt hiển thị UI
        /// </summary>
        private void SetUIVisibility(bool visible)
        {
            if (mobileHUD != null)
            {
                mobileHUD.gameObject.SetActive(visible);
            }

            if (mobileSkillBar != null)
            {
                mobileSkillBar.gameObject.SetActive(visible);
            }
        }

        /// <summary>
        /// Toggle inventory
        /// Bật/tắt inventory
        /// </summary>
        public void ToggleInventory()
        {
            if (mobileInventory != null)
            {
                mobileInventory.Toggle();
            }
        }

        /// <summary>
        /// Toggle menu
        /// Bật/tắt menu
        /// </summary>
        public void ToggleMenu()
        {
            if (mobileMenu != null)
            {
                mobileMenu.Toggle();
            }
        }

        /// <summary>
        /// Set UI scale
        /// Đặt scale UI
        /// </summary>
        public void SetUIScale(float scale)
        {
            uiScale = Mathf.Clamp(scale, 0.5f, 2.0f);
            ApplyUIScale();
        }
    }
}
