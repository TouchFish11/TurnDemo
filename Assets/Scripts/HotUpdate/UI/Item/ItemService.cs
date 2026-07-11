using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Log;
using Core.Serialize.Binary;
using Core.Utility;
using HotUpdate.Base.Service;

using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.UI.Item
{
    /// <summary>
    /// 物品图标服务
    /// </summary>
    public class ItemService : IDisposable
    {
        [Inject] private IBinaryDataManager _binaryDataManager;
        [Inject] private ObjectSpawner _objectSpawner;
        [Inject] private IIconService _iconService;
        
        // 当前选中任务的奖励物品格子列表
        private readonly List<ItemGrid> _items = new();
        
        /// <summary>
        /// 获取物品格子UI
        /// 内部已初始化UI，异常时回调返回null
        /// </summary>
        /// <param name="awardIds"></param>
        /// <param name="parent"></param>
        /// <param name="callback"></param>
        public async Task CreateItemGrid(string awardIds, Transform parent, Action<ItemGrid> callback)
        {
            try
            {
                var itemInfos = new Dictionary<int, int>();
                // 解析奖励ID数组
                TextUtility.SplitMultiple(awardIds, 1, 2, itemInfos.Add);

                foreach (var pair in itemInfos)
                {
                    // 获取UI
                    var itemGrid = await _objectSpawner.SpawnAsync<ItemGrid>(AssetKeys.ItemGrid, parent);
                    // 读取配置
                    var itemInfo = _binaryDataManager.GetConfig<ItemInfoContainer>(EConfigLoadType.Excel).dataDic[pair.Key];
                    // 加载图标
                    var sprite = await _iconService.LoadIconAsync(itemInfo.f_icon);
                    // 初始化
                    itemGrid.Init(sprite, pair.Value, itemInfo.f_quality);
                    // 缓存池化对象
                    _items.Add(itemGrid);
                    callback?.Invoke(itemGrid);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(ELogTags.Item, $"{nameof(ItemService)}: ItemGrid create error,{e.Message}");
            }
        }

        /// <summary>
        /// 获取物品格子UI
        /// 内部已初始化UI，异常时回调返回null
        /// </summary>
        /// <param name="awardIds"></param>
        public async Task<ItemGrid[]> CreateItemGrid(string awardIds)
        {
            try
            {
                var itemInfos = new Dictionary<int, int>();
                // 解析奖励ID数组
                TextUtility.SplitMultiple(awardIds, 1, 2, itemInfos.Add);
                
                var list = new List<ItemGrid>(itemInfos.Count);
                foreach (var pair in itemInfos)
                {
                    // 获取UI
                    var itemGrid = await _objectSpawner.SpawnAsync<ItemGrid>(AssetKeys.ItemGrid);
                    // 读取配置
                    var itemInfo = _binaryDataManager.GetConfig<ItemInfoContainer>(EConfigLoadType.Excel).dataDic[pair.Key];
                    // 加载图标
                    var sprite = await _iconService.LoadIconAsync(itemInfo.f_icon);
                    // 初始化
                    itemGrid.Init(sprite, pair.Value, itemInfo.f_quality);
                    list.Add(itemGrid);
                }
                
                _items.AddRange(list);
                return list.ToArray();
            }
            catch (Exception e)
            {
                Logger.LogError(ELogTags.Item, $"{nameof(ItemService)}: ItemGrid create error,{e.Message}");
                return Array.Empty<ItemGrid>();
            }
        }

        public void Clear()
        {
            foreach (var itemGrid in _items)
            {
                _objectSpawner.Release(itemGrid);
            }
            _items.Clear();
            _iconService.ReleaseAll();
        }

        public void Dispose()
        {
            Clear();
            _objectSpawner.Dispose();
            _objectSpawner = null;
            _iconService = null;
            _binaryDataManager = null;
        }
    }
}
