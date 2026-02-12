using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Loader.Sprites
{
    /// <summary>
    /// 精灵加载器接口
    /// </summary>
    public interface ISpriteLoader : IAssetLoader
    {
        /// <summary>
        /// 异步加载Sprite
        /// </summary>
        /// <param name="atlasName"></param>
        /// <param name="assetName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<Sprite> LoadSpriteAsync(string atlasName, string assetName, CancellationToken token = default);

        void UnloadSpriteAsync(string atlasName, string spriteName, bool unloadAllLoadedObjects = false);
    }
}
