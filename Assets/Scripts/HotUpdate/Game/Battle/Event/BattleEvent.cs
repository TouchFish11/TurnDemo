using HotUpdate.Game.Battle.Context;

namespace HotUpdate.Game.Battle.Event
{
    /// <summary>
    /// 战斗事件
    /// </summary>
    public abstract class BattleEvent
    {
        /// <summary>
        /// 战斗上下文
        /// </summary>
        public IBattleContext Context { get; }

        protected BattleEvent(IBattleContext context)
        {
            Context = context;
        }
    }
}
