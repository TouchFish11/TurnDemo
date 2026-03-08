using Core.Components;
using HotUpdate.Core.Module;

namespace HotUpdate.Core.Interact
{
    public interface IInteractModule : IModule
    {
        IInteractComponent AddInteractComponent(IEntityObject entityObject);
    }
}
