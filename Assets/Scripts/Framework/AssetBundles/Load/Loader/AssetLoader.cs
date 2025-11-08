using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Framework
{
    /// <summary>
    /// 资源加载器
    /// </summary>
    public class AssetLoader
    {
        //资源对象
        private Object _asset;
        //资源名
        private readonly string _assetName;
        //是否卸载
        private bool _isUnload;
        //异步加载协程
        private Coroutine _coroutine;
        //资源加载回调委托
        private UnityAction<Object> _assetCallBack;
        //资源引用计数
        private uint _refCount;
        //资源加载回调事件
        public event UnityAction<Object> AssetCallBack
        {
            add
            {
                if (value != null)
                {
                    ++_refCount;
                    LogMgr.Log($"{_assetName}资源被使用，引用：{_refCount}");
                    _assetCallBack += value;
                }
                else
                {
                    _assetCallBack = null;
                }
            }

            remove
            {
                _assetCallBack -= value;
            }
        }

        public AssetLoader(string assetName, UnityAction<Object> assetCallBack = null)
        {
            this._assetName = assetName;
            this.AssetCallBack += assetCallBack;
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetBundle"></param>
        /// <returns></returns>
        public T LoadAsset<T>(AssetBundle assetBundle) where T : Object
        {
            //停止资源异步加载
            StopAssetLoadAsync();
            //转为同步加载
            _asset = assetBundle.LoadAsset<T>(_assetName);
            if (_asset == null)
            {
                LogMgr.LogError($"资源加载失败：AB包名：{assetBundle.name}，资源名：{_assetName}");
            }
            return GetAsset() as T;
        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="assetBundle"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Object LoadAsset(AssetBundle assetBundle, System.Type type)
        {
            //停止资源异步加载
            StopAssetLoadAsync();
            //转为同步加载
            _asset = assetBundle.LoadAsset(_assetName, type);
            if (_asset == null)
            {
                LogMgr.LogError($"资源加载失败：AB包名：{assetBundle.name}，资源名：{_assetName}");
            }
            return GetAsset();
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetBundle"></param>
        public void LoadAssetAsync<T>(AssetBundle assetBundle)
        {
            _coroutine = MonoManager.Instance.StartCoroutine(LoadAssetAsync_Cor());

            IEnumerator LoadAssetAsync_Cor()
            {
                //加载逻辑
                AssetBundleRequest abr = assetBundle.LoadAssetAsync<T>(_assetName);
                yield return abr;
                if (abr.asset == null)
                {
                    LogMgr.LogError($"资源加载失败：AB包名：{assetBundle.name}，资源名：{_assetName}");
                }
                //缓存资源
                this._asset = abr.asset;
                //加载完成后清空句柄
                _coroutine = null;
            }
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetBundle"></param>
        /// <param name="type"></param>
        public void LoadAssetAsync(AssetBundle assetBundle, System.Type type)
        {
            _coroutine = MonoManager.Instance.StartCoroutine(LoadAssetAsync_Cor());

            IEnumerator LoadAssetAsync_Cor()
            {
                //加载逻辑
                AssetBundleRequest abr = assetBundle.LoadAssetAsync(_assetName, type);
                yield return abr;
                if (abr.asset == null)
                {
                    LogMgr.LogError($"资源加载失败：AB包名：{assetBundle.name}，资源名：{_assetName}");
                }
                this._asset = abr.asset;
                //加载完成后清空句柄
                _coroutine = null;
            }
        }

        /// <summary>
        /// 停止指定资源的异步加载
        /// </summary>
        private void StopAssetLoadAsync()
        {
            if (_coroutine == null)
                return;

            _assetCallBack = null;
            MonoManager.Instance.StopCoroutine(_coroutine);
        }

        /// <summary>
        /// 是否存在资源
        /// </summary>
        /// <returns></returns>
        public bool ContainAsset()
        {
            return this._asset != null;
        }

        /// <summary>
        /// 尝试设置资源
        /// 若的资源不为空，则设置失败；否则设置成功
        /// </summary>
        /// <param name="asset"></param>
        /// <returns>是否设置成功</returns>
        public bool TrySetAsset(Object asset)
        {
            if (_asset == null)
            {
                this._asset = asset;
                return true;
            }
            LogMgr.LogError($"尝试设置资源失败，资源：{asset}");
            return false;
        }

        /// <summary>
        /// 获取资源
        /// </summary>
        /// <returns></returns>
        public Object GetAsset()
        {
            ++_refCount;
            LogMgr.Log($"{_assetName}资源被使用，引用：{_refCount}");
            return _asset;
        }

        /// <summary>
        /// 执行回调
        /// </summary>
        public void Invoke()
        {
            this._assetCallBack?.Invoke(_asset);
            this._assetCallBack = null;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Release()
        {
            --_refCount;
            LogMgr.Log($"{_assetName}资源尝试释放，引用：{_refCount}");
        }

        /// <summary>
        /// 资源名称
        /// </summary>
        public string Name { get => _assetName; }

        /// <summary>
        /// 是否卸载
        /// </summary>
        public bool IsUnload { get => this._isUnload; }

        /// <summary>
        /// 是否加载完成
        /// </summary>
        public bool IsDone { get => this._coroutine == null; }

        /// <summary>
        /// 资源引用计数
        /// </summary>
        public uint RefCount { get => this._refCount; }
    }
}
