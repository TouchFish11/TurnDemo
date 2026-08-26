using Core.DI;
using Core.GlobalEvent;
using Core.GlobalEvent.Events;
using HotUpdate.Game.Dialogue;

namespace HotUpdate.Game.Interact
{
    /// <summary>
    /// 对话交互策略
    /// </summary>
    public class DialogueInteractStrategy : IInteractStrategy
    {
        [Inject] private IDialogueManager _dialogueManager;
        [Inject] private IEventCenter _eventCenter;
        
        public void Interact(IInteractable interactable)
        {
            if (interactable.InteractType != EInteractType.Dialogue) 
                return;
            
            // 开始对话
            if (!_dialogueManager.IsDialogueActive)
            {
                if(interactable is NpcObject npcObject)
                    _dialogueManager.StartDialogue(npcObject.NpcInfo.f_dialogueId);
                else
                {
                    var messageEvent = EventSource.Get<GlobalMessageEvent>();
                    messageEvent.Message = "当前对象暂不支持对话";
                    _eventCenter.TriggerEventAsync(messageEvent);
                }
            }
            else
            {
                // 继续对话
                _dialogueManager.NextDialogue();
            }
        }
    }
}
