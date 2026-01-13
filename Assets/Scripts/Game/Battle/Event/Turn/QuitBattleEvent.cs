
namespace Game.Battle
{
    /// <summary>
    /// 退出战斗事件
    /// </summary>
    public class QuitBattleEvent : BattleEvent
    {
        public QuitBattleEvent(IBattleContext context) : base(context)
        {

        }
    }
}
