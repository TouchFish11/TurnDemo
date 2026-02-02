using System.Collections.Generic;
using Core.Pool;
using Core.Service;
using GameHotUpdate.UI.General;
using GameHotUpdate.UI.MVC;

namespace GameHotUpdate.UI.Task
{
    /// <summary>
    /// 任务系统数据模型类
    /// 负责管理任务相关的所有数据逻辑，包括任务容器、当前选中任务信息、奖励物品等
    /// 实现 ITaskModel 接口，继承自 UIModel 基类
    /// </summary>
    public class TaskModel : UIModel
    {
        // 任务类型与对应任务容器的映射字典
        // Key：任务类型ID  Value：该类型下的任务容器
        private readonly Dictionary<int, TaskTypeContainer> taskTypeToContainerMap = new();
        // 当前选中任务的奖励物品格子列表
        private readonly List<ItemGrid> rewardItems = new();
        
        /// <summary>
        /// 是否正在追踪（跟随）当前任务
        /// 标记玩家是否开启了该任务的追踪功能
        /// </summary>
        public bool IsFollowingTask { get; set; }
        
        /// <summary>
        /// 当前选中的任务详情信息
        /// </summary>
        public TaskInfo CurrentTaskInfo { get; set; }

        /// <summary>
        /// 获取所有任务类型容器的枚举集合
        /// </summary>
        /// <returns>所有任务类型容器的可枚举序列</returns>
        public IEnumerable<TaskTypeContainer> GetContainers()
        {
            foreach (var taskTypeContainer in taskTypeToContainerMap.Values)
            {
                yield return taskTypeContainer;
            }
        }

        /// <summary>
        /// 检查是否存在任何任务容器
        /// </summary>
        /// <returns>存在任务容器返回true，否则返回false</returns>
        public bool HasTask()
        {
            return taskTypeToContainerMap.Count > 0;
        }

        /// <summary>
        /// 检查指定类型的任务容器是否存在
        /// </summary>
        /// <param name="taskType">任务类型ID</param>
        /// <returns>存在返回true，否则返回false</returns>
        public bool ContainContainer(int taskType)
        {
            return taskTypeToContainerMap.ContainsKey(taskType);
        }

        /// <summary>
        /// 添加任务类型容器到映射字典
        /// </summary>
        /// <param name="taskType">任务类型ID</param>
        /// <param name="taskTypeContainer">该类型对应的任务容器实例</param>
        public void AddTaskTypeContainers(int taskType, TaskTypeContainer taskTypeContainer)
        {
            taskTypeToContainerMap.Add(taskType, taskTypeContainer);
        }

        /// <summary>
        /// 根据任务类型ID获取对应的任务容器
        /// </summary>
        /// <param name="taskType">任务类型ID</param>
        /// <returns>对应类型的ITaskTypeContainer实例</returns>
        /// <exception cref="KeyNotFoundException">当指定任务类型不存在时抛出</exception>
        public TaskTypeContainer GetContainer(int taskType)
        {
            return taskTypeToContainerMap[taskType];
        }

        /// <summary>
        /// 获取第一个任务类型容器
        /// 用于默认选中首个任务分类的场景
        /// </summary>
        /// <returns>第一个ITaskTypeContainer实例，无容器时返回null</returns>
        public TaskTypeContainer GetFirstContainer()
        {
            foreach (var container in taskTypeToContainerMap.Values)
            {
                return container;
            }

            return null;
        }

        public IEnumerable<ItemGrid> GetItemGrids()
        {
            foreach (var item in rewardItems)
            {
                yield return item;
            }
        }

        public void AddItemGrid(ItemGrid itemGrid)
        {
            rewardItems.Add(itemGrid);
        }

        public void ClearItemGrid()
        {
            rewardItems.Clear();
        }

        /// <summary>
        /// 清空模型内所有数据
        /// 重写自UIModel基类，用于UI关闭/销毁时的资源回收与数据清理
        /// </summary>
        public override void ClearData()
        {
            // 清空奖励物品列表
            rewardItems.Clear();

            // 清空所有任务容器内的子项
            foreach (var container in taskTypeToContainerMap.Values)
            {
                container.ClearItem();
            }
            // 清空任务容器映射字典
            taskTypeToContainerMap.Clear();

            // 回收任务项和物品格子的对象池
            ServiceLocator.Get<IPoolManager>().ClearTypes(typeof(TaskItem), typeof(ItemGrid));
        }
    }
}