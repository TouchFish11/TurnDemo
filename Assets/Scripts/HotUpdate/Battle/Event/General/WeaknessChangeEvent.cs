using HotUpdate.Battle.Context;

namespace HotUpdate.Battle.Event.General
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
