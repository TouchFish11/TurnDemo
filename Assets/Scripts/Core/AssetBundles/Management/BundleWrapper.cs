using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.DI;
using Core.Exceptions;
using Core.Log;
using Core.Tasks;
using Core.Tasks.Extensions;
using Core.Time;
using UnityEngine;
using Logger = Core.Log.Logger;
using Object = UnityEngine.Object;
using Task = System.Threading.Tasks.Task;

namespace Core.AssetBundles.Management
{
    /// <summary>
    /// 包包装器
    /// </summary>
    public class BundleWrapper
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
        internal string BundleName { get; }

        /// <summary>
        /// 包加载路径
        /// </summary>
        internal string LoadPath { get; }
        
        /// <summary>
        /// 包大小（字节）
        /// </summary>
        internal long BundleSize { get; }
        
        /// <summary>
        /// 包引用数，当前存活的资源引用数
        /// </summary>
        internal uint RefCount { get; private set; }
        
        /// <summary>
        /// 上次访问的时间
        /// </summary>
        internal double LastAccessTime { get; private set; }

        /// <summary>
        /// 获取当前包 LFU 热度值
        /// </summary>
        internal int AccessCount => _window.GetCurrentHotness();
        
        /// <summary>
        /// 在访问资源时触发回调
        /// </summary>
        internal Action<BundleWrapper> OnAccessAsset;

        /// <summary>
        /// 包装载器
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="path"></param>
        /// <param name="size"></param>
        /// <param name="assetBundleManager"></param>
        /// <param name="window"></param>
        internal BundleWrapper(string abName, string path, long size, IAssetBundleManager assetBundleManager, LFUSlidingWindow window)
        {
            BundleName = abName;
            LoadPath = path;
            BundleSize = size;
            _assetBundleManager = assetBundleManager;
            _window = window;
        }

        /// <summary>
        /// 记录访问次数（热度）
        /// </summary>
        internal void RecordAccess()
        {
            LastAccessTime =  TimeUtil.RealtimeSinceStartupAsDouble;
            _window.RecordAccess();
            OnAccessAsset?.Invoke(this);
        }
        
        /// <summary>
        /// 从文件加载AssetBundle
        /// </summary>
        /// <returns></returns>
        internal void LoadFromFile()
        {
            try
            {
                // 已加载完成，直接返回，避免重复加载
                if (AssetBundle)
                {
                    return;
                }
                
                // 加载AB包
                AssetBundle = AssetBundle.LoadFromFile(LoadPath);
                Logger.LogDebug(ELogTags.Asset, $"'{BundleName}' assetBundle is load");
            }
            catch (Exception e)
            {
                Logger.LogError(ELogTags.Asset, $"[BundleWrapper]: '{BundleName}' assetBundle Load fail, {e.Message}");
            }
        }
        
        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="assetKey"></param>
        /// <param name="assetName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal AssetWrapper LoadAsset<T>(string assetKey, string assetName) where T : Object
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
        internal Task<bool> LoadFromFileAsync(CancellationToken token = default)
        {
            // 已加载完成，直接返回，避免重复加载
            if (AssetBundle)
            {
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
                Logger.LogDebug(ELogTags.Asset, $"'{BundleName}' assetBundle is load");
                return true;
            }
            catch (Exception e) when(e is not OperationCanceledException)
            {
                throw ExceptionHelper.ThrowAssetBundleLoadException(BundleName, e);
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
        internal async Task<AssetWrapper> LoadAssetAsync<T>(string assetKey, string assetName, CancellationToken token = default) where T : class
        {
            // 正在加载资源，存在缓存任务，返回同一个任务
            if (_assetLoadingTasks.TryGetValue(assetKey, out var cacheTask))
                return await cacheTask;

            // 异步加载资源
            var loadingTask = LoadAssetAsyncInternal<T>(assetKey, assetName, token);
            // 缓存正在加载的任务
            if (!loadingTask.IsCompleted && !_assetLoadingTasks.TryAdd(assetKey, loadingTask))
            {
                // 理论上不会进到这里
                loadingTask = _assetLoadingTasks[assetKey];
            }

            try
            {
                return await loadingTask;
            }
            finally
            {
                _assetLoadingTasks.Remove(assetKey);
            }
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
            TaskHandle<T> taskHandle = default;
            try
            {
                taskHandle = AssetBundle.LoadAssetAsync<T>(assetName).ToTask<T>(token);
                var asset = await taskHandle.Task;
                Retain();
                return DIContainer.Create<AssetWrapper>(parameterValues: new object[] { asset, assetKey, this });
            }
            catch (Exception e) when(e is not OperationCanceledException)
            {
                // 转换异常类型
                throw ExceptionHelper.ThrowAssetLoadException(assetName, e);
            }
            finally
            {
                taskHandle.Dispose();
            }
        }
        
        /// <summary>
        /// 异步加载所有资源
        /// </summary>
        /// <param name="token"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal Task<AssetWrapper[]> LoadAllAssetAsync<T>(CancellationToken token = default) where T : Object
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
                    assetWrappers.Add(DIContainer.Create<AssetWrapper>(parameterValues: new object[] { asset, asset.name, this }));
                    Retain();
                }

                return assetWrappers.ToArray();
            }
            catch (Exception e) when(e is not OperationCanceledException)
            {
                throw ExceptionHelper.ThrowAssetsLoadException(BundleName, typeof(T), e);
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
        internal void Retain()
        {
            ++RefCount;
            Logger.LogDebug(ELogTags.Asset, $"'{BundleName}' assetBundle is referenced, refCount updated to {RefCount}");
        }

        /// <summary>
        /// 释放指定AssetBundle，仅减少引用计数
        /// </summary>
        /// <returns></returns>
        internal void Release()
        {
            if (RefCount > 0)
            {
                --RefCount;
                Logger.LogDebug(ELogTags.Asset, $"'{BundleName}' assetBundle is released, refCount updated to {RefCount}");
                
                if (RefCount != 0) 
                    return;
            
                // 释放包引用计数
                _assetBundleManager.ReleaseDependencies(BundleName);
                return;
            }

            Logger.LogWarning(ELogTags.Asset, $"'{BundleName}' assetBundle refCount repeated release");
        }

        /// <summary>
        /// 尝试异步卸载AB包
        /// </summary>
        /// <param name="unloadAllLoadedObjects"></param>
        internal Task TryUnloadAsync(bool unloadAllLoadedObjects)
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
                Logger.LogDebug(ELogTags.Asset, $"'{BundleName}' is unload, final refCount is {RefCount}");
            }
            catch (Exception e)
            {
                throw ExceptionHelper.ThrowAssetBundleUnloadException(BundleName, RefCount, e);
            }
            finally
            {
                taskHandle.Dispose();
                _assetBundleUnloadTask = null;
            }
        }
    }
}
