using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Framework
{
    /// <summary>
    /// 基础UI行为类
    /// </summary>
    public abstract class BaseUIBehaviour : UIBehaviour
    {
        protected UIComponentBinder binder;

        protected override void Awake()
        {
            binder = new UIComponentBinder(this);
            binder.OnButtonClick += OnButtonClick;
            binder.OnSliderValueChanged += OnSliderValueChanged;
            binder.OnInputFieldValueChanged += OnInputFieldValueChanged;
            binder.OnToggleValueChanged += OnToggleValueChanged;

            ScanFieldAndPropertyInstance();
            ScanTransformInstance();
        }

        /// <summary>
        /// 扫描字段和属性实例
        /// </summary>
        private void ScanFieldAndPropertyInstance()
        {
            Type type = this.GetType();
            MemberInfo[] memberInfos = type.GetMembers(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (MemberInfo memberInfo in memberInfos)
            {
                InjectAttribute attribute = memberInfo.GetCustomAttribute<InjectAttribute>();
                if (attribute == null)
                {
                    continue;
                }

                if (memberInfo is FieldInfo fieldInfo)
                {
                    fieldInfo.SetValue(this, binder.GetControl(fieldInfo.Name, fieldInfo.FieldType));
                }
                else if (memberInfo is PropertyInfo propertyInfo)
                {
                    propertyInfo.SetValue(this, binder.GetControl(propertyInfo.Name, propertyInfo.PropertyType));
                }
            }
        }

        /// <summary>
        /// 扫描RectTransform属性实例
        /// </summary>
        private void ScanTransformInstance()
        {
            Dictionary<string, MemberInfo> dic = new Dictionary<string, MemberInfo>();
            Type type = this.GetType();
            MemberInfo[] memberInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (MemberInfo memberInfo in memberInfos)
            {
                InjectAttribute attribute = memberInfo.GetCustomAttribute<InjectAttribute>();
                if (attribute == null || attribute.RectTransformFlag == default)
                {
                    continue;
                }

                dic.Add(memberInfo.Name, memberInfo);
            }

            List<RectTransform> rectTransforms = new List<RectTransform>(this.GetComponentsInChildren<RectTransform>());
            foreach (RectTransform transform in rectTransforms)
            {
                if (dic.TryGetValue(transform.name, out var info))
                {
                    if (info is FieldInfo fieldInfo)
                    {
                        fieldInfo.SetValue(this, transform);
                    }
                    else if (info is PropertyInfo propertyInfo)
                    {
                        propertyInfo.SetValue(this, transform);
                    }
                }
            }
        }

        protected virtual void OnButtonClick(string btnName) { }

        protected virtual void OnSliderValueChanged(string sliderName, float value) { }

        protected virtual void OnInputFieldValueChanged(string inputFieldName, string value) { }

        protected virtual void OnToggleValueChanged(string togName, bool isOn) { }

        protected override void OnDestroy()
        {
            binder.Clear();
        }
    }
}
