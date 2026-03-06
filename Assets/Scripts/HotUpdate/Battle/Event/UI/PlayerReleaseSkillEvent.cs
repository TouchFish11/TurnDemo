using HotUpdate.Battle.Context;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Event;

namespace HotUpdate.Battle.Event.UI
{
    /// <summary>
    ///  玩家释放技能事件
    /// 终结技不触发该事件
    /// </summary>
    public class PlayerReleaseSkillEvent : BattleEvent
    {
        public PlayerReleaseSkillEvent(IBattleContext context) : base(context)
        {

        }
    }
}
