using System;
using System.Collections.Generic;
using Core.DI;
using Core.Log;
using Core.Pool;
using UnityEngine;
using UnityEngine.UI;
using Logger = Core.Log.Logger;
using Object = UnityEngine.Object;

namespace HotUpdate.Game.Slot
{
    /// <summary>
    /// 格子生成器构建器
    /// </summary>
    /// <typeparam name="T">格子展示的数据类型</typeparam>
    /// <typeparam name="K">格子组件类型，必须继承自 Object 并实现 IGridBase&lt;T&gt; 接口</typeparam>
    public class GridGeneratorBuilder<T, K> where K : Object, IGridBase<T> where T : class
    {
        [Inject] private IPoolManager _poolManager;
        private static readonly Dictionary<EGridLayout, Func<GridLayout>> _gridLayouts = new();
        // 构建的对象
        private GridGenerator<T, K> _gridGenerator;
        
        static GridGeneratorBuilder()
        {
            _gridLayouts.Add(EGridLayout.Horizontal, () => new HorizontalGridLayout());
            _gridLayouts.Add(EGridLayout.Vertical, () => new VerticalGridLayout());
        }
        
        public GridGeneratorBuilder<T, K> CreateGenerator(EGridLayout gridLayout)
        {
            _gridGenerator = _poolManager.GetData<GridGenerator<T, K>>();
            _gridGenerator.gridLayout = _gridLayouts[gridLayout]();
            return this;
        }
        
        public GridGeneratorBuilder<T, K> SetParent(ScrollRect scrollRect)
        {
            _gridGenerator.gridLayout._sv = scrollRect;
            _gridGenerator.gridLayout._content = scrollRect.content;
            return this;
        }
        
        public GridGeneratorBuilder<T, K> SetOriginOffset(float offsetX, float offsetY)
        {
            _gridGenerator.gridLayout.originOffset = new Vector2(offsetX, offsetY);
            return this;
        }

        public GridGeneratorBuilder<T, K> SetGridSize(float gridWidth, float gridHeight)
        {
            _gridGenerator.gridLayout._gridWidth = gridWidth;
            _gridGenerator.gridLayout._gridHeight = gridHeight;
            return this;
        }
        
        public GridGeneratorBuilder<T, K> SetGridSpace(float gridXSpace, float gridYSpace)
        {
            _gridGenerator.gridLayout._gridXSpace = gridXSpace;
            _gridGenerator.gridLayout._gridYSpace = gridYSpace;
            return this;
        }
        
        public GridGeneratorBuilder<T, K> SetColumn(int maxCol)
        {
            if (_gridGenerator.gridLayout is VerticalGridLayout verticalGridLayout)
                verticalGridLayout.maxCol = maxCol;
            else
                Logger.LogError(ELogTags.Item, $"{nameof(GridGeneratorBuilder<T, K>)} can only be used with {nameof(VerticalGridLayout)}");
            return this;
        }
        
        public GridGeneratorBuilder<T, K> SetRow(int maxRow)
        {
            if (_gridGenerator.gridLayout is HorizontalGridLayout horizontalGridLayout)
                horizontalGridLayout.maxRow = maxRow;
            else
                Logger.LogError(ELogTags.Item, $"{nameof(GridGeneratorBuilder<T, K>)} can only be used with {nameof(HorizontalGridLayout)}");
            return this;
        }

        /// <summary>
        /// 构建格子生成器，返回配置好的生成器，同时计算内容显示的长度，依赖于SetDatas的数据
        /// </summary>
        /// <returns></returns>
        public GridGenerator<T, K> Build()
        {
            return _gridGenerator;
        }
        
        public static GridGeneratorBuilder<T, K> Create()
        {
            return DIContainer.Create<GridGeneratorBuilder<T, K>>();
        }
    }
}
