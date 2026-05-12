namespace Core.Tasks
{
    /// <summary>
    /// 任务句柄辅助器
    /// </summary>
    internal static class TaskHandleHelper
    {
        // 任务句柄全局ID，调试使用
        private static int _taskHandleGlobalId;

        /// <summary>
        /// 获取任务句柄全局ID，不复用
        /// </summary>
        /// <returns></returns>
        public static int GetGlobalId()
        {
            return ++_taskHandleGlobalId;
        }
    }
}
