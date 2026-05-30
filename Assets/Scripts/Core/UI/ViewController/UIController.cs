using System;
using System.Threading.Tasks;
using Core.GlobalEvent;
using Core.GlobalEvent.Events;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace Core.UI.ViewController
{
    /// <summary>
    /// UI控制器
    /// </summary>
    public abstract class UIController<TView> : IuiController where TView : UIView
    {
        [DI.Inject] protected IUIManager uiManager;
        [DI.Inject] protected IEventCenter eventCenter;

        // 控制器界面状态
        private EControllerState _controllerState;
        // 控制器（界面）唯一ID
        public int panelId;
        protected TView view;

        public int PanelId => panelId;
        
        /// <summary>
        /// 界面打开时光标是否可见
        /// </summary>
        protected virtual bool IsCursorVisible { get; set; }

        public async Task Init(int id, IuiView view)
        {
            _controllerState = EControllerState.Initializing;
            panelId = id;
            this.view = (TView)view;
            await OnInit();
            await Activate();
        }

        /// <summary>
        /// 激活
        /// </summary>
        /// <returns></returns>
        public async Task Activate()
        {
            // 正在激活
            _controllerState = EControllerState.Activating;
            view.ViewObj.SetActive(true);
            
            // 监听鼠标显隐事件
            if(IsCursorVisible)
                eventCenter.TriggerEvent(new MouseVisibleChangedEvent { IsVisible = true, SourceName = ToString() });
            // 触发界面打开事件
            eventCenter.TriggerEvent(new OpenViewEvent { UIController = this });
            
            // 监听界面UI事件
            view.GetBinder().OnButtonClick += ButtonOnClick;
            view.GetBinder().OnSliderValueChanged += SliderValueChanged;
            view.GetBinder().OnToggleValueChanged += ToggleValueChanged;
            view.GetBinder().OnInputFieldValueChanged += InputFieldValueChanged;
            view.GetBinder().OnScrollRectValueChanged += ScrollRectValueChanged;
            view.GetBinder().OnDropdownValueChanged += DropdownValueChanged;
            // 界面显示时执行
            await OnActive();
            // 先执行显示逻辑，再改变界面状态标识，激活完成可用
            _controllerState = EControllerState.Ready;
        }
        
        /// <summary>
        /// 失活
        /// </summary>
        /// <returns></returns>
        public async Task InActivate()
        {
            // 先改变界面状态标识，再执行失活逻辑
            _controllerState = EControllerState.InActivating;
            // 注销监听鼠标显隐事件
            if(IsCursorVisible)
                eventCenter.TriggerEvent(new MouseVisibleChangedEvent { IsVisible = false, SourceName = ToString() });
            // 触发界面关闭事件
            eventCenter.TriggerEvent(new CloseViewEvent { UIController = this });
            
            // 注销监听界面UI事件
            view.GetBinder().OnButtonClick -= ButtonOnClick;
            view.GetBinder().OnSliderValueChanged -= SliderValueChanged;
            view.GetBinder().OnToggleValueChanged -= ToggleValueChanged;
            view.GetBinder().OnInputFieldValueChanged -= InputFieldValueChanged;
            view.GetBinder().OnScrollRectValueChanged -= ScrollRectValueChanged;
            view.GetBinder().OnDropdownValueChanged -= DropdownValueChanged;
            await OnInactivate();
            view.ViewObj.SetActive(false);
        }

        /// <summary>
        /// 初始化逻辑，仅当界面被创建时执行一次
        /// 可执行界面首次创建初始化操作
        /// </summary>
        protected abstract Task OnInit();
        
        /// <summary>
        /// 当界面被激活（显示）时执行，在这里执行界面初始化操作，每次显示时都会执行（若未被销毁）
        /// </summary>
        /// <returns></returns>
        protected abstract Task OnActive();

        /// <summary>
        /// 当界面被失活（隐藏）时执行，可以在此执行界面清理操作
        /// </summary>
        /// <returns></returns>
        protected abstract Task OnInactivate();

        /// <summary>
        /// 能否触发UI事件，只当界面准备就绪时才能触发
        /// </summary>
        /// <returns></returns>
        private bool CanTriggerEvent()
        {
            return _controllerState == EControllerState.Ready;
        }

        /// <summary>
        /// 按钮点击监听
        /// </summary>
        private void ButtonOnClick(string btnName)
        {
            if(!CanTriggerEvent())
                return;
            OnButtonClick(btnName);
        }
        
        /// <summary>
        /// 按钮点击时触发
        /// </summary>
        /// <param name="btnName">按钮名</param>
        protected virtual void OnButtonClick(string btnName){ }

        /// <summary>
        /// 滑动条滑动监听
        /// </summary>
        private void SliderValueChanged(string sliderName, float value)
        {
            if(!CanTriggerEvent())
                return;
            OnSliderValueChanged(sliderName, value);
        }
        
        /// <summary>
        /// 滑动条滑动时触发
        /// </summary>
        /// <param name="sliderName">滑动条名</param>
        /// <param name="value">滑动条值</param>
        protected virtual void OnSliderValueChanged(string sliderName, float value){ }

        /// <summary>
        /// 开关选中监听
        /// </summary>
        private void ToggleValueChanged(string toggleName, bool isOn)
        {
            if(!CanTriggerEvent())
                return;
            OnToggleValueChanged(toggleName, isOn);
        }
        
        /// <summary>
        /// 开关选中监听
        /// </summary>
        /// <param name="toggleName">选项框名</param>
        /// <param name="isOn">是否选中</param>
        protected virtual void OnToggleValueChanged(string toggleName, bool isOn) { }

        /// <summary>
        /// 输入框输入监听
        /// </summary>
        private void InputFieldValueChanged(string fieldName, string inputStr)
        {
            if(!CanTriggerEvent())
                return;
            OnInputFieldValueChanged(fieldName, inputStr);
        }
        
        /// <summary>
        /// 输入框输入监听
        /// </summary>
        /// <param name="fieldName">输入框名</param>
        /// <param name="inputStr">输入内容</param>
        protected virtual void OnInputFieldValueChanged(string fieldName, string inputStr) { }

        /// <summary>
        /// 滚动视图滚动监听
        /// </summary>
        private void ScrollRectValueChanged(string scrollViewName, Vector2 pos)
        {
            if(!CanTriggerEvent())
                return;
            OnScrollRectValueChanged(scrollViewName, pos);
        }
        
        /// <summary>
        /// 滚动视图滚动监听
        /// </summary>
        /// <param name="scrollViewName">ScrollRect的名称</param>
        /// <param name="pos">滚动区域的归一化位置（Normalized Position）</param>
        protected virtual void OnScrollRectValueChanged(string scrollViewName, Vector2 pos) { }

        /// <summary>
        /// 下拉菜单监听
        /// </summary>
        private void DropdownValueChanged(string dropdownName, int index)
        {
            if(!CanTriggerEvent())
                return;
            OnDropdownValueChanged(dropdownName, index);
        }
        
        /// <summary>
        /// 下拉菜单监听
        /// </summary>
        /// <param name="dropdownName">下拉菜单名</param>
        /// <param name="index">选中的索引</param>
        protected virtual void OnDropdownValueChanged(string dropdownName, int index) { }

        public async Task Destroy()
        {
            await InActivate();
            // 界面被销毁
            _controllerState = EControllerState.Destroyed;
            view.Destroy();
            await OnDestroy();
        }
        
        protected virtual Task OnDestroy()
        {
            return Task.CompletedTask;
        }
    }
}
