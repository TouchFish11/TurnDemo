using System.Collections.Generic;
using Core.AssetBundles.Management;
using Core.UI;
using Core.UI.ViewController;
using HotUpdate.Common.Config.Quest;
using HotUpdate.Common.Config.Quest.Config;
using HotUpdate.Common.Item.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.Game.Quests.UI
{
    /// <summary>
    /// 任务界面视图层
    /// 负责任务界面的UI显示、组件引用和交互状态更新
    /// 实现ITaskView接口，为控制层提供UI操作入口
    /// </summary>
    public class TaskView : UIView
    {
        #region 组件引用
        /// <summary>
        /// 任务列表滚动视图
        /// </summary>
        [InjectUI] private ScrollRect svTask;

        /// <summary>
        /// 接受/追踪任务按钮
        /// </summary>
        [InjectUI] private Button btnAcceptTask;

        /// <summary>
        /// 任务项单选组（保证同一时间仅选中一个任务）
        /// </summary>
        [InjectUI] private ToggleGroup taskContent;

        /// <summary>
        /// 任务名称显示文本
        /// </summary>
        [InjectUI] private TextMeshProUGUI txtTaskName;

        /// <summary>
        /// 任务描述显示文本
        /// </summary>
        [InjectUI] private TextMeshProUGUI txtTaskDescription;

        /// <summary>
        /// 任务接受/追踪状态提示文本
        /// </summary>
        [InjectUI] private TextMeshProUGUI txtAccceptInfo;

        /// <summary>
        /// 任务详情面板根节点
        /// </summary>
        [InjectUI(1)] private RectTransform detailView;

        /// <summary>
        /// 任务奖励展示容器
        /// </summary>
        [InjectUI(1)] private RectTransform rewardBox;

        /// <summary>
        /// 有任务时的显示面板
        /// </summary>
        [InjectUI(1)] private RectTransform hasTaskView;

        /// <summary>
        /// 无任务时的显示面板
        /// </summary>
        [InjectUI(1)] private RectTransform noTaskView;
        #endregion

        // 任务类型与对应任务容器的映射字典，Key：任务类型ID  Value：该类型下的任务容器
        private readonly Dictionary<EQuestType, PoolObject<QuestTypeContainer>> taskTypeToContainerMap = new();
        // 当前选中任务的奖励物品格子列表
        private readonly List<PoolObject<ItemGrid>> rewardItems = new();
        
        #region 公共属性
        /// <summary>
        /// 任务列表滚动视图的内容容器（用于挂载任务项预制体）
        /// </summary>
        public Transform TaskContent => svTask.content;

        /// <summary>
        /// 任务项单选组对外暴露属性
        /// </summary>
        public ToggleGroup TaskItemGroup => taskContent;
        
        /// <summary>
        /// 奖励框
        /// </summary>
        public RectTransform RewardBox => rewardBox;
        #endregion
        
        #region 公共方法

        /// <summary>
        /// 更新任务详情面板显示
        /// </summary>
        /// <param name="questNodeConfig">当前选中的任务信息数据</param>
        public void UpdateTaskDetail(QuestNodeConfig questNodeConfig)
        {
            // 更新任务名称和描述
            txtTaskName.text = questNodeConfig.name;
            txtTaskDescription.text = questNodeConfig.description;
        }

        /// <summary>
        /// 切换有无任务的显示面板
        /// </summary>
        /// <param name="hasTasks">是否有可显示的任务</param>
        public void HasTasks(bool hasTasks)
        {
            hasTaskView.gameObject.SetActive(hasTasks);
            noTaskView.gameObject.SetActive(!hasTasks);
        }

        /// <summary>
        /// 更新任务追踪状态的文本提示
        /// </summary>
        /// <param name="isFollowingTask">是否正在追踪当前任务</param>
        public void UpdateFollowTask(bool isFollowingTask)
        {
            txtAccceptInfo.text = isFollowingTask ? "取消追踪" : "开始追踪";
        }
        
        /// <summary>
        /// 获取所有任务类型容器的枚举集合
        /// </summary>
        /// <returns>所有任务类型容器的可枚举序列</returns>
        public IEnumerable<QuestTypeContainer> GetContainers()
        {
            foreach (var taskTypeContainer in taskTypeToContainerMap.Values)
            {
                yield return taskTypeContainer.Obj;
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
        public void AddTaskTypeContainers(EQuestType taskType, PoolObject<QuestTypeContainer> questTypeContainer)
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
            return taskTypeToContainerMap[taskType].Obj;
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
                return container.Obj;
            }

            return null;
        }

        public void ClearItemGrid()
        {
            foreach (var poolObject in rewardItems)
            {
                poolObject.Collect();
            }
            rewardItems.Clear();
        }

        #endregion

        public override void Destroy()
        {
            ClearItemGrid();
        }
    }
}