using Core.Components;
using Core.Serialize.Binary;
using Core.Service;
using HotUpdate.Core.Dialogue;
using HotUpdate.Core.Interact;
using HotUpdate.Core.Main;
using HotUpdate.Core.Main.Object;
using UnityEngine;

namespace HotUpdate.Interact
{
    /// <summary>
    /// NPC对象
    /// </summary>
    public class NpcObject : EntityObject, IInteractable, INpcObject
    {
        public Transform Transform => this.gameObject.transform;

        public bool IsShowFloatingText { get; set; }

        public NpcInfo NpcInfo { get; private set; }
        
        public void InitNpc(int id)
        {
            BaseInit(id);
        }

        private InteractTrigger _interactTrigger;
        private Transform transform1;

        public override void BaseInit(int id)
        {
            NpcInfo = ServiceLocator.Get<IBinaryDataManager>().GetConfig<NpcInfoContainer>(EConfigLoadType.Excel).dataDic[id];
            _interactTrigger = AddComponent<InteractTrigger>();
            _interactTrigger.Init(this);
            ServiceLocator.Get<IFloatingTextManager>().AddNpc(this);
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
