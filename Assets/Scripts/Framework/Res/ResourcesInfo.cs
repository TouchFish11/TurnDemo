using UnityEngine;
using UnityEngine.Events;

namespace Framework
{
    /// <summary>
    /// 资源信息类
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    public class ResourcesInfo<T> : BaseResourcesInfo where T : Object
    {
        //存储的资源对象
        private T _asset;
        //是否要删除的标识
        private bool _isDelete;
        //资源协程对象
        private Coroutine _resCoroutine;
        //资源回调函数
        public event UnityAction<T> ResCallBack;

        /// <summary>
        /// 资源
        /// </summary>
        public T Asset { get { return _asset; } set { _asset = value; } }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get { return _isDelete; } set { _isDelete = value; } }

        /// <summary>
        /// 协程对象
        /// </summary>
        public Coroutine ResCoroutine { get { return _resCoroutine; } set { _resCoroutine = value; } }

        public ResourcesInfo(UnityAction<T> assetCallBack)
        {
            ++_refCount;
            this.ResCallBack += assetCallBack;
        }

        /// <summary>
        /// 执行回调
        /// </summary>
        public void Invoke()
        {
            ResCallBack?.Invoke(_asset);
            ResCallBack = null;
        }
    }
}
