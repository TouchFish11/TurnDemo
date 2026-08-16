using HotUpdate.Base.ECModule;

namespace HotUpdate.Game.Interact
{
    /// <summary>
    /// NPC对象
    /// </summary>
    public class NpcObject : InteractObject
    {
        /// <summary>
        /// 是否显示对象头顶浮动文本
        /// </summary>
        public bool IsShowFloatingText { get; set; }

        /// <summary>
        /// NPC对象信息
        /// </summary>
        public NpcInfo NpcInfo { get; private set; }

        /// <summary>
        /// NPC专用初始化
        /// </summary>
        /// <param name="npcInfo"></param>
        public void InitNpc(NpcInfo npcInfo)
        {
            NpcInfo = npcInfo;
        }
    }
}
