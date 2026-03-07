using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Event;

namespace HotUpdate.Battle.Event.Skill
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
