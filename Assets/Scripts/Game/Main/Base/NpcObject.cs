using Framework;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// NPC对象
    /// </summary>
    public class NpcObject : EntityObject, IInteractable
    {
        // NPC配置
        [SerializeField] private NpcConfig npcConfig;

        public bool IsShowFloatingText { get; set; }
        public NpcConfig NpcConfig => npcConfig;

        public void OnInteract(IEntityObject entityObject)
        {
            // 显示对话界面
            if (!DialogueManager.Instance.IsDialogueActive)
            {
                DialogueManager.Instance.StartDialogue(1);
            }
            else
            {
                // 已有对话时推进文本
                DialogueManager.Instance.NextDialogue();
            }

            if (!DialogueManager.Instance.IsDialogueActive)
            {
                // 交互完毕
                entityObject.GetComponent<InteractComponent>().QuitInteract();
            }
        }
    }
}
