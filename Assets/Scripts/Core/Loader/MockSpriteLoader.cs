using System.Threading.Tasks;
using Core.EditorRes;
using Core.Loader.Loaders;
using Core.Service;
using UnityEngine;

namespace Core.Loader
{
    /// <summary>
    /// ģ�⾫�������
    /// </summary>
    public class MockSpriteLoader : ISpriteLoader
    {
        public async Task<Sprite> GetSpriteAsync(string atlasName, string assetName)
        {
            await Task.CompletedTask;
            // �첽����ͼ��
            Sprite sprite = ServiceLocator.Get<IEditorResManager>().LoadEditorAsset<Sprite>(assetName);
            return sprite;
        }
    }
}
