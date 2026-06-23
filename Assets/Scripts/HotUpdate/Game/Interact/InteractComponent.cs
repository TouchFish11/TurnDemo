using HotUpdate.Base.Component;

namespace HotUpdate.Game.Interact
{
    /// <summary>
    /// 交互组件
    /// 负责管理实体的交互逻辑，包括交互对象的添加/移除、交互触发、对话结束后退出交互等核心逻辑
    /// </summary>
    [ComponentId(typeof(InteractComponent))]
    [ComponentCore(typeof(InteractComponentCore))]
    public class InteractComponent : BaseComponent
    {
        private InteractComponentCore _interactComponentCore;

        protected override void OnInit()
        {
            _interactComponentCore = (InteractComponentCore)ComponentCore;
        }

        /// <summary>
        /// 添加可交互对象
        /// 将目标交互对象加入管理列表，并触发交互对象列表更新事件
        /// </summary>
        /// <param name="interactable">待添加的可交互对象（实现IInteractable接口）</param>
        public void AddInteract(IInteractable interactable)
        {
            _interactComponentCore.AddInteract(interactable);
        }

        /// <summary>
        /// 移除可交互对象
        /// 将目标交互对象从管理列表中移除，并触发交互对象列表更新事件
        /// </summary>
        /// <param name="interactable">待移除的可交互对象（实现IInteractable接口）</param>
        public void RemoveInteract(IInteractable interactable)
        {
            _interactComponentCore.RemoveInteract(interactable);
        }
        
        protected override void OnDestroyBase()
        {
            _interactComponentCore = null;
        }
    }
}