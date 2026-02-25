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
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns>是否初始化成功</returns>
        Task Init();

        /// <summary>
        /// 卸载指定AB包
        /// 减少引用计数
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="unloadAllLoadedObjects"></param>
        /// <returns></returns>
        void UnloadBundle(string abName, bool unloadAllLoadedObjects = false);

        /// <summary>
        /// 异步加载指定AB包
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<AssetBundle> LoadBundleAsync(string abName, CancellationToken token = default);

        /// <summary>
        /// 卸载所有已加载的AssetBundle
        /// 调用该方法后，若需要加载AB包，需重新初始化（Init）管理器
        /// </summary>
        /// <param name="unloadAllObjects"></param>
        Task UnloadAllBundles(bool unloadAllObjects);

        /// <summary>
        /// 初始化默认包
        /// 更新使用
        /// </summary>
        /// <param name="defaultName"></param>
        Task InitDefault(string defaultName);

        /// <summary>
        /// 强制卸载未使用的AB包
        /// </summary>
        Task ForceUnloadUnuseBundle();
    }
}
