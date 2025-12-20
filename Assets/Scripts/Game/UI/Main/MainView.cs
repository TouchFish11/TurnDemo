using Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// 主界面
/// </summary>
public class MainView : UIView
{
    private ScrollRect svInteract;
    private GameObject taskPart;
    private TextMeshProUGUI txtTaskTitle;
    private TextMeshProUGUI txtTaskDescription;

    protected override void Awake()
    {
        base.Awake();

        svInteract = binder.GetControl<ScrollRect>(nameof(svInteract));
        taskPart = this.transform.Find(nameof(taskPart)).gameObject;
        txtTaskTitle = binder.GetControl<TextMeshProUGUI>(nameof(txtTaskTitle));
        txtTaskDescription = binder.GetControl<TextMeshProUGUI>(nameof(txtTaskDescription));
    }

    public override void UpdateView(string key, object value)
    {
        switch (key)
        {
            case "interactUIs":
                UpdateInteract(value);
                break;
            case "isActiveTaskbar":
                taskPart.SetActive((bool)value);
                break;
            case "UpdateTask":
                UpdateTask(value);
                break;
        }
    }

    private void UpdateInteract(object value)
    {
        List<InteractUI> interactUIs = value as List<InteractUI>;
        // 显示交互UI
        foreach (InteractUI interactUI in interactUIs)
        {
            interactUI.transform.SetParent(svInteract.content, false);
        }
    }

    private void UpdateTask(object value)
    {
        (TaskInfo taskInfo, TaskData taskData) = ((TaskInfo taskInfo, TaskData taskData))value;
        // 初始化任务名称
        txtTaskTitle.text = taskInfo.f_taskName;
        // 获取任务条件
        TaskConditionInfo taskCondition = BinaryDataMgr.Instance.GetConfig<TaskConditionInfoContainer>(E_ConfigLoadType.Excel).dataDic[taskInfo.f_completionConditionId];
        // 初始化描述和当前进度
        txtTaskDescription.text = $"{taskInfo.f_taskDescription}  {taskData.currentPro}/{taskCondition.f_maxPro}";
    }
}
