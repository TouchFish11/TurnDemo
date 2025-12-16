using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 主界面任务逻辑类
/// </summary>
public class TaskLogic : MainLogic
{
    public TaskLogic(MainController mainController, MainModel mainModel, MainView mainView) : base(mainController, mainModel, mainView)
    {

    }

    /// <summary>
    /// 初始化
    /// </summary>
    public override void Init()
    {
        // 检测任务状态
        TaskManager.Instance.CheckTaskState();
    }

    /// <summary>
    /// 更新任务任务
    /// </summary>
    /// <param name="currentTaskInfo"></param>
    /// <param name="currentTaskData"></param>
    public void UpdateTask(TaskInfo currentTaskInfo, TaskData currentTaskData)
    {
        // 更新任务栏状态
        mainModel.IsActiveTaskbar = currentTaskData.isCompleted ? !currentTaskData.isCompleted : currentTaskData.isTracking;

        if (mainModel.IsActiveTaskbar)
        {
            // 更新任务栏任务显示
            mainModel.UpdateTask(currentTaskInfo, currentTaskData);
        }
    }

    /// <summary>
    /// 取消任务
    /// </summary>
    public void CancelTask()
    {
        SetTaskbarActive(false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isActive"></param>
    public void SetTaskbarActive(bool isActive)
    {
        mainModel.IsActiveTaskbar = isActive;
    }
}
