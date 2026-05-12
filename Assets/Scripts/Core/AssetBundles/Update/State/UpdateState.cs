using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.AssetBundles.Update.Core;
using Core.DI;
using Core.Pool;
using Core.Serialize.Json;

namespace Core.AssetBundles.Update.State
{
    /// <summary>
    /// 更新状态抽象基类
    /// 定义所有更新状态的通用接口和基础逻辑，包括状态切换、文件解析、更新终止等
    /// </summary>
    public abstract class UpdateState : IUpdateState
    {
        // 持有AssetBundle更新器实例
        [Inject] protected readonly IAssetBundleUpdater assetBundleUpdater;
        // 对象池管理器接口
        [Inject] protected readonly IPoolManager poolManager;
        // Json管理器接口
        [Inject] protected readonly IJsonManager jsonManager;
        // 更新服务
        [Inject] protected readonly UpdateService updateService;

        /// <summary>
        /// 进入状态时的回调
        /// </summary>
        public virtual void Enter()
        {
            assetBundleUpdater.GetContext().UpdatePhase(UpdatePhase);
        }

        /// <summary>
        /// 执行状态核心逻辑
        /// </summary>
        /// <returns>是否执行成功</returns>
        public abstract Task<UpdateResult> Execute();

        /// <summary>
        /// 退出状态时的回调
        /// </summary>
        public virtual void Exit()
        {

        }

        /// <summary>
        /// 解析AssetBundle对比文件（本地/远程清单）
        /// 将JSON格式的清单内容反序列化为包集合，并加入对应上下文集合
        /// </summary>
        /// <param name="catalogJson">目录Json</param>
        /// <param name="analyzeType">解析类型（本地/远程）</param>
        protected void AnalyzeCatalog(string catalogJson, EFileAnalyzeType analyzeType)
        {
            // 反序列化JSON到包集合
            var catalog = jsonManager.FromJson<AssetCatalog>(catalogJson);
            var collection = catalog.ABPackageCollection;
            
            // 根据解析类型，将包信息加入本地/远程集合
            if (analyzeType == EFileAnalyzeType.Local)
            {
                foreach (var info in collection)
                {
                    assetBundleUpdater.GetContext().LocalPackageCollection.TryAdd(info.Key, info.Value);
                }
            }
            else
            {
                foreach (var info in collection)
                {
                    assetBundleUpdater.GetContext().RemotePackageCollection.TryAdd(info.Key, info.Value);
                }
            }
        }

        /// <summary>
        /// 当前状态对应的更新阶段
        /// </summary>
        public abstract EUpdatePhase UpdatePhase { get; }
    }
}