using HotUpdate.Base.ECModule;

namespace HotUpdate.Game.Interact
{
    /// <summary>
    /// 可交互接口
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// 交互类型
        /// </summary>
        EInteractType InteractType { get; }
            
        /// <summary>
        /// 交互提示文本
        /// </summary>
        string InteractTip { get; }

        /// <summary>
        /// 设置交互策略逻辑
        /// </summary>
        /// <param name="interactType"></param>
        /// <param name="strategy"></param>
        public void SetInteractStrategy(EInteractType interactType, IInteractStrategy strategy);
        
        /// <summary>
        /// 执行交互逻辑
        /// </summary>
        /// <param name="entityObject"></param>
        void Interact(IEntityObject entityObject);
    }
}
