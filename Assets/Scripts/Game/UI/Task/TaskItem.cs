using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 任务项
/// </summary>
public class TaskItem : UIBehaviour
{
    private UIComponentBinder uIComponentBinder;
    private TextMeshProUGUI txtTaskName;
    private Image imgSel;
    private Toggle toggle;

    private string taskId;

    /// <summary>
    /// 任务选择事件
    /// </summary>
    public event Action<string> OnSelectedTask;

    protected override void Awake()
    {
        uIComponentBinder = new UIComponentBinder(this);
        uIComponentBinder.OnToggleValueChanged += OnToggleValueChanged;

        txtTaskName = uIComponentBinder.GetControl<TextMeshProUGUI>(nameof(txtTaskName));
        imgSel = uIComponentBinder.GetControl<Image>(nameof(imgSel));
        imgSel.gameObject.SetActive(false);
        toggle = uIComponentBinder.GetControl<Toggle>(this.gameObject.name);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="taskInfo"></param>
    public void Init(TaskInfo taskInfo, ToggleGroup group)
    {
        this.taskId = taskInfo.f_id;
        this.toggle.group = group;

        txtTaskName.text = taskInfo.f_taskName;
    }

    private void OnToggleValueChanged(string togName, bool isOn)
    {
        imgSel.gameObject.SetActive(isOn);
        if (isOn)
        {
            OnSelectedTask?.Invoke(taskId);
        }
    }

    /// <summary>
    /// 选择
    /// </summary>
    /// <param name="isOn"></param>
    public void Select()
    {
        toggle.isOn = true;
    }

    public string TaskId => taskId;
}
