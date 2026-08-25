using UnityEngine;

namespace Core.Tasks
{
    /// <summary>
    /// AssetBundle单个资源请求任务类
    /// </summary>
    /// <typeparam name="TResult">要加载的资源类型</typeparam>
    internal class AssetBundleRequestTask<TResult> : AoTask<TResult> where TResult : class
    {
        protected override void OnRequestCompleted()
        {
            result = ((AssetBundleRequest)operation).asset as TResult;
        }
        
        public override void Dispose()
        {
            poolManager.PushData(this);
        }
    }
}