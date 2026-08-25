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

namespace HotUpdate.UI.Items
{
    /// <summary>
    /// 物品图标服务
    /// </summary>
    public class ItemService : IDisposable
    {
        [Inject] private IBinaryDataManager _binaryDataManager;
        [Inject] private ObjectSpawner _objectSpawner;
        [Inject] private IIconService _iconService;
        
        // 当前加载的物品格子缓存
        private readonly Stack<ItemGrid> _items = new();

        /// <summary>
        /// 预加载格子对象
        /// </summary>
        /// <param name="count">加载数量</param>
        /// <param name="spriteNames">加载的图片资源</param>
        public async Task PreloadAsync(int count, params string[] spriteNames)
        {
            var caches = new ItemGrid[count];
            for (var i = 0; i < count; i++)
            {
                caches[i] = await _objectSpawner.SpawnAsync<ItemGrid>(AssetKeys.ItemGrid);
            }
            _objectSpawner.Release(caches);
            
            await _iconService.PreLoadSpriteAsync(spriteNames);
        }
        
        /// <summary>
        /// 获取物品格子UI
        /// 内部已初始化UI，异常时回调返回空数组
        /// </summary>
        /// <param name="awardIds"></param>
        /// <param name="parent"></param>
        /// <param name="worldSpace"></param>
        public async Task<ItemGrid[]> CreateItemGrids(string awardIds, Transform parent = null, bool worldSpace = false)
        {
            try
            {
                var itemInfos = new Dictionary<int, int>();
                // 解析奖励ID数组
                TextUtility.SplitMultiple(awardIds, 1, 2, itemInfos.Add);
                foreach (var pair in itemInfos)
                {
                    // 读取配置
                    var itemInfo = _binaryDataManager.GetConfig<ItemInfoContainer>(EConfigLoadType.Excel).dataDic[pair.Key];
                    // 加载图标
                    var sprite = await _iconService.LoadIconAsync(itemInfo.f_icon);
                    // 获取UI
                    var itemGrid = await _objectSpawner.SpawnAsync<ItemGrid>(AssetKeys.ItemGrid, parent, worldSpace: worldSpace);
                    // 初始化
                    itemGrid.Init(sprite, pair.Value, itemInfo.f_quality);
                    _items.Push(itemGrid);
                }
                return _items.ToArray();
            }
            catch (Exception e)
            {
                Logger.LogException(ELogTags.Item, e);
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
