using HotUpdate.Game.Interact;

namespace HotUpdate.Game.Main.FloatingText
{
    public interface IFloatingTextManager
    {
        /// <summary>
        /// 注册并分配浮动文本对象。添加需要管理浮动文本的NPC
        /// </summary>
        /// <param name="npcObject">目标NPC对象</param>
        void RegisterAndAssign(NpcObject npcObject);
        
        /// <summary>
        /// 移除Npc对象和浮动文本，需要重新注册
        /// </summary>
        /// <param name="npcObject"></param>
        bool RemoveNpc(NpcObject npcObject);
        
        /// <summary>
        /// 清空所有NPC和浮动文本映射，回收文本对象
        /// </summary>
        void ClearCache();
    }
}
