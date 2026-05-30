using HotUpdate.Base.Collection;

namespace HotUpdate.Base.Manager
{
    public interface IQuestDataManager : IDataManager
    {
        IQuestCollection QuestCollection { get; }
    }
}
