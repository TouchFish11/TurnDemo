using HotUpdate.Base.Collection;

namespace HotUpdate.Base.Data
{
    public interface IQuestDataProvider : IDataProvider
    {
        IQuestCollection QuestCollection { get; }
    }
}
