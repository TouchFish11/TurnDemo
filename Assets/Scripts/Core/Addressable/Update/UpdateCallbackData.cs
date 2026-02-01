#if DISABLE_ADDRESSABLES

#else
using System;

namespace Framework.Addressable.Update
{
    /// <summary>
    /// 更新回调数据
    /// </summary>
    public struct UpdateCallbackData
    {
        /// <summary>
        /// 当前更新状态
        /// </summary>
        public EUpdateState State { get; }    
            
        /// <summary>
        /// 已下载字节
        /// </summary>
        public long DownloadedBytes { get; }
            
        /// <summary>
        /// 总字节
        /// </summary>
        public long TotalBytes { get; }  
            
        /// <summary>
        /// 异常信息（失败时）
        /// </summary>
        public Exception Error { get; }

        public UpdateCallbackData(EUpdateState state, long downloadedBytes, long totalBytes, Exception error)
        {
            State = state;
            DownloadedBytes = downloadedBytes;
            TotalBytes = totalBytes;
            Error = error;
        }
    }
}
#endif

