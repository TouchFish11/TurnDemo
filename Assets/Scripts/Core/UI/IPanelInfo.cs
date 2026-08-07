using System;
using Core.UI.ViewController;

namespace Core.UI
{
    [Obsolete("Legacy", true)]
    public interface IPanelInfo
    {
        /// <summary>
        /// 界面控制器接口
        /// </summary>
        IuiController Controller { get; }
        
        /// <summary>
        /// 界面视图接口
        /// </summary>
        UIView View { get; }
    }
}
