using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AssetBundles.Management;
using Core.DI;
using Core.Log;
using Core.Mono;
using Core.Pool;
using Core.Tasks;
using Core.UI.ViewController;
using HotUpdate.Base.UI;
using HotUpdate.Common.Config.Inventory;
using HotUpdate.Game.InventoryModule.Items;
using HotUpdate.Game.InventoryModule.Sorts;
using HotUpdate.Game.Slot;
using HotUpdate.UI.Inventory.State;
using HotUpdate.UI.Inventory.ViewModel;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.UI.Inventory
{
    /// <summary>
    /// 背包界面控制器
    /// </summary>
    public class InventoryController : UIController<InventoryPanel>, IBlockOperation
    {
        [Inject] private IPoolManager _poolManager;
        [Inject] protected IInventoryManager _inventoryManager;
        [Inject] protected ObjectSpawner _objectSpawner;
        [Inject] protected IMonoAdapter monoAdapter;
        [Inject] private IUIService _uiService;
        
        private InventoryDeleteViewModel _inventoryDeleteViewModel;
        private readonly Dictionary<Type, IInventoryState> _inventoryStates = new();
        // 当前背包界面所处的状态
        private IInventoryState _currentInventoryState;
        // 物品排序委托
        public Comparison<Item> sortComparison = InventorySorter.Default(1);

        protected override bool IsCursorVisible => true;

        public bool BlockOperation => true;

        /// <summary>
        /// 当前显示的物品类型
        /// </summary>
        public EItemType CurrentItemType { get; set; }
        
        /// <summary>
        /// 格子生成器
        /// </summary>
        public GridGenerator<Item, ItemCell> GridGenerator { get; set; }
        
        /// <summary>
        /// 详细界面工厂
        /// </summary>
        public InventoryDetailViewCreateFactory DetailPanelFactory { get; set; }
        
        public event Action<string> OnButtonClickEvent;
        
        public event Action<string, string> OnInputFieldValueChangedEvent;
        
        public event Action<string, float> OnSliderValueChangedEvent;
        
        protected override async Task OnInit()
        {
            _inventoryDeleteViewModel = DIContainer.Create<InventoryDeleteViewModel>();
            view.SetViewModel(_inventoryDeleteViewModel);
            
            // 初始化背包界面所有状态
            _inventoryStates.Add(typeof(InventoryDeleteState), DIContainer.Create<InventoryDeleteState>(parameterValues: new object[]
            {
                _inventoryDeleteViewModel,
                this
            }));
            
            // 初始化工厂
            DetailPanelFactory = _poolManager.GetData<InventoryDetailViewCreateFactory>();
            // 初始化格子生成器
            InitGridGenerator();
            // 创建选项
            await InitTypeOpt();
        }
        
        protected override Task OnActive()
        {
            _currentInventoryState = null; // 默认就是普通模式
            return Task.CompletedTask;
        }

        protected override async Task OnInactivate()
        {
            await TransitionTo(null);
            // 显示主界面
            await _uiService.ShowAsync(_uiService.GetPanel(EUIPanelId.MainPanel).PanelId);
        }
        
        public void InitGridGenerator()
        {
            // 创建格子生成器
            var builder = GridGeneratorBuilder<Item, ItemCell>.Create();
            var generator = builder.CreateGenerator(EGridLayout.Vertical)
                .SetParent(view.svItems)
                .SetOriginOffset(15, -15)
                .SetGridSize(100, 100)
                .SetGridSpace(15, 15)
                .SetColumn(8)
                .Build();
            
            // 保存生成器
            GridGenerator = generator;
        }
        
        /// <summary>
        /// 初始化侧面选项
        /// </summary>
        public async Task InitTypeOpt()
        {
            view.OptGroup.allowSwitchOff = true;
            foreach (var itemType in Enum.GetValues(typeof(EItemType)))
            {
                var opt = await _objectSpawner.SpawnAsync<ItemTypeOpt>(AssetKeys.ItemTypeOpt, view.svOpts.content);
                opt.InitOption((EItemType)itemType, null, view.OptGroup);
                opt.OnItemTypeOptChange += UpdateItemsByType;
                view.AddItemTypeOpt(opt);
            }
            // // 默认选择第一个选项
            view.GetFirstItemTypeOpt().Select();
            view.OptGroup.allowSwitchOff = false;
        }
        
        /// <summary>
        /// 根据物品类型更新物品数据
        /// </summary>
        /// <param name="itemType"></param>
        public async void UpdateItemsByType(EItemType itemType)
        {
            try
            {
                // 先通知当前状态执行逻辑
                _currentInventoryState?.OnBeforeRefreshItem();
                // 根据当前数据创建，先 await，避免惯性滚动触发创建
                var items = await _inventoryManager.CreateItemsAsync(itemType);
                /*
                 * 清空上次显示的格子，清空格子一定要在await后执行，因为当调用UpdateItemsByType方法时，若先清空，但是界面可能还在因为惯性在滚动
                 * 导致又触发ScrollRectValueChanged事件创建新的格子，导致清空后又有格子，会出现显示异常和重复创建格子的问题
                 * 因为这个异步方法会被挂起的副作用，所以等待await后再执行清理格子，就能避免这个问题
                 */
                GridGenerator.ClearGrids();
                // 排序物品数据DTO，默认按照品质类型排序    
                items.Sort(sortComparison);
                // 初始化生成器
                GridGenerator.AddClickListener(ItemClick);
                GridGenerator.SetDatas(items);
                // 手动更新一次，将协程转换为Task等待
                await TaskUtility.WaitForCoroutine(GridGenerator.FadeUpdateGrid(), monoAdapter);
                // 记录当前选择的物品类型
                CurrentItemType = itemType;
                // 显示第一个物品的详细信息
                if (items.Count > 0)
                {
                    await UpdateDetail(items[0]);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(ELogTags.Item, $"item update fail, {e.Message}");
            }
        }
        
        protected async void ItemClick(Item item)
        {
            try
            {
                // 是否点击了格子，移除new标识
                _inventoryManager.UpdateGridNewState(item);
                // 删除模式才执行
                if (_currentInventoryState != null)
                {
                    // 更新待删除状态
                    UpdateGridDeleteState(item);
                    await _currentInventoryState.OnItemClick(item);
                }
                
                // 更新详细界面
                await UpdateDetail(item);
            }
            catch (Exception e)
            {
                Logger.LogError(ELogTags.Item, $"item update fail, {e.Message}");
            }
        }
        
        /// <summary>
        /// 更新详细界面
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task UpdateDetail(Item item)
        {
            IInventoryDetailPanel inventoryDetailPanel = null;
            try
            {
                var itemConfig = item.itemConfig;
                var itemData = _inventoryManager.GetData(item);

                if (!view.DetailPanel || CurrentItemType != itemConfig.itemType)
                {
                    if (view.DetailPanel)
                        DetailPanelFactory.Release(view.DetailPanel);
                    // 工厂创建详细界面
                    inventoryDetailPanel = await DetailPanelFactory.CreateDetailPanel(itemConfig.itemType, view.DetailArea);
                    view.DetailPanel = (InventoryDetailPanel)inventoryDetailPanel;
                }

                // 初始化详细界面
                view.DetailPanel.UpdateInfo(itemConfig, itemData);
            }
            catch (Exception e)
            {
                _objectSpawner.Release((InventoryDetailPanel)inventoryDetailPanel);
                Logger.LogError(ELogTags.Item, $"{nameof(InventoryController)}: Create detail panel fail, {e.Message}");
            }
        }

        /// <summary>
        /// 更新格子的删除标志
        /// </summary>
        /// <param name="item"></param>
        private void UpdateGridDeleteState(Item item)
        {
            // 更新格子的删除状态标志
            item.isDeleted = !item.isDeleted;
        }

        /// <summary>
        /// 过渡到指定状态
        /// </summary>
        /// <param name="nextState"></param>
        public async Task TransitionTo(Type nextState)
        {
            if(_currentInventoryState != null)
                await _currentInventoryState.Exit();

            if (nextState == null)
            {
                _currentInventoryState = null;
            }
            else
            {
                _currentInventoryState = _inventoryStates[nextState];
                await _currentInventoryState.Enter();
            }
        }
        
        /// <summary>
        /// 切换排序
        /// </summary>
        /// <param name="itemSort"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void SwitchSort(EItemSort itemSort)
        {
            sortComparison = itemSort switch
            {
                EItemSort.Default => InventorySorter.Default(1),
                EItemSort.Quality => InventorySorter.Get<QualitySorter>(-1),
                _ => throw new ArgumentOutOfRangeException(nameof(itemSort), itemSort, null)
            };
            UpdateItemsByType(CurrentItemType);
        }
        
        /// <summary>
        /// 删除物品
        /// </summary>
        /// <param name="deletedItems"></param>
        public void ExecuteDelete(Dictionary<Item, int> deletedItems)
        {
            foreach (var (item, num) in deletedItems)
            {
                _inventoryManager.DeleteItem(item.itemConfig.itemId, num, item.itemConfig.isPile ? null : item.persistentId);
            }
        }

        private Task EnterDeleteState()
        {
            // 切换为删除状态
            return TransitionTo(typeof(InventoryDeleteState));
        }

        public Task ExitDeleteState()
        {
            ClearGridDeleteState();
            // 切换为正常状态
            return TransitionTo(null);
        }

        /// <summary>
        /// 清理格子的删除状态
        /// </summary>
        public void ClearGridDeleteState()
        {
            // 获取当前显示物品，重置Item删除标记
            foreach (var item in _inventoryManager.GetAllItems())
            {
                item.isDeleted = false;
            }

            // 清理删除状态
            foreach (var itemCell in GridGenerator.GetAllCell())
            {
                itemCell.ClearDeleteState();
            }
        }
        
        protected override async void OnButtonClick(string btnName)
        {
            try
            {
                if (btnName == nameof(view.btnClose))
                {
                    await ExitDeleteState();
                    // 关闭背包界面
                    await uiManager.DestroyView(panelId);
                    Logger.LogDebug(ELogTags.Item, $"{view.name} closed");
                }
                else if (btnName == nameof(view.btnRequestDelete))
                {
                    await EnterDeleteState();
                }
                else
                {
                    OnButtonClickEvent?.Invoke(btnName);
                }
            }
            catch (Exception e)
            {
                Logger.LogException(ELogTags.Item, new Exception("button click logic execute error", e));
            }
        }
        
        protected override void OnScrollRectValueChanged(string scrollViewName, Vector2 pos)
        {
            if (scrollViewName == nameof(view.svItems))
            {
                GridGenerator?.UpdateGrid();
            }
        }
        
        protected override void OnDropdownValueChanged(string dropdownName, int index)
        {
            try
            {
                if (dropdownName != nameof(view.dpSorts))
                    return;
                
                switch (index)
                {
                    case 0:
                        SwitchSort(EItemSort.Default);
                        break;
                    case 1:
                        SwitchSort(EItemSort.Quality);
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.LogError(ELogTags.Item, $"[{nameof(InventoryController)}]: switch item sort error, {e.Message}");
            }
        }

        protected override void OnInputFieldValueChanged(string fieldName, string inputStr)
        {
            OnInputFieldValueChangedEvent?.Invoke(fieldName, inputStr);
        }
        
        protected override void OnSliderValueChanged(string sliderName, float value)
        {
            OnSliderValueChangedEvent?.Invoke(sliderName, value);
        }

        protected override Task OnDispose()
        {
            // 清理选项UI
            _objectSpawner.Release(view.ItemTypeOpts);
            _objectSpawner.Dispose();
            
            // 清理删除状态缓存
            _inventoryStates.Clear();
            _currentInventoryState = null;
            
            // 清理详细界面UI
            _poolManager.PushData(DetailPanelFactory);
            DetailPanelFactory = null;
            
            // 清理物品格子UI
            _poolManager.PushData(GridGenerator);
            GridGenerator = null;
            
            _inventoryDeleteViewModel.Dispose();
            _inventoryDeleteViewModel = null;

            // 清理其它对象
            sortComparison = null;
            _poolManager = null;
            return Task.CompletedTask;
        }
    }
}
