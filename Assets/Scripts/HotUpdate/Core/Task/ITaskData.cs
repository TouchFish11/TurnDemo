using HotUpdate.Core.Data;

namespace HotUpdate.Core.Task
{
    public interface ITaskData : IData<ITaskData>
    {
        public string CurrentTaskId { get; }

        public int CurrentPro { get; set; }

        public bool IsCompleted { get; set; }

        public bool IsTracking { get; set; }
    }
}
