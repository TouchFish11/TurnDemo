using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Systems.Memorys;

namespace Core.AssetBundles.Management
{
    /// <summary>
    /// AB包管理器接口
    /// </summary>
    internal interface IAssetBundleManager : IMemoryListener
    {
        /// <summary>
        /// 资源目录
        /// </summary>
        AssetCatalog Catalog { get; }
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns>是否初始化成功</returns>
        Task Init();
        
        /// <summary>
        /// 同步加载指定AB包
        /// </summary>
        /// <param name="abName">AB包名称（不含拓展名） </param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException">abName 没有被找到时抛出</exception>
        BundleWrapper LoadBundle(string abName);

        /// <summary>
        /// 异步加载指定AB包
        /// </summary>
        /// <param name="abName">AB包名称（不含拓展名）</param>
        /// <param name="token">取消令牌</param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException">abName 没有被找到时抛出</exception>
        Task<BundleWrapper> LoadBundleAsync(string abName, CancellationToken token = default);

        /// <summary>
        /// 释放指定包的所有依赖包，用于减少依赖项的引用计数
        /// </summary>
        /// <param name="abName">AB包名称（不含拓展名）</param>
        void ReleaseDependencies(string abName);
        
        /// <summary>
        /// 卸载所有已加载的AssetBundle
        /// </summary>
        /// <param name="unloadAllObjects"></param>
        Task UnloadAllBundles(bool unloadAllObjects);
    }
}
