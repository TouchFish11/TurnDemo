using System.Threading.Tasks;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 模拟精灵加载器
    /// </summary>
    public class MockSpriteLoader : ISpriteLoader
    {
        public async Task<Sprite> GetSpriteAsync(string atlasName, string assetName)
        {
            await Task.CompletedTask;
            // 异步加载图集
            Sprite sprite = ServiceLocator.Get<IEditorResManager>().LoadEditorAsset<Sprite>(assetName);
            return sprite;
        }
    }
}
