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
        // 更新结果工厂
        [Inject] protected UpdateResultFactory updateResultFactory;
        // 持有AssetBundle更新器实例
        [Inject] protected IAssetBundleUpdater assetBundleUpdater;
        // 对象池管理器接口
        [Inject] protected IPoolManager poolManager;
        // Json管理器接口
        [Inject] protected IJsonManager jsonManager;
        // 更新服务
        [Inject] protected UpdateService updateService;
        
        /// <summary>
        /// 进入状态
        /// </summary>
        public void Enter()
        {
            assetBundleUpdater.GetContext().UpdatePhase(UpdatePhase);
            OnEnter();
        }
        
        protected abstract void OnEnter();

        /// <summary>
        /// 退出状态时
        /// </summary>
        public void Exit()
        {
            OnExit();
        }
        
        protected abstract void OnExit();

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