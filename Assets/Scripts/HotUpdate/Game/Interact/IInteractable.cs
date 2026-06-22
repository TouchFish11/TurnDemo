using HotUpdate.Base.Object;
using HotUpdate.Common.Config.ExcelInfo.Info;

namespace HotUpdate.Game.Interact
{
    /// <summary>
    /// 可交互的
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Npc信息
        /// </summary>
        public NpcInfo NpcInfo { get; }

        /// <summary>
        /// 是否显示浮动文本
        /// </summary>
        public bool IsShowFloatingText { get; }

        /// <summary>
        /// 交互
        /// </summary>
        void Interact(IEntityObject entityObject);
    }
}
