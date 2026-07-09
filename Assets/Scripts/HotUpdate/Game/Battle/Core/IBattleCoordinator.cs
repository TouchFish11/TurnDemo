using HotUpdate.Game.Battle.Context;

namespace HotUpdate.Game.Battle.Core
{
    public interface IBattleCoordinator
    {
        /// <summary>
        /// 初始化战斗协调器
        /// </summary>
        /// <param name="battleContext"></param>
        void Init(IBattleContext battleContext);

        void Reset();
    }
}
