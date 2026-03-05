namespace HotUpdate.Task.Data
{
    public interface ITaskDataCollection
    {
        /// <summary>
        /// 检查是否包含指定标识的任务（按任务主ID匹配，截取前7位作为主ID）
        /// </summary>
        /// <param name="id">待检查的任务完整标识</param>
        /// <returns>存在返回true，不存在返回false</returns>
        bool ContainTask(string id);

        /// <summary>
        /// 检查指定ID的任务是否已完成
        /// </summary>
        /// <param name="taskId">任务唯一标识</param>
        /// <returns>任务完成返回true，未完成/不存在抛出键不存在异常</returns>
        bool IsFinished(string taskId);

        /// <summary>
        /// 检查是否存在正在追踪的任务
        /// </summary>
        /// <param name="taskData">输出参数，返回第一个正在追踪的任务数据；无则返回null</param>
        /// <returns>存在正在追踪的任务返回true，否则返回false</returns>
        bool IsTracking(out ITaskData taskData);
    }
}
