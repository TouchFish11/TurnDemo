using Core.Inputs;
using HotUpdate.Base.Animation;
using HotUpdate.Base.Dialogue;
using HotUpdate.Base.ECModule;
using HotUpdate.Game.Animation.Component;
using HotUpdate.Game.Inputs;
using HotUpdate.Game.Main.Move;

namespace HotUpdate.Game.Dialogue
{
    /// <summary>
    /// 对话组件
    /// </summary>
    [ComponentId]
    [ComponentCore(typeof(DialogueComponentCore))]
    public class DialogueComponent : BaseComponent, IDialable
    {
        void IDialable.OnDialogueStart()
        {
            // 只允许交互输入
            EntityObject.GetComponent<InputComponent>().LimitInput(nameof(MainActionMapData.Interact));
            // 重置为待机动画
            EntityObject.GetComponent<NormalAnimationComponent>().Play(EAnimationType.Idle);
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
    }
}
