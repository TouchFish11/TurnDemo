using HotUpdate.Base.Battle;
using HotUpdate.Base.Battle.Event;

namespace HotUpdate.Game.Battle.Event.Skill
{
    /// <summary>
    /// 技能释放后事件
    /// </summary>
    public class PostCastEvent : BattleEvent
    {
        public PostCastEvent(IBattleContext context) : base(context)
        {
        
        }
    }
}
