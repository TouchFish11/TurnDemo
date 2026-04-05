using System;

namespace Core
{
    /// <summary>
    /// 响应式属性
    /// </summary>
    public class ReactiveProperty<T>
    {
        private T _value;
        private event Action<T> _onValueChanged;

        public T Value
        {
            get => _value;
            set
            {
                if(Equals(_value, value)) return;
                _value = value;
                _onValueChanged?.Invoke(value);
            }
        }

        public void Subscribe(Action<T> onValueChanged)
        {
            _onValueChanged += onValueChanged;
            _onValueChanged?.Invoke(_value);
        }

        public void Unsubscribe(Action<T> onValueChanged)
        {
            _onValueChanged -= onValueChanged;
        }

        public ReactiveProperty(T initialValue = default)
        {
            Value = initialValue;    
        }
    }
}
