using System;

namespace Core
{
    /// <summary>
    /// 响应式属性
    /// </summary>
    public class ReactiveProperty<T>
    {
        private T _value;
        
        public event Action<T> OnValueChanged;

        public T Value
        {
            get => _value;
            set
            {
                if(Equals(_value, value)) return;
                _value = value;
                OnValueChanged?.Invoke(value);
            }
        }

        public ReactiveProperty(T initialValue = default)
        {
            Value = initialValue;    
        }
    }
}
