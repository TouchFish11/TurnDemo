using System.Threading;
using Core.Pool;
using UnityEngine;
using UnityEngine.Networking;

namespace Core.Tasks
{
    /// <summary>
    /// 任务工厂：用于创建各类AssetBundle相关任务实例
    /// </summary>
    internal class TaskFactory
    {
        private readonly IPoolManager _poolManager;

        private TaskFactory(IPoolManager poolManager)
        {
            _poolManager = poolManager;
        }
        
        /// <summary>
        /// 创建AssetBundle创建请求任务
        /// </summary>
        /// <param name="req">AB创建请求</param>
        /// <param name="token">取消令牌</param>
        /// <returns>AB创建请求任务实例</returns>
        public AssetBundleCreateRequestTask Create(AssetBundleCreateRequest req, CancellationToken token = default)
        {
            var assetBundleCreateRequestTask = _poolManager.GetData<AssetBundleCreateRequestTask>();
            assetBundleCreateRequestTask.Init(req, token);
            return assetBundleCreateRequestTask;
        }
        
        /// <summary>
        /// 创建泛型AssetBundle资源请求任务
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="req">AB资源请求</param>
        /// <param name="token">取消令牌</param>
        /// <returns>泛型AB资源请求任务实例</returns>
        public AssetBundleRequestTask<T> Create<T>(AssetBundleRequest req, CancellationToken token = default) where T : class
        {
            var assetBundleRequestTask = _poolManager.GetData<AssetBundleRequestTask<T>>();
            assetBundleRequestTask.Init(req, token);
            return assetBundleRequestTask;
        }

        /// <summary>
        /// 创建泛型AssetBundle资源请求任务
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="req">AB资源请求</param>
        /// <param name="token">取消令牌</param>
        /// <returns>泛型AB资源请求任务实例</returns>
        public AssetBundleRequestsTask<T> Creates<T>(AssetBundleRequest req, CancellationToken token = default) where T : class
        {
            var assetBundleRequestsTask = _poolManager.GetData<AssetBundleRequestsTask<T>>();
            assetBundleRequestsTask.Init(req, token);
            return assetBundleRequestsTask;
        }

        /// <summary>
        /// 创建AssetBundle卸载操作任务
        /// </summary>
        /// <param name="req">AB卸载操作请求</param>
        /// <returns>AB卸载操作任务实例</returns>
        public AssetBundleUnloadOperationTask Create(AssetBundleUnloadOperation req)
        {
            var assetBundleUnloadOperationTask = _poolManager.GetData<AssetBundleUnloadOperationTask>();
            assetBundleUnloadOperationTask.Init(req);
            return assetBundleUnloadOperationTask;
        }
        
        /// <summary>
        /// 创建UnityWebRequest异步操作任务
        /// </summary>
        /// <param name="req">UnityWebRequest异步操作</param>
        /// <param name="token">取消令牌</param>
        /// <returns>UnityWebRequest异步操作任务实例</returns>
        public UnityWebRequestAsyncOperationTask Create(UnityWebRequestAsyncOperation req, CancellationToken token = default)
        {
            var unityWebRequestAsyncOperationTask = _poolManager.GetData<UnityWebRequestAsyncOperationTask>();
            unityWebRequestAsyncOperationTask.Init(req, token);
            return unityWebRequestAsyncOperationTask;
        }
    }
}