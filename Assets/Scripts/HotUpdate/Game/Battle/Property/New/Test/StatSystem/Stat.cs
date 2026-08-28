using System.Collections.Generic;

namespace HotUpdate.Game.Battle.Property.New.Test.StatSystem
{
    /// <summary>
    /// 属性
    /// </summary>
    public class Stat
    {
        private float _baseValue;   // 白值
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
        /// 基础属性值（白值）
        /// </summary>
        public float BaseValue
        {
            get
            {
                if(!_baseValueDirty)
                    return _baseValue;
                _baseValueDirty = false;
                
                return _baseValueformula.Calculate(new StatEvaluationContext(0, _baseValuemodifiers.Values));
            }
        }

        /// <summary>
        /// 最终属性值
        /// </summary>
        public float FinalValue
        {
            get
            {
                if(!_finalValueDirty)
                    return _finalValue;
                _finalValueDirty = false;
                return _finalValueformula.Calculate(new StatEvaluationContext(BaseValue, _finalValuemodifiers.Values));
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
            _baseValuemodifiers.Add(modifier.ModifierId, modifier);
        }
        
        public bool RemoveBaseModifier(long modifierId, out IStatModifier modifier)
        {
            _baseValueDirty = true;
            _finalValueDirty = true;
            return _baseValuemodifiers.Remove(modifierId, out modifier);
        }
        
        public void AddModifier(IStatModifier modifier)
        {
            _finalValueDirty = true;
            _finalValuemodifiers.Add(modifier.ModifierId, modifier);
        }

        public bool RemoveModifier(long modifierId, out IStatModifier modifier)
        {
            _finalValueDirty = true;
            return _finalValuemodifiers.Remove(modifierId, out modifier);
        }
    }
}
