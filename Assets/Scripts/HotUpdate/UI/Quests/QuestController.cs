using System;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Serialize.Json;
using Core.UI.ViewController;
using Core.Utility;
using HotUpdate.Base.Collection;
using HotUpdate.Base.Manager;
using HotUpdate.Base.UI;
using HotUpdate.Common.Config.Quest;
using HotUpdate.Common.Config.Quest.Config;

using HotUpdate.UI.Item;
using UnityEngine;

namespace HotUpdate.UI.Quests
{
    /// <summary>
    /// 任务控制器类
    /// 处理任务UI的交互逻辑、数据初始化、视图更新等核心逻辑
    /// </summary>
    public class QuestController : UIController<TaskView>, IBlockOperation
    {
        [Inject] private IJsonManager _jsonManager;
        [Inject] private ObjectSpawner _objectSpawner;
        [Inject] private ItemService _itemService;
        [Inject] private IQuestDataManager _questDataManager;
        [Inject] private IUIService _uiservice;
        [Inject] private IQuestManager _questManager;
        
        // 任务数据集合，存储当前所有任务的状态数据
        private IQuestCollection _questCollection;
        
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
        
        public bool BlockOperation { get; } = true;

        protected override bool IsCursorVisible { get; set; } = true;

        protected override Task OnInit()
        {
            return Task.CompletedTask;
        }

        protected override async Task OnActive()
        {
            // 初始化所有任务数据和UI展示
            await InitTasks();
            // 判断是否存在任务数据
            var hasTask = view.HasTask();
            view.HasTasks(hasTask);
            if (hasTask)
            {
                // 检查是否有正在追踪的任务
                if (_questCollection.TryGetTrackQuest(out var questData))
                {
                    // 标记当前处于追踪任务状态
                    IsFollowingTask = true;
                    view.UpdateFollowTask(IsFollowingTask);
                    // 选中当前正在追踪的任务
                    SelectTrackingQuest(questData.QuestId);
                }
                else
                {
                    // 标记当前未追踪任务
                    IsFollowingTask = false;
                    // 默认选中第一个任务分类下的第一个任务
                    view.GetFirstContainer().SelectFirstQuest();
                }
            }

            // 设置任务分组的Toggle不允许取消选中，确保始终有一个任务处于选中状态
            view.TaskItemGroup.allowSwitchOff = false;
        }

        protected override Task OnInactivate()
        {
            // 显示主界面
            return _uiservice.ShowAsync(_uiservice.GetPanel(EUIPanelId.MainPanel).PanelId);
        }

        /// <summary>
        /// 选中追踪的任务
        /// </summary>
        /// <param name="id"></param>
        private void SelectTrackingQuest(int id)
        {
            foreach (var taskTypeContainer in view.GetContainers())
            {
                if (taskTypeContainer.SelectQuest(id))
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 按钮点击事件处理方法
        /// 统一处理任务UI中所有按钮的点击逻辑
        /// </summary>
        /// <param name="btnName">按钮名称（标识）</param>
        protected override void OnButtonClick(string btnName)
        {
            switch (btnName)
            {
                case "btnClose":
                    // 关闭任务UI视图
                    uiManager.DestroyView(panelId);
                    break;
                case "btnAcceptTask":
                    // 切换任务追踪状态（接受/取消追踪）
                    IsFollowingTask = !IsFollowingTask;
                    // 更新按钮显示
                    view.UpdateFollowTask(IsFollowingTask);
                    if (IsFollowingTask)
                    {
                        // 接受当前选中的任务，开始追踪
                        _questManager.AcceptQuest(CurrentQuestItemInfo.id);
                    }
                    else
                    {
                        // 取消当前追踪的任务
                        _questManager.CancelQuest();
                    }
                    break;
            }
        }

        /// <summary>
        /// 初始化任务数据和UI展示
        /// 加载任务配置、筛选任务状态、创建任务分类和任务项UI
        /// </summary>
        /// <returns>异步任务</returns>
        private async Task InitTasks()
        {
            // 临时设置任务分组允许取消选中，避免初始化过程中Toggle无法响应事件
            view.TaskItemGroup.allowSwitchOff = true;
            // 获取任务数据集合
            _questCollection = _questDataManager.QuestCollection;
            if (_questCollection == null)
                throw new NullReferenceException($"{nameof(_questCollection)} is null");
            
            // 加载资源
            using var handle = await GameAsset.LoadAssetAsync<TextAsset>(AssetKeys.QuestConfig);
            // 解析Json
            QuestConfig = _jsonManager.FromJson<QuestConfig>(handle.Asset.text, settings:NewtonsoftJsonUtility.SerializerSettings);
            
            foreach (var quest in _questManager.GetQuests())
            {
                // 初始化并显示任务列表UI
                QuestTypeContainer questTypeContainer;
                // 检查数据缓存中是否已存在当前任务类型的容器
                if (!view.ContainContainer(quest.QuestItem.questType))
                {
                    // 不存在则创建新的任务类型容器
                    questTypeContainer = await CreateQuestTypeContainer(quest.QuestItem.questType);
                }
                else
                {
                    // 存在则直接获取已有容器
                    questTypeContainer = view.GetContainer(quest.QuestItem.questType);
                }
                
                // 检查当前任务类型容器中是否已包含该任务项
                if (!questTypeContainer.ContainQuest(quest.QuestItem.id))
                {
                    QuestData questData = null;
                    if (_questCollection.TryGetValue(quest.QuestItem.id, out var data)) questData = data;
                    // 不存在则创建新的任务项并添加到容器中
                    await CreateTaskItem(quest.QuestItem, questData, questTypeContainer);
                }
            }
        }

        /// <summary>
        /// 创建任务类型容器（用于分类展示不同类型的任务）
        /// </summary>
        /// <param name="questType">任务信息，用于获取任务类型</param>
        /// <returns>创建好的任务类型容器接口实例</returns>
        private async Task<QuestTypeContainer> CreateQuestTypeContainer(EQuestType questType)
        {
            // 从资源包中异步加载任务类型容器预制体并创建实例
            var questTypeContainer = await _objectSpawner.SpawnAsync<QuestTypeContainer>(AssetKeys.QuestTypeContainer, view.TaskContent);
            // 初始化任务类型容器（设置对应的任务类型）
            questTypeContainer.Init(questType);
            // 将创建的容器添加到模型中管理
            view.AddTaskTypeContainers(questType, questTypeContainer);
            return questTypeContainer;
        }

        /// <summary>
        /// 创建任务项UI实例
        /// </summary>
        /// <param name="questItem">任务信息（配置数据）</param>
        /// <param name="questData"></param>
        /// <param name="container">该任务项所属的任务类型容器</param>
        /// <returns>异步任务</returns>
        private async Task CreateTaskItem(QuestConfig.QuestItem questItem, QuestData questData, QuestTypeContainer container)
        {
            // 从资源包中异步加载任务项预制体，并挂载到对应任务类型容器的Transform下
            var taskItem = await _objectSpawner.SpawnAsync<TaskItem>(AssetKeys.QuestItem, container.transform);
            // 注册任务项选中事件，选中时更新任务详情展示
            taskItem.OnSelectedTask += UpdateQuestDetail;
            // 初始化任务项UI（传入任务信息和任务分组组件）
            var questNodeName = string.Empty;
            // 用户没有该任务数据，显示该任务的第一个节点名称
            if (questData == null)
            {
                questNodeName = questItem.nodeConfigs[0].name;
            }
            else
            {
                // 显示第一个找到的进行中或未接取的任务节点显示，跳过已完成节点的显示，因为任务是线性的，所以可以这样处理
                foreach (var questNodeData in questData.GetNodeDatas())
                {
                    if (questNodeData.Phase == EQuestPhase.Complete) continue;
                    questNodeName = questItem.nodeConfigs.Find(config => config.nodeId == questNodeData.NodeId).name;
                    break;
                }
            }
            
            taskItem.Init(questItem.id, questNodeName, view.TaskItemGroup);
            // 将任务项添加到所属的任务类型容器中管理
            container.AddQuestItem(taskItem);
        }

        /// <summary>
        /// 更新任务详情展示
        /// 当任务项被选中时，触发该方法更新详情面板的任务信息
        /// </summary>
        /// <param name="id">选中的任务ID</param>
        private void UpdateQuestDetail(int id)
        {
            // 从配置中获取任务配置信息
            var selectConfig = QuestConfig.questItems.Find(item => item.id == id);
            // 相等不用处理
            if (CurrentQuestItemInfo != null && selectConfig == CurrentQuestItemInfo) return;

            // 更新当前任务信息为选中的任务信息
            CurrentQuestItemInfo = selectConfig;
            view.ClearItemGrid(_objectSpawner);

            var questCollection = _questDataManager.QuestCollection;
            if (!questCollection.TryGetValue(id, out var questData))
                throw new NullReferenceException($"{nameof(questData)} is null");

            QuestNodeConfig nodeConfig = null;
            foreach (var questNodeData in questData.GetNodeDatas())
            {
                if(questNodeData.Phase == EQuestPhase.Complete) continue;
                nodeConfig = selectConfig.nodeConfigs.Find(config => config.nodeId == questNodeData.NodeId);
                break;
            }
            
            if(nodeConfig == null)
                throw new NullReferenceException($"{nameof(nodeConfig)} is null");
            
            // 解析奖励ID数组，获取物品格子
            _itemService.GetItemGrid(nodeConfig.rewardItemIds, view.RewardBox, null);
            
            // 同步任务追踪状态：从任务数据集合中获取当前任务的追踪标记
            IsFollowingTask = questCollection.TryGetValue(id, out var data) && data.IsTracking;
            // 更新按钮显示
            view.UpdateFollowTask(IsFollowingTask);
            // 更新文本显示
            view.UpdateTaskDetail(nodeConfig);
        }

        protected override Task OnDestroy()
        {
            _itemService.Dispose();
            return base.OnDestroy();
        }
    }
}