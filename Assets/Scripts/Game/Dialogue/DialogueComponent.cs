using Framework;
using Game;

/// <summary>
/// 对话组件
/// </summary>
public class DialogueComponent : BaseComponent, IDialable
{
    public override void Init(IEntityObject entityObject)
    {
        // 监听对话结束事件
        ServiceLocator.Instance.Get<IDialogueManager>().OnDialogueEnd += (this as IDialable).OnDialogueEnd;
        // 监听对话开始事件
        ServiceLocator.Instance.Get<IDialogueManager>().OnDialogueStart += (this as IDialable).OnDialogueStart;
    }

    void IDialable.OnDialogueStart()
    {
        // 禁用输入
        this.EntityObject.GetComponent<InputComponent>().DisEnableInput();
        // 切换为待机动画
        this.EntityObject.GetComponent<NormalAnimationComponent>().SetAnimationState(E_AnimationType.Idle);
        // 禁用移动
        this.EntityObject.GetComponent<MoveComponent>().Disable();
    }

    void IDialable.OnDialogueEnd()
    {
        // 启用输入
        this.EntityObject.GetComponent<InputComponent>().EnableInput();
        // 启用移动
        this.EntityObject.GetComponent<MoveComponent>().Enable();
    }

    public override void Destroy()
    {
        // 取消监听对话开始事件
        ServiceLocator.Instance.Get<IDialogueManager>().OnDialogueStart -= (this as IDialable).OnDialogueStart;
        // 取消监听对话结束事件
        ServiceLocator.Instance.Get<IDialogueManager>().OnDialogueEnd -= (this as IDialable).OnDialogueEnd;

        base.Destroy();
    }
}
