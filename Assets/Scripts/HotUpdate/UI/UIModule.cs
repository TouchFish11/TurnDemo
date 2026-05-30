using System.Threading.Tasks;
using Core.DI;
using Game.Module;
using HotUpdate.Base.Module;
using HotUpdate.Base.UI;

namespace HotUpdate.UI
{
    /// <summary>
    /// UI模块
    /// </summary>
    [ModuleExport(typeof(IUIModule))]
    public class UIModule : IUIModule
    {
        public int Priority => 8;
        public void Register()
        {
            DIContainer.BindSingleton<IUIService, UIService>();
        }

        public Task InitModuleAsync()
        {
            return Task.CompletedTask;
        }
    }
}
