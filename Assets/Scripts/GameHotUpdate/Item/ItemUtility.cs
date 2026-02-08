using System;
using Core.AssetBundles.Management;
using Core.Config;
using Core.DataPersistence.Binary;
using Core.Loader;
using Core.Log;
using Core.Reflection;
using Core.Service;
using Core.Utility;
using Game.Objects;
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
        public static void GetItemGrid(string awardIds, Transform parent, Action<ItemGrid> callback)
        {
            // 解析奖励ID数组
            TextUtility.SplitMultiple(awardIds, 1, 2, async (int awardId, int num) =>
            {
                try
                {
                    // 获取UI
                    var itemGrid = await ServiceLocator.Get<IObjectBuilder>()
                        .GetHotfixUIObject<ItemGrid>(EAssetBundleType.UI, ResKeyCollection.ItemGrid, parent);
                    // 读取配置
                    var itemInfo = ServiceLocator.Get<IBinaryDataManager>()
                        .GetConfig<ItemInfoContainer>(EConfigLoadType.Excel).dataDic[awardId];
                    // 加载图标
                    var itemIcon = await ServiceLocator.Get<IFactoryManager>()
                        .GetFactory<IAssetLoaderFactory, AssetLoaderFactory>().GetSpriteLoader()
                        .GetSpriteAsync(ResKeyCollection.Atlas_Icon_Item, itemInfo.f_icon);
                    // 初始化
                    itemGrid.Init(itemIcon, num, itemInfo.f_quality);
                    callback?.Invoke(itemGrid);
                }
                catch (Exception e)
                {
                    LogManager.LogError($"{nameof(ItemUtility)}.{nameof(GetItemGrid)}：{e.Message}");
                }
                finally
                {
                    callback?.Invoke(null);
                }
            });
        }
        
        /// <summary>
        /// 获取物品格子UI
        /// 内部已初始化UI，异常时回调返回null
        /// </summary>
        /// <param name="awardIds"></param>
        /// <param name="callback"></param>
        public static void GetItemGrid(string awardIds, Action<ItemGrid> callback)
        {
            // 解析奖励ID数组
            TextUtility.SplitMultiple(awardIds, 1, 2, async (int awardId, int num) =>
            {
                try
                {
                    // 获取UI
                    var itemGrid = await ServiceLocator.Get<IObjectBuilder>()
                        .GetHotfixUIObject<ItemGrid>(EAssetBundleType.UI, ResKeyCollection.ItemGrid, null);
                    // 读取配置
                    var itemInfo = ServiceLocator.Get<IBinaryDataManager>()
                        .GetConfig<ItemInfoContainer>(EConfigLoadType.Excel).dataDic[awardId];
                    // 加载图标
                    var itemIcon = await ServiceLocator.Get<IFactoryManager>()
                        .GetFactory<IAssetLoaderFactory, AssetLoaderFactory>().GetSpriteLoader()
                        .GetSpriteAsync(ResKeyCollection.Atlas_Icon_Item, itemInfo.f_icon);
                    // 初始化
                    itemGrid.Init(itemIcon, num, itemInfo.f_quality);
                    callback?.Invoke(itemGrid);
                }
                catch (Exception e)
                {
                    LogManager.LogError($"{nameof(ItemUtility)}.{nameof(GetItemGrid)}：{e.Message}");
                }
                finally
                {
                    callback?.Invoke(null);
                }
            });
        }
    }
}
