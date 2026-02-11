using UnityEngine;

namespace Core.AssetBundles.Management
{
    /// <summary>
    /// AB包信息
    /// </summary>
    public class BundleInfo
    {
        // AssetBundle对象
        public AssetBundle assetBundle { get; private set; }
        
        // AssetBundle名称
        public string bundelName { get; private set; }
        
        // AssetBundle本地加载路径
        public string loadPath{get; private set; }
        
        // AB包资源引用计数
        public uint refCount{get; private set; }
    }
}
