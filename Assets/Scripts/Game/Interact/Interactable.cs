using Core.Components;

namespace Game.Interact
{
    /// <summary>
    /// �ɽ�����
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Npc信息
        /// </summary>
        NpcInfo NpcInfo { get; }

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
