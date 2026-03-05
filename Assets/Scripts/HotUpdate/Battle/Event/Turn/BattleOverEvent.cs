using HotUpdate.Battle.Context;

namespace HotUpdate.Battle.Event.Turn
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
