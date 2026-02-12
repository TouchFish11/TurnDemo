using Core.Log;
using UnityEngine;

namespace Core.AssetBundles.Management
{
    /// <summary>
    /// 资源信息
    /// </summary>
    public class AssetInfo
    {
        // 资源对象
        private readonly Object _asset;
        // 所属AB包的名称
        private string _assetBundleName;

        public AssetInfo(string assetBundleName, Object asset)
        {
            _assetBundleName = assetBundleName;
            RefCount = 1;
            _asset = asset;
        }

        /// <summary>
        /// 卸载资源
        /// 减少引用计数
        /// </summary>
        public void Unload()
        {
            if (RefCount <= 0)
            {
                return;
            }
            
            --RefCount;
            LogManager.Log($"尝试卸载：{_assetBundleName}包的：{AssetName}资源，更新资源引用数为：{RefCount}");
        }

        /// <summary>
        /// 获取资源对象
        /// </summary>
        /// <returns></returns>
        public Object GetAsset()
        {
            ++RefCount;
            //LogManager.Log($"{assetInfo.AssetName}资源被引用，{bundelName}包引用数：{RefCount}；资源引用数：{assetInfo.RefCount}");
            return _asset;
        }

        /// <summary>
        /// 引用计数
        /// </summary>
        public uint RefCount { get; private set; }

        /// <summary>
        /// 资源名称
        /// </summary>
        public string AssetName => _asset.name;
    }
}
