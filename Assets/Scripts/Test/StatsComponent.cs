using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    /// <summary>
    /// 属性组件
    /// </summary>
    public class StatsComponent : MonoBehaviour
    {
        private readonly Dictionary<EStatType, Stat> _stats = new();
        private readonly Dictionary<EStatType, List<EStatType>> _dependencies = new(); // 依赖图
        private StatsModifier _statsmodifier;
    
        public void Init(StatsModifier statsModifier) 
        {
            _statsmodifier = statsModifier;

            _stats.Add(EStatType.Hp, new Stat());
            _stats.Add(EStatType.Atk, new Stat());
            _stats.Add(EStatType.Def, new Stat());
            
            SetBaseValueChanged(EStatType.Hp, 100);
            SetBaseValueChanged(EStatType.Atk, 100);
            SetBaseValueChanged(EStatType.Def, 100);
            
            // 注册依赖关系：力量 -> 攻击力
            // AddDependency(EStatType.Strength, EStatType.Attack);
            // AddDependency(EStatType.Agility, EStatType.Attack);
            // AddDependency(EStatType.Attack, EStatType.SkillDamage);
        }
    
        private void AddDependency(EStatType from, EStatType to) 
        {
            if (!_dependencies.ContainsKey(from))
                _dependencies[from] = new List<EStatType>();
            _dependencies[from].Add(to);
        }

        public Stat GetStat(EStatType statType)
        {
            return _stats[statType];
        }
    
        /// <summary>
        /// 获取最终值，调用后才去计算最终值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public float GetFinalValue(EStatType type) 
        {
            return _stats[type].FinalValue;
        }
    
        /// <summary>
        /// 设置属性类型的新基础值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="newBaseValue"></param>
        public void SetBaseValueChanged(EStatType type, float newBaseValue) 
        {
            _stats[type].BaseValue = newBaseValue;
            Propagate(type);
        }
    
        /// <summary>
        /// 当加成变化时由管理器调用
        /// </summary>
        /// <param name="type"></param>
        public void OnModifiersChanged(EStatType type) 
        {
            Propagate(type);
        }
    
        /// <summary>
        /// 传播到依赖此属性的其他属性，若属性之间存在循环依赖，则会造成死循环
        /// </summary>
        /// <param name="type"></param>
        private void Propagate(EStatType type) 
        {
            _stats[type].UpdateValue(_statsmodifier.GetBuildBonus(type), _statsmodifier.GetPercentBonus(type));
            // 传播到依赖此属性的其他属性
            if (_dependencies.TryGetValue(type, out var dependents)) 
            {
                foreach (var dependent in dependents) 
                {
                    Propagate(dependent);
                }
            }
        }
    }
}
