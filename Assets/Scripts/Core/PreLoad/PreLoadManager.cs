using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Singleton;
using UnityEngine;

namespace Core.PreLoad
{
    /// <summary>
    /// 预加载管理器
    /// </summary>
    public class PreLoadManager : IPreLoadManager
    {
        private IAssetBundleManager _assetBundleManager;
        
        private PreLoadManager(IAssetBundleManager assetBundleManager)
        {
            _assetBundleManager = assetBundleManager;
        }

        /// <summary>
        /// 预加载资源
        /// </summary>
        /// <param name="preLoadDatas">预加载数据</param>
        public async Task PreLoads(params PreLoadData[] preLoadDatas)
        {
            foreach (var preLoadData in preLoadDatas)
            {
                await GameAsset.LoadAssetAsync<Object>(preLoadData.AssetName);
            }
        }
    }
}
