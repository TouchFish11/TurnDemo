using System.Threading.Tasks;
using Core.Service;
using Core.UI;
using HotUpdate.Core.Main;
using HotUpdate.Core.Manager;
using HotUpdate.Core.Module;
using HotUpdate.Core.Scene;
using HotUpdate.Core.UI;
using HotUpdate.Core.UI.Helper;
using HotUpdate.Core.VFX;
using HotUpdate.Main.FloatingText;
using HotUpdate.Main.Player;
using HotUpdate.Main.VFX;

namespace HotUpdate.Main
{
    /// <summary>
    /// 热更主模块
    /// </summary>
    public class MainModule : IModule
    {
        public Task InitModuleAsync()
        {
            // 注册游戏管理器
            ServiceLocator.Register<IFloatingTextManager>(FloatingTextManager.Instance);
            ServiceLocator.Register<IPlayerManager>(PlayerManager.Instance);
            ServiceLocator.Register<IVFXManager>(VFXManager.Instance);
            ServiceLocator.Register<IMainUiHelper>(new MainUiHelper(ServiceLocator.Get<IUIManager>()));
            // 初始化场景生成器
            SceneGeneratorHelper.Init(new SceneGenerator());
            return Task.CompletedTask;
        }
    }
}
