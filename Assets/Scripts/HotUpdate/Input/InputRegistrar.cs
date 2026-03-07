using Core.Service;
using HotUpdate.Core.Input;
using HotUpdate.Core.Manager;

namespace HotUpdate.Input
{
    /// <summary>
    /// 输入模块注册器
    /// </summary>
    public class InputRegistrar : IGameServiceRegistrar
    {
        public void RegisterServices()
        {
            ServiceLocator.Register<IMouseManager>(MouseManager.Instance);
        }
    }
}
