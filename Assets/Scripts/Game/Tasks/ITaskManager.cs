using System;

namespace Game.Tasks
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
