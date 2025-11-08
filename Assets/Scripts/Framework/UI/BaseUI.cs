using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework
{
    /// <summary>
    /// UI基类
    /// 需要自动寻找控件的非面板UI对象都要继承该类
    /// </summary>
    public abstract class BaseUI : UIBehaviour
    {
        //存储所有找到的满足条件的UI控件
        protected Dictionary<string, List<UIBehaviour>> controlDic = new Dictionary<string, List<UIBehaviour>>();
        //存储默认的控件名列表
        private readonly List<string> _defaultControlNameList = new List<string>()
        {
            "Image", "Text (TMP)", "RawImage", "Panel", "Toggle", "Slider", "Scrollbar",
            "Scroll View", "Button", "Dropdown", "InputField (TMP)", "Background", "Checkmark",
            "Label", "Fill", "Handle", "Viewport", "Arrow",
        };

        protected override void Awake()
        {
            FindChildrenControl<Button>();
            FindChildrenControl<Toggle>();
            FindChildrenControl<ToggleGroup>();
            FindChildrenControl<Slider>();
            FindChildrenControl<InputField>();
            FindChildrenControl<ScrollRect>();
            FindChildrenControl<Dropdown>();
            FindChildrenControl<TextMeshProUGUI>();
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
            if (controlDic.ContainsKey(controlName))
            {
                List<UIBehaviour> uiList = controlDic[controlName];
                for (int i = 0; i < uiList.Count; i++)
                {
                    if (uiList[i].GetType() == typeof(T))
                        return uiList[i] as T;
                }
            }
            return null;
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

        /// <summary>
        /// 寻找某种类型子控件
        /// </summary>
        /// <typeparam name="T">控件类型</typeparam>
        private void FindChildrenControl<T>() where T : UIBehaviour
        {
            //获取该面板上所有该类型的控件
            T[] controls = this.GetComponentsInChildren<T>();
            for (int i = 0; i < controls.Length; i++)
            {
                //用临时变量记录控件名，防止闭包影响
                string controlName = controls[i].name;

                //跳过不需要存储的控件、跳过存储过的控件、跳过面板
                if (_defaultControlNameList.Contains(controlName))
                    continue;

                //之前存储过该名称的控件
                if (controlDic.ContainsKey(controlName))
                {
                    //若之前存储的控件和当前不一样，才去存储
                    if (!controlDic[controlName].Contains(controls[i]))
                        controlDic[controlName].Add(controls[i]);
                }
                //第一次存储该名称的控件
                else
                {
                    //存储控件
                    controlDic.Add(controlName, new List<UIBehaviour>() { controls[i] });
                }

                //事件监听
                if (controls[i] is Button button)
                    button.onClick.AddListener(() => { ButtonOnClick(controlName); });
                else if (controls[i] is Slider slider)
                    slider.onValueChanged.AddListener((value) => { SliderValueChanged(controlName, value); });
                else if (controls[i] is Toggle toggle)
                    toggle.onValueChanged.AddListener((isOn) => { ToggleValueChanged(controlName, isOn); });
                else if (controls[i] is InputField inputField)
                    inputField.onValueChanged.AddListener((inputValue) => { InputFieldValueChanged(controlName, inputValue); });
            }
        }

        protected override void OnDestroy()
        {
            controlDic.Clear();
            controlDic = null;
        }
    }
}


