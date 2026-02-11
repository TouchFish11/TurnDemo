using System.Threading;
using Core.Pool;
using Core.Service;
using UnityEngine;

namespace Core.Tasks
{
    /// <summary>
    /// 任务工厂：用于创建各类AssetBundle相关任务实例
    /// </summary>
    public static class TaskFactory
    {
        /// <summary>
        /// 创建AssetBundle创建请求任务
        /// </summary>
        /// <param name="req">AB创建请求</param>
        /// <param name="token">取消令牌</param>
        /// <returns>AB创建请求任务实例</returns>
        public static AssetBundleCreateRequestTask Create(AssetBundleCreateRequest req, CancellationToken token = default)
        {
            var assetBundleCreateRequestTask = ServiceLocator.Get<IPoolManager>().GetData<AssetBundleCreateRequestTask>();
            assetBundleCreateRequestTask.Init(req, token);
            return assetBundleCreateRequestTask;
        }
        
        /// <summary>
        /// 创建泛型AssetBundle资源请求任务
        /// </summary>
        /// <typeparam name="T">资源类型（继承自UnityEngine.Object）</typeparam>
        /// <param name="req">AB资源请求</param>
        /// <param name="token">取消令牌</param>
        /// <returns>泛型AB资源请求任务实例</returns>
        public static AssetBundleRequestTask<T> Create<T>(AssetBundleRequest req, CancellationToken token = default) where T : Object
        {
            var assetBundleRequestTask = ServiceLocator.Get<IPoolManager>().GetData<AssetBundleRequestTask<T>>();
            assetBundleRequestTask.Init(req, token);
            return assetBundleRequestTask;
        }
        
        /// <summary>
        /// 创建AssetBundle卸载操作任务
        /// </summary>
        /// <param name="req">AB卸载操作请求</param>
        /// <param name="token">取消令牌</param>
        /// <returns>AB卸载操作任务实例</returns>
        public static AssetBundleUnloadOperationTask Create(AssetBundleUnloadOperation req, CancellationToken token = default)
        {
            var assetBundleUnloadOperationTask = ServiceLocator.Get<IPoolManager>().GetData<AssetBundleUnloadOperationTask>();
            assetBundleUnloadOperationTask.Init(req);
            return assetBundleUnloadOperationTask;
        }
    }
}