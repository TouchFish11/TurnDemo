using Core.DI;
using Core.Log;
using Core.Pool;

namespace Core.AssetBundles.Update.Core
{
    /// <summary>
    /// 更新结果
    /// </summary>
    public class UpdateResult : IPoolData
    {
        public enum EUpdateError
        {
            /// <summary>
            /// 无
            /// 更新正常
            /// </summary>
            None,
            
            /// <summary>
            /// 下载错误
            /// 例如达到最大重试次数
            /// </summary>
            DownloadFailure,
            
            /// <summary>
            /// 本地清单文件相关错误
            /// </summary>
            LocalListFile,
            
            /// <summary>
            /// 分析AB包差异错误
            /// </summary>
            AnalyzeAssetBundle,
            
            /// <summary>
            /// 设备存储空间不足
            /// </summary>
            DriveStorage,
            
            /// <summary>
            /// AB包损坏
            /// 以Hash为校验标准
            /// </summary>
            AssetBunleBroken,
            
            /// <summary>
            /// AB包下载不完整
            /// 实际下载数和理论下载数不一致
            /// </summary>
            AssetBunleIncomplete,
            
            /// <summary>
            /// 未知
            /// 非继承UpdateException的异常归类为该类型
            /// </summary>
            Unknown,
        }
        
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// 更新异常
        /// </summary>
        public System.Exception UpdateException { get; set; }
        
        public EUpdateError UpdateError { get; set; }

        public static UpdateResult CreateSuccess()
        {
            var result = DIContainer.GetInstance<IPoolManager>().GetData<UpdateResult>();
            result.Success = true;
            result.UpdateException = null;
            result.UpdateError = EUpdateError.None;
            return result;
        }
        
        public static UpdateResult CreateFailure(EUpdateError updateError, System.Exception exception)
        {
            var result = DIContainer.GetInstance<IPoolManager>().GetData<UpdateResult>();
            result.Success = false;
            result.UpdateException = exception;
            result.UpdateError = updateError;
            
            // 记录日志
            Logger.LogError($"{nameof(UpdateResult)}.{nameof(CreateFailure)}：错误类型：{updateError}；异常：{exception.Message}");
            return result;
        }

        public void ResetData()
        {
            Success = false;
            UpdateException = null;
            UpdateError = EUpdateError.None;
        }
    }
}
