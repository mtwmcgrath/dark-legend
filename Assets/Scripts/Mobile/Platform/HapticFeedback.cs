using UnityEngine;

namespace DarkLegend.Mobile.Platform
{
    /// <summary>
    /// Haptic feedback manager
    /// Quản lý rung phản hồi
    /// </summary>
    public class HapticFeedback : MonoBehaviour
    {
        #region Singleton
        private static HapticFeedback instance;
        public static HapticFeedback Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("HapticFeedback");
                    instance = go.AddComponent<HapticFeedback>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }
        #endregion

        [Header("Haptic Settings")]
        public bool enableHaptic = true;
        public float hapticIntensity = 1.0f;

        [Header("Vibration Durations (ms)")]
        public int lightDuration = 10;
        public int mediumDuration = 20;
        public int heavyDuration = 50;
        public int successDuration = 30;
        public int errorDuration = 40;

        #if UNITY_ANDROID && !UNITY_EDITOR
        private AndroidJavaObject vibrator;
        private AndroidJavaObject vibratorEffect;
        #endif

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeHaptic();
        }

        /// <summary>
        /// Initialize haptic system
        /// Khởi tạo hệ thống haptic
        /// </summary>
        private void InitializeHaptic()
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
                
                vibrator = context.Call<AndroidJavaObject>("getSystemService", "vibrator");
                Debug.Log("[HapticFeedback] Initialized successfully");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[HapticFeedback] Initialization failed: {e.Message}");
            }
            #endif

            // Load settings
            enableHaptic = PlayerPrefs.GetInt("HapticEnabled", 1) == 1;
        }

        /// <summary>
        /// Trigger haptic feedback
        /// Kích hoạt rung phản hồi
        /// </summary>
        public static void TriggerHaptic(HapticType type)
        {
            Instance.Trigger(type);
        }

        /// <summary>
        /// Trigger haptic feedback (instance method)
        /// Kích hoạt rung phản hồi (phương thức instance)
        /// </summary>
        public void Trigger(HapticType type)
        {
            if (!enableHaptic)
                return;

            int duration = GetDuration(type);
            Vibrate(duration);
        }

        /// <summary>
        /// Get duration for haptic type
        /// Lấy thời lượng cho loại haptic
        /// </summary>
        private int GetDuration(HapticType type)
        {
            switch (type)
            {
                case HapticType.Light:
                    return lightDuration;
                case HapticType.Medium:
                    return mediumDuration;
                case HapticType.Heavy:
                    return heavyDuration;
                case HapticType.Success:
                    return successDuration;
                case HapticType.Error:
                    return errorDuration;
                default:
                    return mediumDuration;
            }
        }

        /// <summary>
        /// Vibrate device
        /// Rung thiết bị
        /// </summary>
        private void Vibrate(int milliseconds)
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                if (vibrator != null)
                {
                    // Check Android version
                    AndroidJavaClass buildVersion = new AndroidJavaClass("android.os.Build$VERSION");
                    int sdkInt = buildVersion.GetStatic<int>("SDK_INT");
                    
                    if (sdkInt >= 26) // Android 8.0+
                    {
                        // Use VibrationEffect
                        AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
                        vibratorEffect = vibrationEffectClass.CallStatic<AndroidJavaObject>(
                            "createOneShot",
                            (long)milliseconds,
                            (int)(255 * hapticIntensity)
                        );
                        vibrator.Call("vibrate", vibratorEffect);
                    }
                    else
                    {
                        // Legacy vibration
                        vibrator.Call("vibrate", (long)milliseconds);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[HapticFeedback] Vibrate failed: {e.Message}");
            }
            #elif UNITY_IOS && !UNITY_EDITOR
            // iOS Haptic feedback
            Handheld.Vibrate();
            #else
            // Editor - just log
            Debug.Log($"[HapticFeedback] Vibrate {milliseconds}ms");
            #endif
        }

        /// <summary>
        /// Vibrate with pattern
        /// Rung theo mẫu
        /// </summary>
        public void VibratePattern(long[] pattern, int repeat = -1)
        {
            if (!enableHaptic)
                return;

            #if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                if (vibrator != null)
                {
                    vibrator.Call("vibrate", pattern, repeat);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[HapticFeedback] VibratePattern failed: {e.Message}");
            }
            #else
            Debug.Log($"[HapticFeedback] Vibrate pattern with {pattern.Length} steps");
            #endif
        }

        /// <summary>
        /// Cancel vibration
        /// Hủy rung
        /// </summary>
        public void CancelVibration()
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                if (vibrator != null)
                {
                    vibrator.Call("cancel");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[HapticFeedback] CancelVibration failed: {e.Message}");
            }
            #endif
        }

        /// <summary>
        /// Enable/disable haptic
        /// Bật/tắt haptic
        /// </summary>
        public void SetHapticEnabled(bool enabled)
        {
            enableHaptic = enabled;
            PlayerPrefs.SetInt("HapticEnabled", enabled ? 1 : 0);
            PlayerPrefs.Save();
            
            Debug.Log($"[HapticFeedback] Haptic {(enabled ? "enabled" : "disabled")}");
        }

        /// <summary>
        /// Set haptic intensity
        /// Đặt cường độ haptic
        /// </summary>
        public void SetHapticIntensity(float intensity)
        {
            hapticIntensity = Mathf.Clamp01(intensity);
        }

        /// <summary>
        /// Check if haptic is supported
        /// Kiểm tra haptic có được hỗ trợ không
        /// </summary>
        public bool IsHapticSupported()
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                if (vibrator != null)
                {
                    return vibrator.Call<bool>("hasVibrator");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[HapticFeedback] IsHapticSupported check failed: {e.Message}");
            }
            return false;
            #else
            return true; // Assume supported in editor/iOS
            #endif
        }
    }

    public enum HapticType
    {
        Light,      // UI feedback
        Medium,     // Skill cast
        Heavy,      // Damage received
        Success,    // Level up, rare drop
        Error       // Failed action
    }
}
