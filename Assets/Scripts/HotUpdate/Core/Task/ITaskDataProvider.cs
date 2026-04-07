using HotUpdate.Core.Provider;

namespace HotUpdate.Core.Task
{
    public interface ITaskDataProvider : IDataProvider
    {
        /// <summary>
        /// 任务数据集合
        /// </summary>
        ITaskDataCollection TaskDataCollection { get; }
        
        IQuestCollection QuestCollection { get; }
    }
}
