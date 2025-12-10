using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 任务类型容器
/// </summary>
public class TaskTypeContainer : UIBehaviour
{
    private UIComponentBinder uIComponentBinder;
    private TextMeshProUGUI txtTaskName;

    private readonly List<TaskInfo> taskInfos = new List<TaskInfo>();
    private readonly List<TaskItem> taskItems = new List<TaskItem>();

    private int taskType;
    private bool isExpand = true;

    /// <summary>
    /// 点击任务概述事件
    /// </summary>
    public event Action<string> OnClickTaskOverview;

    protected override void Awake()
    {
        uIComponentBinder = new UIComponentBinder(this);
        uIComponentBinder.OnButtonClick += OnButtonClick;

        txtTaskName = uIComponentBinder.GetControl<TextMeshProUGUI>(nameof(txtTaskName));
    }

    private void OnButtonClick(string btnName)
    {
        switch (btnName)
        {
            case "btnTaskSummary":
                if(isExpand)
                {
                    Fold();
                }
                else
                {
                    Expand();
                }
                isExpand = !isExpand;
                break;
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="taskType"></param>
    /// <param name="taskName"></param>
    public void Init(int taskType, string taskTypeName)
    {
        this.taskType = taskType;
        //txtTaskName.text = taskName;
    }

    /// <summary>
    /// 添加任务项
    /// </summary>
    /// <param name="taskItem"></param>
    public void AddItem(TaskItem taskItem, TaskInfo taskInfo)
    {
        taskItems.Add(taskItem);
        taskInfos.Add(taskInfo);
    }

    /// <summary>
    /// 折叠
    /// </summary>
    private void Fold()
    {
        foreach (TaskItem taskItem in taskItems)
        {
            PoolManager.Instance.PushObj(taskItem.gameObject);
        }
        taskItems.Clear();
    }

    /// <summary>
    /// 展开
    /// </summary>
    private async void Expand()
    {
        for (int i = 0; i < taskInfos.Count; i++)
        {
           GameObject taskItemObj = await PoolManager.Instance.GetAssetBundleObjAsync(E_AssetBundleType.UI, "TaskItem");
            taskItemObj.transform.SetParent(this.transform, false);
            TaskItem taskItem = taskItemObj.GetComponent<TaskItem>();
            taskItem.Init(taskInfos[i], null);  // 暂时传空
            taskItems.Add(taskItem);
        }
    }

    /// <summary>
    /// 清除任务项
    /// </summary>
    public void ClearItem()
    {
        taskItems.Clear();
    }
}
