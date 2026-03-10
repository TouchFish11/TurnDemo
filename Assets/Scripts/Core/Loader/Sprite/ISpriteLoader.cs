using System.Threading;
using System.Threading.Tasks;

namespace Core.Loader.Sprite
{
    /// <summary>
    /// 精灵加载器接口
    /// </summary>
    public interface ISpriteLoader : IAssetLoader
    {
        /// <summary>
        /// 异步加载Sprite
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="atlasName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        Task<UnityEngine.Sprite> LoadSpriteAsync(string abName, string atlasName, string assetName);

        void ReleaseSprite(string abName, string atlasName, string spriteName);
    }
}
