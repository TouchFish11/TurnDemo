using System.Collections.Generic;
using Core.Loader.Object;
using Core.Service;
using Core.UI;
using Core.Utility;
using HotUpdate.Config;
using HotUpdate.Extension;
using TMPro;

namespace HotUpdate.Task.UI
{
    /// <summary>
    /// 任务类型容器
    /// </summary>
    public class TaskTypeContainer : UIBehaviourBase
    {
        [Inject] private TextMeshProUGUI txtTaskName;

        private IPrefabLoader _prefabLoader;
        private readonly List<TaskItem> taskItems = new();
        private readonly Dictionary<string, TaskItem> idToItemMap = new();
        private int taskType;
        private bool isExpand = true;

        protected override void Awake()
        {
            base.Awake();
            _prefabLoader = ServiceLocator.Get<IPrefabLoader>();
        }

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
        /// <param name="taskType"></param>
        public void Init(int taskType)
        {
            this.taskType = taskType;
            txtTaskName.text = taskType.TaskTypeToStr();
        }

        /// <summary>
        /// 是否包含该ID的任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ContainTask(string id)
        {
            foreach (var cacheId in idToItemMap.Keys)
            {
                if (TextUtility.Split(cacheId, 7)[0] == TextUtility.Split(id, 7)[0])
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="taskItem"></param>
        public void AddItem(TaskItem taskItem)
        {
            //taskDatas.Add(taskData);
            taskItems.Add(taskItem);
            idToItemMap.Add(taskItem.TaskId, taskItem);
        }

        /// <summary>
        /// 默认选择第一个任务项
        /// </summary>
        public void DefaultSelectFirstTask()
        {
            if (taskItems.Count > 0)
            {
                taskItems[0].Select();
            }
        }

        /// <summary>
        /// 选择任务
        /// 使该任务项被选中
        /// </summary>
        /// <param name="id"></param>
        public void SelectTask(string id)
        {
            if (idToItemMap.TryGetValue(id, out var taskItem))
            {
                taskItem.Select();
            }
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
            foreach (var taskItem in taskItems)
            {
                taskItem.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 清理任务项
        /// </summary>
        public void ClearItem()
        {
            foreach (var taskItem in taskItems)
            {
                _prefabLoader.CollectAsset(taskItem.gameObject);
            }
            taskItems.Clear();
            _prefabLoader.RealseAsset(AbKeyCollection.Ui,  ResKeyCollection.TaskItem);
        }
    }
}
