using System.Threading.Tasks;

namespace Framework
{
    /// <summary>
    /// 更新状态基类
    /// </summary>
    public abstract class UpdateState : IUpdateState
    {
        // AB包更新器对象
        protected AssetBundleUpdater assetBundleUpdater;

        protected UpdateState(AssetBundleUpdater updater)
        {
            this.assetBundleUpdater = updater;
        }

        public virtual void Enter()
        {
            assetBundleUpdater.GetContext().UpdatePhase(UpdatePhase);
        }

        public abstract Task<bool> Execute();

        public virtual void Exit()
        {

        }

        /// <summary>
        /// 完成更新
        /// </summary>
        /// <param name="isSuccess"></param>
        public void FinishUpdate()
        {
            assetBundleUpdater.ChangeState(E_UpdatePhase.Finished);
        }

        /// <summary>
        /// 解析AB包对比文件信息
        /// </summary>
        /// <param name="listInfo">AB包信息</param>
        public void AnalyzeCompareFileInfo(string listInfo, E_FileAnalyzeType analyzeType)
        {
            ABPackageCollection collection = JsonManager.Instance.FromJson<ABPackageCollection>(listInfo);
            // json反序列化
            if (analyzeType == E_FileAnalyzeType.Local)
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

        public abstract E_UpdatePhase UpdatePhase { get; }

        public bool IsSuceess { get; set; }
    }
}
