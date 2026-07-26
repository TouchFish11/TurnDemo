using Core.AssetBundles.Management;
using Core.DI;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.Game.Slot
{
    /// <summary>
    /// 格子布局对象
    /// </summary>
    internal abstract class GridLayout
    {
        [Inject] protected ObjectSpawner objectSpawner;
        // 滚动视图组件
        internal ScrollRect _sv;
        // ScrollRect 的 Content 对象，所有格子的父节点
        internal RectTransform _content;
        // 单个格子宽度
        internal float _gridWidth;
        // 单个格子高度
        internal float _gridHeight;
        // 格子之间的水平间距
        internal float _gridXSpace;
        // 格子之间的垂直间距
        internal float _gridYSpace;
        // 格子总数
        internal int dataCount;
        // 格子起始偏移
        internal Vector2 originOffset;
        
        /// <summary>
        /// 计算格子索引起始索引和结束索引
        /// </summary>
        /// <returns></returns>
        public abstract (int minIndex, int maxIndex) CalcIndex();

        /// <summary>
        /// 计算格子位置
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public abstract Vector3 CalcPosition(int index);

        /// <summary>
        /// 计算内容区域大小
        /// </summary>
        /// <param name="dataCount"></param>
        public virtual void CalcContentSize(int dataCount)
        {
            this.dataCount = dataCount;
        }
    }
}
