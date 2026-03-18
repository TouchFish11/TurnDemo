using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.Service;
using Core.Singleton;
using Core.Tasks.Extensions;
using UnityEngine;

namespace Core.PreLoad
{
    /// <summary>
    /// 预加载管理器
    /// </summary>
    public class PreLoadManager : SingletonBase<PreLoadManager>, IPreLoadManager
    {
        public override int InitPriority => 0;

        private PreLoadManager()
        {
        
        }

        public override Task InitAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 预加载资源
        /// </summary>
        /// <param name="preLoadDatas">预加载数据</param>
        public async Task PreLoads(params PreLoadData[] preLoadDatas)
        {
            foreach (var preLoadData in preLoadDatas)
            {
                var assetBundle = await ServiceLocator.Get<IAssetBundleManager>().LoadBundleAsync(preLoadData.AbName);
                await assetBundle.LoadAssetAsync(preLoadData.AssetName, preLoadData.AssetType).ToTask<Object>();
            }
        }
    }
}
