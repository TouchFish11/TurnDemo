using System;
using UnityEngine;

namespace Core.UI.MVC
{
    public interface IuiView
    {
        /// <summary>
        /// 界面对象
        /// </summary>
        GameObject ViewObj { get; }
        
        /// <summary>
        /// 显示
        /// </summary>
        void Show();

        /// <summary>
        /// 隐藏
        /// </summary>
        /// <param name="hideCallBack"></param>
        void Hide(Action hideCallBack = null);

        /// <summary>
        /// 获取绑定器
        /// </summary>
        /// <returns></returns>
        UIComponentBinder GetBinder();
    }
}
