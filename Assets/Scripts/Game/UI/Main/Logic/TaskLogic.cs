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
    /// 更新任务任务
    /// </summary>
    /// <param name="currentTaskInfo"></param>
    /// <param name="currentTaskData"></param>
    public void UpdateTask(TaskInfo currentTaskInfo, TaskData currentTaskData)
    {
        // 激活任务栏并更新任务显示
        mainModel.IsActiveTaskbar = true;
        mainModel.UpdateTask(currentTaskInfo, currentTaskData);
    }

    /// <summary>
    /// 取消任务
    /// </summary>
    public void CancelTask()
    {
        mainModel.IsActiveTaskbar = false;
    }
}
