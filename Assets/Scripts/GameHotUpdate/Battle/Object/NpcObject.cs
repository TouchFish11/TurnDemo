using Core.Components;
using Core.Serialize.Binary;
using Core.Service;
using GameHotUpdate.Dialogue;
using GameHotUpdate.Interact;
using GameHotUpdate.Main.Object;

namespace GameHotUpdate.Battle.Object
{
    /// <summary>
    /// NPC����
    /// </summary>
    public class NpcObject : EntityObject, IInteractable
    {
        public bool IsShowFloatingText { get; set; }

        public NpcInfo NpcInfo { get; private set; }
        
        private InteractTrigger _interactTrigger;

        public override void BaseInit(int id)
        {
            NpcInfo = ServiceLocator.Get<IBinaryDataManager>().GetConfig<NpcInfoContainer>(EConfigLoadType.Excel).dataDic[id];
            _interactTrigger = AddComponent<InteractTrigger>();
            _interactTrigger.Init(this);
        }

        public void Interact(IEntityObject entityObject)
        {
            // ��ʾ�Ի�����
            if (!ServiceLocator.Get<IDialogueManager>().IsDialogueActive)
            {
                ServiceLocator.Get<IDialogueManager>().StartDialogue(NpcInfo.f_dialogueId);
            }
            else
            {
                // ���жԻ�ʱ�ƽ��ı�
                ServiceLocator.Get<IDialogueManager>().NextDialogue();
            }
        }
    }
}
