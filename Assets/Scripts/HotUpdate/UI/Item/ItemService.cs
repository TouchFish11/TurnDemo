using System;
using System.Collections.Generic;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Serialize.Binary;
using Core.Utility;
using HotUpdate.Common.Config.ExcelInfo.Container;

using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.UI.Item
{
    /// <summary>
    /// 物品工具类
    /// </summary>
    public class ItemService : IDisposable
    {
        [Inject] private IBinaryDataManager _binaryDataManager;
        [Inject] private ObjectSpawner _objectSpawner;
        
        // 当前选中任务的奖励物品格子列表
        private readonly List<PoolObject> _items = new();
        
        /// <summary>
        /// 获取物品格子UI
        /// 内部已初始化UI，异常时回调返回null
        /// </summary>
        /// <param name="awardIds"></param>
        /// <param name="parent"></param>
        /// <param name="callback"></param>
        public async void GetItemGrid(string awardIds, Transform parent, Action<ItemGrid> callback)
        {
            try
            {
                var itemInfos = new Dictionary<int, int>();
                // 解析奖励ID数组
                TextUtility.SplitMultiple(awardIds, 1, 2, itemInfos.Add);

                foreach (var pair in itemInfos)
                {
                    var poolObject = await _objectSpawner.SpawnAsync<ItemGrid>(AssetKeys.ItemGrid, parent);
                    // 获取UI
                    var itemGrid = poolObject.Obj;
                    // 读取配置
                    var itemInfo = _binaryDataManager.GetConfig<ItemInfoContainer>(EConfigLoadType.Excel).dataDic[pair.Key];
                    // 加载图标
                    var handle = await GameAsset.LoadAssetAsync<Sprite>(itemInfo.f_icon);
                    // 初始化
                    itemGrid.Init(handle.Asset, pair.Value, itemInfo.f_quality);
                    // 缓存池化对象
                    _items.Add(poolObject);
                    callback?.Invoke(itemGrid);
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(ItemService)}.{nameof(GetItemGrid)}：{e.Message}");
            }
        }
        
        /// <summary>
        /// 获取物品格子UI
        /// 内部已初始化UI，异常时回调返回null
        /// </summary>
        /// <param name="awardIds"></param>
        /// <param name="callback"></param>
        public async void GetItemGrid(string awardIds, Action<ItemGrid> callback)
        {
            try
            {
                var itemInfos = new Dictionary<int, int>();
                // 解析奖励ID数组
                TextUtility.SplitMultiple(awardIds, 1, 2, itemInfos.Add);
            
                foreach (var pair in itemInfos)
                {
                    var poolObject = await _objectSpawner.SpawnAsync<ItemGrid>(AssetKeys.ItemGrid);
                    // 获取UI
                    var itemGrid = poolObject.Obj;
                    // 读取配置
                    var itemInfo = _binaryDataManager.GetConfig<ItemInfoContainer>(EConfigLoadType.Excel).dataDic[pair.Key];
                    // 加载图标
                    var handle = await GameAsset.LoadAssetAsync<Sprite>(itemInfo.f_icon);
                    // 初始化
                    itemGrid.Init(handle.Asset, pair.Value, itemInfo.f_quality);
                    callback?.Invoke(itemGrid);
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(ItemService)}.{nameof(GetItemGrid)}：{e.Message}");
            }
            finally
            {
                callback?.Invoke(null);
            }
        }

        public void Dispose()
        {
            foreach (var poolObject in _items)
            {
                poolObject.Collect();
            }
            _objectSpawner.Dispose();
            _objectSpawner = null;
        }
    }
}
