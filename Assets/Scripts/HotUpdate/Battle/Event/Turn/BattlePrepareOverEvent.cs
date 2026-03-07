using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Event;
using HotUpdate.Core.Battle.Object;

namespace HotUpdate.Battle.Event.Turn
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
