using System.Threading.Tasks;
using Core.Service;
using Core.UI;
using HotUpdate.Core.Manager;
using HotUpdate.Core.Module;
using HotUpdate.Core.Scene;
using HotUpdate.Core.UI;
using HotUpdate.Core.UI.Helper;

namespace HotUpdate.Main
{
    /// <summary>
    /// 热更主模块
    /// </summary>
    public class MainModule : IModule
    {
        private readonly MainRegistrar _mainRegistrar;

        public MainModule()
        {
            _mainRegistrar = new MainRegistrar();
        }
        
        public Task InitModuleAsync()
        {
            // 注册游戏管理器
            ServiceLocator.Register<IGameManager>(GameManager.Instance);
            // 注册服务注册器
            ServiceLocator.Get<IGameManager>().GameServiceManager.AddRegistrar(_mainRegistrar);
            return Task.CompletedTask;
        }
    }
}
