using Core.Service;
using HotUpdate.Battle.Core;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Manager;

namespace HotUpdate.Battle
{
    /// <summary>
    /// 游戏战斗模块注册器
    /// </summary>
    public class BattleRegistrar : IGameServiceRegistrar
    {
        public void RegisterServices()
        {
            ServiceLocator.Register<IBattleManager>(BattleManager.Instance);
        }
    }
}
