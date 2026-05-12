using HotUpdate.Base.Battle;
using HotUpdate.Base.Battle.Event;
using HotUpdate.Base.Battle.Object;

namespace HotUpdate.Game.Battle.Event.Turn
{
    /// <summary>
    /// ս��׼������¼�
    /// </summary>
    public class BattlePrepareOverEvent : BattleEvent
    {
        public IBattleEntityObject GoFirstBattleEntity { get; private set; }

        public BattlePrepareOverEvent(IBattleContext context, IBattleEntityObject goFirstBattleEntity) : base(context)
        {
            GoFirstBattleEntity = goFirstBattleEntity;
        }
    }
}
