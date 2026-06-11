using System.Threading.Tasks;
using Core.AssetBundles.Management;
using UnityEngine;

namespace Core.PreLoad
{
    /// <summary>
    /// 资源预加载管理器
    /// </summary>
    public class PreLoadManager : IPreLoadManager
    {
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
