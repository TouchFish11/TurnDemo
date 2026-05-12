using HotUpdate.Base.Battle;
using HotUpdate.Base.Battle.Event;

namespace HotUpdate.Game.Battle.Event.General
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
