using Framework;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.UI
{
    /// <summary>
    /// 任务界面数据
    /// </summary>
    public class TaskModel : UIModel
    {
        // 任务类型到任务容器映射
        private readonly Dictionary<int, TaskTypeContainer> taskTypeToContainerMap = new Dictionary<int, TaskTypeContainer>();
        // 当前任务信息的奖励列表
        private readonly List<ItemGrid> rewardItems = new List<ItemGrid>();
        // 当前任务信息
        private TaskInfo currentTaskInfo;
        // 是否有任务
        private bool hasTasks;
        // 是否正在追踪任务
        private bool isFollowingTask;

        /// <summary>
        /// 是否正在追踪任务
        /// </summary>
        public bool IsFollowingTask
        {
            get => isFollowingTask;
            set
            {
                isFollowingTask = value;
                TriggerDataChanged(nameof(isFollowingTask), value);
            }
        }

        public bool HasTask()
        {
            hasTasks = taskTypeToContainerMap.Count > 0;
            TriggerDataChanged(nameof(hasTasks), hasTasks);
            return hasTasks;
        }

        /// <summary>
        /// 获取当前选择的任务
        /// </summary>
        /// <returns></returns>
        public TaskInfo GetCurrentSelectTaskInfo()
        {
            return currentTaskInfo;
        }

        /// <summary>
        /// 是否包含该容器
        /// </summary>
        /// <param name="taskType"></param>
        /// <returns></returns>
        public bool ContainContainer(int taskType)
        {
            return taskTypeToContainerMap.ContainsKey(taskType);
        }

        /// <summary>
        /// 添加任务类型容器
        /// </summary>
        /// <param name="taskType"></param>
        /// <param name="taskTypeContainer"></param>
        public void AddTaskTypeContainers(int taskType, TaskTypeContainer taskTypeContainer)
        {
            taskTypeToContainerMap.Add(taskType, taskTypeContainer);
            TriggerDataChanged(nameof(taskTypeContainer), taskTypeContainer);
        }

        /// <summary>
        /// 获取容器
        /// </summary>
        /// <param name="taskType"></param>
        /// <returns></returns>
        public TaskTypeContainer GetContainer(int taskType)
        {
            return taskTypeToContainerMap[taskType];
        }

        public void SelectTrackingTask(string id)
        {
            foreach (var item in taskTypeToContainerMap.Values)
            {
                item.SelectTask(id);
            }
        }

        /// <summary>
        /// 获取第一个添加的容器
        /// </summary>
        /// <returns></returns>
        public TaskTypeContainer GetFirstContainer()
        {
            foreach (TaskTypeContainer container in taskTypeToContainerMap.Values)
            {
                return container;
            }

            return null;
        }

        /// <summary>
        /// 更新任务信息
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task UpdateTaskInfoById(string taskId)
        {
            foreach (var item in rewardItems)
            {
                PoolManager.Instance.PushObj(item.gameObject);
            }
            rewardItems.Clear();

            currentTaskInfo = BinaryDataManager.Instance.GetConfig<TaskInfoContainer>(E_ConfigLoadType.Excel).dataDic[taskId];
            int[] rewardIds = TextUtility.SplitToIntArr(currentTaskInfo.f_taskRewrardIds, 2);
            foreach (int id in rewardIds)
            {
                ItemGrid itemGrid = await ObjectBuilder.GetOrCreateInstance<ItemGrid>(E_AssetBundleType.UI, ResKeyCollection.ItemGrid, null);
                itemGrid.Init();
                rewardItems.Add(itemGrid);
            }

            TriggerDataChanged(nameof(currentTaskInfo), (currentTaskInfo, rewardItems));
            if (GameDataManager.Instance.TaskDataCollection.TryGetValue(taskId, out TaskData taskData))
            {
                IsFollowingTask = taskData.isTracking;
            }
            else
            {
                IsFollowingTask = false;
            }
        }

        public override void ClearData()
        {
            rewardItems.Clear();

            foreach (var container in taskTypeToContainerMap.Values)
            {
                container.ClearItem();
            }
            taskTypeToContainerMap.Clear();

            PoolManager.Instance.ClearType<TaskItem>();
            PoolManager.Instance.ClearType<ItemGrid>();
        }
    }
}
