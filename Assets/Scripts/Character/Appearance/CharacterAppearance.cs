using UnityEngine;
using System;

namespace DarkLegend.Character
{
    /// <summary>
    /// Character appearance data / Dữ liệu ngoại hình nhân vật
    /// </summary>
    [System.Serializable]
    public class CharacterAppearanceData
    {
        [Header("Face / Khuôn mặt")]
        public int FaceIndex; // 0-9 (10 options)
        
        [Header("Hair / Tóc")]
        public int HairStyleIndex; // 0-14 (15 options)
        public Color HairColor; // 20 color options
        
        [Header("Skin / Da")]
        public int SkinToneIndex; // 0-9 (10 options)
        
        [Header("Body / Cơ thể")]
        public float Height = 1.0f;
        public float BodyScale = 1.0f;
        
        public CharacterAppearanceData()
        {
            FaceIndex = 0;
            HairStyleIndex = 0;
            HairColor = Color.black;
            SkinToneIndex = 0;
            Height = 1.0f;
            BodyScale = 1.0f;
        }
    }
    
    /// <summary>
    /// Character appearance customization / Tùy chỉnh ngoại hình nhân vật
    /// </summary>
    public class CharacterAppearance : MonoBehaviour
    {
        [Header("Appearance Data / Dữ liệu ngoại hình")]
        [SerializeField] private CharacterAppearanceData currentAppearance;
        
        [Header("Model Parts / Phần mô hình")]
        [SerializeField] private GameObject[] faceMeshes; // 10 face options
        [SerializeField] private GameObject[] hairMeshes; // 15 hair options
        [SerializeField] private Renderer hairRenderer;
        [SerializeField] private Renderer skinRenderer;
        [SerializeField] private Transform bodyTransform;
        
        [Header("Skin Tones / Màu da")]
        [SerializeField] private Color[] skinTones = new Color[10]; // 10 skin tone options
        
        [Header("Hair Colors / Màu tóc")]
        [SerializeField] private Color[] hairColors = new Color[20]; // 20 hair color options
        
        // Events / Sự kiện
        public event Action<CharacterAppearanceData> OnAppearanceChanged;
        
        private void Awake()
        {
            if (currentAppearance == null)
            {
                currentAppearance = new CharacterAppearanceData();
            }
            
            InitializeDefaultColors();
        }
        
        /// <summary>
        /// Initialize default colors / Khởi tạo màu mặc định
        /// </summary>
        private void InitializeDefaultColors()
        {
            // Default skin tones / Màu da mặc định
            if (skinTones.Length == 0 || skinTones[0] == Color.clear)
            {
                skinTones = new Color[10]
                {
                    new Color(1.0f, 0.87f, 0.78f),  // Light
                    new Color(0.96f, 0.80f, 0.69f), // Peach
                    new Color(0.92f, 0.74f, 0.56f), // Tan
                    new Color(0.82f, 0.62f, 0.44f), // Medium
                    new Color(0.72f, 0.53f, 0.37f), // Olive
                    new Color(0.61f, 0.43f, 0.29f), // Brown
                    new Color(0.49f, 0.33f, 0.22f), // Dark Brown
                    new Color(0.36f, 0.24f, 0.16f), // Deep Brown
                    new Color(0.27f, 0.18f, 0.12f), // Very Dark
                    new Color(0.18f, 0.12f, 0.08f)  // Darkest
                };
            }
            
            // Default hair colors / Màu tóc mặc định
            if (hairColors.Length == 0 || hairColors[0] == Color.clear)
            {
                hairColors = new Color[20]
                {
                    Color.black,
                    new Color(0.2f, 0.1f, 0.05f),  // Dark Brown
                    new Color(0.4f, 0.2f, 0.1f),   // Brown
                    new Color(0.6f, 0.4f, 0.2f),   // Light Brown
                    new Color(0.9f, 0.7f, 0.3f),   // Blonde
                    new Color(1.0f, 0.9f, 0.5f),   // Light Blonde
                    new Color(0.8f, 0.1f, 0.1f),   // Red
                    new Color(0.9f, 0.3f, 0.2f),   // Auburn
                    new Color(0.5f, 0.5f, 0.5f),   // Gray
                    Color.white,
                    new Color(0.1f, 0.3f, 0.8f),   // Blue
                    new Color(0.8f, 0.1f, 0.8f),   // Purple
                    new Color(0.1f, 0.8f, 0.3f),   // Green
                    new Color(0.9f, 0.5f, 0.1f),   // Orange
                    new Color(1.0f, 0.8f, 0.9f),   // Pink
                    new Color(0.3f, 0.9f, 0.9f),   // Cyan
                    new Color(0.9f, 0.9f, 0.1f),   // Yellow
                    new Color(0.5f, 0.2f, 0.7f),   // Violet
                    new Color(0.3f, 0.5f, 0.3f),   // Dark Green
                    new Color(0.6f, 0.3f, 0.1f)    // Copper
                };
            }
        }
        
        /// <summary>
        /// Set appearance / Đặt ngoại hình
        /// </summary>
        public void SetAppearance(CharacterAppearanceData appearance)
        {
            currentAppearance = appearance;
            ApplyAppearance();
            OnAppearanceChanged?.Invoke(appearance);
        }
        
        /// <summary>
        /// Apply appearance to model / Áp dụng ngoại hình lên mô hình
        /// </summary>
        private void ApplyAppearance()
        {
            ApplyFace(currentAppearance.FaceIndex);
            ApplyHair(currentAppearance.HairStyleIndex);
            ApplyHairColor(currentAppearance.HairColor);
            ApplySkinTone(currentAppearance.SkinToneIndex);
            ApplyBodyScale(currentAppearance.Height, currentAppearance.BodyScale);
        }
        
        /// <summary>
        /// Set face / Đặt khuôn mặt
        /// </summary>
        public void SetFace(int faceIndex)
        {
            currentAppearance.FaceIndex = Mathf.Clamp(faceIndex, 0, 9);
            ApplyFace(currentAppearance.FaceIndex);
            OnAppearanceChanged?.Invoke(currentAppearance);
        }
        
        private void ApplyFace(int faceIndex)
        {
            if (faceMeshes == null || faceMeshes.Length == 0)
                return;
                
            for (int i = 0; i < faceMeshes.Length; i++)
            {
                if (faceMeshes[i] != null)
                    faceMeshes[i].SetActive(i == faceIndex);
            }
        }
        
        /// <summary>
        /// Set hair style / Đặt kiểu tóc
        /// </summary>
        public void SetHairStyle(int hairIndex)
        {
            currentAppearance.HairStyleIndex = Mathf.Clamp(hairIndex, 0, 14);
            ApplyHair(currentAppearance.HairStyleIndex);
            OnAppearanceChanged?.Invoke(currentAppearance);
        }
        
        private void ApplyHair(int hairIndex)
        {
            if (hairMeshes == null || hairMeshes.Length == 0)
                return;
                
            for (int i = 0; i < hairMeshes.Length; i++)
            {
                if (hairMeshes[i] != null)
                    hairMeshes[i].SetActive(i == hairIndex);
            }
        }
        
        /// <summary>
        /// Set hair color / Đặt màu tóc
        /// </summary>
        public void SetHairColor(Color color)
        {
            currentAppearance.HairColor = color;
            ApplyHairColor(color);
            OnAppearanceChanged?.Invoke(currentAppearance);
        }
        
        /// <summary>
        /// Set hair color by index / Đặt màu tóc theo chỉ số
        /// </summary>
        public void SetHairColorByIndex(int colorIndex)
        {
            if (colorIndex >= 0 && colorIndex < hairColors.Length)
            {
                SetHairColor(hairColors[colorIndex]);
            }
        }
        
        private void ApplyHairColor(Color color)
        {
            if (hairRenderer != null)
            {
                hairRenderer.material.color = color;
            }
        }
        
        /// <summary>
        /// Set skin tone / Đặt màu da
        /// </summary>
        public void SetSkinTone(int toneIndex)
        {
            currentAppearance.SkinToneIndex = Mathf.Clamp(toneIndex, 0, 9);
            ApplySkinTone(currentAppearance.SkinToneIndex);
            OnAppearanceChanged?.Invoke(currentAppearance);
        }
        
        private void ApplySkinTone(int toneIndex)
        {
            if (skinRenderer != null && toneIndex < skinTones.Length)
            {
                skinRenderer.material.color = skinTones[toneIndex];
            }
        }
        
        /// <summary>
        /// Set body scale / Đặt kích thước cơ thể
        /// </summary>
        public void SetBodyScale(float height, float scale)
        {
            currentAppearance.Height = Mathf.Clamp(height, 0.8f, 1.2f);
            currentAppearance.BodyScale = Mathf.Clamp(scale, 0.8f, 1.2f);
            ApplyBodyScale(currentAppearance.Height, currentAppearance.BodyScale);
            OnAppearanceChanged?.Invoke(currentAppearance);
        }
        
        private void ApplyBodyScale(float height, float scale)
        {
            if (bodyTransform != null)
            {
                bodyTransform.localScale = new Vector3(scale, height, scale);
            }
        }
        
        /// <summary>
        /// Get current appearance / Lấy ngoại hình hiện tại
        /// </summary>
        public CharacterAppearanceData GetCurrentAppearance()
        {
            return currentAppearance;
        }
        
        /// <summary>
        /// Get available hair colors / Lấy các màu tóc có sẵn
        /// </summary>
        public Color[] GetAvailableHairColors()
        {
            return hairColors;
        }
        
        /// <summary>
        /// Get available skin tones / Lấy các màu da có sẵn
        /// </summary>
        public Color[] GetAvailableSkinTones()
        {
            return skinTones;
        }
    }
}
