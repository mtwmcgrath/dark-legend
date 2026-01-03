using UnityEngine;

namespace DarkLegend.Mobile.Platform
{
    /// <summary>
    /// Screen manager for orientation and safe area
    /// Quản lý màn hình cho orientation và safe area
    /// </summary>
    public class ScreenManager : MonoBehaviour
    {
        [Header("Safe Area")]
        public RectTransform safeAreaRect;

        [Header("Orientation")]
        public bool allowPortrait = false;
        public bool allowLandscape = true;
        public bool allowAutoRotate = true;

        [Header("Notch Handling")]
        public bool avoidNotch = true;
        public float notchPadding = 20f;

        [Header("Screen Settings")]
        public bool keepScreenOn = true;
        public float screenBrightness = 1.0f;

        private Rect lastSafeArea = new Rect(0, 0, 0, 0);
        private ScreenOrientation lastOrientation = ScreenOrientation.Portrait;

        private void Start()
        {
            InitializeScreen();
        }

        private void Update()
        {
            // Check for safe area changes
            if (Screen.safeArea != lastSafeArea)
            {
                ApplySafeArea();
            }

            // Check for orientation changes
            if (Screen.orientation != lastOrientation)
            {
                OnOrientationChanged();
            }
        }

        /// <summary>
        /// Initialize screen settings
        /// Khởi tạo cài đặt màn hình
        /// </summary>
        private void InitializeScreen()
        {
            // Set screen orientation
            SetScreenOrientation();

            // Apply safe area
            ApplySafeArea();

            // Set screen sleep timeout
            Screen.sleepTimeout = keepScreenOn ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;

            // Set screen brightness (Android only)
            #if UNITY_ANDROID && !UNITY_EDITOR
            SetScreenBrightness(screenBrightness);
            #endif

            Debug.Log($"[ScreenManager] Screen initialized - Resolution: {Screen.width}x{Screen.height}");
        }

        /// <summary>
        /// Set screen orientation
        /// Đặt hướng màn hình
        /// </summary>
        private void SetScreenOrientation()
        {
            if (allowAutoRotate)
            {
                Screen.autorotateToPortrait = allowPortrait;
                Screen.autorotateToPortraitUpsideDown = allowPortrait;
                Screen.autorotateToLandscapeLeft = allowLandscape;
                Screen.autorotateToLandscapeRight = allowLandscape;
                Screen.orientation = ScreenOrientation.AutoRotation;
            }
            else
            {
                if (allowLandscape)
                {
                    Screen.orientation = ScreenOrientation.LandscapeLeft;
                }
                else if (allowPortrait)
                {
                    Screen.orientation = ScreenOrientation.Portrait;
                }
            }

            lastOrientation = Screen.orientation;
        }

        /// <summary>
        /// Apply safe area to UI
        /// Áp dụng safe area cho UI
        /// </summary>
        private void ApplySafeArea()
        {
            if (safeAreaRect == null)
                return;

            Rect safeArea = Screen.safeArea;
            lastSafeArea = safeArea;

            // Calculate anchor min/max
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            // Normalize to screen size
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            // Apply notch padding if needed
            if (avoidNotch)
            {
                float paddingX = notchPadding / Screen.width;
                float paddingY = notchPadding / Screen.height;

                anchorMin.x += paddingX;
                anchorMin.y += paddingY;
                anchorMax.x -= paddingX;
                anchorMax.y -= paddingY;
            }

            // Set anchors
            safeAreaRect.anchorMin = anchorMin;
            safeAreaRect.anchorMax = anchorMax;

            Debug.Log($"[ScreenManager] Safe area applied: {safeArea}");
        }

        /// <summary>
        /// Handle orientation change
        /// Xử lý thay đổi hướng màn hình
        /// </summary>
        private void OnOrientationChanged()
        {
            lastOrientation = Screen.orientation;
            Debug.Log($"[ScreenManager] Orientation changed to: {lastOrientation}");

            // Re-apply safe area
            ApplySafeArea();
        }

        /// <summary>
        /// Set screen brightness (Android)
        /// Đặt độ sáng màn hình (Android)
        /// </summary>
        public void SetScreenBrightness(float brightness)
        {
            screenBrightness = Mathf.Clamp01(brightness);

            #if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject window = currentActivity.Call<AndroidJavaObject>("getWindow");
                AndroidJavaObject layoutParams = window.Call<AndroidJavaObject>("getAttributes");
                
                // Set screen brightness (-1 for system brightness, 0-1 for manual)
                layoutParams.Set("screenBrightness", brightness);
                window.Call("setAttributes", layoutParams);
                
                Debug.Log($"[ScreenManager] Screen brightness set to: {brightness}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[ScreenManager] SetScreenBrightness failed: {e.Message}");
            }
            #else
            Screen.brightness = brightness;
            #endif
        }

        /// <summary>
        /// Lock orientation
        /// Khóa hướng màn hình
        /// </summary>
        public void LockOrientation(ScreenOrientation orientation)
        {
            Screen.orientation = orientation;
            allowAutoRotate = false;
            Debug.Log($"[ScreenManager] Orientation locked to: {orientation}");
        }

        /// <summary>
        /// Unlock orientation
        /// Mở khóa hướng màn hình
        /// </summary>
        public void UnlockOrientation()
        {
            allowAutoRotate = true;
            SetScreenOrientation();
            Debug.Log("[ScreenManager] Orientation unlocked");
        }

        /// <summary>
        /// Set keep screen on
        /// Đặt giữ màn hình sáng
        /// </summary>
        public void SetKeepScreenOn(bool keepOn)
        {
            keepScreenOn = keepOn;
            Screen.sleepTimeout = keepOn ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;
            Debug.Log($"[ScreenManager] Keep screen on: {keepOn}");
        }

        /// <summary>
        /// Get screen info
        /// Lấy thông tin màn hình
        /// </summary>
        public string GetScreenInfo()
        {
            return $"Screen Info:\n" +
                   $"Resolution: {Screen.width}x{Screen.height}\n" +
                   $"DPI: {Screen.dpi}\n" +
                   $"Orientation: {Screen.orientation}\n" +
                   $"Safe Area: {Screen.safeArea}\n" +
                   $"Refresh Rate: {Screen.currentResolution.refreshRate}Hz";
        }

        /// <summary>
        /// Is landscape orientation
        /// Có phải hướng ngang không
        /// </summary>
        public bool IsLandscape()
        {
            return Screen.orientation == ScreenOrientation.LandscapeLeft ||
                   Screen.orientation == ScreenOrientation.LandscapeRight;
        }

        /// <summary>
        /// Is portrait orientation
        /// Có phải hướng dọc không
        /// </summary>
        public bool IsPortrait()
        {
            return Screen.orientation == ScreenOrientation.Portrait ||
                   Screen.orientation == ScreenOrientation.PortraitUpsideDown;
        }
    }
}
