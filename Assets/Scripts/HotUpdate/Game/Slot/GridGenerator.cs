using System;
using System.Collections;
using System.Collections.Generic;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Log;
using Core.Mono;
using Core.Pool;
using HotUpdate.Common.Config.Inventory;
using HotUpdate.Game.InventoryModule.Items;
using UnityEngine;
using Logger = Core.Log.Logger;
using Object = UnityEngine.Object;

namespace HotUpdate.Game.Slot
{
    /// <summary>
    /// 可见格子生成器（虚拟列表核心实现）
    /// 仅创建和显示当前视口内的格子，超出视口的格子回收至对象池，支持海量数据的高性能滚动。
    /// </summary>
    /// <typeparam name="T">格子展示的数据类型</typeparam>
    /// <typeparam name="K">格子组件类型，必须继承自 Object 并实现 IGridBase; 接口</typeparam>
    public sealed class GridGenerator<T, K> : IPoolData where K : Object, IGridBase<T> where T : class
    {
        // 对象生成器（支持异步实例化与对象池）
        [Inject] private ObjectSpawner _objectSpawner;
        [Inject] private IMonoAdapter _monoAdapter;
        [Inject] private IInventoryManager _inventoryManager;
        
        // 当前显示的格子字典，Key：数据索引，Value：对象池包装对象
        private readonly Dictionary<int, K> _nowShowGridDic = new();
        // 全部数据列表
        private readonly List<T> _dataList = new();

        // 上一次可见索引范围（用于判断回收）
        private int oldMinIndex = -1;
        private int oldMaxIndex = -1;
        
        // 点击事件注册
        private event Action<T> _clickCallback;
        /// 每帧创建格子数
        private const int CreateGridPerFrame = 25;
        // 当前布局类型
        internal GridLayout gridLayout;
        // 正在逐个创建格子，不允许滑动
        private bool _queueCreateGrid = true;
        // 渐变创建协程
        private Coroutine _fadeCreateCor;

        public K GetGrid(Item item)
        {
            var index = _dataList.FindIndex(i => i == item as T);
            return _nowShowGridDic.GetValueOrDefault(index);
        }
        
        public IEnumerable<K> GetAllCell()
        {
            foreach (var grid in _nowShowGridDic.Values)
            {
                yield return grid;
            }
        }
        
        /// <summary>
        /// 渐变创建格子，仅在第一次打开或切换类型时使用，只是为了呈现一个好的动画效果
        /// </summary>
        public IEnumerator FadeUpdateGrid()
        {
            StopFadeCreateGrid();
            _fadeCreateCor = _monoAdapter.StartCoroutine(FadeCreate_Cor());
            yield return _fadeCreateCor;
        }
        
        /// <summary>
        /// 更新可见格子
        /// 通常在 ScrollRect.onValueChanged 事件或 Update 中调用
        /// </summary>
        public void UpdateGrid()
        {
            // 检查能否更新格子
            if(!CanUpdateGrid())
                return;
            
            // 计算索引
            var (minIndex, maxIndex) = gridLayout.CalcIndex();
            // 与上一次索引范围比较，回收移出视口的格子
            if (minIndex != oldMinIndex || maxIndex != oldMaxIndex)
            {
                // 向上滑动（内容向上，minIndex 变大）：回收顶部移出的格子
                // 范围：oldMinIndex 到 minIndex
                for (var i = oldMinIndex; i < minIndex; i++)
                {
                    if (_nowShowGridDic.TryGetValue(i, out var gridBase))
                    {
                        // 放回对象池
                        _objectSpawner.Release(gridBase);
                        _nowShowGridDic.Remove(i);
                    }
                }

                // 向下滑动（内容向下，maxIndex 变小）：回收底部移出的格子
                // 范围：maxIndex+1 到 oldMaxIndex
                // 注意从 maxIndex+1 开始，因为 maxIndex 是当前可见的最后一个索引，必须保留
                for (var i = maxIndex + 1; i <= oldMaxIndex; i++)
                {
                    if (_nowShowGridDic.TryGetValue(i, out var gridBase))
                    {
                        _objectSpawner.Release(gridBase);
                        _nowShowGridDic.Remove(i);
                    }
                }
            }

            // 记录当前索引范围为上一次范围，供下一帧使用
            oldMinIndex = minIndex;
            oldMaxIndex = maxIndex;
            
            // 创建新进入视口的格子
            for (var i = minIndex; i <= maxIndex; ++i)
            {
                // 尝试向字典添加占位（若已存在则跳过，防止重复创建）
                if (!_nowShowGridDic.TryAdd(i, null))
                    continue;

                // 异步创建格子（字典中已占位 null，回调成功后会替换为实际对象）
                CreateGridAsync(i);
            }
        }

        /// <summary>
        /// 设置显示的数据
        /// </summary>
        /// <param name="datas"></param>
        public void SetDatas(List<T> datas)
        {
            _dataList.Clear();
            _dataList.AddRange(datas);
            /*
             * 内容区域大小依赖于数据的设置（数据数量），又因为通过构建器构建时不会设置数据数据，而是在生成器自己初始化时设置数据
             * 所以延迟到设置完数据后，再计算总内容区域大小。构建时保证其它固定的数据会被设置，这里只需补充缺少的数据数量即可计算
             */ 
            CalcContentSize();
        }

        /// <summary>
        /// 添加点击事件，让格子监听该事件
        /// </summary>
        /// <param name="callbacks"></param>
        public void AddClickListener(params Action<T>[] callbacks)
        {
            // 每次添加事件前，都要先清空上次的事件，否则会重复添加多个事件
            _clickCallback = null;
            foreach (var callback in callbacks)
            {
                _clickCallback += callback;
            }
        }
        
        /// <summary>
        /// 清空所有格子
        /// </summary>
        public void ClearGrids()
        {
            foreach (var gridBase in _nowShowGridDic.Values)
            {
                _objectSpawner.Release(gridBase);
            }
            _nowShowGridDic.Clear();
        }

        /// <summary>
        /// 计算内容总大小，要在最后调用计算
        /// </summary>
        internal void CalcContentSize()
        {
            gridLayout.CalcContentSize(_dataList.Count);
        }
        
        /// <summary>
        /// 异步创建指定索引的格子
        /// </summary>
        /// <param name="index">数据索引</param>
        private async void CreateGridAsync(int index)
        {
            try
            {
                // 异步从对象池获取格子实例（自动处理实例化、激活、父节点设置）
                var pos = gridLayout.CalcPosition(index);
                var gridCell = await _objectSpawner.SpawnAsync<K>(AssetKeys.ItemCell, gridLayout._content, pos, Quaternion.identity);
                // 初始化格子数据
                gridCell.InitGrid(_dataList[index], _inventoryManager.IconService);
                // 二次确认：异步加载期间该索引是否仍有效（未被回收）
                if (_nowShowGridDic.ContainsKey(index))
                {
                    // 有效：将实际对象替换占位
                    _nowShowGridDic[index] = gridCell;
                    // 注册交互事件
                    gridCell.SetClick(_clickCallback);
                }
                else
                {
                    // 无效：说明在异步等待期间该索引已被回收，直接将对象放回池子
                    _objectSpawner.Release(gridCell);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(ELogTags.Item, $"{nameof(GridGenerator<T, K>)}: {e.Message}");
            }
        }

        /// <summary>
        /// 能否更新格子
        /// </summary>
        /// <returns></returns>
        private bool CanUpdateGrid()
        {
            /*
             * 正在逐个创建格子的时不能滑动，也就不能执行UpdateGrid更新格子，但是协程中可能会改变Content的位置，
             * 导致进入UpdateGrid，意外更新格子，所以需要这个标识来防御
             */
            return !_queueCreateGrid;
        }
        
        /// <summary>
        /// 渐变创建格子
        /// </summary>
        /// <returns></returns>
        private IEnumerator FadeCreate_Cor()
        {
            // 重置标识
            _queueCreateGrid = true;
            // 创建格子时禁用对应方向的滑动
            SetSlide(false);
            
            // 视图自动回到顶部
            gridLayout._content.anchoredPosition = Vector2.zero;
            
            // 计算索引
            var (minIndex, maxIndex) = gridLayout.CalcIndex();
            // 记录当前索引范围为上一次范围，供真正滑动时使用
            oldMinIndex = minIndex;
            oldMaxIndex = maxIndex;
            for (var i = minIndex; i <= maxIndex; i++)
            {
                // 异步从对象池获取格子实例（自动处理实例化、激活、父节点设置）
                var pos = gridLayout.CalcPosition(i);
                var poolObj = _objectSpawner.Spawn<K>(AssetKeys.ItemCell, gridLayout._content, pos, Quaternion.identity);
                // 初始化格子数据
                poolObj.InitGrid(_dataList[i], _inventoryManager.IconService);
                // 有效：将实际对象替换占位
                _nowShowGridDic[i] = poolObj;
                // 注册交互事件
                poolObj.SetClick(_clickCallback);
                // 每帧创建数
                if ((i - minIndex + 1) % CreateGridPerFrame == 0)
                    yield return null;
            }
            
            // 格子创建完成，启用对应方向的滑动，重置标识
            SetSlide(true);
            _queueCreateGrid = false;
        }

        /// <summary>
        /// 停止协程
        /// </summary>
        private void StopFadeCreateGrid()
        {
            if (_fadeCreateCor != null)
            {
                _monoAdapter.StopCoroutine(_fadeCreateCor);
                ClearGrids();
            }
        }
        
        /// <summary>
        /// 设置是否可滑动，根据不同的布局启用/禁用对应方向的滑动
        /// </summary>
        /// <param name="slide"></param>
        private void SetSlide(bool slide)
        {
            if (gridLayout is VerticalGridLayout)
                gridLayout._sv.vertical = slide;
            else
                gridLayout._sv.horizontal = slide;
        }
        
        /// <summary>
        /// 重置数据（实现 IPoolData 接口，用于对象池回收生成器自身时清理引用）
        /// </summary>
        void IPoolData.ResetData()
        {
            StopFadeCreateGrid();
            oldMinIndex = -1;
            oldMaxIndex = -1;
            _dataList.Clear();
            _monoAdapter = null;
            _fadeCreateCor = null;
            ClearGrids();
            _objectSpawner.Dispose();
            _objectSpawner = null;
        }
    }
}