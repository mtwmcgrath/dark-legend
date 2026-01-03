using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DarkLegend.Maps.Minimap
{
    /// <summary>
    /// Hệ thống minimap / Minimap system
    /// Displays minimap with player position and icons
    /// </summary>
    public class MinimapSystem : MonoBehaviour
    {
        [Header("Minimap Configuration")]
        [Tooltip("Camera minimap / Minimap camera")]
        [SerializeField] private Camera minimapCamera;
        
        [Tooltip("Kích thước minimap / Minimap size")]
        [SerializeField] private Vector2 minimapSize = new Vector2(200, 200);
        
        [Tooltip("Zoom level / Zoom level")]
        [SerializeField] private float zoomLevel = 1f;
        
        [Tooltip("Rotation với player / Rotate with player")]
        [SerializeField] private bool rotateWithPlayer = true;
        
        [Header("UI Elements")]
        [Tooltip("RawImage minimap / Minimap RawImage")]
        [SerializeField] private RawImage minimapImage;
        
        [Tooltip("Player icon / Player icon")]
        [SerializeField] private Image playerIcon;
        
        [Header("Display Settings")]
        [Tooltip("Hiển thị NPCs / Show NPCs")]
        [SerializeField] private bool showNPCs = true;
        
        [Tooltip("Hiển thị monsters / Show monsters")]
        [SerializeField] private bool showMonsters = false;
        
        [Tooltip("Hiển thị portals / Show portals")]
        [SerializeField] private bool showPortals = true;
        
        [Tooltip("Hiển thị party members / Show party")]
        [SerializeField] private bool showPartyMembers = true;
        
        private GameObject player;
        private List<MinimapIcon> activeIcons = new List<MinimapIcon>();
        
        private void Start()
        {
            InitializeMinimap();
        }
        
        private void Update()
        {
            UpdateMinimapPosition();
            UpdateMinimapRotation();
        }
        
        /// <summary>
        /// Khởi tạo minimap / Initialize minimap
        /// </summary>
        private void InitializeMinimap()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            
            if (minimapCamera == null)
            {
                CreateMinimapCamera();
            }
            
            Debug.Log("[MinimapSystem] Minimap initialized");
        }
        
        /// <summary>
        /// Tạo camera minimap / Create minimap camera
        /// </summary>
        private void CreateMinimapCamera()
        {
            GameObject camObj = new GameObject("MinimapCamera");
            minimapCamera = camObj.AddComponent<Camera>();
            minimapCamera.orthographic = true;
            minimapCamera.orthographicSize = 50f / zoomLevel;
            minimapCamera.cullingMask = LayerMask.GetMask("Minimap");
            minimapCamera.clearFlags = CameraClearFlags.SolidColor;
            minimapCamera.backgroundColor = Color.black;
            
            // Set camera to render to texture
            RenderTexture rt = new RenderTexture((int)minimapSize.x, (int)minimapSize.y, 16);
            minimapCamera.targetTexture = rt;
            
            if (minimapImage != null)
            {
                minimapImage.texture = rt;
            }
        }
        
        /// <summary>
        /// Cập nhật vị trí minimap / Update minimap position
        /// </summary>
        private void UpdateMinimapPosition()
        {
            if (player != null && minimapCamera != null)
            {
                Vector3 playerPos = player.transform.position;
                minimapCamera.transform.position = new Vector3(playerPos.x, playerPos.y + 100f, playerPos.z);
            }
        }
        
        /// <summary>
        /// Cập nhật rotation minimap / Update minimap rotation
        /// </summary>
        private void UpdateMinimapRotation()
        {
            if (rotateWithPlayer && player != null && minimapCamera != null)
            {
                float playerRotation = player.transform.eulerAngles.y;
                minimapCamera.transform.rotation = Quaternion.Euler(90f, playerRotation, 0f);
            }
        }
        
        /// <summary>
        /// Đăng ký icon / Register minimap icon
        /// </summary>
        public void RegisterIcon(MinimapIcon icon)
        {
            if (!activeIcons.Contains(icon))
            {
                activeIcons.Add(icon);
            }
        }
        
        /// <summary>
        /// Hủy đăng ký icon / Unregister minimap icon
        /// </summary>
        public void UnregisterIcon(MinimapIcon icon)
        {
            activeIcons.Remove(icon);
        }
        
        /// <summary>
        /// Set zoom level / Set zoom level
        /// </summary>
        public void SetZoom(float zoom)
        {
            zoomLevel = Mathf.Clamp(zoom, 0.5f, 3f);
            
            if (minimapCamera != null)
            {
                minimapCamera.orthographicSize = 50f / zoomLevel;
            }
        }
        
        /// <summary>
        /// Toggle minimap / Toggle minimap visibility
        /// </summary>
        public void ToggleMinimap()
        {
            if (minimapImage != null)
            {
                minimapImage.gameObject.SetActive(!minimapImage.gameObject.activeSelf);
            }
        }
    }
}
