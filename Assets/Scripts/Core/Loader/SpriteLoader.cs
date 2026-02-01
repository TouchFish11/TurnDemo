using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Loader.Loaders;
using Core.Log;
using Core.Service;
using UnityEngine;
using UnityEngine.U2D;

namespace Core.Loader
{
    /// <summary>
    /// 精灵图片加载器
    /// 负责从SpriteAtlas（精灵图集）中异步加载指定名称的Sprite（精灵图片）
    /// 当图集或指定精灵加载失败时，返回默认精灵（当前默认返回null）
    /// </summary>
    public class SpriteLoader : ISpriteLoader
    {
        /// <summary>
        /// 异步获取指定图集内的指定精灵图片
        /// </summary>
        /// <param name="atlasName">精灵图集名称（AssetBundle中图集的标识）</param>
        /// <param name="assetName">图集内精灵图片的名称</param>
        /// <returns>加载成功则返回目标Sprite，加载失败则返回默认Sprite（当前为null）</returns>
        public async Task<Sprite> GetSpriteAsync(string atlasName, string assetName)
        {
            // 从AssetBundle异步加载指定名称的精灵图集
            var spriteAtlas = await ServiceLocator.Get<IAssetBundleManager>().LoadAssetAsync<SpriteAtlas>(EAssetBundleType.SpriteAtlas, atlasName);
            
            // 图集加载失败（返回null），则返回默认精灵
            if (!spriteAtlas)
            {
                LogManager.LogWarning($"{typeof(SpriteLoader)}.{nameof(GetSpriteAsync)}，图集获取失败");
                return await GetDefault();
            }

            // 图集加载成功，从图集中获取指定名称的精灵
            var sprite = spriteAtlas.GetSprite(assetName);
            
            // 精灵获取失败（返回null），则返回默认精灵；否则返回目标精灵
            if (!sprite)
            {
                LogManager.LogWarning($"{typeof(SpriteLoader)}.{nameof(GetSpriteAsync)}，精灵获取失败");
                return await GetDefault();
            }
            
            return sprite;
        }

        /// <summary>
        /// 异步获取默认精灵图片（兜底逻辑）
        /// 注：当前逻辑为返回null，可根据业务需求扩展为加载默认占位图
        /// </summary>
        /// <returns>默认精灵图片（当前返回null）</returns>
        private async Task<Sprite> GetDefault()
        {
            // 以下为预留的默认图集加载逻辑（已注释），可根据业务启用：
            // 1. 加载默认精灵图集
            // var spriteAtlas = await ServiceLocator.Get<IAssetBundleManager>()
            //     .LoadAssetAsync<SpriteAtlas>(EAssetBundleType.SpriteAtlas, ResKeyCollection.Atlas_Default);
            // 2. 从默认图集中获取预设的默认精灵（如白色占位图）
            // return spriteAtlas.GetSprite(ResKeyCollection.WhiteDefaultImage);

            // 当前默认返回null，无兜底图片
            await Task.CompletedTask;
            return null;
        }
    }
}