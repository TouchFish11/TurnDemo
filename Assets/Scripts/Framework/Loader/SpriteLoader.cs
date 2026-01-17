using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;

namespace Framework
{
    /// <summary>
    /// 精灵加载器
    /// </summary>
    public class SpriteLoader : ISpriteLoader
    {
        /// <summary>
        /// 异步加载精灵图片
        /// </summary>
        /// <param name="atlasName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public async Task<Sprite> GetSpriteAsync(string atlasName,string assetName)
        {
            // 异步加载图集
            SpriteAtlas spriteAtlas = await ServiceLocator.Get<IAssetBundleManager>().LoadAssetAsync<SpriteAtlas>(E_AssetBundleType.SpriteAtlas, atlasName);
            // 未找到图集，使用默认资源
            if (spriteAtlas == null)
            {
                return await GetDefault();
            }

            // 加载精灵图片
            Sprite sprite = spriteAtlas.GetSprite(assetName);
            // 未找到精灵图片，使用默认资源
            if (sprite == null)
            {
                return await GetDefault();
            }
            return sprite;
        }

        /// <summary>
        /// 获取默认图片
        /// </summary>
        /// <returns></returns>
        private async Task<Sprite> GetDefault()
        {
            SpriteAtlas spriteAtlas = await ServiceLocator.Get<IAssetBundleManager>().LoadAssetAsync<SpriteAtlas>(E_AssetBundleType.SpriteAtlas, ResKeyCollection.Atlas_Default);
            return spriteAtlas.GetSprite(ResKeyCollection.WhiteDefaultImage);
        }
    }
}
