using Core.Components;
using Core.DI;
using Core.Serialize.Binary;
using HotUpdate.Base.Dialogue;
using HotUpdate.Base.Interact;
using HotUpdate.Base.Main;
using HotUpdate.Base.Main.Object;
using HotUpdate.Common.Config.ExcelInfo.Container;
using HotUpdate.Common.Config.ExcelInfo.Info;
using UnityEngine;

namespace HotUpdate.Game.Interact
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
            NpcInfo = DIContainer.GetInstance<IBinaryDataManager>().GetConfig<NpcInfoContainer>(EConfigLoadType.Excel).dataDic[id];
            _interactTrigger = AddComponent<InteractTrigger>();
            _interactTrigger.Init(this);
            DIContainer.GetInstance<IFloatingTextManager>().AddNpc(this);
        }

        public void Interact(IEntityObject entityObject)
        {
            // ��ʾ�Ի�����
            if (!DIContainer.GetInstance<IDialogueManager>().IsDialogueActive)
            {
                DIContainer.GetInstance<IDialogueManager>().StartDialogue(NpcInfo.f_dialogueId);
            }
            else
            {
                // ���жԻ�ʱ�ƽ��ı�
                DIContainer.GetInstance<IDialogueManager>().NextDialogue();
            }
        }
    }
}
