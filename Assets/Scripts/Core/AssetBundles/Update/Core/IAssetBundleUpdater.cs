namespace Core.AssetBundles.Update.Core
{
    /// <summary>
    /// AB包更新器接口
    /// </summary>
    public interface IAssetBundleUpdater
    {
        void CheckUpdate();
        
        ABUpdateContext GetContext();

        /// <summary>
        ///  更新阶段
        /// </summary>
        EUpdatePhase UpdatePhase { get; }

        /// 更新服务
        UpdateService UpdateService { get; }

        /// <summary>
        /// 初始化更新管理器
        /// 执行更新前先调用该方法
        /// </summary>
        void Init();
    }
}
