using System.Threading.Tasks;
using UnityEngine;

namespace Core.Loader.Loaders
{
    /// <summary>
    /// 精灵加载器接口
    /// </summary>
    public interface ISpriteLoader : IAssetLoader
    {
        /// <summary>
        /// 异步加载精灵图片
        /// </summary>
        /// <param name="atlasName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        Task<Sprite> GetSpriteAsync(string atlasName, string assetName);
    }
}
