#if DISABLE_ADDRESSABLES

#else
namespace Framework.Addressable.Update
{
    /// <summary>
    /// 更新状态
    /// </summary>
    public enum EUpdateState
    {
        /// 未开始
        None,        
        /// 检查目录更新中
        Checking,    
        /// 目录检查完成（有/无更新）
        CheckSuccess,
        /// 目录检查失败
        CheckFailed, 
        /// 资源下载中
        Updating,    
        /// 资源更新完成
        UpdateSuccess,
        /// 资源更新失败
        UpdateFailed, 
    }
}
#endif
