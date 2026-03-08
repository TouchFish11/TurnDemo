using Core.Components;
using HotUpdate.Core.Module;

namespace HotUpdate.Core.Dialogue
{
    public interface IDialogueModule : IModule
    {
        IDialogueComponent AddDialogueComponent(IEntityObject entityObject);
    }
}
