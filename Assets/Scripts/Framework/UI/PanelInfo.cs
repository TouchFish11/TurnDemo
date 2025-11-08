using UnityEngine.Events;

namespace Framework
{
    /// <summary>
    /// 面板信息类
    /// </summary>
    /// <typeparam name="T">面板类型</typeparam>
    public class PanelInfo<T> : BasePanelInfo
    {
        //面板对象
        private T _panel;
        //是否销毁
        private bool _isDestroy;
        //事件回调
        public event UnityAction<T> CallBack;

        public PanelInfo(UnityAction<T> callBack)
        {
            this.CallBack += callBack;
        }

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="panel"></param>
        public void Invoke(T panel)
        {
            CallBack?.Invoke(panel);
            CallBack = null;
        }

        /// <summary>
        /// 面板对象
        /// </summary>
        public T Panel { get => _panel; set => _panel = value; }
        /// <summary>
        /// 是否销毁
        /// </summary>
        public bool IsDestroy { get => _isDestroy; set => _isDestroy = value; }
    }
}
