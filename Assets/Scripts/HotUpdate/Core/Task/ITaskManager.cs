using System;

namespace HotUpdate.Core.Task
{
    /// <summary>
    /// 任务管理器接口
    /// </summary>
    public interface ITaskManager
    {
        /// <summary>
        /// 任务更新事件（任务信息/进度变化时触发）
        /// 参数：当前任务信息、当前任务运行时数据
        /// </summary>
        event Action<TaskInfo, ITaskData> OnUpdateTask;
        
        /// <summary>
        /// 任务取消事件（取消当前追踪任务时触发）
        /// </summary>
        event Action OnCancelTask;

        /// <summary>
        /// 接受指定ID的任务（开始追踪该任务）
        /// 若已有正在追踪的任务，先取消原有任务
        /// </summary>
        /// <param name="id">要接受的任务ID</param>
        void AcceptTask(string id);
        
        /// <summary>
        /// 取消当前追踪的任务
        /// 移除事件监听、重置任务数据、触发取消事件
        /// </summary>
        void CancelTask();
        
        /// <summary>
        /// 检查当前任务状态（初始化/恢复任务追踪）
        /// 从游戏管理器中获取正在追踪的任务，加载对应配置并监听事件
        /// </summary>
        void CheckTaskState();
    }
}
