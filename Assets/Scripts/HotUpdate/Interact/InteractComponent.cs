using System.Collections.Generic;
using Core.Components;
using Core.GlobalEvent;
using Core.Service;
using HotUpdate.Core.Component;
using HotUpdate.Core.Dialogue;
using HotUpdate.Core.Interact;
using HotUpdate.Input;

namespace HotUpdate.Interact
{
    /// <summary>
    /// 交互组件
    /// 负责管理实体的交互逻辑，包括交互对象的添加/移除、交互触发、对话结束后退出交互等核心逻辑
    /// </summary>
    [ComponentId(typeof(InteractComponent))]
    public class InteractComponent : BaseComponent, IInteractComponent
    {
        // 存储当前可交互的所有交互对象
        private readonly List<IInteractable> interactables = new();
        // 当前正在进行交互的目标对象
        private IInteractable currentInteractable;

        /// <summary>
        /// 组件初始化方法
        /// 注册对话结束回调和交互输入回调
        /// </summary>
        /// <param name="entityObject">当前挂载该组件的实体对象</param>
        public override void Init(IEntityObject entityObject)
        {
            // 注册对话结束事件的回调，对话结束时退出交互状态
            ServiceLocator.Get<IDialogueManager>().OnDialogueEnd += QuitInteract;
            // 获取输入组件，注册交互输入触发的回调
            EntityObject.GetComponent<InputComponent>().OnIniteract += OnIniteract;
        }

        /// <summary>
        /// 交互输入事件的回调方法
        /// 当玩家触发交互输入时，执行对应的交互逻辑
        /// </summary>
        private void OnIniteract()
        {
            // 若无可交互对象，直接返回
            if (interactables.Count == 0)
            {
                return;
            }

            // 如果已有正在交互的对象，直接触发该对象的交互逻辑
            currentInteractable ??= interactables[0];
            // 触发选中交互对象的交互逻辑，传入当前实体对象作为交互发起方
            currentInteractable.Interact(EntityObject);
        }

        /// <summary>
        /// 添加可交互对象
        /// 将目标交互对象加入管理列表，并触发交互对象列表更新事件
        /// </summary>
        /// <param name="interactable">待添加的可交互对象（实现IInteractable接口）</param>
        public void AddInteract(IInteractable interactable)
        {
            interactables.Add(interactable);
            // 触发交互事件，通知外部交互对象列表已更新
            ServiceLocator.Get<IEventCenter>().TriggerEvent(new InteractEvent { Interactables = interactables });
        }

        /// <summary>
        /// 移除可交互对象
        /// 将目标交互对象从管理列表中移除，并触发交互对象列表更新事件
        /// </summary>
        /// <param name="interactable">待移除的可交互对象（实现IInteractable接口）</param>
        public void RemoveInteract(IInteractable interactable)
        {
            interactables.Remove(interactable);
            // 触发交互事件，通知外部交互对象列表已更新
            ServiceLocator.Get<IEventCenter>().TriggerEvent(new InteractEvent { Interactables = interactables });
        }

        /// <summary>
        /// 退出交互状态
        /// 清空当前交互对象，重置交互状态（主要在对话结束时调用）
        /// </summary>
        private void QuitInteract()
        {
            currentInteractable = null;
        }
    }
}