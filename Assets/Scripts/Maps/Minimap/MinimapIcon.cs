using UnityEngine;
using UnityEngine.UI;

namespace DarkLegend.Maps.Minimap
{
    /// <summary>
    /// Icon trên minimap / Minimap icon
    /// Represents objects on the minimap
    /// </summary>
    public class MinimapIcon : MonoBehaviour
    {
        [Header("Icon Configuration")]
        [Tooltip("Loại icon / Icon type")]
        [SerializeField] private IconType iconType = IconType.Monster;
        
        [Tooltip("Sprite icon / Icon sprite")]
        [SerializeField] private Sprite iconSprite;
        
        [Tooltip("Màu icon / Icon color")]
        [SerializeField] private Color iconColor = Color.white;
        
        [Tooltip("Kích thước / Icon size")]
        [SerializeField] private float iconSize = 1f;
        
        [Tooltip("Hiển thị / Is visible")]
        [SerializeField] private bool isVisible = true;
        
        [Header("Behavior")]
        [Tooltip("Xoay theo hướng / Rotate with object")]
        [SerializeField] private bool rotateWithObject = false;
        
        [Tooltip("Fade khi xa / Fade with distance")]
        [SerializeField] private bool fadeWithDistance = false;
        
        [Tooltip("Khoảng cách fade / Fade distance")]
        [SerializeField] private float fadeDistance = 100f;
        
        private GameObject iconObject;
        private SpriteRenderer iconRenderer;
        
        private void Start()
        {
            CreateIcon();
            RegisterWithMinimap();
        }
        
        private void Update()
        {
            if (!isVisible) return;
            
            UpdateIconPosition();
            UpdateIconRotation();
            UpdateIconFade();
        }
        
        private void OnDestroy()
        {
            UnregisterFromMinimap();
            
            if (iconObject != null)
            {
                Destroy(iconObject);
            }
        }
        
        /// <summary>
        /// Tạo icon object / Create icon object
        /// </summary>
        private void CreateIcon()
        {
            iconObject = new GameObject($"MinimapIcon_{iconType}");
            iconObject.transform.SetParent(transform);
            iconObject.layer = LayerMask.NameToLayer("Minimap");
            
            iconRenderer = iconObject.AddComponent<SpriteRenderer>();
            iconRenderer.sprite = iconSprite;
            iconRenderer.color = iconColor;
            iconRenderer.sortingOrder = GetSortingOrder();
            
            iconObject.transform.localScale = Vector3.one * iconSize;
            iconObject.transform.localPosition = Vector3.zero;
        }
        
        /// <summary>
        /// Cập nhật vị trí icon / Update icon position
        /// </summary>
        private void UpdateIconPosition()
        {
            if (iconObject != null)
            {
                iconObject.transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y + 100f, // Minimap layer height
                    transform.position.z
                );
            }
        }
        
        /// <summary>
        /// Cập nhật rotation icon / Update icon rotation
        /// </summary>
        private void UpdateIconRotation()
        {
            if (rotateWithObject && iconObject != null)
            {
                iconObject.transform.rotation = Quaternion.Euler(90f, 0f, -transform.eulerAngles.y);
            }
        }
        
        /// <summary>
        /// Cập nhật fade icon / Update icon fade
        /// </summary>
        private void UpdateIconFade()
        {
            if (!fadeWithDistance || iconRenderer == null) return;
            
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                float alpha = 1f - Mathf.Clamp01(distance / fadeDistance);
                
                Color color = iconRenderer.color;
                color.a = alpha;
                iconRenderer.color = color;
            }
        }
        
        /// <summary>
        /// Đăng ký với minimap system / Register with minimap
        /// </summary>
        private void RegisterWithMinimap()
        {
            MinimapSystem minimap = FindObjectOfType<MinimapSystem>();
            if (minimap != null)
            {
                minimap.RegisterIcon(this);
            }
        }
        
        /// <summary>
        /// Hủy đăng ký khỏi minimap / Unregister from minimap
        /// </summary>
        private void UnregisterFromMinimap()
        {
            MinimapSystem minimap = FindObjectOfType<MinimapSystem>();
            if (minimap != null)
            {
                minimap.UnregisterIcon(this);
            }
        }
        
        /// <summary>
        /// Lấy sorting order / Get sorting order
        /// </summary>
        private int GetSortingOrder()
        {
            switch (iconType)
            {
                case IconType.Player:
                    return 100;
                case IconType.PartyMember:
                    return 90;
                case IconType.NPC:
                    return 80;
                case IconType.Portal:
                    return 70;
                case IconType.Boss:
                    return 60;
                case IconType.Monster:
                    return 50;
                case IconType.Item:
                    return 40;
                default:
                    return 0;
            }
        }
        
        /// <summary>
        /// Set visibility / Set icon visibility
        /// </summary>
        public void SetVisible(bool visible)
        {
            isVisible = visible;
            
            if (iconObject != null)
            {
                iconObject.SetActive(visible);
            }
        }
        
        /// <summary>
        /// Set color / Set icon color
        /// </summary>
        public void SetColor(Color color)
        {
            iconColor = color;
            
            if (iconRenderer != null)
            {
                iconRenderer.color = color;
            }
        }
    }
    
    /// <summary>
    /// Loại icon / Icon types
    /// </summary>
    public enum IconType
    {
        Player,         // Player
        PartyMember,    // Thành viên party
        NPC,            // NPC
        Portal,         // Portal
        Boss,           // Boss
        Monster,        // Quái thường
        Item,           // Item
        Objective       // Mục tiêu
    }
}
