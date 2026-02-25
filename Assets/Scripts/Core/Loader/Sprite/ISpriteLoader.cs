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
        /// <param name="token"></param>
        /// <returns></returns>
        Task<UnityEngine.Sprite> LoadSpriteAsync(string abName, string atlasName, string assetName,
            CancellationToken token = default);

        void UnloadSpriteAsync(string abName, string atlasName, string spriteName, bool unloadAllLoadedObjects = false);
    }
}
