using System.Threading.Tasks;
using Core.Service;
using Core.UI;
using HotUpdate.Core.Manager;
using HotUpdate.Core.Module;
using HotUpdate.Core.UI;
using HotUpdate.Core.UI.Helper;

namespace HotUpdate.Battle
{
    /// <summary>
    /// 热更战斗模块
    /// </summary>
    public class BattleModule : IBattleModule
    {
        private BattleRegistrar _battleRegistrar;
        
        public Task InitModuleAsync()
        {
            // 注册服务注册器
            ServiceLocator.Get<IGameManager>().GameServiceManager.AddRegistrar(_battleRegistrar);
            // 初始化UIHelper
            ServiceLocator.Register<IBattleUiHelper>(new BattleUiHelper(ServiceLocator.Get<IUIManager>()));
            ServiceLocator.Get<IGameManager>().GameServiceManager.AddRegistrar(new BattleRegistrar());
            return Task.CompletedTask;
        }
    }
}
