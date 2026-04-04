using Core.Pool;
using UnityEngine;

namespace Core.Loader.Text
{
    public class TextData : IPoolData
    {
        /// <summary>
        /// 文本资源
        /// </summary>
        public TextAsset TextAsset { get; private set; }
        
        /// <summary>
        /// 该资源的引用计数
        /// </summary>
        public int RefCount { get; private set; }
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="textAsset"></param>
        /// <param name="refCount"></param>
        /// <returns></returns>
        public TextData Init(TextAsset textAsset, int refCount)
        {
            this.TextAsset = textAsset;
            this.RefCount = refCount;
            return this;
        }

        public void ResetData()
        {
            TextAsset = null;
            RefCount = 0;
        }
    }
}
