using Core.Service;
using HotUpdate.Core.Main;
using HotUpdate.Core.Manager;
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
        }
    }
}
