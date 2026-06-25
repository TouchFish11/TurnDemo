using Core.DI;
using HotUpdate.Base.Component;
using HotUpdate.Base.Dialogue;
using HotUpdate.Base.Manager;

namespace HotUpdate.Game.Dialogue
{
    /// <summary>
    /// 对话组件逻辑对象
    /// </summary>
    public class DialogueComponentCore : ComponentCore<DialogueComponent>
    {
        [Inject] private IDialogueManager _dialogueManager;

        protected override void OnInit()
        {
            base.OnInit();
            // 监听对话结束事件
            _dialogueManager.OnDialogueEnd += ((IDialable)Component).OnDialogueEnd;
            // 监听对话开始事件
            _dialogueManager.OnDialogueStart += ((IDialable)Component).OnDialogueStart;
        }
        
        protected override void OnDispose()
        {
            // 取消监听
            _dialogueManager.OnDialogueStart -= ((IDialable)Component).OnDialogueStart;
            _dialogueManager.OnDialogueEnd -= ((IDialable)Component).OnDialogueEnd;
            
            _dialogueManager = null;
            base.OnDispose();
        }
    }
}
