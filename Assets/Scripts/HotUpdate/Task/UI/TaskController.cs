using System;
using System.Threading.Tasks;
using Core.Loader.Text;
using Core.Serialize.Json;
using Core.Service;
using Core.UI.MVC;
using Core.Utility;
using HotUpdate.Common;
using HotUpdate.Common.Item;
using HotUpdate.Config.Quest;
using HotUpdate.Core.Task;
using HotUpdate.Core.UI.MVC;
using HotUpdate.Task.Quest;
using TaskUtility = HotUpdate.Task.Core.TaskUtility;

namespace HotUpdate.Task.UI
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 任务控制器类
    /// 处理任务UI的交互逻辑、数据初始化、视图更新等核心逻辑
    /// </summary>
    public class TaskController : UIController<TaskView, TaskModel>, ITaskController
    {
        // 任务数据集合，存储当前所有任务的状态数据
        private IQuestCollection _questCollection;
        private readonly IQuestManager _questManager = ServiceLocator.Get<IQuestManager>();
        
        protected override async Task OnShow()
        {
            // 初始化所有任务数据和UI展示
            await InitTasks();
            // 判断是否存在任务数据
            var hasTask = model.HasTask();
            view.HasTasks(hasTask);
            if (hasTask)
            {
                // 检查是否有正在追踪的任务
                if (_questCollection.TryGetTrackQuest(out var questData))
                {
                    // 标记当前处于追踪任务状态
                    model.IsFollowingTask = true;
                    view.UpdateFollowTask(model.IsFollowingTask);
                    // 选中当前正在追踪的任务
                    SelectTrackingQuest(questData.QuestId);
                }
                else
                {
                    // 标记当前未追踪任务
                    model.IsFollowingTask = false;
                    // 默认选中第一个任务分类下的第一个任务
                    model.GetFirstContainer().DefaultSelectFirstTask();
                }
            }

            // 设置任务分组的Toggle不允许取消选中，确保始终有一个任务处于选中状态
            view.TaskItemGroup.allowSwitchOff = false;
        }

        protected override Task OnHide()
        {
            // 显示主界面
            return uiManager.SetViewActive(uiManager.GetController<IMainController>(), true);
        }
        
        protected override Task OnInit()
        {
            return Task.CompletedTask;
        }
        
        /// <summary>
        /// 选中追踪的任务
        /// </summary>
        /// <param name="id"></param>
        private void SelectTrackingQuest(int id)
        {
            foreach (var taskTypeContainer in model.GetContainers())
            {
                taskTypeContainer.SelectTask(id);
            }
        }

        /// <summary>
        /// 按钮点击事件处理方法
        /// 统一处理任务UI中所有按钮的点击逻辑
        /// </summary>
        /// <param name="btnName">按钮名称（标识）</param>
        protected override void ButtonOnClick(string btnName)
        {
            switch (btnName)
            {
                case "btnClose":
                    // 关闭任务UI视图
                    uiManager.DestroyView(AbKeyCollection.Ui, this);
                    break;
                case "btnAcceptTask":
                    // 切换任务追踪状态（接受/取消追踪）
                    model.IsFollowingTask = !model.IsFollowingTask;
                    // 更新按钮显示
                    view.UpdateFollowTask(model.IsFollowingTask);
                    if (model.IsFollowingTask)
                    {
                        // 接受当前选中的任务，开始追踪
                        _questManager.AcceptQuest(model.CurrentQuestItemInfo.id);
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
            // 获取全局任务数据集合实例
            _questCollection = TaskUtility.GetTaskDataCollection();
            if (_questCollection == null)
                throw new NullReferenceException($"{nameof(_questCollection)} is null");
            
            // AB包加载资源
            var textAsset = await ServiceLocator.Get<ITextLoader>().LoadAssetAsync(AbKeyCollection.Gameconfig, ResKeyCollection.QuestConfig);
            // 解析Json
            model.QuestConfig = ServiceLocator.Get<IJsonManager>().FromJson<QuestConfig>(textAsset.text);
            // 加载本地任务数据
            var questCollection = await ServiceLocator.Get<IJsonManager>().FromJsonAsync<QuestCollection>(PathUtility.GetUserDataLocalSavePath(FileUtility.LocalTaskDataFileName));
            // 初始化任务管理器
            ServiceLocator.Get<IQuestManager>().InitQuests(model.QuestConfig, questCollection);
            
            foreach (var quest in ServiceLocator.Get<IQuestManager>().GetQuests())
            {
                // 初始化并显示任务列表UI
                QuestTypeContainer questTypeContainer;
                // 检查数据缓存中是否已存在当前任务类型的容器
                if (!model.ContainContainer(quest.QuestItem.questType))
                {
                    // 不存在则创建新的任务类型容器
                    questTypeContainer = await CreateQuestTypeContainer(quest.QuestItem.questType);
                }
                else
                {
                    // 存在则直接获取已有容器
                    questTypeContainer = model.GetContainer(quest.QuestItem.questType);
                }
                
                // 检查当前任务类型容器中是否已包含该任务项
                if (!questTypeContainer.ContainTask(quest.QuestItem.id))
                {
                    QuestData questData = null;
                    if (questCollection.TryGetValue(quest.QuestItem.id, out var data)) questData = data;
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
            var taskTypeContainerWrapper = await prefabLoader.GetObjectAsync<QuestTypeContainer>(AbKeyCollection.Ui, ResKeyCollection.QuestTypeContainer, view.TaskContent);
            // 初始化任务类型容器（设置对应的任务类型）
            taskTypeContainerWrapper.Init(questType);
            // 将创建的容器添加到模型中管理
            model.AddTaskTypeContainers(questType, taskTypeContainerWrapper);
            return taskTypeContainerWrapper;
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
            var taskItem = await prefabLoader.GetObjectAsync<TaskItem>(AbKeyCollection.Ui, ResKeyCollection.QuestItem, container.transform);
            // 注册任务项选中事件，选中时更新任务详情展示
            taskItem.OnSelectedTask += UpdateTaskDetail;
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
            container.AddItem(taskItem);
        }

        /// <summary>
        /// 更新任务详情展示
        /// 当任务项被选中时，触发该方法更新详情面板的任务信息
        /// </summary>
        /// <param name="id">选中的任务ID</param>
        private async void UpdateTaskDetail(int id)
        {
            // 从配置中获取任务配置信息
            var selectConfig = model.QuestConfig.questItems.Find(item => item.id == id);
            // 相等不用处理
            if (model.CurrentQuestItemInfo != null && selectConfig == model.CurrentQuestItemInfo) return;

            // 更新当前任务信息为选中的任务信息
            model.CurrentQuestItemInfo = selectConfig;
            model.ClearItemGrid();
            
            var questCollection = await ServiceLocator.Get<IJsonManager>().FromJsonAsync<QuestCollection>(PathUtility.GetUserDataLocalSavePath(FileUtility.LocalTaskDataFileName));
            var questData = questCollection[id];

            QuestNodeConfig nodeConfig = null;
            foreach (var questNodeData in questData.GetNodeDatas())
            {
                if(questNodeData.Phase == EQuestPhase.Complete) continue;
                nodeConfig = selectConfig.nodeConfigs.Find(nodeConfig => nodeConfig.nodeId == questNodeData.NodeId);
                break;
            }
            
            if(nodeConfig == null)
                throw new NullReferenceException($"{nameof(nodeConfig)} is null");
            
            // 解析奖励ID数组，获取物品格子
            ItemUtility.GetItemGrid(nodeConfig.rewardItemIds, view.RewardBox, grid => model.AddItemGrid(grid));
            
            // 同步任务追踪状态：从任务数据集合中获取当前任务的追踪标记
            model.IsFollowingTask = questCollection.TryGetValue(id, out var data) && data.IsTracking;
            // 更新按钮显示
            view.UpdateFollowTask(model.IsFollowingTask);
            // 更新文本显示
            view.UpdateTaskDetail(nodeConfig);
        }
    }
}