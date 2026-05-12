using System.Collections.Generic;
using System.Reflection;
using Core.DI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.UI
{
    /// <summary>
    /// UIBehaviour基类
    /// 对原生UIBehaviour的封装
    /// </summary>
    public abstract class UIBehaviourBase : UIBehaviour, IUiBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerMoveHandler,
        IPointerDownHandler, IPointerUpHandler
    {
        // UI组件绑定器
        protected UIComponentBinder binder;

        protected override void Awake()
        {
            binder = DIContainer.Create<UIComponentBinder>(parameterValues: this);
            binder.OnButtonClick += OnButtonClick;
            binder.OnSliderValueChanged += OnSliderValueChanged;
            binder.OnInputFieldValueChanged += OnInputFieldValueChanged;
            binder.OnToggleValueChanged += OnToggleValueChanged;
            binder.OnScrollRectValueChanged += OnScrollRectValueChanged;
            binder.OnDropdownValueChanged += OnDropdownValueChanged;
            
            ScanFieldAndPropertyInstance();
            ScanTransformInstance();
            
            DIContainer.InjectIntoInstance(this);
        }
        
        /// <summary>
        /// 获取UI控件
        /// </summary>
        /// <returns></returns>
        public T GetControl<T>(string controlName) where T : UIBehaviour
        {
            return binder.GetControl<T>(controlName);
        }

        protected virtual void OnButtonClick(string btnName) { }

        protected virtual void OnSliderValueChanged(string sliderName, float value) { }

        protected virtual void OnInputFieldValueChanged(string inputFieldName, string value) { }

        protected virtual void OnToggleValueChanged(string togName, bool isOn) { }

        protected virtual void OnScrollRectValueChanged(string svName, Vector2 pos) { }
        
        protected virtual void OnDropdownValueChanged(string ddName, int index) { }
        
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnter(eventData);
        }

        protected virtual void OnPointerEnter(PointerEventData eventData){ }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            OnPointerExit(eventData);
        }
        
        protected virtual void OnPointerExit(PointerEventData eventData){ }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            OnPointerClick(eventData);
        }
        
        protected virtual void OnPointerClick(PointerEventData eventData){ }

        void IPointerMoveHandler.OnPointerMove(PointerEventData eventData)
        {
            OnPointerMove(eventData);
        }
        
        protected virtual void OnPointerMove(PointerEventData eventData){ }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            OnPointerDown(eventData);
        }
        
        protected virtual void OnPointerDown(PointerEventData eventData){ }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            OnPointerUp(eventData);
        }
        
        protected virtual void OnPointerUp(PointerEventData eventData){ }
        
        protected override void OnDestroy()
        {
            binder.Clear();
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
                var attribute = memberInfo.GetCustomAttribute<InjectUIAttribute>();
                if (attribute == null)
                    continue;

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
                var attribute = memberInfo.GetCustomAttribute<InjectUIAttribute>();
                if (attribute == null || attribute.RectTransformFlag == 0)
                    continue;
                dic.Add(memberInfo.Name, memberInfo);
            }

            var rectTransforms = new List<RectTransform>(GetComponentsInChildren<RectTransform>());
            foreach (var rectTransform in rectTransforms)
            {
                if (!dic.TryGetValue(rectTransform.name, out var info))
                    continue;

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
    }
}
