using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DI;
using Core.Log;
using Core.Mono;
using Core.Pool;
using Core.Tasks;
using Core.UI;
using HotUpdate.Base.Data;
using HotUpdate.Base.UI;
using HotUpdate.Common.Config.Inventory;
using HotUpdate.Game.InventoryModule.Items;
using HotUpdate.Game.Slot;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Logger = Core.Log.Logger;

namespace HotUpdate.UI.Inventory
{
    /// <summary>
    /// 背包删除物品确认内容UI
    /// </summary>
    public class DeleteItemConfirmContent : UIBehaviourBase, IConfirmContent
    {
        [Inject] private IPoolManager _poolManager;
        [Inject] private IMonoAdapter _monoAdapter;
        [Inject] private ItemCreateFactory _itemCreateFactory;
        
        [InjectUI] private TextMeshProUGUI txtDelTip;
        [InjectUI] private ScrollRect svDel;
        
        private GridGenerator<Item, ItemCell> _generator;
        
        public async void DrawContent(ConfirmData confirmData)
        {
            try
            {
                if (confirmData.ContentData is not Dictionary<Item, int> deleteItems)
                    throw new ArgumentException($"tipDatas is not dictionary type {typeof(Dictionary<Item, int>)}");
                
                // 创建格子生成器
                var builder = GridGeneratorBuilder<Item, ItemCell>.Create();
                _generator = builder.CreateGenerator(EGridLayout.Horizontal)
                    .SetParent(svDel)
                    .SetOriginOffset(15, -10)
                    .SetGridSize(100, 100)
                    .SetGridSpace(15, 15)
                    .SetRow(1)
                    .Build();
                
                // 初始化生成器
                _generator.AddClickListener(ItemClick);
                _generator.SetDatas(await CreateItems(deleteItems));
                // 手动更新一次，将协程转换为Task等待
                await TaskUtility.WaitForCoroutine(_generator.FadeUpdateGrid(), _monoAdapter);
            }
            catch (Exception e)
            {
                Logger.LogError(ELogTags.Item, $"Draw content data error, {e.Message}");
            }
        }

        private Task<List<Item>> CreateItems(Dictionary<Item, int> deleteItems)
        {
            var items = new List<Item>(deleteItems.Count);
            foreach (var (item, delNum) in deleteItems)
            {
                // 获取物品对象对象
                var newItem = _itemCreateFactory.CreateItem();
                newItem.persistentId = item.persistentId;
                newItem.itemConfig = item.itemConfig;
                // 赋值为删除数量
                newItem.auxValue = delNum;
                items.Add(newItem);
            }

            return Task.FromResult(items);
        }

        private void ItemClick(Item item)
        {
            // 显示物品详细界面
            // ...
        }

        public void ClearContent()
        {
            _poolManager.PushData(_generator);
            _poolManager.PushData(_itemCreateFactory);
            _generator = null;
        }

        protected override void OnScrollRectValueChanged(string svName, Vector2 pos)
        {
            if (svName == nameof(svDel))
            {
                _generator.UpdateGrid();
            }
        }
    }
}
