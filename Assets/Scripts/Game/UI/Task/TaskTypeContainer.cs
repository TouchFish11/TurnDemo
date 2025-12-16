using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 任务类型容器
/// </summary>
public class TaskTypeContainer : UIBehaviour
{
    private UIComponentBinder uIComponentBinder;
    private TextMeshProUGUI txtTaskName;

    private readonly List<TaskItem> taskItems = new List<TaskItem>();
    private readonly Dictionary<string, TaskItem> idToItemMap = new Dictionary<string, TaskItem>();

    private int taskType;
    private bool isExpand = true;

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
    public void Init(int taskType)
    {
        this.taskType = taskType;
        txtTaskName.text = taskType.TaskTypeToStr();
    }

    public bool ContainTask(string id)
    {
        foreach (string cacheId in idToItemMap.Keys)
        {
            if (TextUtility.Split(cacheId, 7)[0] == TextUtility.Split(id, 7)[0])
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 添加任务项
    /// </summary>
    /// <param name="taskItem"></param>
    public void AddItem(TaskItem taskItem)
    {
        //taskDatas.Add(taskData);
        taskItems.Add(taskItem);
        idToItemMap.Add(taskItem.TaskId, taskItem);
    }

    /// <summary>
    /// 默认选中第一个任务
    /// </summary>
    public void DefaultSelectFirstTask()
    {
        if (taskItems.Count > 0)
        {
            taskItems[0].Select();
        }
    }

    /// <summary>
    /// 选择指定任务
    /// </summary>
    /// <param name="id"></param>
    public void SelectTask(string id)
    {
        if (idToItemMap.TryGetValue(id, out TaskItem taskItem))
        {
            taskItem.Select();
        }
    }

    /// <summary>
    /// 折叠
    /// </summary>
    private void Fold()
    {
        foreach (TaskItem taskItem in taskItems)
        {
            taskItem.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 展开
    /// </summary>
    private void Expand()
    {
        foreach (TaskItem taskItem in taskItems)
        {
            taskItem.gameObject.SetActive(true);
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
