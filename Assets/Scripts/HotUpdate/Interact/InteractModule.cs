using System.Threading.Tasks;
using Core.Components;
using Core.Log;
using HotUpdate.Core.Interact;

namespace HotUpdate.Interact
{
    /// <summary>
    /// 交互模块
    /// </summary>
    public class InteractModule : IInteractModule
    {
        public int Priority => 0;
        
        public Task InitModuleAsync()
        {
            LogManager.Log($"{nameof(InteractModule)}.{nameof(InitModuleAsync)}：初始化完成");
            return Task.CompletedTask;
        }

        public IInteractComponent AddInteractComponent(IEntityObject entityObject)
        {
            return entityObject.AddComponent<InteractComponent>();
        }
    }
}
