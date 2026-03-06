using HotUpdate.Core.Data;

namespace HotUpdate.Core.Task
{
    public interface ITaskData : IData
    {
        public string CurrentTaskId { get; }

        public int CurrentPro { get; set; }

        public bool IsCompleted { get; set; }

        public bool IsTracking { get; set; }
    }
}
