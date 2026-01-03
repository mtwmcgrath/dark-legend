using UnityEngine;

namespace DarkLegend.Mobile.Platform
{
    /// <summary>
    /// Android bridge for native features
    /// Cầu nối Android cho tính năng native
    /// </summary>
    public class AndroidBridge : MonoBehaviour
    {
        #region Singleton
        private static AndroidBridge instance;
        public static AndroidBridge Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("AndroidBridge");
                    instance = go.AddComponent<AndroidBridge>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }
        #endregion

        #if UNITY_ANDROID && !UNITY_EDITOR
        private AndroidJavaObject currentActivity;
        private AndroidJavaClass unityPlayer;
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

            InitializeAndroidBridge();
        }

        /// <summary>
        /// Initialize Android bridge
        /// Khởi tạo cầu nối Android
        /// </summary>
        private void InitializeAndroidBridge()
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                Debug.Log("[AndroidBridge] Initialized successfully");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[AndroidBridge] Initialization failed: {e.Message}");
            }
            #endif
        }

        /// <summary>
        /// Show Android toast message
        /// Hiển thị toast message Android
        /// </summary>
        public void ShowToast(string message, bool longDuration = false)
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                int duration = longDuration ? 1 : 0; // Toast.LENGTH_LONG : Toast.LENGTH_SHORT
                
                currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toast = toastClass.CallStatic<AndroidJavaObject>(
                        "makeText", currentActivity, message, duration);
                    toast.Call("show");
                }));
                
                Debug.Log($"[AndroidBridge] Toast shown: {message}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[AndroidBridge] ShowToast failed: {e.Message}");
            }
            #else
            Debug.Log($"[AndroidBridge] Toast (Editor): {message}");
            #endif
        }

        /// <summary>
        /// Get Android device info
        /// Lấy thông tin thiết bị Android
        /// </summary>
        public string GetDeviceInfo()
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                AndroidJavaClass buildClass = new AndroidJavaClass("android.os.Build");
                string manufacturer = buildClass.GetStatic<string>("MANUFACTURER");
                string model = buildClass.GetStatic<string>("MODEL");
                string version = buildClass.GetStatic<string>("VERSION").GetStatic<string>("RELEASE");
                int sdkInt = buildClass.GetStatic<AndroidJavaObject>("VERSION").GetStatic<int>("SDK_INT");
                
                return $"Device: {manufacturer} {model}\nAndroid: {version} (SDK {sdkInt})";
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[AndroidBridge] GetDeviceInfo failed: {e.Message}");
                return "Unknown Device";
            }
            #else
            return SystemInfo.deviceModel;
            #endif
        }

        /// <summary>
        /// Request Android permissions
        /// Yêu cầu quyền Android
        /// </summary>
        public void RequestPermission(string permission)
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(permission))
                {
                    UnityEngine.Android.Permission.RequestUserPermission(permission);
                    Debug.Log($"[AndroidBridge] Requesting permission: {permission}");
                }
                else
                {
                    Debug.Log($"[AndroidBridge] Permission already granted: {permission}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[AndroidBridge] RequestPermission failed: {e.Message}");
            }
            #endif
        }

        /// <summary>
        /// Check if permission is granted
        /// Kiểm tra quyền đã được cấp chưa
        /// </summary>
        public bool HasPermission(string permission)
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            return UnityEngine.Android.Permission.HasUserAuthorizedPermission(permission);
            #else
            return true;
            #endif
        }

        /// <summary>
        /// Open Android settings
        /// Mở cài đặt Android
        /// </summary>
        public void OpenSettings()
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
                string actionSettings = intentClass.GetStatic<string>("ACTION_APPLICATION_DETAILS_SETTINGS");
                AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", actionSettings);
                
                string packageName = Application.identifier;
                AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
                AndroidJavaObject uri = uriClass.CallStatic<AndroidJavaObject>("parse", "package:" + packageName);
                intent.Call<AndroidJavaObject>("setData", uri);
                
                currentActivity.Call("startActivity", intent);
                Debug.Log("[AndroidBridge] Opened settings");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[AndroidBridge] OpenSettings failed: {e.Message}");
            }
            #endif
        }

        /// <summary>
        /// Share text/image
        /// Chia sẻ text/ảnh
        /// </summary>
        public void Share(string text, string imagePath = null)
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
                AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent");
                intent.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
                intent.Call<AndroidJavaObject>("setType", "text/plain");
                intent.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), text);
                
                AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>(
                    "createChooser", intent, "Share via");
                currentActivity.Call("startActivity", chooser);
                
                Debug.Log($"[AndroidBridge] Sharing: {text}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[AndroidBridge] Share failed: {e.Message}");
            }
            #else
            Debug.Log($"[AndroidBridge] Share (Editor): {text}");
            #endif
        }

        /// <summary>
        /// Keep screen on
        /// Giữ màn hình sáng
        /// </summary>
        public void SetKeepScreenOn(bool keepOn)
        {
            Screen.sleepTimeout = keepOn ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;
            Debug.Log($"[AndroidBridge] Keep screen on: {keepOn}");
        }
    }
}
