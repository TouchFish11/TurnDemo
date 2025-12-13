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
        public readonly struct DetailData
        {
            public TaskInfo TaskInfo { get; }
            public List<ItemGrid> RewardItems { get; }

            public DetailData(TaskInfo taskInfo, List<ItemGrid> rewardItems)
            {
                this.TaskInfo = taskInfo;
                this.RewardItems = rewardItems;
            }
        }

        // 任务类型到任务容器映射
        private readonly Dictionary<int, TaskTypeContainer> taskTypeToContainerMap = new Dictionary<int, TaskTypeContainer>();
        // 当前任务信息的奖励列表
        private readonly List<ItemGrid> rewardItems = new List<ItemGrid>();
        // 当前任务信息
        private TaskInfo currentTaskInfo;
        // 是否有任务
        private bool hasTasks;

        public bool HasTasks
        {
            get => hasTasks;
            set
            {
                hasTasks = value;
                TriggerDataChanged(nameof(hasTasks), value);
            }
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
        /// <param name="taskInfo"></param>
        /// <returns></returns>
        public async Task UpdateTaskInfo(TaskInfo taskInfo)
        {
            foreach (var item in rewardItems)
            {
                PoolManager.Instance.PushObj(item.gameObject);
            }
            rewardItems.Clear();

            currentTaskInfo = taskInfo;
            int[] rewardIds = TextUtility.SplitToIntArr(taskInfo.f_taskRewrardIds, 2);
            foreach (int id in rewardIds)
            {
                ItemGrid itemGrid = await ObjectBuilder.GetOrCreateInstance<ItemGrid>(E_AssetBundleType.UI, ResConfigCollection.ItemGrid, null);
                itemGrid.Init();
                rewardItems.Add(itemGrid);
            }

            DetailData detailData = new DetailData(currentTaskInfo, rewardItems);
            TriggerDataChanged(nameof(currentTaskInfo), detailData);
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
