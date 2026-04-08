using System.Collections.Generic;
using Core.Loader.Object;
using Core.Service;
using Core.UI.MVC;
using HotUpdate.Common;
using HotUpdate.Common.Item.UI;
using HotUpdate.Config.Quest;
using HotUpdate.Config.Quest.Config;
using HotUpdate.Task.Quest;

namespace HotUpdate.Task.UI
{
    /// <summary>
    /// 任务系统数据模型类
    /// 负责管理任务相关的所有数据逻辑，包括任务容器、当前选中任务信息、奖励物品等
    /// 实现 ITaskModel 接口，继承自 UIModel 基类
    /// </summary>
    public class TaskModel : UIModel
    {
        private readonly IPrefabLoader _prefabLoader = ServiceLocator.Get<IPrefabLoader>();
        // 任务类型与对应任务容器的映射字典，Key：任务类型ID  Value：该类型下的任务容器
        private readonly Dictionary<EQuestType, QuestTypeContainer> taskTypeToContainerMap = new();
        // 当前选中任务的奖励物品格子列表
        private readonly List<ItemGrid> rewardItems = new();
        
        /// <summary>
        /// 任务配置缓存
        /// </summary>
        public QuestConfig QuestConfig { get; set; }
        
        /// <summary>
        /// 是否正在追踪（跟随）当前任务
        /// 标记玩家是否开启了该任务的追踪功能
        /// </summary>
        public bool IsFollowingTask { get; set; }
        
        /// <summary>
        /// 当前选中的任务详情信息
        /// </summary>
        public QuestConfig.QuestItem CurrentQuestItemInfo { get; set; }

        /// <summary>
        /// 获取所有任务类型容器的枚举集合
        /// </summary>
        /// <returns>所有任务类型容器的可枚举序列</returns>
        public IEnumerable<QuestTypeContainer> GetContainers()
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
        /// <param name="questType">任务类型ID</param>
        /// <returns>存在返回true，否则返回false</returns>
        public bool ContainContainer(EQuestType questType)
        {
            return taskTypeToContainerMap.ContainsKey(questType);
        }

        /// <summary>
        /// 添加任务类型容器到映射字典
        /// </summary>
        /// <param name="taskType">任务类型ID</param>
        /// <param name="questTypeContainer">该类型对应的任务容器实例</param>
        public void AddTaskTypeContainers(EQuestType taskType, QuestTypeContainer questTypeContainer)
        {
            taskTypeToContainerMap.Add(taskType, questTypeContainer);
        }

        /// <summary>
        /// 根据任务类型ID获取对应的任务容器
        /// </summary>
        /// <param name="taskType">任务类型ID</param>
        /// <returns>对应类型的ITaskTypeContainer实例</returns>
        /// <exception cref="KeyNotFoundException">当指定任务类型不存在时抛出</exception>
        public QuestTypeContainer GetContainer(EQuestType taskType)
        {
            return taskTypeToContainerMap[taskType];
        }

        /// <summary>
        /// 获取第一个任务类型容器
        /// 用于默认选中首个任务分类的场景
        /// </summary>
        /// <returns>第一个ITaskTypeContainer实例，无容器时返回null</returns>
        public QuestTypeContainer GetFirstContainer()
        {
            foreach (var container in taskTypeToContainerMap.Values)
            {
                return container;
            }

            return null;
        }

        public void AddItemGrid(ItemGrid itemGrid)
        {
            rewardItems.Add(itemGrid);
        }

        public void ClearItemGrid()
        {
            foreach (var rewardItem in rewardItems)
            {
                ServiceLocator.Get<IPrefabLoader>().CollectAsset(rewardItem.gameObject);
            }
            rewardItems.Clear();
        }

        /// <summary>
        /// 清空模型内所有数据
        /// 重写自UIModel基类，用于UI关闭/销毁时的资源回收与数据清理
        /// </summary>
        public override void ClearData()
        {
            // 清空奖励物品列表
            ClearItemGrid();
            _prefabLoader.RealseAsset(AbKeyCollection.Ui, ResKeyCollection.ItemGrid);
            // 清空所有任务容器内的子项
            foreach (var container in taskTypeToContainerMap.Values)
            {
                container.ClearItem();
            }
            // 清空任务容器映射字典
            taskTypeToContainerMap.Clear();
            _prefabLoader.RealseAsset(AbKeyCollection.Ui, ResKeyCollection.ItemGrid);
        }
    }
}