using GameHotUpdate.Battle.Context;

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
