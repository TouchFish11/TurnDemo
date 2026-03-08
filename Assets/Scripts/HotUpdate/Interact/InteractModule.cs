using System.Threading.Tasks;
using Core.Components;
using HotUpdate.Core.Interact;

namespace HotUpdate.Interact
{
    public class InteractModule : IInteractModule
    {
        public Task InitModuleAsync()
        {
        
            return Task.CompletedTask;
        }

        public IInteractComponent AddInteractComponent(IEntityObject entityObject)
        {
            return entityObject.AddComponent<InteractComponent>();
        }
    }
}
