using Core.Components;
using HotUpdate.Core.Input;
using HotUpdate.Core.Main.Object;

namespace HotUpdate.Core.Module
{
    public interface IInputModule : IModule
    {
        IInputComponent AddInputComponent(IEntityObject entity);
    }
}
