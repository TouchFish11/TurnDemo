using Framework;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// NPC对象
    /// </summary>
    public class NpcObject : EntityObject, IInteractable
    {
        public bool IsShowFloatingText { get; set; }

        public NpcInfo NpcInfo { get; private set; }

        public override void BaseInit(int id)
        {
            NpcInfo = BinaryDataMgr.Instance.GetConfig<NpcInfoContainer>(E_ConfigLoadType.Excel).dataDic[id];
        }

        public void OnInteract(IEntityObject entityObject)
        {
            // 显示对话界面
            if (!DialogueManager.Instance.IsDialogueActive)
            {
                DialogueManager.Instance.StartDialogue(NpcInfo.f_dialogueId);
            }
            else
            {
                // 已有对话时推进文本
                DialogueManager.Instance.NextDialogue();
            }
        }
    }
}
