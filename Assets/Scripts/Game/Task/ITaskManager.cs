using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 任务管理器
/// </summary>
public interface ITaskManager
{
    event Action<TaskInfo, TaskData> OnUpdateTask;

    event Action OnCancelTask;

    void AcceptTask(string id);
    void CancelTask();
    void CheckTaskState();
}
