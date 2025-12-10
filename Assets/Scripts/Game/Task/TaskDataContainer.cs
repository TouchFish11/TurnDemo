using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 任务数据容器
/// </summary>
public class TaskDataContainer : ISerializationCallbackReceiver
{
    [SerializeField] private List<string> taskIds = new List<string>();
    [SerializeField] private List<TaskData> taskDatas = new List<TaskData>();

    public Dictionary<string, TaskData> idToDataMap = new Dictionary<string, TaskData>();

    public TaskData this[string taskId]
    {
        get
        {
            if (idToDataMap.TryGetValue(taskId, out var taskData))
            {
                return taskData;
            }
            return null;
        }
    }

    public bool Contain(string taskId)
    {
        return idToDataMap.ContainsKey(taskId);
    }

    public bool IsFinished(string taskId)
    {
        return idToDataMap[taskId].isFinished;
    }

    public void OnBeforeSerialize()
    {
        taskIds.Clear();
        taskDatas.Clear();

        foreach (KeyValuePair<string, TaskData> pair in idToDataMap)
        {
            taskIds.Add(pair.Key);
            taskDatas.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        idToDataMap.Clear();

        int count1 = taskIds.Count;
        int count2 = taskDatas.Count;

        int count = Mathf.Min(count1, count2);

        for (int i = 0; i < count; i++)
        {
            idToDataMap.Add(taskIds[i], taskDatas[i]);
        }
    }
}
