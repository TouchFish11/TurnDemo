using Core.Components;
using Core.Input.ActionAsset;
using Core.Service;
using Game.Animation;
using Game.Components;
using Game.Dialogue;
using GameHotUpdate.Animation;
using GameHotUpdate.Input;
using GameHotUpdate.Move;

namespace GameHotUpdate.Dialogue
{
    /// <summary>
    /// �Ի����
    /// </summary>
    [ComponentId(typeof(DialogueComponent))]
    public class DialogueComponent : BaseComponent, IDialable
    {
        public override void Init(IEntityObject entityObject)
        {
            // �����Ի������¼�
            ServiceLocator.Get<IDialogueManager>().OnDialogueEnd += (this as IDialable).OnDialogueEnd;
            // �����Ի���ʼ�¼�
            ServiceLocator.Get<IDialogueManager>().OnDialogueStart += (this as IDialable).OnDialogueStart;
        }

        void IDialable.OnDialogueStart()
        {
            // ���ó��������������
            EntityObject.GetComponent<InputComponent>().LimitInput(nameof(MainActionMapData.Interact));
            // �л�Ϊ��������
            EntityObject.GetComponent<NormalAnimationComponent>().SetAnimationState(E_AnimationType.Idle);
            // �����ƶ�
            EntityObject.GetComponent<MoveComponent>().Disable();
        }

        void IDialable.OnDialogueEnd()
        {
            // ��������
            EntityObject.GetComponent<InputComponent>().CancelLimitInput(nameof(MainActionMapData.Interact));
            // �����ƶ�
            EntityObject.GetComponent<MoveComponent>().Enable();
        }

        public override void Destroy()
        {
            // ȡ�������Ի���ʼ�¼�
            ServiceLocator.Get<IDialogueManager>().OnDialogueStart -= (this as IDialable).OnDialogueStart;
            // ȡ�������Ի������¼�
            ServiceLocator.Get<IDialogueManager>().OnDialogueEnd -= (this as IDialable).OnDialogueEnd;

            base.Destroy();
        }
    }
}
