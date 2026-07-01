using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using UnityEngine;
using UnityEngine.U2D;
using Logger = Core.Log.Logger;

namespace HotUpdate.Base.Service
{
    /// <summary>
    /// 图片服务
    /// </summary>
    public class IconService : IIconService
    {
        // 正在加载图片的任务缓存
        private readonly Dictionary<string, Task<AssetHandle<Sprite>>> _keyToSpriteHandleTaskMap = new();
        // 精灵图片资源句柄缓存，唯一资源key映射句柄列表
        private readonly Dictionary<string, AssetHandle<Sprite>> _keyToSpriteHandleMap = new();
        // 正在加载图集的任务缓存
        private readonly Dictionary<string, AssetHandle<SpriteAtlas>> _keyToAtlasHandleMap = new();
        // 图集资源句柄缓存，唯一资源key映射句柄列表
        private readonly Dictionary<string, Task<AssetHandle<SpriteAtlas>>> _keyToAtlasHandleTaskMap = new();

        public async Task PreLoadAtlasAsync(params string[] atlasNames)
        {
            var tasks = new List<Task<SpriteAtlas>>();
            foreach (var atlasName in atlasNames)
            {
                tasks.Add(LoadAtlasAsync(atlasName));
            }
            
            await Task.WhenAll(tasks);
        }
        
        public async Task<SpriteAtlas> LoadAtlasAsync(string atlasKey)
        {
            // 存在缓存，直接返回
            if (_keyToAtlasHandleMap.TryGetValue(atlasKey, out var cacheHandle))
            {
                return cacheHandle.Asset;
            }
            
            // 正在加载，返回同一个加载任务
            if (_keyToAtlasHandleTaskMap.TryGetValue(atlasKey, out var cacheTask))
            {
                var handle = await cacheTask;
                return handle.Asset;
            }
            
            // 首次加载资源
            var newTask = GameAsset.LoadAssetAsync<SpriteAtlas>(atlasKey);
            // 缓存正在加载的资源任务
            if (!_keyToAtlasHandleTaskMap.TryAdd(atlasKey, newTask))
            {
                newTask = _keyToAtlasHandleTaskMap[atlasKey];
            }

            try
            {
                var newHandle = await newTask;
                // 缓存句柄
                _keyToAtlasHandleMap.Add(atlasKey, newHandle);
                return newHandle.Asset;
            }
            catch (Exception e)
            {
                Logger.LogError($"[{nameof(IconService)}]: '{atlasKey}' asset load fail, {e.Message}");
                return null;
            }
            finally
            {
                // 移除正在加载的任务
                _keyToSpriteHandleTaskMap.Remove(atlasKey);
            }
        }
        
        public async Task PreLoadSpriteAsync(params string[] spriteNames)
        {
            if(spriteNames == null)
                throw new ArgumentNullException(nameof(spriteNames));
            
            var tasks = new List<Task<Sprite>>();
            foreach (var spriteName in spriteNames)
            {
                tasks.Add(LoadIconAsync(spriteName));
            }
            
            await Task.WhenAll(tasks);
        }
        
        public async Task<Sprite> LoadIconAsync(string iconKey)
        {
            // 存在缓存，直接返回
            if (_keyToSpriteHandleMap.TryGetValue(iconKey, out var cacheHandle))
            {
                return cacheHandle.Asset;
            }
            
            // 正在加载，返回同一个加载任务
            if (_keyToSpriteHandleTaskMap.TryGetValue(iconKey, out var cacheTask))
            {
                var handle = await cacheTask;
                return handle.Asset;
            }

            // 首次加载资源
            var newTask = GameAsset.LoadAssetAsync<Sprite>(iconKey);
            // 缓存正在加载的资源任务
            if (!_keyToSpriteHandleTaskMap.TryAdd(iconKey, newTask))
            {
                newTask = _keyToSpriteHandleTaskMap[iconKey];
            }

            try
            {
                var newHandle = await newTask;
                // 缓存句柄
                _keyToSpriteHandleMap.Add(iconKey, newHandle);
                return newHandle.Asset;
            }
            catch (Exception e)
            {
                Logger.LogError($"[{nameof(IconService)}]: '{iconKey}' asset load fail, {e.Message}");
                return null;
            }
            finally
            {
                // 移除正在加载的任务
                _keyToSpriteHandleTaskMap.Remove(iconKey);
            }
        }

        public bool TryGetIcon(string iconKey, out Sprite icon)
        {
            if (_keyToSpriteHandleMap.TryGetValue(iconKey, out var handle))
            {
                icon = handle.Asset;
                return true;
            }

            icon = null;
            return false;
        }
        
        public bool Release(string iconKey)
        {
            if(!_keyToSpriteHandleMap.TryGetValue(iconKey, out var handle))
                return false;
            
            // 释放句柄资源
            GameAsset.Release(handle);
            // 移除句柄缓存
            _keyToSpriteHandleMap.Remove(iconKey);
            // 清理正在加载的任务缓存，正常来说这里不会有遗留
            _keyToSpriteHandleTaskMap.Remove(iconKey);
            return true;
        } 
        
        public void ReleaseAll()
        {
            // 避免迭代中修改集合
            var kes = new List<string>(_keyToSpriteHandleMap.Keys);
            foreach (var iconKey in kes)
            {
                Release(iconKey);
            }
        }

        public void Dispose()
        {
            ReleaseAll();
            _keyToSpriteHandleTaskMap.Clear();
            _keyToSpriteHandleMap.Clear();
        }
    }
}
