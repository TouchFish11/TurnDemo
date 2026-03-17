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
        public int Priority => 1;
        
        public Task InitModuleAsync()
        {
            LogManager.Log($"{nameof(InteractModule)}.{nameof(InitModuleAsync)}:Interact module initialization completed");
            return Task.CompletedTask;
        }

        public IInteractComponent AddInteractComponent(IEntityObject entityObject)
        {
            if (entityObject != null) return entityObject.AddComponent<InteractComponent>();
            LogManager.LogError($"{nameof(InteractModule)}.{nameof(AddInteractComponent)}: entityObject is null");
            return null;
        }
    }
}
