using HotUpdate.Battle.Context;
using HotUpdate.Battle.Object;

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
