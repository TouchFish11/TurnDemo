using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Log;
using Core.Service;
using Core.Tasks.Extensions;
using UnityEngine.U2D;

namespace Core.Loader.Sprite
{
    /// <summary>
    /// 精灵图片加载器
    /// 负责从SpriteAtlas（精灵图集）中异步加载指定名称的Sprite（精灵图片）
    /// 当图集或指定精灵加载失败时，返回默认精灵（当前默认返回null）
    /// </summary>
    public class SpriteLoader : ISpriteLoader
    {
        // 图集缓存
        private readonly Dictionary<string, AtlasData> _atlasDatas =  new();
        
        private SpriteCache _spriteCache;

        /// <summary>
        /// 异步加载Sprite
        /// </summary>
        /// <param name="atlasName"></param>
        /// <param name="assetName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<UnityEngine.Sprite> LoadSpriteAsync(string atlasName, string assetName, CancellationToken token = default)
        {
            // 存在图集
            if (_atlasDatas.TryGetValue(atlasName, out var atlasData))
            {
                // 存在Sprite
                if (atlasData.TryGetSprite(assetName, out var cacheSprite))
                {
                    return cacheSprite;
                }
                
                // 从图集中获取指定名称的精灵
                var sprite = atlasData.Atlas.GetSprite(assetName);
                // 缓存Sprite
                if (sprite)
                {
                    atlasData.TryAdd(assetName, sprite);
                    return sprite;
                }

                LogManager.LogWarning($"{nameof(SpriteLoader)}.{nameof(LoadSpriteAsync)}，精灵：{assetName}，获取失败，返回默认Sprite");
                return null;
            }
            else
            {
                // 加载图集包
                var assetBundle = await ServiceLocator.Get<IAssetBundleManager>().LoadBundleAsync(EAssetBundleType.SpriteAtlas, token);
                // 加载指定图集
                var atlas = await assetBundle.LoadAssetAsync<SpriteAtlas>(atlasName).ToTask<SpriteAtlas>(token);
                // 图集加载失败，则返回默认精灵
                if (!atlas)
                {
                    LogManager.LogWarning($"{nameof(SpriteLoader)}.{nameof(LoadSpriteAsync)}，图集：{atlasName}，加载失败，加载默认Atlas");
                    return null;
                }
                
                // 缓存图集
                var newAtlasData = new AtlasData(atlas);
                if (!_atlasDatas.TryAdd(atlasName, newAtlasData))
                {
                    LogManager.LogWarning($"{nameof(AtlasData)}.{nameof(LoadSpriteAsync)}：重复缓存SpriteAtlas，{atlasName}");
                }
                
                // 图集加载成功，从图集中获取指定名称的精灵
                var sprite = atlas.GetSprite(assetName);
                if (sprite)
                {
                    newAtlasData.TryAdd(assetName, sprite);
                    return sprite;
                }
            
                LogManager.LogWarning($"{nameof(SpriteLoader)}.{nameof(LoadSpriteAsync)}，精灵：{assetName}，获取失败，返回默认Sprite");
                return null;
            }
        }
        
        public void UnloadSpriteAsync(string atlasName, string spriteName, bool unloadAllLoadedObjects = false)
        {
            if (!_atlasDatas.TryGetValue(atlasName, out var atlasData))
            {
                return;
            }
            
            atlasData.Unload(spriteName);
            if(atlasData.GetRefCount() == 0)
            {
                _atlasDatas.Remove(atlasName);
            }

            if (_atlasDatas.Count == 0)
            {
                ServiceLocator.Get<IAssetBundleManager>().UnloadBundleAsync(EAssetBundleType.SpriteAtlas, unloadAllLoadedObjects);
            }
        }
    }
}