using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace Core.Tasks.Extensions
{
    /// <summary>
    /// 任务等待器拓展类
    /// 为Unity的AssetBundle相关异步操作提供Task封装拓展方法，方便异步等待和取消
    /// </summary>
    internal static class TaskAwaiterExtensions
    {
        /// <summary>
        /// 将AssetBundleCreateRequest异步请求封装为可等待的Task
        /// </summary>
        /// <param name="req">AssetBundle创建请求实例</param>
        /// <param name="token">取消令牌，可选参数，用于取消异步操作</param>
        /// <returns>封装后的AssetBundleCreateRequestTask任务实例</returns>
        public static TaskHandle<AssetBundle> ToTask(this AssetBundleCreateRequest req, CancellationToken token = default)
        {
            var task = TaskFactory.Create(req, token);
            return new TaskHandle<AssetBundle>(task);
        }
        
        /// <summary>
        /// 将泛型AssetBundleRequest异步请求封装为可等待的泛型Task
        /// </summary>
        /// <typeparam name="T">加载的资源类型，继承自UnityEngine.Object</typeparam>
        /// <param name="req">AssetBundle资源请求实例</param>
        /// <param name="token">取消令牌，可选参数，用于取消异步操作</param>
        /// <returns>封装后的泛型AssetBundleRequestTask任务实例</returns>
        public static TaskHandle<T> ToTask<T>(this AssetBundleRequest req, CancellationToken token = default) where  T : class
        {
            var task = TaskFactory.Create<T>(req, token);
            return new TaskHandle<T>(task);
        }

        /// <summary>
        /// 将泛型AssetBundleRequest异步请求封装为可等待的泛型Task
        /// </summary>
        /// <typeparam name="T">加载的资源类型，继承自UnityEngine.Object</typeparam>
        /// <param name="req">AssetBundle资源请求实例</param>
        /// <param name="token">取消令牌，可选参数，用于取消异步操作</param>
        /// <returns>封装后的泛型AssetBundleRequestTask任务实例</returns>
        public static TaskHandle<IReadOnlyList<T>> ToTasks<T>(this AssetBundleRequest req, CancellationToken token = default) where  T : class
        {
            var task = TaskFactory.Creates<T>(req, token);
            return new TaskHandle<IReadOnlyList<T>>(task);
        }
        
        /// <summary>
        /// 将AssetBundleUnloadOperation卸载操作封装为可等待的Task
        /// </summary>
        /// <param name="req">AssetBundle卸载操作实例</param>
        /// <returns>封装后的AssetBundleUnloadOperationTask任务实例</returns>
        public static TaskHandle ToTask(this AssetBundleUnloadOperation req)
        {
            var task = TaskFactory.Create(req);
            return new TaskHandle(task);
        }

        /// <summary>
        /// 将UnityWebRequestAsyncOperation操作封装为可等待的Task
        /// </summary>
        /// <param name="req"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static TaskHandle ToTask(this UnityWebRequestAsyncOperation req, CancellationToken token = default)
        {
            var task = TaskFactory.Create(req, token);
            return new TaskHandle(task);
        }
    }
}