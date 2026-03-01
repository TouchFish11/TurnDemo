namespace Core.AssetBundles.Update.Enum
{
    /// <summary>
    /// 下载更新阶段枚举
    /// </summary>
    public enum EUpdatePhase : byte
    {
        [StateConfig(Order = -1, IsEnabled = false)]
        None = 0,
        
        /// <summary>
        /// 下载远端清单文件
        /// </summary>
        [StateConfig(Order = 0, IsEnabled = true)]
        DownLoadRemoteListFile,

        /// <summary>
        /// 读取本地清单文件
        /// </summary>
        [StateConfig(Order = 1, IsEnabled = true)]
        GetLocalCompareFile,

        /// <summary>
        /// 对比差异
        /// </summary>
        [StateConfig(Order = 2, IsEnabled = true)]
        CompareContrast,
        
        /// <summary>
        /// 检查设备存储
        /// </summary>
        [StateConfig(Order = 3, IsEnabled = true)]
        CheckDeviceStorage,

        /// <summary>
        /// 下载资源
        /// </summary>
        [StateConfig(Order = 4, IsEnabled = true)]
        DownLoadAssets,

        /// <summary>
        /// 校验完整性
        /// </summary>
        [StateConfig(Order = 5, IsEnabled = true)]
        CheckAssetsIntegrity,

        /// <summary>
        /// 完成
        /// </summary>
        [StateConfig(Order = 6, IsEnabled = true)]
        Finished,
    }
}
