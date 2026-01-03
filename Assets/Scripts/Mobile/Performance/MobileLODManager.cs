using UnityEngine;

namespace DarkLegend.Mobile.Performance
{
    /// <summary>
    /// LOD Manager for mobile
    /// Quản lý LOD cho mobile
    /// </summary>
    public class MobileLODManager : MonoBehaviour
    {
        [Header("LOD Settings")]
        public float lodBias = 1.0f;
        public int maxLODLevel = 2;
        public float lodDistance = 50f;

        [Header("Distance Ranges")]
        public float highQualityDistance = 20f;
        public float mediumQualityDistance = 40f;
        public float lowQualityDistance = 60f;

        [Header("Performance")]
        public bool dynamicLOD = true;
        public float updateInterval = 1f;

        private Transform playerTransform;
        private LODGroup[] lodGroups;
        private float updateTimer = 0f;

        private void Start()
        {
            InitializeLODManager();
        }

        private void Update()
        {
            if (dynamicLOD)
            {
                updateTimer += Time.deltaTime;
                if (updateTimer >= updateInterval)
                {
                    UpdateLODs();
                    updateTimer = 0f;
                }
            }
        }

        /// <summary>
        /// Initialize LOD manager
        /// Khởi tạo LOD manager
        /// </summary>
        private void InitializeLODManager()
        {
            // Set LOD bias
            QualitySettings.lodBias = lodBias;
            QualitySettings.maximumLODLevel = maxLODLevel;

            // Find all LOD groups
            lodGroups = FindObjectsOfType<LODGroup>();

            // Find player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }

            Debug.Log($"[MobileLODManager] Initialized with {lodGroups.Length} LOD groups");
        }

        /// <summary>
        /// Update LODs based on distance
        /// Cập nhật LOD dựa trên khoảng cách
        /// </summary>
        private void UpdateLODs()
        {
            if (playerTransform == null || lodGroups == null)
                return;

            foreach (LODGroup lodGroup in lodGroups)
            {
                if (lodGroup == null)
                    continue;

                float distance = Vector3.Distance(playerTransform.position, lodGroup.transform.position);
                AdjustLODGroup(lodGroup, distance);
            }
        }

        /// <summary>
        /// Adjust LOD group based on distance
        /// Điều chỉnh LOD group dựa trên khoảng cách
        /// </summary>
        private void AdjustLODGroup(LODGroup lodGroup, float distance)
        {
            LOD[] lods = lodGroup.GetLODs();
            
            if (distance < highQualityDistance)
            {
                // High quality - use LOD 0
                lodGroup.ForceLOD(0);
            }
            else if (distance < mediumQualityDistance)
            {
                // Medium quality - use LOD 1
                lodGroup.ForceLOD(lods.Length > 1 ? 1 : 0);
            }
            else if (distance < lowQualityDistance)
            {
                // Low quality - use LOD 2
                lodGroup.ForceLOD(lods.Length > 2 ? 2 : lods.Length - 1);
            }
            else
            {
                // Very far - disable
                lodGroup.ForceLOD(-1);
            }
        }

        /// <summary>
        /// Set LOD quality
        /// Đặt chất lượng LOD
        /// </summary>
        public void SetLODQuality(int quality)
        {
            switch (quality)
            {
                case 0: // Low
                    lodBias = 0.5f;
                    maxLODLevel = 2;
                    lodDistance = 30f;
                    break;

                case 1: // Medium
                    lodBias = 1.0f;
                    maxLODLevel = 1;
                    lodDistance = 50f;
                    break;

                case 2: // High
                    lodBias = 2.0f;
                    maxLODLevel = 0;
                    lodDistance = 80f;
                    break;
            }

            QualitySettings.lodBias = lodBias;
            QualitySettings.maximumLODLevel = maxLODLevel;
        }

        /// <summary>
        /// Disable all LODs
        /// Tắt tất cả LOD
        /// </summary>
        public void DisableAllLODs()
        {
            foreach (LODGroup lodGroup in lodGroups)
            {
                if (lodGroup != null)
                {
                    lodGroup.enabled = false;
                }
            }
        }

        /// <summary>
        /// Enable all LODs
        /// Bật tất cả LOD
        /// </summary>
        public void EnableAllLODs()
        {
            foreach (LODGroup lodGroup in lodGroups)
            {
                if (lodGroup != null)
                {
                    lodGroup.enabled = true;
                }
            }
        }
    }
}
