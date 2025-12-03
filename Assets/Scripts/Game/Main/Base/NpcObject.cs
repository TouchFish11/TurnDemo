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

        public string NpcName => "测试名称1";

        public string NpcTip => "无业游民";

        public void OnInteract(IEntityObject entityObject)
        {
            // 显示对话界面
            LogMgr.Log($"交互中");


            // 交互完毕
            entityObject.GetComponent<InteractComponent>().QuitInteract();
        }
    }
}
