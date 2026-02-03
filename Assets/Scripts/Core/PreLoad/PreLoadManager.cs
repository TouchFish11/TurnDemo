using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Service;
using Core.Singleton;

namespace Core.PreLoad
{
    /// <summary>
    /// 预加载管理器
    /// </summary>
    public class PreLoadManager : SingletonBase<PreLoadManager>, IPreLoadManager
    {
        private PreLoadManager()
        {
        
        }
        
        /// <summary>
        /// 预加载资源
        /// </summary>
        /// <param name="preLoadDatas">预加载数据</param>
        public async Task PreLoads(params PreLoadData[] preLoadDatas)
        {
            foreach (var preLoadData in preLoadDatas)
            {
                await ServiceLocator.Get<IAssetBundleManager>().LoadAssetAsync(preLoadData.assetBundleType, preLoadData.assetName, preLoadData.assetType);
            }
        }
    }
}
