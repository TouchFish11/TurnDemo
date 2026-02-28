namespace Core.AssetBundles.Update.Enum
{
    /// <summary>
    /// 下载更新阶段枚举
    /// </summary>
    public enum EUpdatePhase : byte
    {
        None,
        /// <summary>
        /// 检查设备存储
        /// </summary>
        CheckDeviceStorage,
        /// <summary>
        /// 下载远端清单文件
        /// </summary>
        DownLoadRemoteListFile,

        /// <summary>
        /// 读取本地清单文件
        /// </summary>
        GetLocalCompareFile,

        /// <summary>
        /// 对比差异
        /// </summary>
        CompareContrast,

        /// <summary>
        /// 下载资源
        /// </summary>
        DownLoadAssets,

        /// <summary>
        /// 校验完整性
        /// </summary>
        CheckAssetsIntegrity,

        /// <summary>
        /// 完成
        /// </summary>
        Finished,
    }
}
