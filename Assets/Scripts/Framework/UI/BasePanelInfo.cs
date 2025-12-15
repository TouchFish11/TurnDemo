
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 面板信息基类
    /// </summary>
    public abstract class BasePanelInfo
    {
        public abstract UIView View { get; protected set; }

        public abstract UIModel Model { get; protected set; }

        public abstract IUIController Controller { get; protected set; }
    }
}
