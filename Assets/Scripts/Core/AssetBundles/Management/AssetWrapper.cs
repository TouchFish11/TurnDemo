using System;
using Core.Log;

namespace Core.AssetBundles.Management
{
    /// <summary>
    /// 单个资源包装器，单个资源和其所在的包相映射
    /// </summary>
    internal class AssetWrapper
    {
        // 代表单个包的其中一个资源
        private readonly object _asset;
        // 该资源所在的AB包
        private BundleWrapper _bundleWrapper;
        
        /// <summary>
        /// 资源Key
        /// </summary>
        public string AssetKey { get; }
        
        public string BundleName => _bundleWrapper.BundleName;
        
        public AssetWrapper(object asset, string assetKey, BundleWrapper bundleWrapper)
        {
            _asset = asset;
            AssetKey = assetKey;
            _bundleWrapper = bundleWrapper;
        }
        
        /// <summary>
        /// 引用计数
        /// </summary>
        public uint RefCount { get; private set; }
        
        /// <summary>
        /// 获取资源，同时更新AB包的访问次数(热度)
        /// </summary>
        public object Asset
        {
            get
            {
                RecordAccess();
                return _asset;
            }
        }
        
        /// <summary>
        /// 当资源被卸载的时候，触发该回调，用于通知外部清理
        /// </summary>
        public event Action OnUnload;

        /// <summary>
        /// 增加包访问次数(热度)，对包热度增加方法的封装
        /// </summary>
        public void RecordAccess()
        {
            _bundleWrapper.RecordAccess();
        }
        
        /// <summary>
        /// 增加引用计数
        /// </summary>
        public void Retain()
        {
            ++RefCount;
        }

        /// <summary>
        /// 释放引用计数
        /// </summary>
        public void Release()
        {
            if (RefCount > 0)
            {
                --RefCount;
                if (RefCount != 0) 
                    return;
            
                // 释放包引用计数
                _bundleWrapper.Release();
                _bundleWrapper = null;
                OnUnload?.Invoke();
                OnUnload = null;
                return;
            }

            Logger.LogWarning(ELogTags.Asset, $"'{AssetKey}' asset refCount repeated release");
        }
        
        /// <summary>
        /// 资源是否为空
        /// </summary>
        public bool IsNull => _asset == null;
    }
}
