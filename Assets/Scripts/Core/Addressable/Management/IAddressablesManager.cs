#if DISABLE_ADDRESSABLES

#else
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Framework.Addressable.Management
{
    /// <summary>
    /// Addressables管理器接口
    /// </summary>
    public interface IAddressablesManager
    {
        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<AsyncOperationHandle<T>> LoadAssetAsync<T>(string assetName);
        
        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="mode">加载模式</param>
        /// <param name="keys">资源名或标签名</param>
        Task<T> LoadAssetAsync<T>(Addressables.MergeMode mode, params string[] keys);

        /// <summary>
        /// 异步加载多个资源
        /// </summary>
        /// <param name="mergeMode"></param>
        /// <param name="keys"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [Obsolete("未完善", true)]
        Task<IList<T>> LoadAssetsAsync<T>(Addressables.MergeMode mergeMode, params string[] keys) where T : Object;

        /// <summary>
        /// 释放句柄
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        void Release<T>(string name);

        /// <summary>
        /// 释放句柄
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        void Release<T>(params string[] keys);

        /// <summary>
        /// 清空所有资源
        /// </summary>
        void Clear();
    }
}
#endif


