using System.Threading;
using System.Threading.Tasks;
using Core.Systems.Memorys;
using UnityEngine;

namespace Core.AssetBundles.Management
{
    /// <summary>
    /// AB包管理器接口
    /// </summary>
    public interface IAssetBundleManager : IMemoryListener
    {
        string AbSuffix { get; }
        
        Task Init();
        
        void UnloadBundleAsync(EAssetBundleType assetBundleType, bool unloadAllLoadedObjects = false);

        /// <summary>
        /// 异步加载指定AB包
        /// </summary>
        /// <param name="assetBundleType"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<AssetBundle> LoadBundleAsync(EAssetBundleType assetBundleType, CancellationToken token = default);

        /// <summary>
        /// 卸载所有已加载的AssetBundle
        /// 调用该方法后，若需要加载AB包，需重新初始化（Init）管理器
        /// </summary>
        /// <param name="unloadAllObjects"></param>
        Task UnloadAllBundles(bool unloadAllObjects);
    }
}
