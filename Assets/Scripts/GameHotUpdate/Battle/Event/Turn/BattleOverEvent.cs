using Game.Battle.Context;
using Game.Battle.Event;

namespace GameHotUpdate.Battle.Event.Turn
{
    /// <summary>
    /// ս�������¼�
    /// </summary>
    public class BattleOverEvent : BattleEvent
    {
        public BattleOverEvent(IBattleContext context) : base(context)
        {

        }
    }
}
