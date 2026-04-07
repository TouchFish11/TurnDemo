using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Service;
using Core.Tasks.Extensions;
using UnityEngine;

namespace Core.Loader.Text
{
    public class TextLoader : ITextLoader
    {
        // AB包管理器接口
        private readonly IAssetBundleManager _assetBundleManager = ServiceLocator.Get<IAssetBundleManager>();
        // 缓存池接口
        // 资源名称到文本数据映射
        private readonly Dictionary<string, TextData> _assetNameToData = new();

        public async Task<TextAsset> LoadAssetAsync(string abName, string assetName)
        {
            var assetBundle = await _assetBundleManager.LoadBundleAsync(abName);
            var textAsset = await assetBundle.LoadAssetAsync<TextAsset>(assetName).ToTask<TextAsset>();
            return textAsset;
        }
    }
}
