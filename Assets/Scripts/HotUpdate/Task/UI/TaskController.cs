using System.Threading.Tasks;
using Core.Collection;
using Core.Serialize.Binary;
using Core.Service;
using Core.UI.MVC;
using HotUpdate.Common;
using HotUpdate.Common.Item;
using HotUpdate.Core.Task;
using HotUpdate.Core.UI.MVC;
using HotUpdate.Task.Core;
using HotUpdate.Task.Data;

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
        private ITaskDataCollection taskDataCollection;
        private readonly ITaskManager _taskManager = ServiceLocator.Get<ITaskManager>();
        private readonly IBinaryDataManager _binaryDataManager = ServiceLocator.Get<IBinaryDataManager>();
        
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
                if (taskDataCollection.IsTracking(out var taskData))
                {
                    // 标记当前处于追踪任务状态
                    model.IsFollowingTask = true;
                    view.UpdateFollowTask(model.IsFollowingTask);
                    // 选中当前正在追踪的任务
                    SelectTrackingTask(taskData.CurrentTaskId);
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

        /// <summary>
        /// 控制器初始化
        /// 完成任务数据初始化、视图状态初始化等核心初始化逻辑
        /// </summary>
        /// <returns>异步任务</returns>
        protected override Task OnInit()
        {
            return Task.CompletedTask;
        }
        
        /// <summary>
        /// 选中追踪的任务
        /// </summary>
        /// <param name="id"></param>
        private void SelectTrackingTask(string id)
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
                        _taskManager.AcceptTask(model.CurrentTaskInfo.f_id);
                    }
                    else
                    {
                        // 取消当前追踪的任务
                        _taskManager.CancelTask();
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
            taskDataCollection = TaskUtility.GetTaskDataCollection();
            if (taskDataCollection == null)
            {
                return;
            }
            
            // 从二进制数据管理器加载任务配置表（Excel配置），获取任务ID到任务信息的映射表
            var idToInfoMap = _binaryDataManager.GetConfig<TaskInfoContainer>(EConfigLoadType.Excel).dataDic;

            // 遍历所有任务配置信息
            foreach (var taskInfo in idToInfoMap.Values)
            {
                // 检查任务数据集合中是否包含当前任务ID（判断是否是已解锁/可显示的任务）
                if (((Collection<string, TaskData>)taskDataCollection).ContainsKey(taskInfo.f_id))
                {
                    // 如果任务已完成，则跳过不显示
                    if (taskDataCollection.IsFinished(taskInfo.f_id))
                    {
                        continue;
                    }
                }

                // 初始化并显示任务列表UI
                TaskTypeContainer taskTypeContainer;
                // 检查模型中是否已存在当前任务类型的容器
                if (!model.ContainContainer(taskInfo.f_taskType))
                {
                    // 不存在则创建新的任务类型容器
                    taskTypeContainer = await CreateTaskTypeContainer(taskInfo);
                }
                else
                {
                    // 存在则直接获取已有容器
                    taskTypeContainer = model.GetContainer(taskInfo.f_taskType);
                }

                // 检查当前任务类型容器中是否已包含该任务项
                if (!taskTypeContainer.ContainTask(taskInfo.f_id))
                {
                    // 不存在则创建新的任务项并添加到容器中
                    await CreateTaskItem(taskInfo, taskTypeContainer);
                }
            }
        }

        /// <summary>
        /// 创建任务类型容器（用于分类展示不同类型的任务）
        /// </summary>
        /// <param name="taskInfo">任务信息，用于获取任务类型</param>
        /// <returns>创建好的任务类型容器接口实例</returns>
        private async Task<TaskTypeContainer> CreateTaskTypeContainer(TaskInfo taskInfo)
        {
            // 从资源包中异步加载任务类型容器预制体并创建实例
            var taskTypeContainerWrapper = await prefabLoader.GetObjectAsync<TaskTypeContainer>(AbKeyCollection.Ui, ResKeyCollection.TaskTypeContainer, view.TaskContent);
            // 初始化任务类型容器（设置对应的任务类型）
            taskTypeContainerWrapper.Init(taskInfo.f_taskType);
            // 将创建的容器添加到模型中管理
            model.AddTaskTypeContainers(taskInfo.f_taskType, taskTypeContainerWrapper);
            return taskTypeContainerWrapper;
        }

        /// <summary>
        /// 创建任务项UI实例
        /// </summary>
        /// <param name="taskInfo">任务信息（配置数据）</param>
        /// <param name="container">该任务项所属的任务类型容器</param>
        /// <returns>异步任务</returns>
        private async Task CreateTaskItem(TaskInfo taskInfo, TaskTypeContainer container)
        {
            // 从资源包中异步加载任务项预制体，并挂载到对应任务类型容器的Transform下
            var taskItem = await prefabLoader.GetObjectAsync<TaskItem>(AbKeyCollection.Ui, ResKeyCollection.TaskItem, container.transform);
            // 注册任务项选中事件，选中时更新任务详情展示
            taskItem.OnSelectedTask += UpdateTaskDetail;
            // 初始化任务项UI（传入任务信息和任务分组组件）
            taskItem.Init(taskInfo, view.TaskItemGroup);
            // 将任务项添加到所属的任务类型容器中管理
            container.AddItem(taskItem);
        }

        /// <summary>
        /// 更新任务详情展示
        /// 当任务项被选中时，触发该方法更新详情面板的任务信息
        /// </summary>
        /// <param name="id">选中的任务ID</param>
        private void UpdateTaskDetail(string id)
        {
            // 从配置中获取任务基础信息
            var selectTaskInfo = _binaryDataManager.GetConfig<TaskInfoContainer>(EConfigLoadType.Excel).dataDic[id];
            // 相等不用处理
            if (model.CurrentTaskInfo != null && selectTaskInfo == model.CurrentTaskInfo)
            {
                return;
            }

            // 更新当前任务信息为选中的任务信息
            model.CurrentTaskInfo = selectTaskInfo;
            model.ClearItemGrid();

            // 解析奖励ID数组，获取物品格子
            ItemUtility.GetItemGrid(selectTaskInfo.f_taskRewrardIds, view.RewardBox, grid => model.AddItemGrid(grid));
            
            // 同步任务追踪状态：从任务数据集合中获取当前任务的追踪标记
            if (((Collection<string, TaskData>)taskDataCollection).TryGetValue(id, out var taskData))
            {
                model.IsFollowingTask = taskData.isTracking;
            }
            else
            {
                model.IsFollowingTask = false;
            }
            
            // 更新按钮显示
            view.UpdateFollowTask(model.IsFollowingTask);
            // 更新文本显示
            view.UpdateTaskDetail(selectTaskInfo);
        }
    }
}