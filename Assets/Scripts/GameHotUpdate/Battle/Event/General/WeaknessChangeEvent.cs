using Game.Battle.Context;
using Game.Battle.Event;

namespace GameHotUpdate.Battle.Event.General
{
    /// <summary>
    /// ����仯�¼�
    /// </summary>
    public class WeaknessChangeEvent : BattleEvent
    {
        public WeaknessChangeEvent(IBattleContext context) : base(context)
        {

        }
    }
}
