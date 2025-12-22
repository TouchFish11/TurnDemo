using Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// 任务界面控制器工厂
    /// </summary>
    public class TaskControllerFactory : UIControllerFactory<TaskView, TaskModel, TaskController>
    {
        public override TaskController CreateController(TaskView view, TaskModel model)
        {
            return new TaskController(view, model);
        }

        public override TaskModel CreateModel()
        {
            return new TaskModel();
        }
    }

    /// <summary>
    /// 任务界面控制器
    /// </summary>
    public class TaskController : UIController<TaskView, TaskModel>
    {
        private TaskDataCollection taskDataCollection;

        public TaskController(TaskView view, TaskModel model) : base(view, model)
        {

        }

        protected override async Task OnInit()
        {
            await InitTasks();

            // 存在任务
            if (model.HasTask())
            {
                // 有正在追踪的任务
                if (taskDataCollection.IsTracking(out TaskData taskData))
                {
                    // 显示当前追踪任务和进度
                    model.IsFollowingTask = true;
                    model.SelectTrackingTask(taskData.currentTaskId);
                }
                else
                {
                    // 不显示任务栏
                    model.IsFollowingTask = false;
                    // 默认显示第一个任务
                    model.GetFirstContainer().DefaultSelectFirstTask();
                }
            }

            // 选中完其中任务后，禁止“每一个都不选中”选项
            view.TaskItemGroup.allowSwitchOff = false;
        }

        protected override void ButtonOnClick(string btnName)
        {
            switch (btnName)
            {
                case "btnClose":
                    UIManager.Instance.DestroyView();
                    break;
                case "btnAcceptTask":
                    model.IsFollowingTask = !model.IsFollowingTask;
                    if (model.IsFollowingTask)
                    {
                        TaskManager.Instance.AcceptTask(model.GetCurrentSelectTaskInfo().f_id);
                    }
                    else
                    {
                        TaskManager.Instance.CancelTask();
                    }
                    break;
            }
        }

        /// <summary>
        /// 初始化任务
        /// </summary>
        /// <returns></returns>
        private async Task InitTasks()
        {
            // 暂时允许都不选中，避免任务更新出现Toggle无法响应事件问题
            view.TaskItemGroup.allowSwitchOff = true;
            // 读取任务数据
            taskDataCollection = GameDataMgr.Instance.TaskDataCollection;
            // 读取任务信息
            var idToInfoMap = BinaryDataMgr.Instance.GetConfig<TaskInfoContainer>(E_ConfigLoadType.Excel).dataDic;

            foreach (TaskInfo taskInfo in idToInfoMap.Values)
            {
                // 存在说明接取过
                if (taskDataCollection.ContainsKey(taskInfo.f_id))
                {
                    // 若已完成，跳过显示
                    if (taskDataCollection.IsFinished(taskInfo.f_id))
                    {
                        continue;
                    }
                }

                // 显示任务列表UI
                TaskTypeContainer taskTypeContainer;
                if (!model.ContainContainer(taskInfo.f_taskType))
                {
                    // 创建该任务类型父对象
                    taskTypeContainer = await CreateTaskTypeContainer(taskInfo);
                }
                else
                {
                    taskTypeContainer = model.GetContainer(taskInfo.f_taskType);
                }

                if (!taskTypeContainer.ContainTask(taskInfo.f_id))
                {
                    await CreateTaskItem(taskInfo, taskTypeContainer);
                }
            }
        }

        /// <summary>
        /// 创建任务父对象容器
        /// </summary>
        /// <param name="taskInfo"></param>
        /// <returns></returns>
        private async Task<TaskTypeContainer> CreateTaskTypeContainer(TaskInfo taskInfo)
        {
            // 创建该任务类型父对象
            TaskTypeContainer taskTypeContainer = await ObjectBuilder.GetOrCreateInstance<TaskTypeContainer>(E_AssetBundleType.UI, ResKeyCollection.TaskTypeContainer, null);
            taskTypeContainer.Init(taskInfo.f_taskType);
            model.AddTaskTypeContainers(taskInfo.f_taskType, taskTypeContainer);
            return taskTypeContainer;
        }

        /// <summary>
        /// 创建任务项
        /// </summary>
        /// <param name="taskInfo"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        private async Task CreateTaskItem(TaskInfo taskInfo, TaskTypeContainer container)
        {
            TaskItem taskItem = await ObjectBuilder.GetOrCreateInstance<TaskItem>(E_AssetBundleType.UI, ResKeyCollection.TaskItem, container.transform);
            taskItem.OnSelectedTask += UpdateTaskDetail;
            taskDataCollection.TryGetValue(taskInfo.f_id, out TaskData taskData);
            taskItem.Init(taskInfo, view.TaskItemGroup);
            container.AddItem(taskItem);
        }

        /// <summary>
        /// 更新任务详细
        /// </summary>
        /// <param name="taskInfo"></param>
        private async void UpdateTaskDetail(string id)
        {
            await model.UpdateTaskInfoById(id);
        }
    }
}
