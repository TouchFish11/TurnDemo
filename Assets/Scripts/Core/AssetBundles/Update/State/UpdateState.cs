using System.Threading.Tasks;
using Core.AssetBundles.Update.Collection;
using Core.AssetBundles.Update.Enum;
using Core.DataPersistence.Json;
using Core.Service;

namespace Core.AssetBundles.Update.State
{
    /// <summary>
    /// 更新状态抽象基类
    /// 定义所有更新状态的通用接口和基础逻辑，包括状态切换、文件解析、更新终止等
    /// </summary>
    public abstract class UpdateState : IUpdateState
    {
        // 持有AssetBundle更新器实例（用于访问上下文、切换状态）
        protected readonly AssetBundleUpdater assetBundleUpdater;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="updater">AssetBundle更新器实例</param>
        protected UpdateState(AssetBundleUpdater updater)
        {
            assetBundleUpdater = updater;
        }

        /// <summary>
        /// 进入状态时的回调（默认实现：更新当前阶段标识）
        /// </summary>
        public virtual void Enter()
        {
            assetBundleUpdater.GetContext().UpdatePhase(UpdatePhase);
        }

        /// <summary>
        /// 执行状态核心逻辑（抽象方法，子类实现）
        /// </summary>
        /// <returns>是否执行成功</returns>
        public abstract Task<bool> Execute();

        /// <summary>
        /// 退出状态时的回调（默认空实现，子类可重写）
        /// </summary>
        public virtual void Exit()
        {

        }

        /// <summary>
        /// 终止更新流程（切换到完成状态）
        /// </summary>
        protected void FinishUpdate()
        {
            assetBundleUpdater.ChangeState(EUpdatePhase.Finished);
        }

        /// <summary>
        /// 解析AssetBundle对比文件（本地/远程清单）
        /// 将JSON格式的清单内容反序列化为包集合，并加入对应上下文集合
        /// </summary>
        /// <param name="listInfo">清单文件的JSON内容</param>
        /// <param name="analyzeType">解析类型（本地/远程）</param>
        protected void AnalyzeCompareFileInfo(string listInfo, EFileAnalyzeType analyzeType)
        {
            // 反序列化JSON到包集合
            var collection = ServiceLocator.Get<IJsonManager>().FromJson<ABPackageCollection>(listInfo);
            
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
        /// 当前状态对应的更新阶段（抽象属性，子类实现）
        /// </summary>
        public abstract EUpdatePhase UpdatePhase { get; }

        /// <summary>
        /// 状态执行是否成功
        /// </summary>
        public bool IsSuceess { get; set; }
    }
}