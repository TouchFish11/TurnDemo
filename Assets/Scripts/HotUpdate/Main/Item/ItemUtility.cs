using System;
using System.Collections.Generic;
using Core.Loader.Object;
using Core.Loader.Sprite;
using Core.Log;
using Core.Serialize.Binary;
using Core.Service;
using Core.Utility;
using HotUpdate.Config;
using HotUpdate.Main.Item.UI;
using UnityEngine;

// ReSharper disable RedundantLambdaParameterType

namespace HotUpdate.Main.Item
{
    /// <summary>
    /// 物品工具类
    /// </summary>
    public static class ItemUtility
    {
        private static readonly IPrefabLoader _prefabLoader = ServiceLocator.Get<IPrefabLoader>();
        private static readonly IBinaryDataManager _binaryDataManager = ServiceLocator.Get<IBinaryDataManager>();
        private static readonly ISpriteLoader _spriteLoader = ServiceLocator.Get<ISpriteLoader>();
        
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
                TextUtility.SplitMultiple(awardIds, 1, 2, (int awardId, int num) =>
                {
                    itemInfos.Add(awardId, num);
                });

                foreach (var pair in itemInfos)
                {
                    // 获取UI
                    var itemGrid = await _prefabLoader.GetObjectAsync<ItemGrid>(AbKeyCollection.Ui, ResKeyCollection.ItemGrid, parent);
                    // 读取配置
                    var itemInfo = _binaryDataManager.GetConfig<ItemInfoContainer>(EConfigLoadType.Excel).dataDic[pair.Key];
                    // 加载图标
                    var itemIcon = await _spriteLoader.LoadSpriteAsync(AbKeyCollection.Spriteatlas, ResKeyCollection.Atlas_Icon_Item, 
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
                TextUtility.SplitMultiple(awardIds, 1, 2, (int awardId, int num) =>
                {
                    itemInfos.Add(awardId, num);
                });
            
                foreach (var pair in itemInfos)
                {
                    // 获取UI
                    var itemGrid = await _prefabLoader.GetObjectAsync<ItemGrid>(AbKeyCollection.Ui, ResKeyCollection.ItemGrid, null);
                    // 读取配置
                    var itemInfo = _binaryDataManager.GetConfig<ItemInfoContainer>(EConfigLoadType.Excel).dataDic[pair.Key];
                    // 加载图标
                    var itemIcon = await _spriteLoader.LoadSpriteAsync(AbKeyCollection.Spriteatlas, ResKeyCollection.Atlas_Icon_Item, itemInfo.f_icon);
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
