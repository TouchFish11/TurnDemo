using Framework;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

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
        private TaskDataContainer taskDataContainer;

        public TaskController(TaskView view, TaskModel model) : base(view, model)
        {

        }

        protected override async Task OnInit()
        {
            await InitTasks();
        }

        protected override void ButtonOnClick(string btnName)
        {
            switch (btnName)
            {
                case "btnClose":
                    UIManager.Instance.HideView<TaskView, TaskModel, TaskController>();
                    break;
            }
        }

        /// <summary>
        /// 初始化任务
        /// </summary>
        /// <returns></returns>
        private async Task InitTasks()
        {
            // 读取任务数据
            taskDataContainer = await JsonManager.Instance.FromJsonAsync<TaskDataContainer>(PathManager.GetUserDataLocalSavePath(FileUtility.LocalTaskDataFileName));
            // 读取任务信息
            var idToInfoMap = BinaryDataMgr.Instance.GetTable<TaskInfoContainer>().dataDic;

            foreach (TaskInfo taskInfo in idToInfoMap.Values)
            {
                // 存在说明接取过
                if (taskDataContainer.Contain(taskInfo.f_id))
                {
                    // 若已完成，跳过显示
                    if (taskDataContainer.IsFinished(taskInfo.f_id))
                    {
                        continue;
                    }
                }

                // 显示任务列表UI
                TaskTypeContainer taskTypeContainer = null;
                if (CanCreateContainer(taskInfo.f_taskType))
                {
                    // 创建该任务类型父对象
                    taskTypeContainer = await CreateTaskTypeContainer(taskInfo);
                }
                else
                {
                    taskTypeContainer = _model.GetContainer(taskInfo.f_taskType);
                }
                await CreateTaskItem(taskInfo, taskTypeContainer);
            }
        }

        private bool CanCreateContainer(int taskType)
        {
            return !_model.ContainContainer(taskType);
        }

        /// <summary>
        /// 创建任务父对象容器
        /// </summary>
        /// <param name="taskInfo"></param>
        /// <returns></returns>
        private async Task<TaskTypeContainer> CreateTaskTypeContainer(TaskInfo taskInfo)
        {
            // 创建该任务类型父对象
            GameObject containerObj = await PoolManager.Instance.GetAssetBundleObjAsync(E_AssetBundleType.UI, "TaskTypeContainer");
            TaskTypeContainer taskTypeContainer = containerObj.GetComponent<TaskTypeContainer>();
            taskTypeContainer.Init(taskInfo.f_taskType, taskInfo.f_taskName);
            _model.AddTaskTypeContainers(taskInfo.f_taskType, taskTypeContainer);
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
            GameObject taskItemObj = await PoolManager.Instance.GetAssetBundleObjAsync(E_AssetBundleType.UI, "TaskItem");
            taskItemObj.transform.SetParent(container.transform, false);
            TaskItem taskItem = taskItemObj.GetComponent<TaskItem>();
            taskItem.OnSelectedTask += UpdateTaskDetail;
            taskItem.Init(taskInfo, taskDataContainer[taskInfo.f_id]);
            container.AddItem(taskItem, taskInfo);
        }

        /// <summary>
        /// 更新任务详细
        /// </summary>
        /// <param name="taskInfo"></param>
        private async void UpdateTaskDetail(TaskInfo taskInfo)
        {
            await _model.UpdateTaskInfo(taskInfo);
        }
    }
}
