
using UnityEngine;

namespace HotUpdate.Core.Input
{
    /// <summary>
    /// 鼠标管理器接口
    /// </summary>
    public interface IMouseManager
    {
        /// <summary>
        /// 鼠标当前可见性状态（只读属性）
        /// 对外暴露当前Cursor的可见性
        /// </summary>
        bool Visible { get; }

        CursorLockMode LockState { get; }

        /// <summary>
        /// 强制不可见
        /// </summary>
        void ForceInVisible();
    }
}
