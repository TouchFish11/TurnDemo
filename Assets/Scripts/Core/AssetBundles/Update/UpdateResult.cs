using Core.Log;
using Core.Pool;
using Core.Service;

namespace Core.AssetBundles.Update
{
    /// <summary>
    /// 更新结果
    /// </summary>
    public class UpdateResult : IPoolData
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// 更新异常
        /// </summary>
        public System.Exception UpdateException { get; set; }
        
        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage { get; set; }

        public static UpdateResult CreateSuccess()
        {
            var result = ServiceLocator.Get<IPoolManager>().GetData<UpdateResult>();
            result.Success = true;
            result.UpdateException = null;
            result.ErrorMessage = string.Empty;
            return result;
        }

        public static UpdateResult CreateFailure(string errorMsg, System.Exception exception)
        {
            var result = ServiceLocator.Get<IPoolManager>().GetData<UpdateResult>();
            result.Success = false;
            result.UpdateException = exception;
            result.ErrorMessage = errorMsg;
            
            // 记录日志
            LogManager.LogError($"错误消息：{errorMsg}；异常：{exception.Message}");
            return result;
        }

        public void ResetData()
        {
            Success = false;
            UpdateException = null;
            ErrorMessage =  string.Empty;
        }
    }
}
