using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.UI
{
    /// <summary>
    /// UIBehaviour基类
    /// 对原生UIBehaviour的封装
    /// </summary>
    public abstract class UIBehaviourBase : UIBehaviour, IUiBehaviour
    {
        protected UIComponentBinder binder;

        protected override void Awake()
        {
            binder = new UIComponentBinder(this);
            binder.OnButtonClick += OnButtonClick;
            binder.OnSliderValueChanged += OnSliderValueChanged;
            binder.OnInputFieldValueChanged += OnInputFieldValueChanged;
            binder.OnToggleValueChanged += OnToggleValueChanged;
            binder.OnDropdownValueChanged += OnDropdownValueChanged;

            ScanFieldAndPropertyInstance();
            ScanTransformInstance();
        }

        /// <summary>
        /// 获取UI控件
        /// </summary>
        /// <returns></returns>
        public T GetControl<T>(string controlName) where T : UIBehaviour
        {
            return binder.GetControl<T>(controlName);
        }

        /// <summary>
        /// 扫描该UI字段和属性实例
        /// </summary>
        private void ScanFieldAndPropertyInstance()
        {
            var type = GetType();
            var memberInfos = type.GetMembers(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var memberInfo in memberInfos)
            {
                var attribute = memberInfo.GetCustomAttribute<InjectAttribute>();
                if (attribute == null)
                {
                    continue;
                }

                switch (memberInfo)
                {
                    case FieldInfo fieldInfo:
                        fieldInfo.SetValue(this, binder.GetControl(fieldInfo.Name, fieldInfo.FieldType));
                        break;
                    case PropertyInfo propertyInfo:
                        propertyInfo.SetValue(this, binder.GetControl(propertyInfo.Name, propertyInfo.PropertyType));
                        break;
                }
            }
        }

        /// <summary>
        /// 扫描该UI变换实例
        /// </summary>
        private void ScanTransformInstance()
        {
            var dic = new Dictionary<string, MemberInfo>();
            var type = GetType();
            var memberInfos = type.GetMembers(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var memberInfo in memberInfos)
            {
                var attribute = memberInfo.GetCustomAttribute<InjectAttribute>();
                if (attribute == null || attribute.RectTransformFlag == 0)
                {
                    continue;
                }
                dic.Add(memberInfo.Name, memberInfo);
            }

            var rectTransforms = new List<RectTransform>(GetComponentsInChildren<RectTransform>());
            foreach (var rectTransform in rectTransforms)
            {
                if (!dic.TryGetValue(rectTransform.name, out var info))
                {
                    continue;
                }

                switch (info)
                {
                    case FieldInfo fieldInfo:
                        fieldInfo.SetValue(this, rectTransform);
                        break;
                    case PropertyInfo propertyInfo:
                        propertyInfo.SetValue(this, rectTransform);
                        break;
                }
            }
        }

        protected virtual void OnButtonClick(string btnName) { }

        protected virtual void OnSliderValueChanged(string sliderName, float value) { }

        protected virtual void OnInputFieldValueChanged(string inputFieldName, string value) { }

        protected virtual void OnToggleValueChanged(string togName, bool isOn) { }
        
        protected virtual void OnDropdownValueChanged(string dropdownName, int value) { }

        protected override void OnDestroy()
        {
            binder.Clear();
        }
    }
}
