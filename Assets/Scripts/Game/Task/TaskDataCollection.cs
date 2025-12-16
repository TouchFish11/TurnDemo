using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 任务数据集合
/// </summary>
public class TaskDataCollection : Collection<string, TaskData>
{
    public bool ContainTask(string id)
    {
        foreach (string cacheId in keyToValueMap.Keys)
        {
            if (TextUtility.Split(cacheId, 7)[0] == TextUtility.Split(id, 7)[0])
            {
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// 是否完成
    /// </summary>
    /// <param name="taskId"></param>
    /// <returns></returns>
    public bool IsFinished(string taskId)
    {
        return keyToValueMap[taskId].isCompleted;
    }

    /// <summary>
    /// 是否有正在追踪任务
    /// </summary>
    /// <param name="taskId"></param>
    /// <returns></returns>
    public bool IsTracking(out TaskData taskData)
    {
        foreach (TaskData cacheTaskData in keyToValueMap.Values)
        {
            if (cacheTaskData.isTracking)
            {
                taskData = cacheTaskData;
                return true;
            }
        }
        taskData = null;
        return false;
    }
}
