using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using System;

/// <summary>
/// 任务管理器
/// </summary>
public class TaskManager : SingletonBase<TaskManager>
{
    private TaskInfo currentTaskInfo;
    private TaskData currentTaskData;
    // 是否正在追踪
    private bool isTracking;

    /// <summary>
    /// 更新任务事件
    /// </summary>
    public event Action<TaskInfo, TaskData> OnUpdateTask;

    /// <summary>
    /// 取消任务事件
    /// </summary>
    public event Action OnCancelTask;

    private TaskManager()
    {

    }

    /// <summary>
    /// 接收任务
    /// </summary>
    /// <param name="id"></param>
    public void AcceptTask(string id)
    {
        // 当前正在追踪其它任务
        if (currentTaskInfo != null && currentTaskData != null)
        {
            // 需要切换追踪，取消当前任务的追踪状态
            CancelTask();
        }

        currentTaskInfo = BinaryDataMgr.Instance.GetTable<TaskInfoContainer>().dataDic[id];
        
        if (GameDataMgr.Instance.TaskDataCollection.TryGetValue(id, out TaskData taskData))
        {
            taskData.isTracking = true;
            currentTaskData = taskData;
        }
        else
        {
            TaskData newTaskData = new TaskData() { currentPro = default, currentTaskId = id, isCompleted = false, isTracking = true };
            GameDataMgr.Instance.TaskDataCollection.TryAdd(id, newTaskData);
            currentTaskData = newTaskData;
        }

        // 执行任务更新事件
        OnUpdateTask?.Invoke(currentTaskInfo, currentTaskData);
    }

    /// <summary>
    /// 继续任务
    /// </summary>
    /// <param name="taskData"></param>
    public void ContinueTask(TaskData taskData)
    {
        currentTaskInfo = BinaryDataMgr.Instance.GetTable<TaskInfoContainer>().dataDic[taskData.currentTaskId];
        currentTaskData = taskData;
        OnUpdateTask?.Invoke(currentTaskInfo, currentTaskData);
    }


    /// <summary>
    /// 取消任务
    /// </summary>
    public void CancelTask()
    {
        // 取消追踪当前任务
        currentTaskData.isTracking = false;

        currentTaskInfo = null;
        currentTaskData = null;
        OnCancelTask?.Invoke();
    }
}
