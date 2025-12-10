using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 任务项
/// </summary>
public class TaskItem : UIBehaviour
{
    private UIComponentBinder uIComponentBinder;
    private TextMeshProUGUI txtTaskName;

    private TaskInfo taskInfo;
    private TaskData taskData;

    /// <summary>
    /// 任务选择事件
    /// </summary>
    public event Action<TaskInfo> OnSelectedTask;

    protected override void Awake()
    {
        uIComponentBinder = new UIComponentBinder(this);
        uIComponentBinder.OnButtonClick += OnButtonClick;

        txtTaskName = uIComponentBinder.GetControl<TextMeshProUGUI>(nameof(txtTaskName));
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="taskInfo"></param>
    public void Init(TaskInfo taskInfo, TaskData taskData)
    {
        this.taskInfo = taskInfo;
        this.taskData = taskData;

        txtTaskName.text = taskInfo.f_taskName;
    }

    private void OnButtonClick(string btnName)
    {
        OnSelectedTask?.Invoke(taskInfo);
    }

    public TaskInfo GetTaskInfo()
    {
        return taskInfo;
    }
}
