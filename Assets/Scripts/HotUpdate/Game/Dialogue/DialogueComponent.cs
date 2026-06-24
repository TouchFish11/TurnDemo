using Core.Inputs.ActionAsset;
using HotUpdate.Base.Component;
using HotUpdate.Base.Dialogue;
using HotUpdate.Base.Enums;
using HotUpdate.Game.Animation.Component;
using HotUpdate.Game.Inputs;
using HotUpdate.Game.Main.Move;

namespace HotUpdate.Game.Dialogue
{
    /// <summary>
    /// 对话组件
    /// </summary>
    [ComponentId(typeof(DialogueComponent))]
    [ComponentCore(typeof(DialogueComponentCore))]
    public class DialogueComponent : BaseComponent, IDialable
    {
        private DialogueComponentCore _dialogueComponentCore;
        
        /// <summary>
        /// 对话组件初始化
        /// </summary>
        /// <param name="dialogueComponentCore"></param>
        public void InitDialogue(DialogueComponentCore dialogueComponentCore)
        {
            _dialogueComponentCore = dialogueComponentCore;
        }

        void IDialable.OnDialogueStart()
        {
            // 只允许交互输入
            EntityObject.GetComponent<InputComponent>().LimitInput(nameof(MainActionMapData.Interact));
            // 重置为待机动画
            EntityObject.GetComponent<NormalAnimationComponent>().SetAnimationState((int)E_AnimationType.Idle);
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

        protected override void OnBaseDestroy()
        {
            _dialogueComponentCore = null;
        }
    }
}
