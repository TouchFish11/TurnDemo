using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Core.DI;
using Core.Pool;
using HotUpdate.Base.Data;
using HotUpdate.Base.Service;
using HotUpdate.Common.Config.Inventory;
using HotUpdate.Common.Config.Inventory.Config;
using HotUpdate.Common.Config.Inventory.Data;

namespace HotUpdate.Game.InventoryModule.Items
{
    /// <summary>
    /// 背包管理器
    /// </summary>
    public class InventoryManager : IInventoryManager
    {
        [Inject] private ItemCreateFactory _itemCreateFactory;
        [Inject] private IItemDataProvider _itemDataProvider;
        
        private readonly IPoolManager _poolManager;
        
        // 当前显示的不可堆叠物品持久化ID到物品对象的映射
        private readonly Dictionary<long, Item> _instanceIdToItemMap =  new();
        // 当前显示的可堆叠物品物品ID到物品对象的映射
        private readonly Dictionary<int, Item> _itemIdToItemMap =  new();
        // 当前已经加载过的图标Key
        private readonly HashSet<string> _iconKeys = new();
        
        public IIconService IconService { get; }
        
        public InventoryManager(IPoolManager poolManager, IIconService iconProvider)
        {
            _poolManager = poolManager;
            IconService = iconProvider;
        }

        public IEnumerable<Item> GetAllItems()
        {
            // 当前显示的是不可堆叠物品
            if(_instanceIdToItemMap.Count > 0)
                return _instanceIdToItemMap.Values;
            // 当前显示的是可堆叠物品
            return _itemIdToItemMap.Values;
        }
        
        public void DeleteItem(int itemId, int deleteNum, long? persistentId = null)
        {
            if (persistentId.HasValue)
            {
                _itemDataProvider.RemoveData(itemId, 1, persistentId);
            }
            else
            {
                _itemDataProvider.RemoveData(itemId, deleteNum, null);
            }
        }
        
        public ItemData GetData(Item item)
        {
            // 根据是否可堆叠查找不同的物品数据
            return _itemDataProvider.TryGetData(item.itemConfig.itemId, out var data, item.persistentId) ? data : null;
        }

        public void UpdateGridNewState(Item item)
        {
            item.isNew = false;
            var itemData = GetData(item);
            if (itemData != null && itemData.isNew)
                itemData.isNew = false;
        }
        
        public async Task<List<Item>> CreateItemsAsync(EItemType itemType)
        {
            // 回收Item，清空缓存
            foreach (var item in _instanceIdToItemMap.Values)
                _poolManager.PushData(item);
            _instanceIdToItemMap.Clear();
            
            foreach (var item in _itemIdToItemMap.Values)
                _poolManager.PushData(item);
            _itemIdToItemMap.Clear();
            
            // 创建所有物品对象
            var dtoTasks = new List<Task<Item>>();
            foreach(var itemData in _itemDataProvider.GetItemsByType(itemType))
            {
                var itemConfig = _itemDataProvider.ConfigMap.GetValueOrDefault(itemData.itemId);
                dtoTasks.Add(CreateItem(itemData.persistentId, itemConfig, itemData));
            }

            var itemDTOs = await Task.WhenAll(dtoTasks);
            // 等待所有对象创建完成
            return new List<Item>(itemDTOs);
        }

        public void Clear()
        {
            // 释放已经加载过的图片资源句柄
            foreach (var iconKey in _iconKeys)
            {
                // 释放显示的图标的句柄
                IconService.Release(iconKey);
            }
            _iconKeys.Clear();
            _instanceIdToItemMap.Clear();
        }
        
        /// <summary>
        /// 创建物品对象
        /// </summary>
        /// <param name="persistentId"></param>
        /// <param name="itemConfig"></param>
        /// <param name="itemData"></param>
        /// <returns></returns>
        private async Task<Item> CreateItem(long? persistentId, ItemConfig itemConfig, ItemData itemData)
        {
            // 对象池复用对象
            var item = _itemCreateFactory.CreateItem();
            // 设置实例ID
            item.persistentId = persistentId;
            // 引用物品配置
            item.itemConfig = itemConfig;
            // 根据物品类型决定显示什么数值
            item.auxValue = ItemResolver.ResolveAux(itemData);
            // 是否是新物品
            item.isNew = itemData.isNew;
            // 缓存对象，按是否可堆叠分别缓存
            if(persistentId != ItemPersistentIdGenerator.DefaultNotStackableId)
            {
                Debug.Assert(persistentId != null, $"{nameof(persistentId)} != null");
                _instanceIdToItemMap.Add(persistentId.Value, item);
            }
            else
                _itemIdToItemMap.Add(itemConfig.itemId, item);
            // 加载该物品的图标
            var sprite = await IconService.LoadIconAsync(itemConfig.icon);
            // 缓存加载成功物品图标Key
            if (sprite)
                _iconKeys.Add(itemConfig.icon);
            return item;
        }
    }
}
