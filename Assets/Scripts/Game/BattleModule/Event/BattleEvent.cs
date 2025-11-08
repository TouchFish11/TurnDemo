using GameLogic.BattleMoudule.Core;

namespace GameLogic.BattleMoudule.Event
{
    /// <summary>
    /// 战斗事件事件基类（所有战斗事件继承此类，携带上下文）
    /// </summary>
    public abstract class BattleEvent
    {
        /// <summary>
        /// 战斗上下文（存储当前回合、角色列表等全局数据）
        /// </summary>
        public IBattleContext Context { get; } 

        public BattleEvent(IBattleContext context)
        {
            Context = context;
        }
    }
}
