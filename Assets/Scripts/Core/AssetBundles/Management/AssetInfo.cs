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

        public AssetInfo(string assetName, Object asset)
        {
            AssetName = assetName;
            RefCount = 1;
            _asset = asset;
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        public void Unload()
        {
            if (RefCount <= 0)
            {
                return;
            }
            
            --RefCount;
            LogManager.Log($"尝试卸载{AssetName}资源，引用数：{RefCount}");
        }

        /// <summary>
        /// 获取资源对象
        /// </summary>
        /// <returns></returns>
        public Object GetAsset()
        {
            ++RefCount;
            return _asset;
        }

        /// <summary>
        /// 引用计数
        /// </summary>
        public uint RefCount { get; private set; }

        /// <summary>
        /// 资源名称
        /// </summary>
        public string AssetName { get; }
    }
}
