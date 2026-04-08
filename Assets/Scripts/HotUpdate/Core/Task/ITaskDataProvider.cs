using HotUpdate.Core.Provider;

namespace HotUpdate.Core.Task
{
    public interface ITaskDataProvider : IDataProvider
    {
        IQuestCollection QuestCollection { get; }
    }
}
