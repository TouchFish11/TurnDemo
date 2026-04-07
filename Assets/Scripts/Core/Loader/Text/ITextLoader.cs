using System.Threading.Tasks;
using UnityEngine;

namespace Core.Loader.Text
{
    public interface ITextLoader : IAssetLoader
    {
        Task<TextAsset> LoadAssetAsync(string abName, string assetName);
    }
}
