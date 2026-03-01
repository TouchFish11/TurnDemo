using System;
using GameHotUpdate.Task.Data;

namespace GameHotUpdate.Task.Core
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
