using Core.DI;
using HotUpdate.Game.Dialogue;

namespace HotUpdate.Game.Interact
{
    /// <summary>
    /// 对话交互策略
    /// </summary>
    public class DialogueInteractStrategy : IInteractStrategy
    {
        [Inject] private IDialogueManager _dialogueManager;
        
        public void Interact(IInteractable interactObject)
        {
            if (interactObject is not NpcObject npcObject) 
                return;
            
            // 开始对话
            if (!_dialogueManager.IsDialogueActive)
            {
                _dialogueManager.StartDialogue(npcObject.NpcInfo.f_dialogueId);
            }
            else
            {
                // 继续对话
                _dialogueManager.NextDialogue();
            }
        }
    }
}
