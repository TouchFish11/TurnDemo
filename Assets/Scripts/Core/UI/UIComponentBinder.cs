using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.UI
{
    /// <summary>
    /// UI组件绑定器
    /// </summary>
    public class UIComponentBinder
    {
        private readonly UIBehaviour _componentBehaviour;
        // 存储所有找到的满足条件的UI控件
        private Dictionary<string, List<UIBehaviour>> _controls = new();
        // 存储默认的控件名列表
        private readonly List<string> _defaultControlNames = new()
        {
            "Image", "Text (TMP)", "RawImage", "View", "Toggle", "Slider", "Scrollbar",
            "Scroll View", "Button", "Dropdown", "InputField (TMP)", "Background", "Checkmark",
            "Label", "Fill", "Handle", "Viewport", "Arrow",
        };

        /// <summary>
        /// 按钮点击事件
        /// </summary>
        internal event Action<string> OnButtonClick;

        /// <summary>
        /// 滑动条拖动事件
        /// </summary>
        internal event Action<string, float> OnSliderValueChanged;

        /// <summary>
        /// 开关值变化事件
        /// </summary>
        internal event Action<string, bool> OnToggleValueChanged;

        /// <summary>
        /// 输入值变化事件
        /// </summary>
        internal event Action<string, string> OnInputFieldValueChanged;
        
        /// <summary>
        /// 滚动列表滚动事件
        /// </summary>
        internal event Action<string, Vector2> OnScrollRectValueChanged;
        
        /// <summary>
        /// 下拉菜单选择事件
        /// </summary>
        internal event Action<string, int> OnDropdownValueChanged; 

        internal UIComponentBinder(UIBehaviour uIBehaviour)
        {
            _componentBehaviour = uIBehaviour;

            FindChildrenControl<Button>();
            FindChildrenControl<Toggle>();
            FindChildrenControl<ToggleGroup>();
            FindChildrenControl<Slider>();
            FindChildrenControl<TMP_InputField>();
            FindChildrenControl<ScrollRect>();
            FindChildrenControl<TMP_Dropdown>();
            FindChildrenControl<Dropdown>();
            FindChildrenControl<TextMeshProUGUI>();
            FindChildrenControl<VerticalLayoutGroup>();
            FindChildrenControl<HorizontalLayoutGroup>();
            FindChildrenControl<GridLayoutGroup>();
            FindChildrenControl<RawImage>();
            FindChildrenControl<Text>();
            FindChildrenControl<Image>();
        }

        /// <summary>
        /// 获取控件
        /// </summary>
        /// <typeparam name="T">控件类型</typeparam>
        /// <param name="controlName">控件名</param>
        /// <returns></returns>
        public T GetControl<T>(string controlName) where T : UIBehaviour
        {
            if (!_controls.TryGetValue(controlName, out var uiList))
            {
                return null;
            }
            
            foreach (var ui in uiList)
            {
                if (ui.GetType() == typeof(T))
                {
                    return ui as T;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取控件
        /// </summary>
        /// <param name="controlName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object GetControl(string controlName, Type type)
        {
            if (!_controls.TryGetValue(controlName, out var uiList))
            {
                return null;
            }
            
            foreach (var ui in uiList)
            {
                if (ui.GetType() == type)
                {
                    return ui;
                }
            }
            return null;
        }

        /// <summary>
        /// 寻找某种类型子控件
        /// </summary>
        /// <typeparam name="T">控件类型</typeparam>
        private void FindChildrenControl<T>() where T : UIBehaviour
        {
            // 获取该面板上所有该类型的控件
            var controls = _componentBehaviour.GetComponentsInChildren<T>();
            foreach (var control in controls)
            {
                // 用临时变量记录控件名，防止闭包影响
                var controlName = control.name;
                // 跳过不需要存储的控件、跳过存储过的控件、跳过面板
                if (_defaultControlNames.Contains(controlName))
                    continue;

                // 之前存储过该名称的控件
                if (_controls.ContainsKey(controlName))
                {
                    // 若之前存储的控件和当前不一样，才去存储
                    if (!_controls[controlName].Contains(control))
                        _controls[controlName].Add(control);
                }
                // 第一次存储该名称的控件
                else
                {
                    // 存储控件
                    _controls.Add(controlName, new List<UIBehaviour> { control });
                }

                switch (control)
                {
                    // 事件监听
                    case Button button:
                        button.onClick.AddListener(() => { OnButtonClick?.Invoke(controlName); });
                        break;
                    case Slider slider:
                        slider.onValueChanged.AddListener(value => { OnSliderValueChanged?.Invoke(controlName, value); });
                        break;
                    case Toggle toggle:
                        toggle.onValueChanged.AddListener(isOn => { OnToggleValueChanged?.Invoke(controlName, isOn); });
                        break;
                    case TMP_InputField tMP_InputField:
                        tMP_InputField.onValueChanged.AddListener(inputValue => { OnInputFieldValueChanged?.Invoke(controlName, inputValue); });
                        break;
                    case ScrollRect scrollRect:
                        scrollRect.onValueChanged.AddListener(posValues => OnScrollRectValueChanged?.Invoke(controlName, posValues));
                        break;
                    case TMP_Dropdown tmpDropdown:
                        tmpDropdown.onValueChanged.AddListener(indexValue => { OnDropdownValueChanged?.Invoke(controlName, indexValue); });
                        break;
                }
            }
        }

        /// <summary>
        /// 清空
        /// </summary>
        internal void Clear()
        {
            _controls.Clear();
            _controls = null;
        }
    }
}
