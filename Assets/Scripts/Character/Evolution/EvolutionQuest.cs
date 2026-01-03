using UnityEngine;
using System;

namespace DarkLegend.Character
{
    /// <summary>
    /// Evolution quest / Nhiệm vụ tiến hóa
    /// </summary>
    public class EvolutionQuest : MonoBehaviour
    {
        [Header("Quest Info / Thông tin nhiệm vụ")]
        public string QuestId;
        public string QuestName;
        [TextArea(3, 10)]
        public string QuestDescription;
        public CharacterClassType ForClass;
        
        [Header("Quest Requirements / Yêu cầu nhiệm vụ")]
        public QuestObjective[] Objectives;
        
        [Header("Quest Rewards / Phần thưởng nhiệm vụ")]
        public int ExperienceReward;
        public int ZenReward;
        public string[] ItemRewards;
        
        // Quest state / Trạng thái nhiệm vụ
        private bool isActive;
        private bool isCompleted;
        private int[] objectiveProgress;
        
        // Events / Sự kiện
        public event Action<EvolutionQuest> OnQuestStarted;
        public event Action<EvolutionQuest> OnQuestCompleted;
        public event Action<EvolutionQuest, int> OnObjectiveProgress;
        
        private void Awake()
        {
            objectiveProgress = new int[Objectives.Length];
        }
        
        /// <summary>
        /// Start the quest / Bắt đầu nhiệm vụ
        /// </summary>
        public void StartQuest()
        {
            if (isActive || isCompleted)
                return;
                
            isActive = true;
            Array.Clear(objectiveProgress, 0, objectiveProgress.Length);
            
            OnQuestStarted?.Invoke(this);
            Debug.Log($"Quest started: {QuestName}");
        }
        
        /// <summary>
        /// Update objective progress / Cập nhật tiến độ mục tiêu
        /// </summary>
        public void UpdateObjective(int objectiveIndex, int amount = 1)
        {
            if (!isActive || isCompleted)
                return;
                
            if (objectiveIndex < 0 || objectiveIndex >= Objectives.Length)
                return;
                
            objectiveProgress[objectiveIndex] += amount;
            objectiveProgress[objectiveIndex] = Mathf.Min(
                objectiveProgress[objectiveIndex], 
                Objectives[objectiveIndex].RequiredAmount
            );
            
            OnObjectiveProgress?.Invoke(this, objectiveIndex);
            
            // Check if all objectives complete / Kiểm tra tất cả mục tiêu hoàn thành
            CheckCompletion();
        }
        
        /// <summary>
        /// Check if quest is complete / Kiểm tra nhiệm vụ đã hoàn thành
        /// </summary>
        private void CheckCompletion()
        {
            for (int i = 0; i < Objectives.Length; i++)
            {
                if (objectiveProgress[i] < Objectives[i].RequiredAmount)
                    return;
            }
            
            CompleteQuest();
        }
        
        /// <summary>
        /// Complete the quest / Hoàn thành nhiệm vụ
        /// </summary>
        private void CompleteQuest()
        {
            if (isCompleted)
                return;
                
            isCompleted = true;
            isActive = false;
            
            OnQuestCompleted?.Invoke(this);
            Debug.Log($"Quest completed: {QuestName}");
        }
        
        /// <summary>
        /// Get objective progress / Lấy tiến độ mục tiêu
        /// </summary>
        public int GetObjectiveProgress(int index)
        {
            if (index < 0 || index >= objectiveProgress.Length)
                return 0;
            return objectiveProgress[index];
        }
        
        /// <summary>
        /// Check if quest is active / Kiểm tra nhiệm vụ đang hoạt động
        /// </summary>
        public bool IsActive()
        {
            return isActive;
        }
        
        /// <summary>
        /// Check if quest is completed / Kiểm tra nhiệm vụ đã hoàn thành
        /// </summary>
        public bool IsCompleted()
        {
            return isCompleted;
        }
    }
    
    /// <summary>
    /// Quest objective / Mục tiêu nhiệm vụ
    /// </summary>
    [System.Serializable]
    public class QuestObjective
    {
        public string Description;
        public QuestObjectiveType Type;
        public string TargetId; // Monster ID, Item ID, etc.
        public int RequiredAmount;
    }
    
    /// <summary>
    /// Quest objective types / Loại mục tiêu nhiệm vụ
    /// </summary>
    public enum QuestObjectiveType
    {
        KillMonster,
        CollectItem,
        ReachLocation,
        TalkToNPC,
        UseSkill,
        LevelUp
    }
}
