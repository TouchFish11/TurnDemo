using HotUpdate.Game.Battle.Context;

namespace HotUpdate.Game.Battle.Event
{
    public interface IBattleEventScheduler
    {
        void Init(IBattleContext context);
    }
}
