using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Log;
using Core.Serialize.Json;
using Core.Utility;
using HotUpdate.Base.Attributes;
using HotUpdate.Base.Data;
using HotUpdate.Common.Config.Inventory;
using HotUpdate.Common.Config.Inventory.Config;
using HotUpdate.Common.Config.Inventory.Data;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.Game.InventoryModule.Items
{
    /// <summary>
    /// 玩家物品数据提供器
    /// </summary>
    [DataProviderId(typeof(IItemDataProvider))]
    public class ItemDataProvider : IItemDataProvider, ISOConfigSources
    {
        [Inject] private readonly IJsonManager _jsonManager;
        
        // 物品持久化ID生成器
        private ItemPersistentIdGenerator _idGenerator;
        // 玩家物品数据集合
        private ItemDataCollection _itemDataCollection;
        // 用于可堆叠物品：itemId -> list索引
        private readonly Dictionary<int, int> _stackIndexByItemId =  new();
        // 用于不可堆叠物品：instanceId -> list索引
        private readonly Dictionary<long?, int> _indexByInstanceId =  new();
        
        public Dictionary<int, ItemConfig> ConfigMap { get; } = new();
        
        public void AddData(int itemId, int deltaNum)
        {
            // 获取该ID的物品配置
            var itemConfig = ConfigMap.GetValueOrDefault(itemId);
            if(itemConfig == null)
                throw new KeyNotFoundException($"Item {itemId} id not found");

            if (itemConfig.isPile)
            {
                // 可堆叠：查找已有数据
                if (TryGetStackableData(itemId, out var exist))
                {
                    exist.itemNum += deltaNum;
                    // 注意：如果超出最大堆叠数，需要处理，这里先简化为直接加
                }
                else
                {
                    // 可堆叠物品持久化ID为默认ID
                    var newData = ItemDataCreateFactory.CreateData(itemConfig, deltaNum, ItemPersistentIdGenerator.DefaultNotStackableId);
                    // 新增玩家物品数据
                    _itemDataCollection.items.Add(newData);
                    // 同步缓存
                    _stackIndexByItemId.Add(itemId, _itemDataCollection.items.Count - 1);
                    Logger.LogDebug(ELogTags.Item, $"Item(id = {itemId}, num = {deltaNum}) added");
                }
            }
            else
            {
                // 不可堆叠：每个都应该创建新条目
                for (var i = 0; i < deltaNum; i++)
                {
                    var newData = ItemDataCreateFactory.CreateData(itemConfig, 1, _idGenerator.AllocateId());
                    // 新增玩家物品数据
                    _itemDataCollection.items.Add(newData);
                    // 同步缓存
                    _indexByInstanceId.Add(newData.persistentId, _itemDataCollection.items.Count - 1);
                }
                Logger.LogDebug(ELogTags.Item, $"{nameof(ItemDataCollection)}: Item(id = {itemId}, num = {deltaNum}) added");
            }
        }
        
        public void RemoveData(int id, int deltaNum, long? persistentId)
        {
            // 可堆叠物品的删除逻辑
            if (!persistentId.HasValue)
            {
                // 找到要移除的物品数据在字典中的索引
                if(!_stackIndexByItemId.TryGetValue(id, out var index))
                    return;
                
                // 找到要删除的数据
                var removedData = _itemDataCollection.items[index];
                // 移除对应的数量
                removedData.itemNum -= deltaNum;
                if (removedData.itemNum > 0)
                    return;
                
                // 数量为0，完全移除该物品数据
                var lastIndex = _itemDataCollection.items.Count - 1;
                if (index != lastIndex)
                {
                    // 找到物品数据列表的末尾数据
                    var lastItem = _itemDataCollection.items[lastIndex];
                    // 将末尾数据覆盖要删除的数据
                    _itemDataCollection.items[index] = lastItem;
                    // 字典更新被移动元素的索引
                    _stackIndexByItemId[lastItem.itemId] = index;
                }

                // 移除末尾数据
                _itemDataCollection.items.RemoveAt(lastIndex);
                // 字典删除被移除元素的索引
                _stackIndexByItemId.Remove(removedData.itemId);
            }
            else
            {
                // 找到要移除的物品数据在字典中的索引
                if(!_indexByInstanceId.TryGetValue(persistentId.Value, out var index))
                    return;
                
                // 找到要删除的数据
                var removedData = _itemDataCollection.items[index];
                // 数量为0，完全移除该物品数据
                var lastIndex = _itemDataCollection.items.Count - 1;
                if (index != lastIndex)
                {
                    // 找到物品数据列表的末尾数据
                    var lastItem = _itemDataCollection.items[lastIndex];
                    // 将末尾数据覆盖要删除的数据
                    _itemDataCollection.items[index] = lastItem;
                    // 字典更新被移动元素的索引
                    _indexByInstanceId[lastItem.persistentId] = index;
                }

                // 移除末尾数据
                _itemDataCollection.items.RemoveAt(lastIndex);
                // 字典删除被移除元素的索引
                _indexByInstanceId.Remove(removedData.persistentId);
            }
        }
        
        private bool TryGetStackableData(int itemId, out ItemData itemData)
        {
            if (_stackIndexByItemId.TryGetValue(itemId, out var index))
            {
                if (index >= 0 && index < _itemDataCollection.items.Count)
                {
                    itemData = _itemDataCollection.items[index];
                    return true;
                }
            }
            itemData = null;
            return false;
        }
        
        private bool TryGetInstanceData(long persistentId, out ItemData itemData)
        {
            if (_indexByInstanceId.TryGetValue(persistentId, out var index))
            {
                if (index >= 0 && index < _itemDataCollection.items.Count)
                {
                    itemData = _itemDataCollection.items[index];
                    return true;
                }
            }
            itemData = null;
            return false;
        }
        
        public bool TryGetData(int itemId, out ItemData itemData, long? PersistentId = null)
        {
            // 先查找不可堆叠物品，再查找可堆叠物品
            return PersistentId.HasValue ? TryGetInstanceData(PersistentId.Value, out itemData) : TryGetStackableData(itemId, out itemData);
        }

        /// <summary>
        /// 加载物品配置
        /// </summary>
        public async Task LoadConfigAsync()
        {
            using var handle = await GameAsset.LoadAssetAsync<TextAsset>(AssetKeys.ItemConfigs);
            var itemConfigCollection = _jsonManager.FromJson<ItemConfigCollection>(handle.Asset.text, settings: NewtonsoftJsonUtility.DefaultSerializerSettings);
            // 转存配置
            foreach (var itemConfig in itemConfigCollection.itemConfigs)
            {
                ConfigMap.Add(itemConfig.itemId, itemConfig);
            }
        }
        
        /// <summary>
        /// 加载玩家数据，依赖配置加载完成
        /// </summary>
        public async Task LoadDataAsync()
        {
            _itemDataCollection = await _jsonManager.FromJsonAsync<ItemDataCollection>(PathUtility.GetUserDataLocalSavePath(FileUtility.PlayerItemDataFileName), settings: NewtonsoftJsonUtility.DefaultSerializerSettings);
            // 初始化ID生成器
            _idGenerator = DIContainer.Create<ItemPersistentIdGenerator>(parameterValues: _itemDataCollection.nextPersistentId);
            // 重建字典缓存
            for (var i = 0; i < _itemDataCollection.items.Count; i++)
            {
                var itemData = _itemDataCollection.items[i];
                if (!ConfigMap.TryGetValue(itemData.itemId, out var config))
                    throw new KeyNotFoundException($"[{nameof(ItemDataProvider)}]: Item {itemData.itemId} not found");

                if (config.isPile)
                {
                    _stackIndexByItemId.Add(itemData.itemId, i);
                }
                else
                {
                    _indexByInstanceId.Add(itemData.persistentId, i);
                }
            }
        }
        
        public async Task SaveDataAsync()
        {
            _itemDataCollection.nextPersistentId = _idGenerator.CurrentMaxId + 1;
            await _jsonManager.SaveToJsonAsync(_itemDataCollection, PathUtility.GetUserDataLocalSavePath(FileUtility.PlayerItemDataFileName), settings: NewtonsoftJsonUtility.DefaultSerializerSettings);
            Logger.LogDebug(ELogTags.Main, $"背包数据保存成功，{FileUtility.PlayerItemDataFileName}");
        }
        
        public void LoadData()
        {

        }

        /// <summary>
        /// 保存玩家数据
        /// </summary>
        public void SaveData()
        {
            if (_itemDataCollection != null)
            {
                _itemDataCollection.nextPersistentId = _idGenerator.CurrentMaxId + 1;
                _jsonManager.SaveToJson(_itemDataCollection, PathUtility.GetUserDataLocalSavePath(FileUtility.PlayerItemDataFileName), settings: NewtonsoftJsonUtility.DefaultSerializerSettings);
                Logger.LogDebug(ELogTags.Main, $"背包数据保存成功，{FileUtility.PlayerItemDataFileName}");
            }
        }

        /// <summary>
        /// 通过物品类型获取所有的物品数据
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public IEnumerable<ItemData> GetItemsByType(EItemType itemType)
        {
            foreach (var itemData in _itemDataCollection.items)
            {
                if (ConfigMap.TryGetValue(itemData.itemId, out var config) && config.itemType == itemType)
                {
                    yield return itemData;
                }
            }
        }
    }
}
