using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 任务数据
/// </summary>
[Serializable]
public class TaskData
{
    // 当前任务Id
    public string currentTaskId;
    // 当前任务进度
    public int currentPro;
    // 是否完成
    public bool isCompleted;
    // 是否正在追踪
    public bool isTracking;
}
