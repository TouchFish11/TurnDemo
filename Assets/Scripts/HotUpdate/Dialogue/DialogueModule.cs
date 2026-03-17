using System.Threading.Tasks;
using Core.Components;
using Core.Log;
using Core.Service;
using Core.UI;
using HotUpdate.Core.Dialogue;
using HotUpdate.Core.UI.Helper;

namespace HotUpdate.Dialogue
{
    /// <summary>
    /// 对话模块
    /// </summary>
    public class DialogueModule : IDialogueModule
    {
        public int Priority => 1;
        
        public Task InitModuleAsync()
        {
            ServiceLocator.Register<IDialogueManager>(new DialogueManager());
            ServiceLocator.Register<IDialogueUiHelper>(new DialogueUiHelper(ServiceLocator.Get<IUIManager>()));
            LogManager.Log($"{nameof(DialogueModule)}.{nameof(InitModuleAsync)}:Dialogue module initialization completed");
            return Task.CompletedTask;
        }

        public IDialogueComponent AddDialogueComponent(IEntityObject entityObject)
        {
            if (entityObject != null) return entityObject.AddComponent<DialogueComponent>();
            LogManager.LogError($"{nameof(DialogueModule)}.{nameof(AddDialogueComponent)}: entityObject is null");
            return null;
        }
    }
}
