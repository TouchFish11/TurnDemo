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
        public int Priority => 0;
        
        public Task InitModuleAsync()
        {
            ServiceLocator.Register<IDialogueManager>(new DialogueManager());
            ServiceLocator.Register<IDialogueUiHelper>(new DialogueUiHelper(ServiceLocator.Get<IUIManager>()));
            LogManager.Log($"{nameof(DialogueModule)}.{nameof(InitModuleAsync)}：初始化完成");
            return Task.CompletedTask;
        }

        public IDialogueComponent AddDialogueComponent(IEntityObject entityObject)
        {
            return entityObject.AddComponent<DialogueComponent>();
        }
    }
}
