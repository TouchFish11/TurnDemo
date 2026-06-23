using System;
using Core.DI;
using HotUpdate.Base.Dialogue;
using HotUpdate.Base.Manager;

namespace HotUpdate.Game.Dialogue
{
    /// <summary>
    /// 对话组件逻辑对象
    /// </summary>
    public class DialogueComponentCore : IDisposable
    {
        [Inject] private IDialogueManager _dialogueManager;
        private DialogueComponent _dialogueComponent;

        public void Init(DialogueComponent dialogueComponent)
        {
            _dialogueComponent = dialogueComponent;
            // 监听对话结束事件
            _dialogueManager.OnDialogueEnd += ((IDialable)_dialogueComponent).OnDialogueEnd;
            // 监听对话开始事件
            _dialogueManager.OnDialogueStart += ((IDialable)_dialogueComponent).OnDialogueStart;
        }

        public void Dispose()
        {
            // 取消监听
            _dialogueManager.OnDialogueStart -= ((IDialable)_dialogueComponent).OnDialogueStart;
            _dialogueManager.OnDialogueEnd -= ((IDialable)_dialogueComponent).OnDialogueEnd;
            _dialogueManager = null;
            _dialogueComponent = null;
        }
    }
}
