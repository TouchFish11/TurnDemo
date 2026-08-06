using UnityEngine;

namespace Core.Tasks
{
    /// <summary>
    /// AssetBundle创建请求的异步任务封装类
    /// </summary>
    internal class AssetBundleCreateRequestTask : AoTask<AssetBundle>
    {
        protected override void OnRequestCompleted()
        {
            result = ((AssetBundleCreateRequest)_operation).assetBundle;
        }

        protected override void OnResetData()
        {
            result.Unload(true);
            base.OnResetData();
        }
    }
}