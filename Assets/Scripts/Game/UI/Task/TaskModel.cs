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

        public bool ContainContainer(int taskType)
        {
            return taskTypeToContainerMap.ContainsKey(taskType);
        }

        public void AddTaskTypeContainers(int taskType, TaskTypeContainer taskTypeContainer)
        {
            taskTypeToContainerMap.Add(taskType, taskTypeContainer);
            TriggerDataChanged(nameof(taskTypeContainer), taskTypeContainer);
        }

        public TaskTypeContainer GetContainer(int taskType)
        {
            return taskTypeToContainerMap[taskType];
        }

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
                GameObject rewardObj = await PoolManager.Instance.GetAssetBundleObjAsync(E_AssetBundleType.UI, ResConfigCollection.ItemGrid);
                ItemGrid itemGrid = rewardObj.GetComponent<ItemGrid>();
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
