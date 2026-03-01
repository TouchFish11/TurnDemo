using GameHotUpdate.Battle.Context;

namespace GameHotUpdate.Battle.Event.Skill
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
