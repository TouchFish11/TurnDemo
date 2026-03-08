using System.Threading.Tasks;
using Core.Components;
using Core.Service;
using Core.UI;
using HotUpdate.Core.Dialogue;
using HotUpdate.Core.UI.Helper;

namespace HotUpdate.Dialogue
{
    public class DialogueModule : IDialogueModule
    {
        public Task InitModuleAsync()
        {
            ServiceLocator.Register<IDialogueUiHelper>(new DialogueUiHelper(ServiceLocator.Get<IUIManager>()));
            return Task.CompletedTask;
        }

        public IDialogueComponent AddDialogueComponent(IEntityObject entityObject)
        {
            return entityObject.AddComponent<DialogueComponent>();
        }
    }
}
