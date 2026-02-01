using Core.Service;
using Game.Tasks;

namespace GameHotUpdate.UI.Main.Logic
{
    /// <summary>
    /// 主界面任务相关逻辑处理类
    /// 负责任务状态更新、任务栏激活/取消、任务信息刷新等核心逻辑
    /// </summary>
    public class TaskLogic : MainLogic
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mainController">主界面控制器，用于交互逻辑调度</param>
        /// <param name="mainModel">主界面数据模型，用于数据存储与读取</param>
        /// <param name="mainView">主界面视图层，用于UI渲染与交互</param>
        public TaskLogic(MainController mainController, MainModel mainModel, MainView mainView) : base(mainController, mainModel, mainView)
        {

        }

        /// <summary>
        /// 初始化任务逻辑
        /// 初始化时检查所有任务的当前状态，确保任务栏显示与实际状态一致
        /// </summary>
        public override void Init()
        {
            // 通过服务定位器获取任务管理器，执行任务状态检查
            ServiceLocator.Get<ITaskManager>().CheckTaskState();
        }

        /// <summary>
        /// 更新任务显示信息
        /// 根据任务数据更新任务栏的激活状态和任务详情展示
        /// </summary>
        /// <param name="currentTaskInfo">当前任务的基础信息（如任务名称、描述、目标等）</param>
        /// <param name="currentTaskData">当前任务的运行时数据（如完成状态、追踪状态、进度等）</param>
        public void UpdateTask(TaskInfo currentTaskInfo, TaskData currentTaskData)
        {
            // 确定任务栏激活状态：已完成则取消激活，未完成则根据追踪状态判断
            var isActive = currentTaskData.isCompleted ? !currentTaskData.isCompleted : currentTaskData.isTracking;
            // 设置任务栏激活/禁用状态
            mainView.SetTaskbarActive(isActive);
            
            if (isActive)
            {
                // 任务栏激活时，更新视图层的任务详情展示
                mainView.UpdateTask(currentTaskInfo, currentTaskData);
            }
        }

        /// <summary>
        /// 取消当前任务
        /// 取消任务时直接禁用任务栏，停止任务追踪与显示
        /// </summary>
        public void CancelTask()
        {
            SetTaskbarActive(false);
        }

        /// <summary>
        /// 设置任务栏激活状态
        /// 封装任务栏激活状态的设置逻辑，统一调用入口
        /// </summary>
        /// <param name="isActive">是否激活任务栏：true=激活，false=禁用</param>
        public void SetTaskbarActive(bool isActive)
        {
            // 调用视图层方法更新任务栏激活状态
            mainView.SetTaskbarActive(isActive);
        }
    }
}