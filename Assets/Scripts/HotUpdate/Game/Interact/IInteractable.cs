using HotUpdate.Base.Object;

namespace HotUpdate.Game.Interact
{
    /// <summary>
    /// 可交互的
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// 设置交互策略逻辑
        /// </summary>
        /// <param name="strategy"></param>
        public void SetInteractStrategy(IInteractStrategy strategy);
        
        /// <summary>
        /// 执行交互逻辑
        /// </summary>
        /// <param name="entityObject"></param>
        void Interact(IEntityObject entityObject);
    }
}
