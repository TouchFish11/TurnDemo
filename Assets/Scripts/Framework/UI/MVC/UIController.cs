using Framework;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// UI控制器
/// </summary>
public abstract class UIController<TView, TModel> : IUIController where TView : UIView where TModel : UIModel, new()
{
    protected TView view;
    protected TModel model;

    public UIController(TView view, TModel model)
    {
        this.view = view;
        this.model = model;
    }

    public async Task Init()
    {
        (this as IUIController).BindViewEvents();
        (this as IUIController).BindModelEvents();
        await OnInit();

        EventCenter.Instance.TriggerEvent(new OpenViewEvent() { ControllerName = this.ToString() });
    }

    /// <summary>
    /// 初始化逻辑（子类实现）
    /// </summary>
    protected virtual async Task OnInit()
    {
        await Task.CompletedTask;
    }

    /// <summary>
    /// 绑定 View 事件（监听用户操作）
    /// </summary>
    void IUIController.BindViewEvents()
    {
        view.GetBinder().OnButtonClick += ButtonOnClick;
        view.GetBinder().OnSliderValueChanged += SliderValueChanged;
        view.GetBinder().OnToggleValueChanged += ToggleValueChanged;
        view.GetBinder().OnInputFieldValueChanged += InputFieldValueChanged;
    }

    /// <summary>
    /// 绑定 Model 事件（监听数据变更）
    /// </summary>
    void IUIController.BindModelEvents()
    {
        if (model != null)
        {
            model.OnDataChanged += (this as IUIController).OnHandleModelDataChanged;
        }
    }

    /// <summary>
    /// 处理 Model 数据变更 → 驱动 View 更新
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    void IUIController.OnHandleModelDataChanged(string key, object value)
    {
        view.UpdateView(key, value);
    }

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
        model.OnDataChanged -= (this as IUIController).OnHandleModelDataChanged;
        model.ClearData();

        EventCenter.Instance.TriggerEvent(new CloseViewEvent() { ControllerName = this.ToString() });
    }
}
