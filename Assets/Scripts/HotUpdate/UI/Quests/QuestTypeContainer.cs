using System.Collections.Generic;
using Core.AssetBundles.Management;
using Core.UI;
using HotUpdate.Common.Config.Quest;
using HotUpdate.Game.Quests;
using TMPro;
using UnityEngine.UI;

namespace HotUpdate.UI.Quests
{
    /// <summary>
    /// 任务类型容器
    /// </summary>
    public class QuestTypeContainer : UIBehaviourBase
    {
        [InjectUI] private TextMeshProUGUI txtTaskName;
        [InjectUI] private Button btnTaskSummary;
        
        private readonly List<TaskItem> taskItems = new();
        private readonly Dictionary<int, TaskItem> idToItemMap = new();
        private EQuestType taskType;
        private bool isExpand = true;   // 默认展开
        
        protected override void OnButtonClick(string btnName)
        {
            switch (btnName)
            {
                case nameof(btnTaskSummary):
                    if(isExpand)
                    {
                        Fold();
                    }
                    else
                    {
                        Expand();
                    }
                    isExpand = !isExpand;
                    break;
            }
        }

        /// <summary>
        /// 初始化容器
        /// </summary>
        /// <param name="questType"></param>
        public void Init(EQuestType questType)
        {
            taskType = questType;
            txtTaskName.text = QuestUtil.ConvertQuestTypeToStr(questType);
        }

        /// <summary>
        /// 是否包含该ID的任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ContainQuest(int id)
        {
            return idToItemMap.ContainsKey(id);
        }

        /// <summary>
        /// 添加任务对象
        /// </summary>
        /// <param name="taskItem"></param>
        public void AddQuestItem(TaskItem taskItem)
        {
            taskItems.Add(taskItem);
            idToItemMap.Add(taskItem.TaskId, taskItem);
        }

        /// <summary>
        /// 选择第一个任务项
        /// </summary>
        public void SelectFirstQuest()
        {
            if (taskItems.Count > 0)
            {
                taskItems[0].Select();
            }
        }

        /// <summary>
        /// 选中该ID的任务对象
        /// </summary>
        /// <param name="id"></param>
        public bool SelectQuest(int id)
        {
            if (!idToItemMap.TryGetValue(id, out var taskItem)) 
                return false;
            
            taskItem.Select();
            return true;
        }

        /// <summary>
        /// 折叠隐藏该类型的任务项
        /// </summary>
        private void Fold()
        {
            foreach (var taskItem in taskItems)
            {
                taskItem.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 拓展显示该类型的任务项
        /// </summary>
        private void Expand()
        {
            foreach (var poolObject in taskItems)
            {
                poolObject.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 清理任务项
        /// </summary>
        /// <param name="spawner"></param>
        public void ClearItem(ObjectSpawner spawner)
        {
            foreach (var taskItem in taskItems)
            {
                spawner.Release(taskItem);
            }
            taskItems.Clear();
            idToItemMap.Clear();
        }
    }
}
