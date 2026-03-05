using Core.Components;
using Core.Input.ActionAsset;
using Core.Service;
using HotUpdate.Animation;
using HotUpdate.Animation.Component;
using HotUpdate.Component;
using HotUpdate.Input;
using HotUpdate.Main.Move;

namespace HotUpdate.Dialogue
{
    /// <summary>
    /// 对话组件
    /// </summary>
    [ComponentId(typeof(DialogueComponent))]
    public class DialogueComponent : BaseComponent, IDialable
    {
        private readonly IDialogueManager _dialogueManager = ServiceLocator.Get<IDialogueManager>();
        
        public override void Init(IEntityObject entityObject)
        {
            // 监听对话结束事件
            _dialogueManager.OnDialogueEnd += (this as IDialable).OnDialogueEnd;
            // 监听对话开始事件
            _dialogueManager.OnDialogueStart += (this as IDialable).OnDialogueStart;
        }

        void IDialable.OnDialogueStart()
        {
            // 只允许交互输入
            EntityObject.GetComponent<InputComponent>().LimitInput(nameof(MainActionMapData.Interact));
            // 重置为待机动画
            EntityObject.GetComponent<NormalAnimationComponent>().SetAnimationState(E_AnimationType.Idle);
            // 停止并禁用移动
            EntityObject.GetComponent<MoveComponent>().Disable();
        }

        void IDialable.OnDialogueEnd()
        {
            // 取消输入限制
            EntityObject.GetComponent<InputComponent>().CancelLimitInput(nameof(MainActionMapData.Interact));
            // 允许移动
            EntityObject.GetComponent<MoveComponent>().Enable();
        }

        public override void Destroy()
        {
            // 取消监听
            _dialogueManager.OnDialogueStart -= (this as IDialable).OnDialogueStart;
            _dialogueManager.OnDialogueEnd -= (this as IDialable).OnDialogueEnd;
            base.Destroy();
        }
    }
}
