using HotUpdate.Base.Data;
using HotUpdate.Base.Factory;

namespace HotUpdate.Game.Activity.Core
{
    public interface IActivityDataFactory
    {
        bool tryGetData(int activityId,  out ActivityData data);
    }
}
