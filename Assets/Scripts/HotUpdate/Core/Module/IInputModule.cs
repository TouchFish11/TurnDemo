using Core.Components;
using HotUpdate.Core.Input;

namespace HotUpdate.Core.Module
{
    public interface IInputModule : IModule
    {
        IInputComponent AddInputComponent(IEntityObject entity);
    }
}
