using System.Threading;
using System.Threading.Tasks;
using Core.EditorRes;
using Core.Service;
using UnityEngine;

namespace Core.Loader.Sprites
{
    // public class MockSpriteLoader : ISpriteLoader
    // {
    //     public async Task<Sprite> LoadSpriteAsync(string atlasName, string assetName, CancellationToken token = default)
    //     {
    //         await Task.CompletedTask;
    //
    //         var sprite = ServiceLocator.Get<IEditorResManager>().LoadEditorAsset<Sprite>(assetName);
    //         return sprite;
    //     }
    //
    //     public Task UnloadSpriteAsync(string atlasName, string spriteName)
    //     {
    //         return Task.CompletedTask;
    //     }
    // }
}
