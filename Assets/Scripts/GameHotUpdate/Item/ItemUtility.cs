using System;
using System.Collections.Generic;
using Core.AssetBundles.Management;
using Core.Config;
using Core.DataPersistence.Binary;
using Core.Loader.Sprite;
using Core.Loader.UI;
using Core.Log;
using Core.Service;
using Core.Utility;
using GameHotUpdate.Item.UI;
using UnityEngine;

namespace GameHotUpdate.Item
{
    /// <summary>
    /// 物品工具类
    /// </summary>
    public static class ItemUtility
    {
        /// <summary>
        /// 获取物品格子UI
        /// 内部已初始化UI，异常时回调返回null
        /// </summary>
        /// <param name="awardIds"></param>
        /// <param name="parent"></param>
        /// <param name="callback"></param>
        public static async void GetItemGrid(string awardIds, Transform parent, Action<ItemGrid> callback)
        {
            try
            {
                var itemInfos = new Dictionary<int, int>();
                // 解析奖励ID数组
                TextUtility.SplitMultiple(awardIds, 1, 2, async (int awardId, int num) =>
                {
                    itemInfos.Add(awardId, awardId);
                });

                foreach (var pair in itemInfos)
                {
                    // 获取UI
                    var itemGrid = await ServiceLocator.Get<IUiLoader>()
                        .GetUIObject<ItemGrid>(EAssetBundleType.UI, ResKeyCollection.ItemGrid, parent);
                    // 读取配置
                    var itemInfo = ServiceLocator.Get<IBinaryDataManager>()
                        .GetConfig<ItemInfoContainer>(EConfigLoadType.Excel).dataDic[pair.Key];
                    // 加载图标
                    var itemIcon = await ServiceLocator.Get<ISpriteLoader>().LoadSpriteAsync(
                        ResKeyCollection.Atlas_Icon_Item, 
                        itemInfo.f_icon);
                    // 初始化
                    itemGrid.Init(itemIcon, pair.Value, itemInfo.f_quality);
                    callback?.Invoke(itemGrid);
                }
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(ItemUtility)}.{nameof(GetItemGrid)}：{e.Message}");
            }
            finally
            {
                callback?.Invoke(null);
            }
        }
        
        /// <summary>
        /// 获取物品格子UI
        /// 内部已初始化UI，异常时回调返回null
        /// </summary>
        /// <param name="awardIds"></param>
        /// <param name="callback"></param>
        public static async void GetItemGrid(string awardIds, Action<ItemGrid> callback)
        {
            try
            {
                var itemInfos = new Dictionary<int, int>();
                // 解析奖励ID数组
                TextUtility.SplitMultiple(awardIds, 1, 2, async (int awardId, int num) =>
                {
                    itemInfos.Add(awardId, awardId);
                });
            
                foreach (var pair in itemInfos)
                {
                    // 获取UI
                    var itemGrid = await ServiceLocator.Get<IUiLoader>()
                        .GetUIObject<ItemGrid>(EAssetBundleType.UI, ResKeyCollection.ItemGrid, null);
                    // 读取配置
                    var itemInfo = ServiceLocator.Get<IBinaryDataManager>()
                        .GetConfig<ItemInfoContainer>(EConfigLoadType.Excel).dataDic[pair.Key];
                    // 加载图标
                    var itemIcon = await ServiceLocator.Get<ISpriteLoader>().LoadSpriteAsync(ResKeyCollection.Atlas_Icon_Item, itemInfo.f_icon);
                    // 初始化
                    itemGrid.Init(itemIcon, pair.Value, itemInfo.f_quality);
                    callback?.Invoke(itemGrid);
                }
            }
            catch (Exception e)
            {
                LogManager.LogError($"{nameof(ItemUtility)}.{nameof(GetItemGrid)}：{e.Message}");
            }
            finally
            {
                callback?.Invoke(null);
            }
        }
    }
}
