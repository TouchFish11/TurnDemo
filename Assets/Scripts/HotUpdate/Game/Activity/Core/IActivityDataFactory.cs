using HotUpdate.Base.Data;

namespace HotUpdate.Game.Activity.Core
{
    public interface IActivityDataFactory
    {
        bool tryGetData(int activityId,  out ActivityData data);
    }
}
