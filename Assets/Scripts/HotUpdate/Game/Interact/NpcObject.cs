using Core.Components;
using Core.DI;
using Core.Serialize.Binary;
using HotUpdate.Base.Dialogue;
using HotUpdate.Base.Interact;
using HotUpdate.Base.Manager;
using HotUpdate.Base.Object;
using HotUpdate.Common.Config.ExcelInfo.Container;
using HotUpdate.Common.Config.ExcelInfo.Info;
using HotUpdate.Game.Main.FloatingText;
using UnityEngine;

namespace HotUpdate.Game.Interact
{
    /// <summary>
    /// NPC对象
    /// </summary>
    public class NpcObject : EntityObject, IInteractable, INpcObject
    {
        [Inject] private IDialogueManager _dialogueManager;
        [Inject] private IBinaryDataManager _binaryDataManager;
        [Inject] private IFloatingTextManager _floatingTextManager;
        
        private InteractTrigger _interactTrigger;
        
        public Transform Transform => this.gameObject.transform;

        public bool IsShowFloatingText { get; set; }

        public NpcInfo NpcInfo { get; private set; }
        
        public void InitNpc(int id)
        {
            BaseInit(id);
        }

        public override void BaseInit(int id)
        {
            NpcInfo = _binaryDataManager.GetConfig<NpcInfoContainer>(EConfigLoadType.Excel).dataDic[id];
            _interactTrigger = AddComponent<InteractTrigger>();
            _interactTrigger.Init(this);
            _floatingTextManager.AddNpc(this);
        }

        public void Interact(IEntityObject entityObject)
        {
            // ��ʾ�Ի�����
            if (!_dialogueManager.IsDialogueActive)
            {
                _dialogueManager.StartDialogue(NpcInfo.f_dialogueId);
            }
            else
            {
                // ���жԻ�ʱ�ƽ��ı�
                _dialogueManager.NextDialogue();
            }
        }
    }
}
