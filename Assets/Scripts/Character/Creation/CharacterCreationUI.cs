using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace DarkLegend.Character
{
    /// <summary>
    /// Character creation UI / Giao diện tạo nhân vật
    /// </summary>
    public class CharacterCreationUI : MonoBehaviour
    {
        [Header("UI Panels / Bảng UI")]
        [SerializeField] private GameObject classSelectionPanel;
        [SerializeField] private GameObject appearancePanel;
        [SerializeField] private GameObject nameInputPanel;
        [SerializeField] private GameObject statAllocationPanel;
        [SerializeField] private GameObject confirmationPanel;
        
        [Header("Class Selection / Chọn class")]
        [SerializeField] private Transform classButtonContainer;
        [SerializeField] private Button classButtonPrefab;
        [SerializeField] private TextMeshProUGUI classDescriptionText;
        
        [Header("Appearance / Ngoại hình")]
        [SerializeField] private Slider faceSlider;
        [SerializeField] private Slider hairStyleSlider;
        [SerializeField] private Slider hairColorSlider;
        [SerializeField] private Slider skinToneSlider;
        
        [Header("Name Input / Nhập tên")]
        [SerializeField] private TMP_InputField nameInputField;
        [SerializeField] private Button nameConfirmButton;
        
        [Header("Stat Allocation / Phân bổ chỉ số")]
        [SerializeField] private StatsUI statsUI;
        
        [Header("Navigation / Điều hướng")]
        [SerializeField] private Button nextButton;
        [SerializeField] private Button backButton;
        [SerializeField] private Button createButton;
        
        private CharacterCreation creationSystem;
        private CharacterAppearance appearanceSystem;
        private int currentStep = 0;
        
        private void Start()
        {
            SetupButtons();
            ShowStep(0);
        }
        
        /// <summary>
        /// Setup button listeners / Thiết lập listener cho nút
        /// </summary>
        private void SetupButtons()
        {
            if (nextButton != null)
                nextButton.onClick.AddListener(OnNextClicked);
                
            if (backButton != null)
                backButton.onClick.AddListener(OnBackClicked);
                
            if (createButton != null)
                createButton.onClick.AddListener(OnCreateClicked);
                
            if (nameConfirmButton != null)
                nameConfirmButton.onClick.AddListener(OnNameConfirmed);
                
            if (faceSlider != null)
                faceSlider.onValueChanged.AddListener(OnFaceChanged);
                
            if (hairStyleSlider != null)
                hairStyleSlider.onValueChanged.AddListener(OnHairStyleChanged);
                
            if (hairColorSlider != null)
                hairColorSlider.onValueChanged.AddListener(OnHairColorChanged);
                
            if (skinToneSlider != null)
                skinToneSlider.onValueChanged.AddListener(OnSkinToneChanged);
        }
        
        /// <summary>
        /// Initialize with systems / Khởi tạo với hệ thống
        /// </summary>
        public void Initialize(CharacterCreation creation, CharacterAppearance appearance)
        {
            creationSystem = creation;
            appearanceSystem = appearance;
            
            LoadAvailableClasses();
        }
        
        /// <summary>
        /// Load available classes / Tải các class có sẵn
        /// </summary>
        private void LoadAvailableClasses()
        {
            if (ClassManager.Instance == null)
                return;
                
            var classes = ClassManager.Instance.GetAvailableBaseClasses();
            
            foreach (var classData in classes)
            {
                CreateClassButton(classData);
            }
        }
        
        /// <summary>
        /// Create class button / Tạo nút class
        /// </summary>
        private void CreateClassButton(ClassData classData)
        {
            if (classButtonPrefab == null || classButtonContainer == null)
                return;
                
            var button = Instantiate(classButtonPrefab, classButtonContainer);
            var buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            
            if (buttonText != null)
                buttonText.text = classData.ClassName;
                
            button.onClick.AddListener(() => OnClassSelected(classData));
        }
        
        /// <summary>
        /// Handle class selected / Xử lý class được chọn
        /// </summary>
        private void OnClassSelected(ClassData classData)
        {
            if (creationSystem != null)
            {
                creationSystem.SelectClass(classData);
                SetText(classDescriptionText, classData.Description);
            }
        }
        
        /// <summary>
        /// Show step / Hiển thị bước
        /// </summary>
        private void ShowStep(int step)
        {
            currentStep = step;
            
            // Hide all panels / Ẩn tất cả bảng
            classSelectionPanel?.SetActive(false);
            appearancePanel?.SetActive(false);
            nameInputPanel?.SetActive(false);
            statAllocationPanel?.SetActive(false);
            confirmationPanel?.SetActive(false);
            
            // Show current panel / Hiển thị bảng hiện tại
            switch (step)
            {
                case 0:
                    classSelectionPanel?.SetActive(true);
                    break;
                case 1:
                    appearancePanel?.SetActive(true);
                    break;
                case 2:
                    nameInputPanel?.SetActive(true);
                    break;
                case 3:
                    statAllocationPanel?.SetActive(true);
                    if (creationSystem != null && statsUI != null)
                        statsUI.SetStats(creationSystem.GetCurrentStats());
                    break;
                case 4:
                    confirmationPanel?.SetActive(true);
                    break;
            }
            
            UpdateNavigationButtons();
        }
        
        /// <summary>
        /// Update navigation buttons / Cập nhật nút điều hướng
        /// </summary>
        private void UpdateNavigationButtons()
        {
            if (backButton != null)
                backButton.gameObject.SetActive(currentStep > 0);
                
            if (nextButton != null)
                nextButton.gameObject.SetActive(currentStep < 4);
                
            if (createButton != null)
                createButton.gameObject.SetActive(currentStep == 4);
        }
        
        private void OnNextClicked()
        {
            if (currentStep < 4)
                ShowStep(currentStep + 1);
        }
        
        private void OnBackClicked()
        {
            if (currentStep > 0)
                ShowStep(currentStep - 1);
        }
        
        private void OnCreateClicked()
        {
            if (creationSystem != null)
            {
                var characterData = creationSystem.CreateCharacter();
                if (characterData != null)
                {
                    Debug.Log("Character created successfully!");
                    // TODO: Load game with character
                }
            }
        }
        
        private void OnNameConfirmed()
        {
            if (creationSystem != null && nameInputField != null)
            {
                creationSystem.SetCharacterName(nameInputField.text);
            }
        }
        
        private void OnFaceChanged(float value)
        {
            if (appearanceSystem != null)
                appearanceSystem.SetFace((int)value);
        }
        
        private void OnHairStyleChanged(float value)
        {
            if (appearanceSystem != null)
                appearanceSystem.SetHairStyle((int)value);
        }
        
        private void OnHairColorChanged(float value)
        {
            if (appearanceSystem != null)
                appearanceSystem.SetHairColorByIndex((int)value);
        }
        
        private void OnSkinToneChanged(float value)
        {
            if (appearanceSystem != null)
                appearanceSystem.SetSkinTone((int)value);
        }
        
        private void SetText(TextMeshProUGUI textElement, string text)
        {
            if (textElement != null)
                textElement.text = text;
        }
    }
}
