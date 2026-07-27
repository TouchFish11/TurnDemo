using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DI;
using Core.Log;
using Core.UI;
using HotUpdate.Base.Data;
using HotUpdate.Base.Enums;
using HotUpdate.Base.UI;
using HotUpdate.Common.Config.Inventory;
using HotUpdate.Game.InventoryModule.Items;
using HotUpdate.UI.Inventory.ViewModel;
using HotUpdate.UI.Tip;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.UI.Inventory.State
{
    /// <summary>
    /// 背包界面删除状态
    /// </summary>
    public class InventoryDeleteState : IInventoryState
    {
        [Inject] private IInventoryManager _inventoryManager;
        [Inject] private IUIService _uiService;
        
        private readonly InventoryController _inventoryController;
        private readonly InventoryDeleteViewModel _inventoryDeleteViewModel;
        
        private readonly Dictionary<Item, int> _deletedItems = new();
        
        // 当前正在操作的物品对象
        private Item _currentItem;
        /// 默认最小删除数量
        private const int DefaultDeleteMinNum = 1;
        
        public InventoryDeleteState(InventoryDeleteViewModel viewModel, InventoryController inventoryController)
        {
            _inventoryDeleteViewModel = viewModel;
            _inventoryController = inventoryController;
        }
        
        public Task Enter()
        {
            // 进入状态时绑定
            _inventoryDeleteViewModel.DeleteNum.Subscribe(UpdateDeleteStateUI);
            // 监听添加减少按钮点击
            _inventoryController.OnButtonClickEvent += OnButtonClickEvent;
            // 监听输入框事件
            _inventoryController.OnInputFieldValueChangedEvent += OnInputFieldValueChangedEvent;
            // 监听滑动条事件
            _inventoryController.OnSliderValueChangedEvent += OnSliderValueChanged;
            // 激活删除区域
            _inventoryDeleteViewModel.DeleteAreaActive.Value = true;
            // 隐藏删除盒，因为还没有选择删除的物品
            _inventoryDeleteViewModel.DeleteBoxActive.Value = false;
            return Task.CompletedTask;
        }

        public Task OnItemClick(Item item)
        {
            // 重置部分ViewModel前，先清空上次选择的对象，否则会影响上次选择的数量
            _currentItem = null;
            // 重置部分ViewModel
            _inventoryDeleteViewModel.DeleteSliderExtremum.Value = default;
            _inventoryDeleteViewModel.DeleteNum.Value = 0;
            
            // 先记录当前操作的物品
            _currentItem = item;
            // 更新滑动条的最最大/最小值
            var itemData = _inventoryManager.GetData(_currentItem);
            _inventoryDeleteViewModel.DeleteSliderExtremum.Value = (DefaultDeleteMinNum, itemData.itemNum);
            
            // 点击相同格子移除销毁状态
            if (_deletedItems.Remove(item))
            {
                // 隐藏删除盒的输入
                _inventoryDeleteViewModel.DeleteBoxActive.Value = false;
            }
            // 新增销毁物品
            else
            {
                _deletedItems.Add(item, DefaultDeleteMinNum);
                // 初始化唯一数据源
                _inventoryDeleteViewModel.DeleteNum.Value = DefaultDeleteMinNum; 
                if (item.itemConfig.isPile)
                {
                    // 激活删除盒的输入
                    _inventoryDeleteViewModel.DeleteBoxActive.Value = true;
                }
                else
                {
                    // 隐藏删除盒的输入
                    _inventoryDeleteViewModel.DeleteBoxActive.Value = false;
                }
            }
            return Task.CompletedTask;
        }

        private async void OnButtonClickEvent(string btnName)
        {
            if (btnName == "btnAdd")
            {
                // 能进入这里的逻辑，一定是可堆叠的物品
                _inventoryDeleteViewModel.DeleteNum.Value++;
            }
            else if (btnName == "btnSub")
            {
                // 能进入这里的逻辑，一定是可堆叠的物品
                _inventoryDeleteViewModel.DeleteNum.Value--;
            }
            else if (btnName == "btnDelete")
            {
                await RequestDelete();
            }
            else if (btnName == "btnCancelDelete")
            {
                // 删除完毕后，退出删除状态
                await _inventoryController.ExitDeleteState();
            }
            else if (btnName == "btnMin")
            {
                _inventoryDeleteViewModel.DeleteNum.Value = DefaultDeleteMinNum;
            }
            else if (btnName == "btnMax")
            {
                var itemData = _inventoryManager.GetData(_currentItem);
                _inventoryDeleteViewModel.DeleteNum.Value = itemData.itemNum;
            }
        }

        /// <summary>
        /// 请求删除物品
        /// </summary>
        private async Task RequestDelete()
        {
            // 打开删除确认界面
            var tipController = await _uiService.OpenAsync(EUIPanelId.TipPanel, E_UILayer.Bot) as TipController;
            // 初始化确认数据
            var confirmData = DIContainer.Create<ConfirmData>();
            confirmData.ConfirmTitle = "删除提示";
            confirmData.ConfirmContent = EConfirmContent.ItemDelete;
            confirmData.ContentData = new Dictionary<Item,int>(_deletedItems);
            confirmData.ConfirmMessage = "以下物品将被销毁";
            confirmData.OnConfirm = ExecuteDelete;
            confirmData.OnCancel = ExucuteCancel;
            // 设置提示界面
            tipController.SetTip(confirmData);
        }

        /// <summary>
        /// 执行删除
        /// </summary>
        private async void ExecuteDelete()
        {
            try
            {
                // 删除物品
                _inventoryController.ExecuteDelete(_deletedItems);
                // 刷新当前界面
                _inventoryController.UpdateItemsByType(_inventoryController.CurrentItemType);
                // 删除完毕后，退出删除状态
                await _inventoryController.ExitDeleteState();
            }
            catch (Exception e)
            {
                Logger.LogException(ELogTags.Item, e);
            }
        }

        private async void ExucuteCancel()
        {
            // 退出删除状态
            await _inventoryController.ExitDeleteState();
        }
        
        /// <summary>
        /// 更新所有删除相关的UI显示
        /// </summary>
        /// <param name="currentNum"></param>
        private void UpdateDeleteStateUI(int currentNum)
        {
            if (_currentItem == null) 
                return;
            
            // 获取玩家物品拥有数量
            var itemData = _inventoryManager.GetData(_currentItem);
            // 限制范围
            currentNum = Mathf.Clamp(currentNum, 1, itemData.itemNum);
            
            // 同步字典
            if (_deletedItems.ContainsKey(_currentItem))
                _deletedItems[_currentItem] = currentNum;
            
            // 删除数量不允许超过拥有数量
            if (currentNum == itemData.itemNum && itemData.itemNum != DefaultDeleteMinNum)
            {
                // 禁用增加/最大按钮
                _inventoryDeleteViewModel.AddDeleteBtnEnable.Value = false;
                _inventoryDeleteViewModel.MaxDeleteBtnEnable.Value = false;
                // 启用最小/减少按钮
                _inventoryDeleteViewModel.SubDeleteBtnEnable.Value = true;
                _inventoryDeleteViewModel.MinDeleteBtnEnable.Value = true;
            }
            // 删除数量不允许为负数或0
            else if(currentNum == 1 && itemData.itemNum > DefaultDeleteMinNum)
            {
                // 禁用最小/减少按钮
                _inventoryDeleteViewModel.SubDeleteBtnEnable.Value = false;
                _inventoryDeleteViewModel.MinDeleteBtnEnable.Value = false;
                    
                // 启用增加/最大按钮
                _inventoryDeleteViewModel.AddDeleteBtnEnable.Value = true;
                _inventoryDeleteViewModel.MaxDeleteBtnEnable.Value = true;
            }
            // 只有一个物品的情况
            else if(itemData.itemNum == 1 && currentNum == 1)
            {
                _inventoryDeleteViewModel.AddDeleteBtnEnable.Value = false;
                _inventoryDeleteViewModel.SubDeleteBtnEnable.Value = false;
                _inventoryDeleteViewModel.MaxDeleteBtnEnable.Value = false;
                _inventoryDeleteViewModel.MinDeleteBtnEnable.Value = false;
            }
            else
            {
                _inventoryDeleteViewModel.AddDeleteBtnEnable.Value = true;
                _inventoryDeleteViewModel.SubDeleteBtnEnable.Value = true;
                _inventoryDeleteViewModel.MaxDeleteBtnEnable.Value = true;
                _inventoryDeleteViewModel.MinDeleteBtnEnable.Value = true;
            }
        }
        
        private void OnInputFieldValueChangedEvent(string inputFieldName, string inputFieldValue)
        {
            if (inputFieldName == "inputFieldDeleteNum")
            {
                if(_currentItem == null)
                    return;
                
                // 能进入这里的逻辑，一定是可堆叠的物品
                // 转换成功才处理
                if (int.TryParse(inputFieldValue, out var currentNum))
                {
                    if (!ParseDeleteNum(_currentItem, currentNum, out var legalNum)) 
                        return;
                    
                    if (legalNum == _inventoryDeleteViewModel.DeleteNum.Value)
                    {
                        // 强制刷新UI，避免用户非法输入残留不更新
                        _inventoryDeleteViewModel.DeleteNum.ForceNotify();
                    }
                    else
                    {
                        // UI只更新数据变化
                        _inventoryDeleteViewModel.DeleteNum.Value = legalNum;
                    }
                }
                else
                {
                    Logger.LogDebug(ELogTags.Item, $"[{nameof(InventoryDeleteState)}]: Invalid delete num input: {inputFieldValue}");
                }
            }
        }

        private void OnSliderValueChanged(string sliderName, float value)
        {
            if (sliderName == "sliderNum")
            {
                if (ParseDeleteNum(_currentItem, value, out var legalNum))
                    _inventoryDeleteViewModel.DeleteNum.Value = legalNum;
            }
        }

        public void OnBeforeRefreshItem()
        {
            _inventoryController.ExitDeleteState();
        }

        /// <summary>
        /// 解析原始的删除数量，返回可用的删除数量
        /// </summary>
        /// <param name="currentItem"></param>
        /// <param name="rawDelNum"></param>
        /// <param name="legalNum"></param>
        /// <returns></returns>
        private bool ParseDeleteNum(Item currentItem, float rawDelNum, out int legalNum)
        {
            if (currentItem == null)
            {
                legalNum = -1;
                return false;
            }
            
            var itemData = _inventoryManager.GetData(currentItem);
            // 约束范围
            legalNum = (int)Mathf.Clamp(rawDelNum, DefaultDeleteMinNum, itemData.itemNum);
            return true;
        }

        private void ClearState()
        {
            // 移除添加减少按钮点击
            _inventoryController.OnButtonClickEvent -= OnButtonClickEvent;
            _inventoryController.OnInputFieldValueChangedEvent -= OnInputFieldValueChangedEvent;
            _inventoryController.OnSliderValueChangedEvent -= OnSliderValueChanged;
            
            // 清理删除缓存
            _deletedItems.Clear();
            _currentItem = null;
            
            // 重置VM的属性缓存
            _inventoryDeleteViewModel.ResetData();
        }

        public Task Exit()
        {
            ClearState();
            return Task.CompletedTask;
        }
    }
}
