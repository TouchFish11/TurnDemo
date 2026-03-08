using Core.Service;
using Core.UI;
using HotUpdate.Core.Main;
using HotUpdate.Core.Manager;
using HotUpdate.Core.Scene;
using HotUpdate.Core.UI.Helper;
using HotUpdate.Core.VFX;
using HotUpdate.Main.FloatingText;
using HotUpdate.Main.Player;
using HotUpdate.Main.VFX;

namespace HotUpdate.Main
{
    /// <summary>
    /// 游戏主模块注册器
    /// </summary>
    public class MainRegistrar : IGameServiceRegistrar
    {
        public void RegisterServices()
        {
            ServiceLocator.Register<IFloatingTextManager>(FloatingTextManager.Instance);
            ServiceLocator.Register<IPlayerManager>(PlayerManager.Instance);
            ServiceLocator.Register<IVFXManager>(VFXManager.Instance);
            ServiceLocator.Register<IMainUiHelper>(new MainUiHelper(ServiceLocator.Get<IUIManager>()));
            // 初始化场景生成器
            SceneGeneratorHelper.Init(new SceneGenerator());
        }
    }
}
