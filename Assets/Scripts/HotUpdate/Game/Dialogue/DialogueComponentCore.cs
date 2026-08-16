using Core.DI;
using Core.Inputs;
using HotUpdate.Base.Animation;
using HotUpdate.Base.ECModule;
using HotUpdate.Game.Animation.Component;
using HotUpdate.Game.Inputs;
using HotUpdate.Game.Main.Move;

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
            // 监听对话结束事件
            _dialogueManager.OnDialogueEnd += OnDialogueEnd;
            // 监听对话开始事件
            _dialogueManager.OnDialogueStart += OnDialogueStart;
        }
        
        private void OnDialogueStart()
        {
            // 只允许交互输入
            Component.EntityObject.GetComponent<InputComponent>().LimitInput(nameof(MainActionMapData.Interact));
            // 重置为待机动画
            Component.EntityObject.GetComponent<NormalAnimationComponent>().Play(EAnimationType.Idle);
            // 停止并禁用移动
            Component.EntityObject.GetComponent<MoveComponent>().Disable();
        }

        private void OnDialogueEnd()
        {
            // 取消输入限制
            Component.EntityObject.GetComponent<InputComponent>().CancelLimitInput(nameof(MainActionMapData.Interact));
            // 允许移动
            Component.EntityObject.GetComponent<MoveComponent>().Enable();
        }
        
        protected override void OnDispose()
        {
            // 取消监听
            _dialogueManager.OnDialogueStart -= OnDialogueStart;
            _dialogueManager.OnDialogueEnd -= OnDialogueEnd;
            
            _dialogueManager = null;
        }
    }
}
