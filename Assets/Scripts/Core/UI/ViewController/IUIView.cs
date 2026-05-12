using UnityEngine;

namespace Core.UI.ViewController
{
    public interface IuiView
    {
        /// <summary>
        /// 界面对象
        /// </summary>
        GameObject ViewObj { get; }

        /// <summary>
        /// 获取绑定器
        /// </summary>
        /// <returns></returns>
        UIComponentBinder GetBinder();
    }
}
