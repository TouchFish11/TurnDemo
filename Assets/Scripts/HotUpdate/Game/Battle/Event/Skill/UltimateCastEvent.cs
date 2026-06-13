using HotUpdate.Game.Battle.Context;

namespace HotUpdate.Game.Battle.Event.Skill
{
    /// <summary>
    /// 终结技释放事件
    /// </summary>
    public class UltimateCastEvent : BattleEvent
    {
        public UltimateCastEvent(IBattleContext context) : base(context)
        {
        
        }
    }
}
