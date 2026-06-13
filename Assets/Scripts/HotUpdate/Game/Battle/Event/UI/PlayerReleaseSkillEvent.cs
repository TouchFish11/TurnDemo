using HotUpdate.Game.Battle.Context;

namespace HotUpdate.Game.Battle.Event.UI
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
