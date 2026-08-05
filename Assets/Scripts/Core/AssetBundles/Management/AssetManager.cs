using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Exceptions;
using Core.Log;
using UnityEngine;
using UnityEngine.U2D;
using Logger = Core.Log.Logger;
using Object = UnityEngine.Object;

namespace Core.AssetBundles.Management
{
    /// <summary>
    /// 资源管理器
    /// </summary>
    internal partial class AssetManager
    {
        private readonly IAssetBundleManager _assetBundleManager;
        // 物理资源key到物理资源的缓存映射
        private readonly Dictionary<string, AssetWrapper> _assetWrappers = new();
        // 精灵图片缓存，图集到其所有子图片的映射
        private readonly Dictionary<string, Dictionary<string, Sprite>> _sprites = new();
        // 正在加载中的任务字典，Key 为用户传入的原始资源 Key
        private readonly Dictionary<string, Task<AssetWrapper>> _loadingTasks = new();
        // 批量加载资源任务缓存
        private readonly Dictionary<string, Task<AssetWrapper[]>> _assetsLoadingTasks = new();
        
        public AssetManager(IAssetBundleManager assetBundleManager)
        {
            _assetBundleManager = assetBundleManager;
        }

        /// <summary>
        /// 获取资源条目
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public AssetEntry GetAssetEntry(string key)
        {
            return _assetBundleManager.Catalog.GetEntry(key);
        }
        
        /// <summary>
        /// TODO：当前只处理了非图集资源的同步，后续补充图集资源的同步
        /// 同步加载资源
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public AssetWrapper LoadAsset<T>(string key) where T : Object
        {
            // 存在缓存，增加计数后返回
            if (_assetWrappers.TryGetValue(key, out var assetWrapper))
            {
                assetWrapper.Retain();
                return assetWrapper;
            }

            try
            {
                // 从资源目录中查找指定的资源路径
                var mapEntry = _assetBundleManager.Catalog.GetEntry(key);
                if (mapEntry == null)
                    throw new NullReferenceException($"key({key}) found entry is null");
            
                // 加载AB包
                var bundleWrapper = _assetBundleManager.LoadBundle(mapEntry.bundleName);
                if (bundleWrapper == null)
                    throw new NullReferenceException($"load {mapEntry.bundleName} AssetBundle failed");
            
                // 加载资源
                assetWrapper = bundleWrapper.LoadAsset<T>(key, mapEntry.assetName);
                if (assetWrapper.IsNull)
                    throw new NullReferenceException($"load {mapEntry.assetName} failed, key({key})");
            
                // 初始引用
                assetWrapper.Retain();
                _assetWrappers.Add(key, assetWrapper);
                return assetWrapper;
            }
            catch (Exception e)
            {
                Logger.LogException(ELogTags.Asset, ExceptionHelper.Throw($"({key})Asset load fail", e));
                return null;
            }
        }
        
        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="key">资源Key</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<AssetWrapper> LoadAssetAsync<T>(string key) where T : Object
        {
            // 存在该资源缓存，增加引用计数后直接返回
            if (_assetWrappers.TryGetValue(key, out var assetWrapper))
            {
                assetWrapper.Retain();
                return assetWrapper;
            }

            // 查是否有正在进行的相同 Key 的加载任务
            if (_loadingTasks.TryGetValue(key, out var existingTask))
            {
                var wrapper = await existingTask;
                // 当前请求也需要持有引用
                wrapper.Retain();
                return wrapper;
            }
            
            // 创建新的加载任务
            var task = LoadAssetAsyncInternal<T>(key);
            // 注意：这里的task可能在添加前就已经完成
            if (!_loadingTasks.TryAdd(key, task))
            {
                // 没加进去，说明已经有并发请求正在加载，直接 await 那个任务，此时，单线程下这里一定存在， _loadingTasks 中肯定已存在对应 Key 的任务
                task = _loadingTasks[key];
            }

            try
            {
                return await task;
            }
            finally
            {
                // 无论是否完成都要移除
                _loadingTasks.Remove(key);
            }
        }
        
        /// 异步加载单个资源（内部）
        private Task<AssetWrapper> LoadAssetAsyncInternal<T>(string key) where T : Object
        {
            // 从资源目录中查找指定的资源路径
            var entry = _assetBundleManager.Catalog.GetEntry(key);
            // 未找到抛出异常
            if (entry == null)
                throw new NullReferenceException($"'{key}' key found entry is null");
            
            // 若条目是图片资源
            if (entry is SpriteAssetEntry spriteAssetEntry)
            {
                return LoadSpriteAssetAsync<T>(spriteAssetEntry);
            }
            
            // 非图集资源
            return LoadNonSpriteAssetAsync<T>(entry);
        }

        /// <summary>
        /// 异步加载图片
        /// </summary>
        /// <param name="spriteAssetEntry"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private async Task<AssetWrapper> LoadSpriteAssetAsync<T>(SpriteAssetEntry spriteAssetEntry) where T : Object
        {
            // 图集路径处理
            // 注意：这里应使用 atlasKey 作为物理缓存 Key，但 Retain 只针对当前 Sprite 请求
            // 需确保图集 AB 包引用计数增加正确
            var atlasKey = spriteAssetEntry.atlasKey;
                
            // 如果图集已缓存，直接创建 AssetWrapper（不重复加载图集）
            if (_assetWrappers.TryGetValue(atlasKey, out var atlasWrapper))
            {
                // 因为新 Sprite 需要使用图集，引用+1
                atlasWrapper.Retain();
                // 注意：返回的 AssetKey 应是 atlasKey
                return atlasWrapper;
            }
                
            // 创建新的加载任务，异步加载指定资源AB包
            var bundleWrapper = await _assetBundleManager.LoadBundleAsync(spriteAssetEntry.bundleName);
            // 加载图集资源
            var assetWrapper = await bundleWrapper.LoadAssetAsync<SpriteAtlas>(atlasKey, spriteAssetEntry.atlasAssetPath);
            // 相同图集的不同图片，不允许重复添加图集
            _assetWrappers.TryAdd(atlasKey, assetWrapper);
            // 初始引用
            assetWrapper.Retain();
            return assetWrapper;
        }

        /// <summary>
        /// 异步加载非图集图片资源
        /// </summary>
        /// <param name="assetEntry"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private async Task<AssetWrapper> LoadNonSpriteAssetAsync<T>(AssetEntry assetEntry) where T : Object
        {
            var assetKey = assetEntry.key;
            var bundleName = assetEntry.bundleName;
            var assetName = assetEntry.assetName;
            
            // 异步加载指定资源AB包
            var bundleWrapper = await _assetBundleManager.LoadBundleAsync(bundleName);
            // 加载资源
            var assetWrapper = await bundleWrapper.LoadAssetAsync<T>(assetKey, assetName);
            // 存入缓存
            _assetWrappers.Add(assetKey, assetWrapper);
            assetWrapper.Retain();
            return assetWrapper;
        }
        
        /// <summary>
        /// 异步加载指定包的所有资源
        /// </summary>
        /// <param name="bundleName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<AssetWrapper[]> LoadAllAssetAsync<T>(string bundleName) where T : Object
        {
            // 获取当前AB包的所有资源Key
            var allKeys = new List<string>(_assetBundleManager.Catalog.GetAssetKeysByBundle(bundleName));
            // 添加待加载的资源key
            var assetToLoadKeys = new List<string>();
            foreach (var assetKey in allKeys)
            {
                if (!_assetWrappers.ContainsKey(assetKey))
                    assetToLoadKeys.Add(assetKey);
            }
            
            var newAssetWrappers = new List<AssetWrapper>(assetToLoadKeys.Count);
            // 说明这个包的全部资源都加载过了，直接返回全部缓存即可
            if(assetToLoadKeys.Count == 0)
            {
                foreach (var cacheKey in allKeys)
                {
                    var assetWrapper = _assetWrappers[cacheKey];
                    assetWrapper.Retain();
                    newAssetWrappers.Add(assetWrapper);
                }
                return newAssetWrappers.ToArray();
            }
            
            var cacheBundleKey = $"{bundleName}_{typeof(T)}";
            // 正在批量加载资源，返回同一个任务，同时增加引用计数
            if (_assetsLoadingTasks.TryGetValue(cacheBundleKey, out var cacheTask))
            {
                var assetWrappers = await cacheTask;
                foreach (var assetWrapper in assetWrappers)
                {
                    assetWrapper.Retain();
                }
                return assetWrappers;
            }
            
            // 说明这个包加载过资源，有缓存，加载剩余资源
            if (assetToLoadKeys.Count > 0 && assetToLoadKeys.Count != allKeys.Count)
            {
                var assetTasks = new List<Task<AssetWrapper>>();
                foreach (var assetKey in assetToLoadKeys)
                {
                    assetTasks.Add(LoadAssetAsync<T>(assetKey));
                }
                
                // 等待所有资源加载完成
                newAssetWrappers.AddRange(await Task.WhenAll(assetTasks));
                return newAssetWrappers.ToArray();
            }
            
            // 否则全量加载
            var task = LoadAllAssetAsyncInternal<T>(bundleName);
            if (!_assetsLoadingTasks.TryAdd(cacheBundleKey, task))
            {
                task = _assetsLoadingTasks[cacheBundleKey];
            }

            return await task;
        }

        /// 异步批量加载包所有资源（内部）
        private async Task<AssetWrapper[]> LoadAllAssetAsyncInternal<T>(string bundleName) where T : Object
        {
            var cacheBundleKey = $"{bundleName}_{typeof(T)}";
            try
            {
                var assetWrappers = new List<AssetWrapper>();
                var bundleWrapper = await _assetBundleManager.LoadBundleAsync(bundleName);
                // 等待所有资源加载完成
                assetWrappers.AddRange(await bundleWrapper.LoadAllAssetAsync<T>());
                foreach (var assetWrapper in assetWrappers)
                {
                    assetWrapper.Retain();
                    _assetWrappers.TryAdd(assetWrapper.AssetKey, assetWrapper);
                }
                return assetWrappers.ToArray();
            }
            finally
            {
                _assetsLoadingTasks.Remove(cacheBundleKey);
            }
        }

        /// <summary>
        /// 异步加载场景包
        /// </summary>
        /// <param name="sceneKey"></param>
        /// <returns></returns>
        public Task LoadSceneBundleAsync(string sceneKey)
        {
            // 从资源目录中查找指定的资源路径
            var entry = _assetBundleManager.Catalog.GetEntry(sceneKey);
            // 未找到抛出异常
            return entry == null ? throw new NullReferenceException($"'{sceneKey}' key found entry is null") : _assetBundleManager.LoadBundleAsync(entry.bundleName);
        }
        
        /// <summary>
        /// 获取所有场景路径，返回场景名列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllSceneKey()
        {
            var list = new List<string>();
            foreach (var entry in _assetBundleManager.Catalog.Assets)
            {
                if (entry.assetType == EAssetType.Scene)
                {
                    list.Add(entry.key);
                }
            }
            return list;
        }
        
        /// <summary>
        /// 释放资源，减少引用数
        /// </summary>
        /// <param name="key"></param>
        public void ReleaseWrapper(string key)
        {
            var assetWrapper = _assetWrappers.GetValueOrDefault(key);
            if(assetWrapper == null)
                return;
            
            assetWrapper.Release();
            if(assetWrapper.RefCount == 0)
            {
                _assetWrappers.Remove(key);
                _sprites.Remove(key);
            }
        }
        
        /// <summary>
        /// 获取资源，图集中的图片使用GetSprite
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetAsset(string key)
        {
            var assetWrapper = _assetWrappers.GetValueOrDefault(key);
            return assetWrapper?.Asset;
        }

        /// <summary>
        /// 获取图集中图片资源
        /// </summary>
        /// <param name="atlasKey"></param>
        /// <param name="spriteKey"></param>
        /// <returns></returns>
        public Sprite GetSprite(string atlasKey, string spriteKey)
        {
            if (!_assetWrappers.TryGetValue(atlasKey, out var wrapper)) 
                return null;
            
            // 获取缓存的图片，资源引用计数不需要增加；但是包热度需要增加
            if (_sprites.TryGetValue(atlasKey, out var spriteMap))
            {
                // 增加包热度
                wrapper.RecordAccess();
                // 是否有该图片
                var cacheSprite = spriteMap.GetValueOrDefault(spriteKey);
                if(cacheSprite)
                    return cacheSprite;
            }
            
            // 加载图片并缓存
            var sprite = ((SpriteAtlas)wrapper.Asset).GetSprite(spriteKey);
            // 首次加载该图集，新增缓存
            if(!_sprites.TryGetValue(atlasKey, out var newSpriteMap))
                _sprites.Add(atlasKey, new Dictionary<string, Sprite> { { spriteKey, sprite } });
            // 复用相同图集的缓存
            else
                newSpriteMap.Add(spriteKey, sprite);
            return sprite;
        }
    }
}
