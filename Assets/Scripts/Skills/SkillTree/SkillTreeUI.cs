using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace DarkLegend.Skills
{
    /// <summary>
    /// UI cho skill tree
    /// UI for skill tree
    /// </summary>
    public class SkillTreeUI : MonoBehaviour
    {
        [Header("References")]
        public SkillTree currentTree;
        public GameObject character;
        public SkillManager skillManager;
        
        [Header("UI Elements")]
        public Transform nodeContainer;
        public GameObject nodeButtonPrefab;
        public Text skillPointsText;
        public Text treeNameText;
        
        [Header("Details Panel")]
        public GameObject detailsPanel;
        public Text skillNameText;
        public Text skillDescriptionText;
        public Text skillLevelText;
        public Text skillCostText;
        public Button learnButton;
        public Button levelUpButton;
        
        [Header("Layout Settings")]
        public float tierSpacing = 150f;
        public float nodeSpacing = 100f;
        
        private Dictionary<SkillNode, GameObject> nodeButtons = new Dictionary<SkillNode, GameObject>();
        private SkillNode selectedNode;
        
        /// <summary>
        /// Initialize UI / Khởi tạo UI
        /// </summary>
        private void Start()
        {
            if (character == null)
            {
                character = GameObject.FindGameObjectWithTag("Player");
            }
            
            if (skillManager == null && character != null)
            {
                skillManager = character.GetComponent<SkillManager>();
            }
            
            if (currentTree != null)
            {
                BuildTree();
            }
            
            UpdateUI();
        }
        
        /// <summary>
        /// Xây dựng skill tree UI / Build skill tree UI
        /// </summary>
        public void BuildTree()
        {
            if (currentTree == null) return;
            
            // Clear existing nodes
            ClearTree();
            
            // Set tree name
            if (treeNameText != null)
            {
                treeNameText.text = currentTree.treeName;
            }
            
            // Create node buttons
            foreach (SkillNode node in currentTree.nodes)
            {
                CreateNodeButton(node);
            }
            
            // Draw connections
            DrawConnections();
        }
        
        /// <summary>
        /// Tạo button cho node / Create button for node
        /// </summary>
        private void CreateNodeButton(SkillNode node)
        {
            if (nodeButtonPrefab == null || nodeContainer == null) return;
            
            GameObject buttonObj = Instantiate(nodeButtonPrefab, nodeContainer);
            
            // Position
            RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                float x = node.positionInTier * nodeSpacing;
                float y = -node.tier * tierSpacing;
                rectTransform.anchoredPosition = new Vector2(x, y);
            }
            
            // Setup button
            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnNodeClicked(node));
            }
            
            // Set icon
            if (node.skillData != null && node.skillData.icon != null)
            {
                Image iconImage = buttonObj.GetComponentInChildren<Image>();
                if (iconImage != null)
                {
                    iconImage.sprite = node.skillData.icon;
                }
            }
            
            // Store reference
            nodeButtons[node] = buttonObj;
            
            // Update visual state
            UpdateNodeVisual(node);
        }
        
        /// <summary>
        /// Update node visual dựa trên state / Update node visual based on state
        /// </summary>
        private void UpdateNodeVisual(SkillNode node)
        {
            if (!nodeButtons.ContainsKey(node)) return;
            
            GameObject buttonObj = nodeButtons[node];
            Image image = buttonObj.GetComponent<Image>();
            if (image == null) return;
            
            if (node.isLearned)
            {
                // Learned - green
                image.color = Color.green;
            }
            else if (currentTree.CanUnlockNode(node, character))
            {
                // Can learn - yellow
                image.color = Color.yellow;
            }
            else
            {
                // Locked - gray
                image.color = Color.gray;
            }
        }
        
        /// <summary>
        /// Draw connections giữa các nodes / Draw connections between nodes
        /// </summary>
        private void DrawConnections()
        {
            // TODO: Implement line drawing between connected nodes
            // Có thể dùng LineRenderer hoặc UI Lines
        }
        
        /// <summary>
        /// Clear tree UI / Xóa tree UI
        /// </summary>
        private void ClearTree()
        {
            foreach (var kvp in nodeButtons)
            {
                if (kvp.Value != null)
                {
                    Destroy(kvp.Value);
                }
            }
            
            nodeButtons.Clear();
        }
        
        /// <summary>
        /// Xử lý khi click vào node / Handle node click
        /// </summary>
        private void OnNodeClicked(SkillNode node)
        {
            selectedNode = node;
            ShowNodeDetails(node);
        }
        
        /// <summary>
        /// Hiển thị chi tiết node / Show node details
        /// </summary>
        private void ShowNodeDetails(SkillNode node)
        {
            if (detailsPanel != null)
            {
                detailsPanel.SetActive(true);
            }
            
            if (node.skillData == null) return;
            
            // Skill name
            if (skillNameText != null)
            {
                skillNameText.text = node.skillData.skillName;
            }
            
            // Description
            if (skillDescriptionText != null)
            {
                SkillBase skill = skillManager != null ? skillManager.GetSkill(node.skillData.skillName) : null;
                int level = skill != null ? skill.currentLevel : 1;
                skillDescriptionText.text = node.skillData.GetDescription(level);
            }
            
            // Level info
            if (skillLevelText != null && skillManager != null)
            {
                SkillBase skill = skillManager.GetSkill(node.skillData.skillName);
                if (skill != null)
                {
                    skillLevelText.text = $"Level: {skill.currentLevel}/{node.skillData.maxLevel}";
                }
                else
                {
                    skillLevelText.text = "Not Learned";
                }
            }
            
            // Cost info
            if (skillCostText != null && node.skillData.cost != null)
            {
                skillCostText.text = $"MP Cost: {node.skillData.cost.GetMPCost(1)}";
            }
            
            // Buttons
            UpdateButtonStates(node);
        }
        
        /// <summary>
        /// Update button states / Cập nhật trạng thái buttons
        /// </summary>
        private void UpdateButtonStates(SkillNode node)
        {
            bool isLearned = skillManager != null && skillManager.HasSkill(node.skillData.skillName);
            bool canLearn = !isLearned && currentTree.CanUnlockNode(node, character);
            
            // Learn button
            if (learnButton != null)
            {
                learnButton.gameObject.SetActive(!isLearned);
                learnButton.interactable = canLearn;
                learnButton.onClick.RemoveAllListeners();
                learnButton.onClick.AddListener(() => OnLearnSkill(node));
            }
            
            // Level up button
            if (levelUpButton != null)
            {
                levelUpButton.gameObject.SetActive(isLearned);
                levelUpButton.onClick.RemoveAllListeners();
                levelUpButton.onClick.AddListener(() => OnLevelUpSkill(node));
            }
        }
        
        /// <summary>
        /// Xử lý học skill / Handle learn skill
        /// </summary>
        private void OnLearnSkill(SkillNode node)
        {
            if (skillManager == null) return;
            
            bool learned = node.Learn(skillManager);
            if (learned)
            {
                UpdateUI();
                ShowNodeDetails(node);
                Debug.Log($"Learned skill: {node.skillData.skillName}");
            }
        }
        
        /// <summary>
        /// Xử lý level up skill / Handle level up skill
        /// </summary>
        private void OnLevelUpSkill(SkillNode node)
        {
            if (skillManager == null) return;
            
            bool leveledUp = skillManager.LevelUpSkill(node.skillData.skillName);
            if (leveledUp)
            {
                UpdateUI();
                ShowNodeDetails(node);
                Debug.Log($"Leveled up skill: {node.skillData.skillName}");
            }
        }
        
        /// <summary>
        /// Update toàn bộ UI / Update entire UI
        /// </summary>
        private void UpdateUI()
        {
            // Update skill points
            if (skillPointsText != null && skillManager != null)
            {
                skillPointsText.text = $"Skill Points: {skillManager.availableSkillPoints}";
            }
            
            // Update all node visuals
            foreach (SkillNode node in currentTree.nodes)
            {
                UpdateNodeVisual(node);
            }
        }
    }
}
