using Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 主界面
/// </summary>
public class MainView : UIView
{
    [Inject] private ScrollRect svInteract;
    [Inject(1)] private RectTransform taskPart;
    [Inject] private TextMeshProUGUI txtTaskTitle;
    [Inject] private TextMeshProUGUI txtTaskDescription;

    [System.Obsolete]
    public override void UpdateView(string key, object value)
    {
        switch (key)
        {
            case "interactUIs":
                UpdateInteract(value);
                break;
            case "isActiveTaskbar":
                taskPart.gameObject.SetActive((bool)value);
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
        TaskConditionInfo taskCondition = BinaryDataManager.Instance.GetConfig<TaskConditionInfoContainer>(E_ConfigLoadType.Excel).dataDic[taskInfo.f_completionConditionId];
        // 初始化描述和当前进度
        txtTaskDescription.text = $"{taskInfo.f_taskDescription}  {taskData.currentPro}/{taskCondition.f_maxPro}";
    }
}
