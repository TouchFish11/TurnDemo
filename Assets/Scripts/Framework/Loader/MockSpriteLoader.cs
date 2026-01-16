using System.Threading.Tasks;
using UnityEngine;

namespace Framework
{
    public class MockSpriteLoader : IAssetLoader
    {
        /// <summary>
        /// 异步加载精灵图片
        /// </summary>
        /// <param name="atlasName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public async Task<Sprite> GetSpriteAsync(string atlasName, string assetName)
        {
            // 异步加载图集
            Sprite sprite = ServiceLocator.Get<IEditorResManager>().LoadEditorAsset<Sprite>(assetName);
            return sprite;
        }
    }
}
