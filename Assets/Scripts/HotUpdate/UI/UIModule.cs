using System.Threading.Tasks;
using Core.DI;
using Game.Module;
using HotUpdate.Base.Manager;
using HotUpdate.Base.Module;
using HotUpdate.Base.UI;
using HotUpdate.Game.Dialogue;

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
            // 注册对话管理器
            DIContainer.BindSingleton<IDialogueManager, DialogueManager>();
        }

        public Task InitModuleAsync()
        {
            return Task.CompletedTask;
        }
    }
}
