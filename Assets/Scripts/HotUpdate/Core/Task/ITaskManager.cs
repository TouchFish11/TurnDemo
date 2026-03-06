using System;

namespace HotUpdate.Core.Task
{
    /// <summary>
    /// ���������
    /// </summary>
    public interface ITaskManager
    {
        event Action<TaskInfo, ITaskData> OnUpdateTask;

        event Action OnCancelTask;

        void AcceptTask(string id);
        void CancelTask();
        void CheckTaskState();
    }
}
