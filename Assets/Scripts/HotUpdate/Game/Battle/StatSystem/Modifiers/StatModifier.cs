using System;

namespace HotUpdate.Game.Battle.StatSystem.Modifiers
{
    /// <summary>
    /// 属性修改器
    /// </summary>
    public class StatModifier : IStatModifier
    {
        /// <summary>
        /// 修改器全局ID，从1开始
        /// </summary>
        private static long s_modifierId = 1;
        
        private float _value;

        public long ModifierId { get; }
        
        public EModifierType ModifierType { get; }
        
        public event Action OnChanged;

        public StatModifier(EModifierType modifierType)
        {
            ModifierId = s_modifierId++;
            ModifierType = modifierType;
        }
        
        public void Init(float value)
        {
            SetValue(value);
        }

        public void SetValue(float value)
        {
            _value = value;
            InvokeOnChanged();
        }   
        
        public virtual float GetValue(StatEvaluationContext context)
        {
            return _value;
        }
        
        /// <summary>
        /// 执行内容变化事件
        /// </summary>
        protected void InvokeOnChanged()
        {
            OnChanged?.Invoke();
        }
    }
}
