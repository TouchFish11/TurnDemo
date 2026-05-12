using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.DI;
using Core.Exceptions;
using Core.Tasks.Extensions;
using Core.Utility;
using UnityEngine;
using Logger = Core.Log.Logger;
using Object = UnityEngine.Object;
using Task = System.Threading.Tasks.Task;

namespace Core.AssetBundles.Management
{
    /// <summary>
    /// 包包装器
    /// </summary>
    internal class BundleWrapper
    {
        // AB包管理器
        private readonly IAssetBundleManager _assetBundleManager;
        // AB包异步加载任务(内部)
        private Task<bool> _assetBundleCreateRequestInternalTask;
        // AB包卸载任务(内部)
        private Task _assetBundleUnloadTask;
        // 资源物理文件到加载任务的映射缓存
        private readonly Dictionary<string, Task<AssetWrapper>> _assetLoadingTasks =  new();
        // 批量加载资源任务缓存
        private readonly Dictionary<Type, Task<AssetWrapper[]>> _assetsLoadingTasks = new();
        // LFU滑动窗口
        private readonly LFUSlidingWindow _window;
        
        /// <summary>
        /// AssetBundle对象
        /// </summary>
        private AssetBundle AssetBundle { get; set; }
        
        /// <summary>
        /// 包名称
        /// </summary>
        public string BundleName { get; }

        /// <summary>
        /// 包加载路径
        /// </summary>
        public string LoadPath { get; }
        
        /// <summary>
        /// 包引用数，当前存活的资源引用数
        /// </summary>
        public uint RefCount { get; private set; }
        
        /// <summary>
        /// 上次访问的时间
        /// </summary>
        public double LastAccessTime { get; private set; }
        
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 获取当前包 LFU 热度值
        /// </summary>
        public int AccessCount => _window.GetCurrentHotness();
        
        /// <summary>
        /// 在访问资源时触发回调
        /// </summary>
        public Action<BundleWrapper> OnAccessAsset;
        
        /// <summary>
        /// AB包是否为null
        /// </summary>
        public bool IsNull => !AssetBundle;
        
        /// <summary>
        /// 包装载器
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="path"></param>
        /// <param name="assetBundleManager"></param>
        /// <param name="window"></param>
        public BundleWrapper(string abName, string path, IAssetBundleManager assetBundleManager, LFUSlidingWindow window)
        {
            BundleName = abName;
            LoadPath = path;
            _assetBundleManager = assetBundleManager;
            _window = window;
        }

        /// <summary>
        /// 记录访问次数（热度）
        /// </summary>
        public void RecordAccess()
        {
            LastAccessTime =  TimeUtil.RealtimeSinceStartupAsDouble;
            _window.RecordAccess();
            OnAccessAsset?.Invoke(this);
        }
        
        /// <summary>
        /// 从文件加载AssetBundle
        /// </summary>
        /// <returns></returns>
        public void LoadFromFile()
        {
            try
            {
                // 已加载完成，直接返回，避免重复加载
                if (AssetBundle)
                {
                    IsActive = true;
                    return;
                }
                
                // 加载AB包
                AssetBundle = AssetBundle.LoadFromFile(LoadPath);
                IsActive = true;
                Logger.Log($"[BundleWrapper]: '{BundleName}' assetBundle is load");
            }
            catch (Exception e)
            {
                Logger.LogError($"[BundleWrapper]: '{BundleName}' assetBundle Load fail, {e.Message}");
            }
        }
        
        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="assetKey"></param>
        /// <param name="assetName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public AssetWrapper LoadAsset<T>(string assetKey, string assetName) where T : Object
        {
            var asset = AssetBundle.LoadAsset<T>(assetName);
            Retain();
            return DIContainer.Create<AssetWrapper>(parameterValues: new object[] { asset, assetKey, this });
        }
        
        /// <summary>
        /// 从文件异步加载AssetBundle
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<bool> LoadFromFileAsync(CancellationToken token = default)
        {
            // 已加载完成，直接返回，避免重复加载
            if (AssetBundle)
            {
                IsActive = true;
                return Task.FromResult(true);
            }
            
            // 正在加载相同的AB包，直接返回任务
            if (_assetBundleCreateRequestInternalTask != null)
                return _assetBundleCreateRequestInternalTask;
            
            // 异步加载AB包
            _assetBundleCreateRequestInternalTask = LoadFromFileAsyncInternal(token);
            return _assetBundleCreateRequestInternalTask;
        }

        /// <summary>
        ///  异步加载AB包（内部）
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="AssetBundleLoadException"></exception>
        private async Task<bool> LoadFromFileAsyncInternal(CancellationToken token = default)
        {
            // 异步加载AB包
            var assetBundleCreateRequestTaskHandle = AssetBundle.LoadFromFileAsync(LoadPath).ToTask(token);
            try
            {
                AssetBundle = await assetBundleCreateRequestTaskHandle.Task;
                IsActive = true;
                Logger.Log($"[{nameof(BundleWrapper)}]: '{BundleName}' assetBundle is load");
                return true;
            }
            catch (Exception e) when(e is not OperationCanceledException)
            {
                throw ExceptionFactory.ThrowAssetBundleLoadException(BundleName, e);
            }
            finally
            {
                _assetBundleCreateRequestInternalTask = null;
                assetBundleCreateRequestTaskHandle.Dispose();
            }
        }
        
        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetKey"></param>
        /// <param name="assetName"></param>
        /// <param name="token"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Task<AssetWrapper> LoadAssetAsync<T>(string assetKey, string assetName, CancellationToken token = default) where T : class
        {
            // 正在加载资源，存在缓存任务，返回同一个任务
            if (_assetLoadingTasks.TryGetValue(assetKey, out var cacheTask))
                return cacheTask;

            // 异步加载资源
            var loadingTask = LoadAssetAsyncInternal<T>(assetKey, assetName, token);
            // 缓存正在加载的任务
            if (!_assetLoadingTasks.TryAdd(assetKey, loadingTask))
            {
                // 理论上不会进到这里
                loadingTask = _assetLoadingTasks[assetKey];
            }

            return loadingTask;
        }

        /// <summary>
        /// 异步加载资源（内部）
        /// </summary>
        /// <param name="assetKey"></param>
        /// <param name="assetName"></param>
        /// <param name="token"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="AssetLoadException"></exception>
        private async Task<AssetWrapper> LoadAssetAsyncInternal<T>(string assetKey, string assetName, CancellationToken token = default) where T : class
        {
            var taskHandle = AssetBundle.LoadAssetAsync<T>(assetName).ToTask<T>(token);
            try
            {
                var asset = await taskHandle.Task;
                Retain();
                return DIContainer.Create<AssetWrapper>(parameterValues: new object[] { asset, assetKey, this });
            }
            catch (Exception e) when(e is not OperationCanceledException)
            {
                // 转换异常类型
                throw ExceptionFactory.ThrowAssetLoadException(assetName, e);
            }
            finally
            {
                taskHandle.Dispose();
                _assetLoadingTasks.Remove(assetKey);
            }
        }
        
        /// <summary>
        /// 异步加载所有资源
        /// </summary>
        /// <param name="token"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Task<AssetWrapper[]> LoadAllAssetAsync<T>(CancellationToken token = default) where T : Object
        {
            // 当前包存在该类型的批量加载任务，返回任务
            if (_assetsLoadingTasks.TryGetValue(typeof(T), out var cacheTask))
                return cacheTask;
            
            // 异步加载AB包中的所有资源
            var task = LoadAllAssetAsyncInternal<T>(token);
            if (!_assetsLoadingTasks.TryAdd(typeof(T), task))
            {
                // 理论上不会进到这里
                task = _assetsLoadingTasks[typeof(T)];
            }
            
            return task;
        }

        /// <summary>
        /// 异步加载所有资源（内部）
        /// </summary>
        /// <param name="token"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="AssetsLoadException"></exception>
        private async Task<AssetWrapper[]> LoadAllAssetAsyncInternal<T>(CancellationToken token = default) where T : Object
        {
            var handle = AssetBundle.LoadAllAssetsAsync<T>().ToTasks<T>(token);
            try
            {
                var readOnlyAssets = await handle.Task;
                var assetWrappers = new List<AssetWrapper>(readOnlyAssets.Count);
                foreach (var asset in readOnlyAssets)
                {
                    assetWrappers.Add(
                        DIContainer.Create<AssetWrapper>(parameterValues: new object[] { asset, asset.name, this }));
                    Retain();
                }

                return assetWrappers.ToArray();
            }
            catch (Exception e) when(e is not OperationCanceledException)
            {
                throw ExceptionFactory.ThrowAssetsLoadException(BundleName, typeof(T), e);
            }
            finally
            {
                handle.Dispose();
                // 无论成败都移除，允许后续重新加载
                _assetsLoadingTasks.Remove(typeof(T));
            }
        }
        
        /// <summary>
        /// 增加包引用计数
        /// </summary>
        public void Retain()
        {
            ++RefCount;
            Logger.Log($"[{nameof(BundleWrapper)}]: '{BundleName}' assetBundle is referenced, refCount updated to {RefCount}");
        }

        /// <summary>
        /// 释放指定AssetBundle，仅减少引用计数
        /// </summary>
        /// <returns></returns>
        public void Release()
        {
            if (RefCount > 0)
            {
                --RefCount;
                Logger.Log($"[BundleWrapper]: '{BundleName}' assetBundle is released, refCount updated to {RefCount}");
                
                if (RefCount != 0) 
                    return;
            
                // 释放包引用计数
                IsActive = false;
                _assetBundleManager.ReleaseDependencies(BundleName);
                return;
            }

            Logger.LogWarning($"[{nameof(BundleWrapper)}]: '{BundleName}' assetBundle refCount repeated release");
        }

        /// <summary>
        /// 尝试异步卸载AB包
        /// </summary>
        /// <param name="unloadAllLoadedObjects"></param>
        public Task TryUnloadAsync(bool unloadAllLoadedObjects)
        {
            // 卸载完成返回
            if (!AssetBundle)
            {
                return Task.CompletedTask;
            }

            if (_assetBundleUnloadTask != null)
                return _assetBundleUnloadTask;
            
            // 异步卸载AB包
            _assetBundleUnloadTask = TryUnloadAsyncInternal(unloadAllLoadedObjects);
            return _assetBundleUnloadTask;
        }

        private async Task TryUnloadAsyncInternal(bool unloadAllLoadedObjects)
        {
            // 异步卸载AB包
            var taskHandle = AssetBundle.UnloadAsync(unloadAllLoadedObjects).ToTask();
            try
            {
                await taskHandle.Task;
                // 卸载完成后置空
                AssetBundle = null;
                Logger.Log($"[BundleWrapper]: '{BundleName}' is unload, final refCount is {RefCount}");
            }
            catch (Exception e)
            {
                throw ExceptionFactory.ThrowAssetBundleUnloadException(BundleName, RefCount, e);
            }
            finally
            {
                taskHandle.Dispose();
                _assetBundleUnloadTask = null;
            }
        }
    }
}
