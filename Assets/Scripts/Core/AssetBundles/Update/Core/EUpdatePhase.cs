namespace Core.AssetBundles.Update.Core
{
    /// <summary>
    /// 下载更新阶段枚举
    /// </summary>
    public enum EUpdatePhase : byte
    {
        [UpdateStateConfig(Order = -1, IsEnabled = false)]
        None = 0,
        
        /// <summary>
        /// 下载远端目录文件
        /// </summary>
        [UpdateStateConfig(Order = 0, IsEnabled = true)]
        DownLoadRemoteCatalogFile,

        /// <summary>
        /// 读取本地目录文件
        /// </summary>
        [UpdateStateConfig(Order = 1, IsEnabled = true)]
        LoadLocalCatalogFile,

        /// <summary>
        /// 对比差异
        /// </summary>
        [UpdateStateConfig(Order = 2, IsEnabled = true)]
        CompareContrast,
        
        /// <summary>
        /// 检查设备存储
        /// </summary>
        [UpdateStateConfig(Order = 3, IsEnabled = false)]
        CheckDeviceStorage,

        /// <summary>
        /// 下载资源
        /// </summary>
        [UpdateStateConfig(Order = 4, IsEnabled = true)]
        DownLoadAssets,

        /// <summary>
        /// 校验完整性
        /// </summary>
        [UpdateStateConfig(Order = 5, IsEnabled = true)]
        CheckAssetsIntegrity,

        /// <summary>
        /// 完成
        /// </summary>
        [UpdateStateConfig(Order = 6, IsEnabled = true)]
        Finished,
    }
}
