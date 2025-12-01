
namespace Framework
{
    /// <summary>
    /// 更新阶段枚举
    /// </summary>
    public enum E_UpdatePhase : byte
    {
        None,
        /// <summary>
        /// 正在下载远端清单文件
        /// </summary>
        DownLoadRemoteListFile,

        /// <summary>
        /// 获取本地对比文件
        /// </summary>
        GetLocalCompareFile,

        /// <summary>
        /// 对比差异
        /// </summary>
        CompareContrast,

        /// <summary>
        /// 正在下载资源
        /// </summary>
        DownLoadAssets,

        /// <summary>
        /// 检查资源完整性
        /// </summary>
        CheckAssetsIntegrity,

        /// <summary>
        /// 更新完成
        /// </summary>
        Finished,

        /// <summary>
        /// 空状态（用于退出更新）
        /// </summary>
        NullState,
    }
}
