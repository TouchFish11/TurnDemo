using Core.Pool;
using UnityEngine;

namespace Core.Loader.Object
{
    /// <summary>
    /// 预制体数据
    /// </summary>
    public class PrefabData : IPoolData
    {
        /// <summary>
        /// 预制体资源
        /// </summary>
        public GameObject objAsset;
            
        /// <summary>
        /// 该资源的引用计数，实例化数
        /// </summary>
        public int refCount;

        public PrefabData Init(GameObject objAsset, int refCount)
        {
            this.objAsset = objAsset;
            this.refCount = refCount;
            return this;
        }

        public void ResetData()
        {
            objAsset = null;
            refCount = 0;
        }
    }
}
