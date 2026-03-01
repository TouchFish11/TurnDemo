using GameHotUpdate.Data;

namespace GameHotUpdate.Task.Data
{
    public interface ITaskData : IData
    {
        public string CurrentTaskId { get; }

        public int CurrentPro { get; set; }

        public bool IsCompleted { get; set; }

        public bool IsTracking { get; set; }
    }
}
