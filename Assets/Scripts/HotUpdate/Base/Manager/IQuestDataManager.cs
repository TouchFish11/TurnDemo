using HotUpdate.Base.Collection;
using HotUpdate.Base.Quest;

namespace HotUpdate.Base.Manager
{
    public interface IQuestDataManager : IDataManager
    {
        IQuestCollection QuestCollection { get; }
    }
}
