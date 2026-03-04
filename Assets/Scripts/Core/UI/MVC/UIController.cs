using System.Threading.Tasks;
using Core.GlobalEvent;
using Core.GlobalEvent.Events;
using Core.Loader.Object;
using Core.Pool;
using Core.Service;

namespace Core.UI.MVC
{
    /// <summary>
    /// UI控制器
    /// </summary>
    public abstract class UIController<TView, TModel> : IuiController where TView : IuiView where TModel : IuiModel
    {
        protected TView view;
        protected TModel model;
        
        protected readonly IUIManager uiManager = ServiceLocator.Get<IUIManager>();
        protected readonly IEventCenter eventCenter = ServiceLocator.Get<IEventCenter>();
        protected readonly IPoolManager poolManager = ServiceLocator.Get<IPoolManager>();
        protected readonly IPrefabLoader prefabLoader = ServiceLocator.Get<IPrefabLoader>();

        public async Task Init(IuiView view, IuiModel model)
        {
            this.view = (TView)view;
            this.model = (TModel)model;
            await OnInit();
        }

        /// <summary>
        /// 显示
        /// </summary>
        /// <returns></returns>
        public Task Show()
        {
            view.ViewObj.SetActive(true);
            // 监听鼠标显隐事件
            eventCenter.TriggerEvent(new MouseVisibleChangedEvent
            {
                IsVisible = true,
                SourceName = ToString()
            });

            eventCenter.TriggerEvent(new OpenViewEvent { UIController = this });
            
            // 监听界面UI事件
            view.GetBinder().OnButtonClick += ButtonOnClick;
            view.GetBinder().OnSliderValueChanged += SliderValueChanged;
            view.GetBinder().OnToggleValueChanged += ToggleValueChanged;
            view.GetBinder().OnInputFieldValueChanged += InputFieldValueChanged;
            return OnShow();
        }
        
        /// <summary>
        /// 隐藏
        /// </summary>
        /// <returns></returns>
        public async Task Hide()
        {
            // 注销监听鼠标显隐事件
            eventCenter.TriggerEvent(new MouseVisibleChangedEvent
            {
                IsVisible = false,
                SourceName = ToString()
            });
            
            eventCenter.TriggerEvent(new CloseViewEvent { UIController = this });
            
            // 注销监听界面UI事件
            view.GetBinder().OnButtonClick -= ButtonOnClick;
            view.GetBinder().OnSliderValueChanged -= SliderValueChanged;
            view.GetBinder().OnToggleValueChanged -= ToggleValueChanged;
            view.GetBinder().OnInputFieldValueChanged -= InputFieldValueChanged;
            model.ClearData();
            await OnHide();
            view.ViewObj.SetActive(false);
        }

        /// <summary>
        /// 显示时执行
        /// </summary>
        /// <returns></returns>
        protected abstract Task OnShow();

        /// <summary>
        /// 隐藏时执行
        /// </summary>
        /// <returns></returns>
        protected abstract Task OnHide();
        
        /// <summary>
        /// 初始化逻辑（子类实现）
        /// </summary>
        protected abstract Task OnInit();

        /// <summary>
        /// 按钮点击监听
        /// </summary>
        /// <param name="btnName">按钮名</param>
        protected virtual void ButtonOnClick(string btnName) { }

        /// <summary>
        /// 滑动条滑动监听
        /// </summary>
        /// <param name="sliderName">滑动条名</param>
        /// <param name="value">滑动条值</param>
        protected virtual void SliderValueChanged(string sliderName, float value) { }

        /// <summary>
        /// 开关选中监听
        /// </summary>
        /// <param name="toggleName">选项框名</param>
        /// <param name="isOn">是否选中</param>
        protected virtual void ToggleValueChanged(string toggleName, bool isOn) { }

        /// <summary>
        /// 输入框输入监听
        /// </summary>
        /// <param name="fieldName">输入框名</param>
        /// <param name="inputStr">输入内容</param>
        protected virtual void InputFieldValueChanged(string fieldName, string inputStr) { }

        public async Task Destroy()
        {
            await Hide();
        }
    }
}
