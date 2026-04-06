using System;
using HotUpdate.Core.Task;
using Newtonsoft.Json;

namespace HotUpdate.Task.Data
{
    /// <summary>
    /// 任务数据
    /// </summary>
    [Serializable]
    public class TaskData : ITaskData
    {
        // 当前任务ID
        [JsonProperty] public string currentTaskId;
        // 当前任务进度
        [JsonProperty] public int currentPro;
        // 是否完成
        [JsonProperty] public bool isCompleted;
        // 是否追踪
        [JsonProperty] public bool isTracking;
        
        public string CurrentTaskId => currentTaskId;

        public int CurrentPro
        {
            get => currentPro;
            set
            {
                currentPro = value;
                OnDataChanged?.Invoke(this);
            }
        }

        public bool IsCompleted
        {
            get => isCompleted;
            set
            {
                isCompleted = value;
                OnDataChanged?.Invoke(this);
            }
        }

        public bool IsTracking
        {
            get => isTracking;
            set
            {
                isTracking = value;
                OnDataChanged?.Invoke(this);
            }
        }

        public event Action<ITaskData> OnDataChanged;
    }
}
