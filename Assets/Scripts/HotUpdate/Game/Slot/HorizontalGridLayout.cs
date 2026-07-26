using UnityEngine;

namespace HotUpdate.Game.Slot
{
    /// <summary>
    /// 水平格子布局
    /// </summary>
    internal class HorizontalGridLayout : GridLayout
    {
        // 每列最大行数
        public int maxRow;
        // 视口宽度
        protected float viewportWidth;
        
        public override (int minIndex, int maxIndex) CalcIndex()
        {
            // 单行总高度 = 格子高度 + 垂直间距
            // minIndex：当前视口顶部对应的格子索引
            // _content.anchoredPosition.y 表示 Content 顶部轴心相对于其锚点参考点（通常为视口顶部）的垂直偏移量（正值表示内容向上滚动）
            // 除以单行高度得到当前屏幕顶部已滚过的行数（向下取整），再乘 _maxCol 得到该行第一个格子的索引
            var minIndex = (int)(_content.anchoredPosition.x / -(_gridWidth + _gridXSpace)) * maxRow;

            // maxIndex：当前视口底部对应的格子索引
            // ((RectTransform)_sv.transform).sizeDelta.x 是视口（显示区域）的实际宽度
            // 视口底部位置 = 已滚动偏移量 + 视口高度
            // 同样方式算出底部所在行数，乘 _maxCol 再加 (_maxCol - 1) 得到该行最后一个格子的索引
            var maxIndex = (int)((viewportWidth - _content.anchoredPosition.x) / (_gridWidth + _gridXSpace)) * maxRow + (maxRow - 1);
            
            // 边界保护：不能超出数据范围
            if (minIndex < 0)
                minIndex = 0;
            if (maxIndex >= dataCount)
                maxIndex = dataCount - 1;
            
            return (minIndex, maxIndex);
        }

        public override Vector3 CalcPosition(int index)
        {
            // 计算格子的本地位置
            // X = 列索引 * (格子宽度 + 水平间距)
            // Y = 负的 行索引 * (格子高度 + 垂直间距)
            // 取负是因为 Content 坐标系中 Y 轴向上为正，第一行需位于顶部（Y=0 附近）
            return new Vector3(
                (index / maxRow) * (_gridWidth + _gridXSpace) + originOffset.x,
                -index % maxRow * (_gridHeight + _gridYSpace) + originOffset.y,
                0);
        }

        public override void CalcContentSize(int dataCount)
        {
            base.CalcContentSize(dataCount);
            // 计算 Content 总高度
            // 总行数 = 向上取整(数据总数 / 最大列数)
            // 总高度 = 总行数 * (格子高度 + 垂直间距)
            // 设置 sizeDelta 使滚动条能够正确反映内容总长度
            _content.sizeDelta = new Vector2(Mathf.CeilToInt(dataCount / (float)maxRow) * (_gridWidth + _gridXSpace), 0);
            
            // 暂时这样处理，否则拿不到viewport.rect.width
            Canvas.ForceUpdateCanvases();
            // 初始化视口宽度
            viewportWidth = _sv.viewport.rect.width;
        }
    }
}
