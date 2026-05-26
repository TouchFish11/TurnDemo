using HotUpdate.Base.Data;
using HotUpdate.Base.Factory;

namespace HotUpdate.Game.Activity.Core
{
    public interface IActivityDataFactory : IFactory
    {
        ActivityData GetData(int activityId);
    }
}
