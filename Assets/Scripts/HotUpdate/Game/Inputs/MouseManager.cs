using System.Collections.Generic;
using Core.GlobalEvent;
using Core.GlobalEvent.Events;
using HotUpdate.Base.Input;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.Game.Input
{
    /// <summary>
    /// 鼠标管理器
    /// 负责统一管理鼠标的显示/隐藏、锁定状态，基于栈结构处理多源请求，保证状态切换的有序性
    /// 继承单例基类，确保全局唯一实例；实现IMouseManager接口
    /// </summary>
    public class MouseManager : IMouseManager
    {
        // 默认鼠标锁定模式（锁定到屏幕中心，无法拖动）
        private const CursorLockMode defaultLockMode = CursorLockMode.Locked;
        // 默认鼠标可见性（隐藏）
        private const bool defaultVisible = false;
        // 记录鼠标显示状态的请求来源标识栈，栈顶元素为当前生效的请求来源，保证"最后请求显示的来源，最先释放"的逻辑
        private readonly Stack<string> mouseVisibleSources = new();

        public MouseManager(IEventCenter eventCenter)
        {
            eventCenter.SubscribeEvent<MouseVisibleChangedEvent>(OnMouseVisibleChangedEvent);
            UpdateMouseState();
        }

        /// <summary>
        /// 强制不可见
        /// </summary>
        public void ForceInVisible()
        {
            mouseVisibleSources.Clear();
            UpdateMouseState();
        }

        /// <summary>
        /// 请求显示鼠标
        /// 仅当请求来源合法时，将来源压入栈并更新鼠标状态
        /// </summary>
        /// <param name="source">请求来源的标识（如模块名/控制器名）</param>
        private void RequestMouseVisible(string source)
        {
            // 校验请求来源是否合法，不合法则直接返回
            if (!CanVisible(source))
            {
                return;
            }

            // 若栈顶已是当前来源，说明重复请求，无需处理
            if (mouseVisibleSources.TryPeek(out var topSource) && topSource == source)
            {
                return;
            }

            // 记录请求来源并更新鼠标状态
            mouseVisibleSources.Push(source);
            UpdateMouseState();
            Logger.Log($"{source}请求显示鼠标，来源数：{mouseVisibleSources.Count}");
        }

        /// <summary>
        /// 释放鼠标显示状态
        /// 仅当当前栈顶是该来源时，弹出栈顶并更新鼠标状态（保证释放的顺序性）
        /// </summary>
        /// <param name="source">释放来源的标识（需与请求时的标识一致）</param>
        private void ReleaseMouseVisible(string source)
        {
            // 若栈为空或栈顶不是当前来源，说明不是该来源持有显示状态，无需处理
            if (!mouseVisibleSources.TryPeek(out var topSource) || topSource != source)
            {
                return;
            }

            // 弹出栈顶来源并更新鼠标状态
            mouseVisibleSources.TryPop(out _);
            UpdateMouseState();
            Logger.Log($"{source}释放鼠标显示，来源数：{mouseVisibleSources.Count}");
        }

        /// <summary>
        /// 处理鼠标可见性变更事件的回调方法
        /// 根据事件参数决定是请求显示还是释放显示
        /// </summary>
        /// <param name="mouseVisibleChangedEvent">鼠标可见性变更事件参数</param>
        private void OnMouseVisibleChangedEvent(MouseVisibleChangedEvent mouseVisibleChangedEvent)
        {
            if (mouseVisibleChangedEvent.IsVisible)
            {
                RequestMouseVisible(mouseVisibleChangedEvent.SourceName);
            }
            else
            {
                ReleaseMouseVisible(mouseVisibleChangedEvent.SourceName);
            }
        }

        /// <summary>
        /// 更新鼠标的锁定状态和可见性
        /// 栈中有元素时：显示鼠标，解除锁定；栈为空时：恢复默认状态（隐藏+锁定）
        /// </summary>
        private void UpdateMouseState()
        {
            if (mouseVisibleSources.Count > 0)
            {
                // 有显示请求时，解除鼠标锁定，显示鼠标
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            else
            {
                // 无显示请求时，恢复默认状态
                Cursor.lockState = defaultLockMode;
                Cursor.visible = defaultVisible;
            }
        }

        /// <summary>
        /// 校验请求来源是否允许显示鼠标
        /// 过滤掉MainController、GlobalMessageController来源的请求
        /// </summary>
        /// <param name="source">请求来源标识</param>
        /// <returns>true=允许显示，false=禁止显示</returns>
        private static bool CanVisible(string source)
        {
            return source != "HotUpdate.Main.UI.MainController" && source != "HotUpdate.Main.Global.UI.GlobalMessageController";
        }
        
        public bool Visible => Cursor.visible;

        public CursorLockMode LockState => Cursor.lockState;
    }
}