using UnityEngine;
using UnityEngine.UI;

namespace DarkLegend.UI
{
    /// <summary>
    /// Minimap UI display
    /// Hiển thị minimap UI
    /// </summary>
    public class MinimapUI : MonoBehaviour
    {
        [Header("Minimap Camera")]
        public Camera minimapCamera;
        public float cameraHeight = 20f;
        public float cameraSize = 15f;
        
        [Header("Follow Target")]
        public Transform followTarget;
        
        [Header("UI Elements")]
        public RawImage minimapImage;
        public RectTransform minimapContainer;
        
        [Header("Map Settings")]
        public bool rotateWithPlayer = true;
        public LayerMask minimapLayers;
        
        private void Start()
        {
            // Find player if not assigned
            if (followTarget == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag(Utils.Constants.TAG_PLAYER);
                if (player != null)
                {
                    followTarget = player.transform;
                }
            }
            
            // Setup minimap camera if not assigned
            if (minimapCamera == null)
            {
                SetupMinimapCamera();
            }
            
            // Configure camera
            if (minimapCamera != null)
            {
                minimapCamera.orthographic = true;
                minimapCamera.orthographicSize = cameraSize;
                minimapCamera.cullingMask = minimapLayers;
                
                // Set to render to texture if using RawImage
                if (minimapImage != null)
                {
                    RenderTexture rt = new RenderTexture(256, 256, 16);
                    minimapCamera.targetTexture = rt;
                    minimapImage.texture = rt;
                }
            }
        }
        
        private void LateUpdate()
        {
            if (followTarget != null && minimapCamera != null)
            {
                UpdateMinimapPosition();
            }
        }
        
        /// <summary>
        /// Setup minimap camera
        /// Thiết lập camera minimap
        /// </summary>
        private void SetupMinimapCamera()
        {
            GameObject camObj = new GameObject("MinimapCamera");
            minimapCamera = camObj.AddComponent<Camera>();
            minimapCamera.orthographic = true;
            minimapCamera.orthographicSize = cameraSize;
            minimapCamera.clearFlags = CameraClearFlags.SolidColor;
            minimapCamera.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 1f);
        }
        
        /// <summary>
        /// Update minimap camera position
        /// Cập nhật vị trí camera minimap
        /// </summary>
        private void UpdateMinimapPosition()
        {
            // Position camera above player
            Vector3 newPosition = followTarget.position;
            newPosition.y += cameraHeight;
            minimapCamera.transform.position = newPosition;
            
            // Rotate with player if enabled
            if (rotateWithPlayer)
            {
                Vector3 rotation = minimapCamera.transform.eulerAngles;
                rotation.y = followTarget.eulerAngles.y;
                minimapCamera.transform.eulerAngles = rotation;
            }
            else
            {
                // Look straight down
                minimapCamera.transform.eulerAngles = new Vector3(90f, 0f, 0f);
            }
        }
        
        /// <summary>
        /// Toggle minimap display
        /// Bật/tắt hiển thị minimap
        /// </summary>
        public void ToggleMinimap()
        {
            if (minimapContainer != null)
            {
                minimapContainer.gameObject.SetActive(!minimapContainer.gameObject.activeSelf);
            }
        }
        
        /// <summary>
        /// Set minimap size
        /// Đặt kích thước minimap
        /// </summary>
        public void SetMinimapSize(float size)
        {
            if (minimapCamera != null)
            {
                minimapCamera.orthographicSize = size;
            }
        }
        
        /// <summary>
        /// Set camera height
        /// Đặt độ cao camera
        /// </summary>
        public void SetCameraHeight(float height)
        {
            cameraHeight = height;
        }
    }
}
