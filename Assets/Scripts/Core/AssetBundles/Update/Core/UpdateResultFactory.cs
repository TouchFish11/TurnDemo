using Core.DI;
using Core.Log;
using Core.Pool;

namespace Core.AssetBundles.Update.Core
{
    /// <summary>
    /// 更新结果工厂
    /// </summary>
    public class UpdateResultFactory
    {
        [Inject] private IPoolManager _poolManager;
        
        
        public UpdateResult CreateSuccess()
        {
            var result = _poolManager.GetData<UpdateResult>();
            result.Success = true;
            result.UpdateException = null;
            result.UpdateError = UpdateResult.EUpdateError.None;
            return result;
        }
        
        public UpdateResult CreateFailure(UpdateResult.EUpdateError updateError, System.Exception exception)
        {
            var result = _poolManager.GetData<UpdateResult>();
            result.Success = false;
            result.UpdateException = exception;
            result.UpdateError = updateError;
            
            // 记录日志
            Logger.LogError(ELogTags.HotUpdate, $"{nameof(UpdateResultFactory)}: error type = '{updateError}',exception = {exception.Message}");
            return result;
        }
    }
}
