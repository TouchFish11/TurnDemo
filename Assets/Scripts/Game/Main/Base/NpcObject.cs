using Framework;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// NPC对象
    /// </summary>
    public class NpcObject : EntityObject, IInteractable
    {
        public bool IsInteractable { get; private set; }

        public string NpcName => "测试名称123";

        public void OnInteract(IEntityObject entityObject)
        {
            // 显示对话界面
            LogMgr.Log($"交互中");


            // 交互完毕
            entityObject.GetComponent<InteractComponent>().QuitInteract();
        }
    }
}
