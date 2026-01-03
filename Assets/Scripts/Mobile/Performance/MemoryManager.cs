using UnityEngine;
using System;
using System.Collections;

namespace DarkLegend.Mobile.Performance
{
    /// <summary>
    /// Memory manager for mobile
    /// Quản lý bộ nhớ cho mobile
    /// </summary>
    public class MemoryManager : MonoBehaviour
    {
        [Header("Memory Settings")]
        public bool autoCleanup = true;
        public float cleanupInterval = 60f;
        public float criticalMemoryThreshold = 0.9f;

        [Header("Memory Info")]
        public long totalMemory = 0;
        public long usedMemory = 0;
        public long freeMemory = 0;
        public float memoryUsagePercent = 0f;

        [Header("Monitoring")]
        public bool showMemoryWarnings = true;
        public float warningThreshold = 0.8f;

        private Coroutine cleanupCoroutine;

        // Events
        public event Action OnMemoryWarning;
        public event Action OnMemoryCritical;

        private void Start()
        {
            UpdateMemoryInfo();

            if (autoCleanup)
            {
                StartMemoryCleanup();
            }
        }

        /// <summary>
        /// Start memory cleanup
        /// Bắt đầu dọn dẹp bộ nhớ
        /// </summary>
        private void StartMemoryCleanup()
        {
            if (cleanupCoroutine == null)
            {
                cleanupCoroutine = StartCoroutine(MemoryCleanupRoutine());
            }
        }

        /// <summary>
        /// Stop memory cleanup
        /// Dừng dọn dẹp bộ nhớ
        /// </summary>
        private void StopMemoryCleanup()
        {
            if (cleanupCoroutine != null)
            {
                StopCoroutine(cleanupCoroutine);
                cleanupCoroutine = null;
            }
        }

        /// <summary>
        /// Memory cleanup routine
        /// Quy trình dọn dẹp bộ nhớ
        /// </summary>
        private IEnumerator MemoryCleanupRoutine()
        {
            while (autoCleanup)
            {
                yield return new WaitForSeconds(cleanupInterval);

                UpdateMemoryInfo();

                // Check memory usage
                if (memoryUsagePercent >= criticalMemoryThreshold)
                {
                    Debug.LogWarning($"[MemoryManager] Critical memory usage: {memoryUsagePercent * 100}%");
                    OnMemoryCritical?.Invoke();
                    ForceCleanup();
                }
                else if (memoryUsagePercent >= warningThreshold)
                {
                    Debug.LogWarning($"[MemoryManager] High memory usage: {memoryUsagePercent * 100}%");
                    OnMemoryWarning?.Invoke();
                    CleanupMemory();
                }
                else
                {
                    // Normal cleanup
                    CleanupMemory();
                }
            }
        }

        /// <summary>
        /// Update memory info
        /// Cập nhật thông tin bộ nhớ
        /// </summary>
        public void UpdateMemoryInfo()
        {
            totalMemory = SystemInfo.systemMemorySize * 1024L * 1024L; // Convert MB to bytes
            
            // Get allocated memory (managed heap)
            usedMemory = GC.GetTotalMemory(false);
            freeMemory = totalMemory - usedMemory;
            memoryUsagePercent = (float)usedMemory / totalMemory;
        }

        /// <summary>
        /// Cleanup memory
        /// Dọn dẹp bộ nhớ
        /// </summary>
        public void CleanupMemory()
        {
            // Unload unused assets
            Resources.UnloadUnusedAssets();

            Debug.Log("[MemoryManager] Memory cleanup completed");
        }

        /// <summary>
        /// Force cleanup (aggressive)
        /// Dọn dẹp ép buộc (mạnh)
        /// </summary>
        public void ForceCleanup()
        {
            // Unload unused assets
            Resources.UnloadUnusedAssets();

            // Force garbage collection
            GC.Collect();
            GC.WaitForPendingFinalizers();

            UpdateMemoryInfo();

            Debug.Log($"[MemoryManager] Force cleanup completed - Memory usage: {memoryUsagePercent * 100:F1}%");
        }

        /// <summary>
        /// Get memory info string
        /// Lấy chuỗi thông tin bộ nhớ
        /// </summary>
        public string GetMemoryInfo()
        {
            UpdateMemoryInfo();

            return $"Memory Usage:\n" +
                   $"Total: {totalMemory / (1024 * 1024)} MB\n" +
                   $"Used: {usedMemory / (1024 * 1024)} MB\n" +
                   $"Free: {freeMemory / (1024 * 1024)} MB\n" +
                   $"Usage: {memoryUsagePercent * 100:F1}%";
        }

        /// <summary>
        /// Is memory critical
        /// Bộ nhớ có ở mức nguy hiểm không
        /// </summary>
        public bool IsMemoryCritical()
        {
            UpdateMemoryInfo();
            return memoryUsagePercent >= criticalMemoryThreshold;
        }

        /// <summary>
        /// Is memory warning
        /// Bộ nhớ có ở mức cảnh báo không
        /// </summary>
        public bool IsMemoryWarning()
        {
            UpdateMemoryInfo();
            return memoryUsagePercent >= warningThreshold;
        }

        private void OnDestroy()
        {
            StopMemoryCleanup();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                // App going to background - cleanup memory
                CleanupMemory();
            }
        }
    }
}
