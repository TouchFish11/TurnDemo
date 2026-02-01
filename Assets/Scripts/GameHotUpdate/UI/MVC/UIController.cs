using Core.EventCenter;
using Core.EventCenter.Events;
using Core.Service;
using Core.UI.MVC;

namespace GameHotUpdate.UI.MVC
{
    /// <summary>
    /// UI控制器
    /// </summary>
    public abstract class UIController<TView, TModel> : IuiController where TView : IuiView where TModel : IuiModel
    {
        protected TView view;
        protected TModel model;

        protected UIController()
        {

        }

        public async System.Threading.Tasks.Task Init(IuiView view, IuiModel model)
        {
            this.view = (TView)view;
            this.model = (TModel)model;
            view.Show();
            
            view.GetBinder().OnButtonClick += ButtonOnClick;
            view.GetBinder().OnSliderValueChanged += SliderValueChanged;
            view.GetBinder().OnToggleValueChanged += ToggleValueChanged;
            view.GetBinder().OnInputFieldValueChanged += InputFieldValueChanged;

            ServiceLocator.Get<IEventCenter>().TriggerEvent(new MouseVisibleChangedEvent
            {
                IsVisible = true,
                SourceName = this.ToString()
            });
            
            await OnInit();
        }

        /// <summary>
        /// 初始化逻辑（子类实现）
        /// </summary>
        protected abstract System.Threading.Tasks.Task OnInit();

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

        public virtual void Destroy()
        {
            view.GetBinder().OnButtonClick -= ButtonOnClick;
            view.GetBinder().OnSliderValueChanged -= SliderValueChanged;
            view.GetBinder().OnToggleValueChanged -= ToggleValueChanged;
            view.GetBinder().OnInputFieldValueChanged -= InputFieldValueChanged;
            model.ClearData();

            ServiceLocator.Get<IEventCenter>().TriggerEvent(new MouseVisibleChangedEvent
            {
                IsVisible = false,
                SourceName = this.ToString()
            });
        }
    }
}
