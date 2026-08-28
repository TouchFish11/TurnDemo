using Core.Pool;

namespace HotUpdate.Game.Battle.Property.New.Test.StatSystem
{
    /// <summary>
    /// 属性修改器
    /// </summary>
    public class StatModifier : IStatModifier, IPoolData
    {
        /// <summary>
        /// 修改器全局ID
        /// </summary>
        private static long s_modifierId;
        
        private readonly float _value;

        public long ModifierId { get; private set; }
        
        public EModifierType ModifierType { get; }

        public StatModifier(EModifierType statsModifierType, float value)
        {
            ModifierId = s_modifierId++;
            ModifierType = statsModifierType;
            _value = value;
        }
            
        public virtual float GetValue(StatEvaluationContext context)
        {
            return _value;
        }

        void IPoolData.ResetData()
        {
            
        }
    }
}
