using System.Collections.Generic;
using HotUpdate.Game.Battle.StatSystem.Formulas;
using HotUpdate.Game.Battle.StatSystem.Modifiers;

namespace HotUpdate.Game.Battle.StatSystem
{
    /// <summary>
    /// 属性
    /// </summary>
    public class Stat : IReadOnlyStat
    {
        private float _seedValue;  // 配置底值
        private float _baseValue;   // 实际白值
        private float _finalValue;  // 最终值
        private IStatFormula _baseValueformula;     // 白值计算公式
        private IStatFormula _finalValueformula;    // 最终值计算公式
        private bool _baseValueDirty;   // 白值是否为脏
        private bool _finalValueDirty;  // 最终值是否为脏
        private readonly Dictionary<long, IStatModifier> _baseValuemodifiers = new();   // 当前白值修改器
        private readonly Dictionary<long, IStatModifier> _finalValuemodifiers = new();  // 当最终值修改器

        /// <summary>
        /// 属性类型
        /// </summary>
        public EStatType StatType { get; }

        /// <summary>
        /// 实际基础属性值（白值），根据<see cref="_seedValue"/>和白值修改器计算后的结果
        /// </summary>
        public float BaseValue
        {
            get
            {
                if(!_baseValueDirty)
                    return _baseValue;
                _baseValueDirty = false;
                _baseValue = _baseValueformula.Calculate(new StatEvaluationContext(_seedValue, _baseValue, _baseValuemodifiers.Values));
                return _baseValue;
            }
        }

        /// <summary>
        /// 最终属性值，根据<see cref="BaseValue"/>和修改器计算后的结果
        /// </summary>
        public float FinalValue
        {
            get
            {
                if(!_finalValueDirty)
                    return _finalValue;
                _finalValueDirty = false;
                _finalValue = _finalValueformula.Calculate(new StatEvaluationContext(_seedValue, BaseValue, _finalValuemodifiers.Values));
                return _finalValue;
            }
        }

        public Stat(EStatType statType, IStatFormula baseFormula, IStatFormula statFormula)
        {
            StatType = statType;
            _baseValueformula = baseFormula;
            _finalValueformula = statFormula;
            _baseValueDirty = true;
            _finalValueDirty = true;
        }

        /// <summary>
        /// 设置基础值
        /// </summary>
        /// <param name="seed">配置数据</param>
        public void SetBaseValue(float seed)
        {
            _baseValue = seed;
        }
        
        public void SetBaseFormula(IStatFormula baseFormula)
        {
            _baseValueDirty = true;
            _finalValueDirty = true;
            _baseValueformula = baseFormula;
        }
        
        public void SetFormula(IStatFormula statFormula)
        {
            _finalValueDirty = true;
            _finalValueformula = statFormula;
        }

        public void AddBaseModifier(IStatModifier modifier)
        {
            _baseValueDirty = true;
            _finalValueDirty = true;
            modifier.OnChanged += OnBaseModifierChanged;
            _baseValuemodifiers.Add(modifier.ModifierId, modifier);
        }
        
        public bool RemoveBaseModifier(long modifierId, out IStatModifier modifier)
        {
            _baseValueDirty = true;
            _finalValueDirty = true;
            var remove = _baseValuemodifiers.Remove(modifierId, out modifier);
            modifier.OnChanged -= OnBaseModifierChanged;
            return remove;
        }
        
        public void AddModifier(IStatModifier modifier)
        {
            _finalValueDirty = true;
            modifier.OnChanged += OnModifierChanged;
            _finalValuemodifiers.Add(modifier.ModifierId, modifier);
        }

        public bool RemoveModifier(long modifierId, out IStatModifier modifier)
        {
            _finalValueDirty = true;
            var remove = _finalValuemodifiers.Remove(modifierId, out modifier);
            modifier.OnChanged -= OnModifierChanged;
            return remove;
        }

        public bool TryGetModifier(long id, out IStatModifier modifier)
        {
            return _finalValuemodifiers.TryGetValue(id, out modifier);
        }

        public bool TryGetBaseModifier(long id, out IStatModifier modifier)
        {
            return _baseValuemodifiers.TryGetValue(id, out modifier);
        }

        private void OnBaseModifierChanged()
        {
            _baseValueDirty = true;
        }
        
        private void OnModifierChanged()
        {
            _finalValueDirty = true;
        }
    }
}
