using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 资源信息
    /// </summary>
    public class AssetInfo
    {
        // 资源名
        private readonly string assetName;
        // 资源引用计数
        private uint refCount;
        // 资源对象
        private readonly Object asset;

        public AssetInfo(string assetName, Object asset)
        {
            this.assetName = assetName;
            this.asset = asset;
            this.refCount = 1;
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        public void Unload()
        {
            if (refCount > 0)
            {
                --refCount;
                LogMgr.Log($"尝试卸载{assetName}资源，引用数：{refCount}");
            }
        }

        /// <summary>
        /// 获取资源对象
        /// </summary>
        /// <returns></returns>
        public Object GetAsset()
        {
            ++refCount;
            return asset;
        }

        /// <summary>
        /// 引用计数
        /// </summary>
        public uint RefCount => refCount;

        /// <summary>
        /// 资源名称
        /// </summary>
        public string AssetName => assetName;
    }
}
