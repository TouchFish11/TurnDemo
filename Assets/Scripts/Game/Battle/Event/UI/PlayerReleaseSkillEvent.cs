
namespace Game.Battle
{
    /// <summary>
    /// 玩家释放技能事件
    /// 不包含终结技
    /// 隐藏、更新相关UI
    /// </summary>
    public class PlayerReleaseSkillEvent : BattleEvent
    {
        public PlayerReleaseSkillEvent(IBattleContext context) : base(context)
        {

        }
    }
}
