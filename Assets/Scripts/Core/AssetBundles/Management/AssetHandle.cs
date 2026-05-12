using System;
using System.Collections.Generic;

namespace Core.AssetBundles.Management
{
    /// <summary>
    /// 资源句柄
    /// </summary>
    public struct AssetHandle : IEquatable<AssetHandle>, IDisposable
    {
        /// <summary>
        /// 自身句柄唯一ID
        /// </summary>
        internal int HandleId { get; set; }

        /// <summary>
        /// 资源定位对象的版本号
        /// </summary>
        internal int Version { get; set; }
        
        internal bool IsCombine { get; set; }

        internal List<AssetHandle> CombineHandles { get; set; }
        
        /// <summary>
        /// 转换为泛型句柄
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public AssetHandle<T> ConvertTo<T>() where T : class
        {
            return new AssetHandle<T>(this);
        }
        
        void IDisposable.Dispose()
        {
            if (CombineHandles != null && CombineHandles.Count > 0)
            {
                foreach (var handle in CombineHandles)
                {
                    GameAsset.Release(handle);
                }
            }
            
            GameAsset.Release(this);
        }

        public static bool operator ==(AssetHandle handle1, AssetHandle handle2) => handle1.HandleId == handle2.HandleId && handle1.Version == handle2.Version;
        public static bool operator !=(AssetHandle handle1, AssetHandle handle2) => !(handle1 == handle2);
        public override bool Equals(object obj) => obj is AssetHandle handle && Equals(handle);
        public bool Equals(AssetHandle other) => this == other;
        public override int GetHashCode() => HashCode.Combine(HandleId, Version);
    }
    
    /// <summary>
    /// 泛型资源句柄
    /// </summary>
    public readonly struct AssetHandle<T> : IDisposable where T : class
    {
        // 内部非泛型句柄实例
        private readonly AssetHandle _innerHandle;

        public AssetHandle(AssetHandle inner)
        {
            _innerHandle = inner;
        }

        /// <summary>
        /// 资源，若是组合类型的句柄则返回null，资源存储在列表中
        /// </summary>
        public T Asset => _innerHandle.IsCombine ? null : GameAsset.GetAsset<T>(_innerHandle.HandleId, _innerHandle.Version);

        /// <summary>
        /// 资源列表，若是非组合类型的句柄，返回空列表
        /// </summary>
        public List<T> Assets => !_innerHandle.IsCombine ? new List<T>() : _innerHandle.CombineHandles.ConvertAll(handle => handle.ConvertTo<T>().Asset);

        void IDisposable.Dispose()
        {
            (_innerHandle as IDisposable).Dispose();
        }

        public static implicit operator AssetHandle(AssetHandle<T> handle) => handle._innerHandle;
    }
}
