using UnityEngine;
using System;

namespace DarkLegend.Mobile.Core
{
    /// <summary>
    /// Quản lý tất cả mobile systems
    /// Main manager for all mobile systems
    /// </summary>
    public class MobileManager : MonoBehaviour
    {
        #region Singleton
        private static MobileManager instance;
        public static MobileManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<MobileManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("MobileManager");
                        instance = go.AddComponent<MobileManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }
        #endregion

        [Header("Mobile Systems")]
        public bool isMobilePlatform = false;
        public bool enableTouchInput = true;
        public bool enableMobileUI = true;
        public bool enableMobileOptimization = true;

        [Header("System References")]
        public MobileDetection mobileDetection;
        public MobileSettings mobileSettings;
        public MobileOptimization mobileOptimization;

        // Events
        public event Action OnMobileSystemsInitialized;
        public event Action<bool> OnPlatformChanged;

        private void Awake()
        {
            // Singleton pattern
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeMobileSystems();
        }

        private void Start()
        {
            DetectPlatform();
        }

        /// <summary>
        /// Khởi tạo tất cả mobile systems
        /// Initialize all mobile systems
        /// </summary>
        private void InitializeMobileSystems()
        {
            Debug.Log("[MobileManager] Initializing mobile systems...");

            // Get or create mobile detection
            if (mobileDetection == null)
            {
                mobileDetection = GetComponent<MobileDetection>();
                if (mobileDetection == null)
                {
                    mobileDetection = gameObject.AddComponent<MobileDetection>();
                }
            }

            // Get or create mobile settings
            if (mobileSettings == null)
            {
                mobileSettings = GetComponent<MobileSettings>();
                if (mobileSettings == null)
                {
                    mobileSettings = gameObject.AddComponent<MobileSettings>();
                }
            }

            // Get or create mobile optimization
            if (mobileOptimization == null && enableMobileOptimization)
            {
                mobileOptimization = GetComponent<MobileOptimization>();
                if (mobileOptimization == null)
                {
                    mobileOptimization = gameObject.AddComponent<MobileOptimization>();
                }
            }

            OnMobileSystemsInitialized?.Invoke();
            Debug.Log("[MobileManager] Mobile systems initialized successfully!");
        }

        /// <summary>
        /// Phát hiện platform hiện tại
        /// Detect current platform
        /// </summary>
        private void DetectPlatform()
        {
            isMobilePlatform = mobileDetection.IsMobilePlatform();
            Debug.Log($"[MobileManager] Platform detected: {(isMobilePlatform ? "Mobile" : "PC")}");

            OnPlatformChanged?.Invoke(isMobilePlatform);

            if (isMobilePlatform)
            {
                EnableMobileMode();
            }
            else
            {
                EnablePCMode();
            }
        }

        /// <summary>
        /// Bật chế độ Mobile
        /// Enable mobile mode
        /// </summary>
        public void EnableMobileMode()
        {
            Debug.Log("[MobileManager] Enabling mobile mode...");
            
            if (enableMobileOptimization && mobileOptimization != null)
            {
                mobileOptimization.ApplyMobileOptimizations();
            }

            // Apply mobile settings
            if (mobileSettings != null)
            {
                mobileSettings.ApplyMobileSettings();
            }
        }

        /// <summary>
        /// Bật chế độ PC
        /// Enable PC mode
        /// </summary>
        public void EnablePCMode()
        {
            Debug.Log("[MobileManager] Enabling PC mode...");
            
            // Disable mobile-specific features
            enableTouchInput = false;
            enableMobileUI = false;
        }

        /// <summary>
        /// Kiểm tra có phải mobile platform không
        /// Check if mobile platform
        /// </summary>
        public bool IsMobilePlatform()
        {
            return isMobilePlatform;
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
    }
}
