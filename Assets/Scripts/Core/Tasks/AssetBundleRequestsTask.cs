using System.Collections.Generic;
using UnityEngine;

namespace Core.Tasks
{
    /// <summary>
    /// AB包批量请求资源任务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class AssetBundleRequestsTask<T> : AoTask<IReadOnlyList<T>> where T : class
    {
        // 加载成功后的资源结果
        private readonly List<T> _assets = new();
        
        protected override void OnRequestCompleted()
        {
            var _abr = (AssetBundleRequest)_operation;
            foreach (var asset in _abr.allAssets)
            {
                _assets.Add(asset as T);
            }
            result = _assets;
        }
        
        protected override void OnResetData()
        {
            _assets.Clear();
            base.OnResetData();
        }
    }
}
