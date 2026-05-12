using HotUpdate.Base.Provider;

namespace HotUpdate.Base.Quest
{
    public interface ITaskDataProvider : IDataProvider
    {
        IQuestCollection QuestCollection { get; }
    }
}
