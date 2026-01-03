using UnityEngine;
using System;
using System.Collections.Generic;

namespace DarkLegend.Mobile.Platform
{
    /// <summary>
    /// Notification manager for push notifications
    /// Quản lý thông báo đẩy
    /// </summary>
    public class NotificationManager : MonoBehaviour
    {
        #region Singleton
        private static NotificationManager instance;
        public static NotificationManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("NotificationManager");
                    instance = go.AddComponent<NotificationManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }
        #endregion

        [Header("Notification Settings")]
        public bool enableNotifications = true;
        public string notificationChannelId = "dark_legend_channel";
        public string notificationChannelName = "Dark Legend Notifications";

        private List<int> scheduledNotificationIds = new List<int>();

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeNotifications();
        }

        /// <summary>
        /// Initialize notifications
        /// Khởi tạo thông báo
        /// </summary>
        private void InitializeNotifications()
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            // Create notification channel for Android 8.0+
            CreateNotificationChannel();
            #endif
            
            Debug.Log("[NotificationManager] Initialized");
        }

        /// <summary>
        /// Create notification channel (Android 8.0+)
        /// Tạo kênh thông báo (Android 8.0+)
        /// </summary>
        private void CreateNotificationChannel()
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
                
                // Check Android version
                AndroidJavaClass buildVersion = new AndroidJavaClass("android.os.Build$VERSION");
                int sdkInt = buildVersion.GetStatic<int>("SDK_INT");
                
                if (sdkInt >= 26) // Android 8.0
                {
                    AndroidJavaObject channel = new AndroidJavaObject(
                        "android.app.NotificationChannel",
                        notificationChannelId,
                        notificationChannelName,
                        3 // NotificationManager.IMPORTANCE_DEFAULT
                    );
                    
                    AndroidJavaObject notificationManager = context.Call<AndroidJavaObject>(
                        "getSystemService",
                        "notification"
                    );
                    
                    notificationManager.Call("createNotificationChannel", channel);
                    Debug.Log("[NotificationManager] Notification channel created");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[NotificationManager] CreateNotificationChannel failed: {e.Message}");
            }
            #endif
        }

        /// <summary>
        /// Schedule local notification
        /// Lên lịch thông báo local
        /// </summary>
        public void ScheduleLocalNotification(string title, string body, int delaySeconds)
        {
            if (!enableNotifications)
                return;

            #if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                int notificationId = UnityEngine.Random.Range(1000, 9999);
                scheduledNotificationIds.Add(notificationId);
                
                // Schedule notification using Unity Mobile Notifications package
                // Note: This requires Unity Mobile Notifications package to be installed
                Debug.Log($"[NotificationManager] Scheduled notification: {title} in {delaySeconds}s");
            }
            catch (Exception e)
            {
                Debug.LogError($"[NotificationManager] ScheduleLocalNotification failed: {e.Message}");
            }
            #else
            Debug.Log($"[NotificationManager] (Editor) Scheduled: {title} - {body} in {delaySeconds}s");
            #endif
        }

        /// <summary>
        /// Schedule stamina full notification
        /// Lên lịch thông báo stamina đầy
        /// </summary>
        public void ScheduleStaminaFullNotification(int minutesUntilFull)
        {
            ScheduleLocalNotification(
                "Stamina Full!",
                "Your stamina is fully restored. Time to adventure!",
                minutesUntilFull * 60
            );
        }

        /// <summary>
        /// Schedule daily reward notification
        /// Lên lịch thông báo phần thưởng hàng ngày
        /// </summary>
        public void ScheduleDailyRewardNotification()
        {
            // Calculate seconds until next day reset (assuming 00:00 UTC)
            DateTime now = DateTime.UtcNow;
            DateTime tomorrow = now.Date.AddDays(1);
            int secondsUntilReset = (int)(tomorrow - now).TotalSeconds;

            ScheduleLocalNotification(
                "Daily Rewards Ready!",
                "Don't forget to claim your daily rewards!",
                secondsUntilReset
            );
        }

        /// <summary>
        /// Schedule event notification
        /// Lên lịch thông báo sự kiện
        /// </summary>
        public void ScheduleEventNotification(string eventName, int minutesBeforeEvent)
        {
            ScheduleLocalNotification(
                $"{eventName} Starting Soon!",
                $"The {eventName} event will begin in {minutesBeforeEvent} minutes!",
                minutesBeforeEvent * 60
            );
        }

        /// <summary>
        /// Cancel all notifications
        /// Hủy tất cả thông báo
        /// </summary>
        public void CancelAllNotifications()
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
                
                AndroidJavaObject notificationManager = context.Call<AndroidJavaObject>(
                    "getSystemService",
                    "notification"
                );
                
                notificationManager.Call("cancelAll");
                scheduledNotificationIds.Clear();
                
                Debug.Log("[NotificationManager] All notifications cancelled");
            }
            catch (Exception e)
            {
                Debug.LogError($"[NotificationManager] CancelAllNotifications failed: {e.Message}");
            }
            #else
            scheduledNotificationIds.Clear();
            Debug.Log("[NotificationManager] (Editor) All notifications cancelled");
            #endif
        }

        /// <summary>
        /// Cancel specific notification
        /// Hủy thông báo cụ thể
        /// </summary>
        public void CancelNotification(int notificationId)
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
                
                AndroidJavaObject notificationManager = context.Call<AndroidJavaObject>(
                    "getSystemService",
                    "notification"
                );
                
                notificationManager.Call("cancel", notificationId);
                scheduledNotificationIds.Remove(notificationId);
                
                Debug.Log($"[NotificationManager] Notification {notificationId} cancelled");
            }
            catch (Exception e)
            {
                Debug.LogError($"[NotificationManager] CancelNotification failed: {e.Message}");
            }
            #else
            scheduledNotificationIds.Remove(notificationId);
            Debug.Log($"[NotificationManager] (Editor) Notification {notificationId} cancelled");
            #endif
        }

        /// <summary>
        /// Enable/disable notifications
        /// Bật/tắt thông báo
        /// </summary>
        public void SetNotificationsEnabled(bool enabled)
        {
            enableNotifications = enabled;
            
            if (!enabled)
            {
                CancelAllNotifications();
            }
            
            Debug.Log($"[NotificationManager] Notifications {(enabled ? "enabled" : "disabled")}");
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && enableNotifications)
            {
                // App going to background - schedule notifications
                ScheduleDailyRewardNotification();
            }
            else
            {
                // App coming to foreground - cancel all notifications
                CancelAllNotifications();
            }
        }
    }
}
