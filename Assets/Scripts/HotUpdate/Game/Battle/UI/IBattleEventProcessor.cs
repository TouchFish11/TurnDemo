using System;
using HotUpdate.Game.Battle.Event;

namespace HotUpdate.Game.Battle.UI
{
    public interface IBattleEventProcessor : IDisposable
    {
        /// <summary>
        /// 统一注册所有战斗事件
        /// 将各类战斗事件与对应的处理方法绑定到事件总线
        /// </summary>
        /// <param name="eventBus">战斗事件总线</param>
        void RegisterBattleEvents(IBattleEventBus eventBus);
    }
}
