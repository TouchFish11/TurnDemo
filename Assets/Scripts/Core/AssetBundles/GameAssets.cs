using System.Threading.Tasks;

namespace Core.AssetBundles
{
    public static class GameAssets
    {
        public static Task<T> LoadAssetAsync<T>(string assetName) where T : UnityEngine.Object
        {
            return (Task<T>)Task.CompletedTask;
        }
    }
}
