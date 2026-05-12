using System.Collections.Generic;
using Core.AssetBundles.Management;
using Core.DI;
using Core.UI;
using HotUpdate.Common.Config.Quest;
using HotUpdate.Common.Quest;
using TMPro;

namespace HotUpdate.Game.Quests.UI
{
    /// <summary>
    /// 任务类型容器
    /// </summary>
    public class QuestTypeContainer : UIBehaviourBase
    {
        [Inject] private TextMeshProUGUI txtTaskName;
        
        private readonly List<PoolObject<TaskItem>> taskItems = new();
        private readonly Dictionary<int, PoolObject<TaskItem>> idToItemMap = new();
        private EQuestType taskType;
        private bool isExpand = true;
        
        protected override void OnButtonClick(string btnName)
        {
            switch (btnName)
            {
                case "btnTaskSummary":
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
            this.taskType = questType;
            txtTaskName.text = QuestUtil.ConvetTo(questType);
        }

        /// <summary>
        /// 是否包含该ID的任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ContainTask(int id)
        {
            return idToItemMap.ContainsKey(id);
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="poolObject"></param>
        public void AddItem(PoolObject<TaskItem> poolObject)
        {
            taskItems.Add(poolObject);
            idToItemMap.Add(poolObject.Obj.TaskId, poolObject);
        }

        /// <summary>
        /// 默认选择第一个任务项
        /// </summary>
        public void DefaultSelectFirstTask()
        {
            if (taskItems.Count > 0)
            {
                taskItems[0].Obj.Select();
            }
        }

        /// <summary>
        /// 选择任务
        /// 使该任务项被选中
        /// </summary>
        /// <param name="id"></param>
        public void SelectTask(int id)
        {
            if (idToItemMap.TryGetValue(id, out var taskItem))
            {
                taskItem.Obj.Select();
            }
        }

        /// <summary>
        /// 折叠隐藏该类型的任务项
        /// </summary>
        private void Fold()
        {
            foreach (var taskItem in taskItems)
            {
                taskItem.Obj.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 拓展显示该类型的任务项
        /// </summary>
        private void Expand()
        {
            foreach (var poolObject in taskItems)
            {
                poolObject.Collect();
            }
        }

        /// <summary>
        /// 清理任务项
        /// </summary>
        public void ClearItem()
        {
            foreach (var poolObject in taskItems)
            {
                poolObject.Collect();
            }
            taskItems.Clear();
        }
    }
}
