using Core.Components;
using Core.DataPersistence.Binary;
using Core.Service;
using Game.Dialogue;
using Game.Interact;
using Game.Objects;
using GameHotUpdate.Interact;
using UnityEngine;

namespace GameHotUpdate.Objects
{
    /// <summary>
    /// NPC����
    /// </summary>
    [RequireComponent(typeof(InteractTrigger))]
    public class NpcObject : EntityObject, IInteractable
    {
        public bool IsShowFloatingText { get; set; }

        public NpcInfo NpcInfo { get; private set; }
        
        private InteractTrigger  _interactTrigger;

        public override void BaseInit(int id)
        {
            NpcInfo = ServiceLocator.Get<IBinaryDataManager>().GetConfig<NpcInfoContainer>(EConfigLoadType.Excel).dataDic[id];
            _interactTrigger = this.gameObject.GetComponent<InteractTrigger>();
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
