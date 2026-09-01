using System;
using Core.DI;
using Core.Exceptions;
using HotUpdate.Base.ECModule;
using HotUpdate.Game.Battle.StatSystem.Formulas;
using HotUpdate.Game.Battle.StatSystem.Modifiers;
using HotUpdate.Game.Battle.StatSystem.Providers;
using UnityEngine;

namespace HotUpdate.Game.Battle.StatSystem
{
    public class StatsComponentCore : ComponentCore<StatsComponent>
    {
        [Inject] private StatFactory _statFactory;
        [Inject] private StatModifierFactory _statModifierFactory;
        
        /// <summary>
        /// 属性配置提供器
        /// </summary>
        public IStatConfigProvider StatSetProvider { get; set; }
        
        /// <summary>
        /// 属性集合对象
        /// </summary>
        public StatSet StatSet { get; set; }

        public void InitStats()
        {
            foreach (var value in Enum.GetValues(typeof(EStatType)))
            {
                var statType = (EStatType)value;
                var stat = new Stat(statType, _statFactory.DefaultBaseValueFormula, _statFactory.DefaultLinearFormula);
                stat.SetSeedValue(StatSetProvider.GetConfigValue(statType));
                StatSet.Stats.Add(statType, stat);
            }

            StatSet.CurrentHp = StatSet.Stats[EStatType.Hp].FinalValue;
        }
        
         /// <summary>
        /// 更新当前生命值
        /// </summary>
        /// <param name="amount"></param>
        public void UpdateHealth(float amount)
        {
            StatSet.CurrentHp = Mathf.Max(StatSet.CurrentHp + amount, 0);
        }
        
        /// <summary>
        /// 更新当前护盾量
        /// </summary>
        /// <param name="amount"></param>
        public void UpdateShield(float amount)
        {
            StatSet.CurrentShield = Mathf.Max(StatSet.CurrentShield + amount, 0);
        }
        
        /// <summary>
        /// 添加指定类型的基础属性修改器
        /// </summary>
        /// <param name="statType"></param>
        /// <param name="modifier"></param>
        /// <exception cref="Exception"></exception>
        public void AddBaseStatModifier(EStatType statType, IStatModifier modifier)
        {
            if (modifier == null)
                throw ExceptionHelper.Throw($"{nameof(ArgumentNullException)}:{nameof(modifier)}");
            
            var stat = StatSet.Stats[statType];
            stat.AddBaseModifier(modifier);
        }

        /// <summary>
        /// 移除指定类型的基础属性修改器
        /// </summary>
        /// <param name="statType"></param>
        /// <param name="modifierId"></param>
        /// <param name="modifier"></param>
        /// <returns></returns>
        public bool RemoveBaseStatModifier(EStatType statType, long modifierId, out IStatModifier modifier)
        {
            var stat = StatSet.Stats[statType];
            var remove = stat.RemoveBaseModifier(modifierId, out modifier);
            return remove;
        }
        
        /// <summary>
        /// 添加指定类型的属性修改器
        /// </summary>
        /// <param name="statType"></param>
        /// <param name="modifier"></param>
        /// <exception cref="Exception"></exception>
        public void AddFinalStatModifier(EStatType statType, IStatModifier modifier)
        {
            if (modifier == null)
                throw ExceptionHelper.Throw($"{nameof(ArgumentNullException)}:{nameof(modifier)}");
            
            var stat = StatSet.Stats[statType];
            stat.AddModifier(modifier);
        }

        /// <summary>
        /// 移除指定类型的属性修改器
        /// </summary>
        /// <param name="statType"></param>
        /// <param name="modifierId"></param>
        /// <param name="modifier"></param>
        /// <returns></returns>
        public bool RemoveFinalStatModifier(EStatType statType, long modifierId, out IStatModifier modifier)
        {
            return StatSet.Stats[statType].RemoveModifier(modifierId, out modifier);
        }

        /// <summary>
        /// 更新指定属性白值的计算逻辑
        /// </summary>
        /// <param name="statType"></param>
        /// <param name="formula"></param>
        public void UpdateBaseStatFormula(EStatType statType, IStatFormula formula)
        {
            StatSet.Stats[statType].SetBaseFormula(formula);
        }
        
        /// <summary>
        /// 更新指定属性的计算逻辑
        /// </summary>
        /// <param name="statType"></param>
        /// <param name="formula"></param>
        public void UpdateStatFormula(EStatType statType, IStatFormula formula)
        {
            StatSet.Stats[statType].SetFormula(formula);
        }

        /// <summary>
        /// 获取指定属性
        /// </summary>
        /// <param name="statType"></param>
        /// <returns></returns>
        public Stat GetStat(EStatType statType)
        {
            return StatSet.Stats[statType];
        }
    
        /// <summary>
        /// 获取指定属性的最终值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public float GetFinalValue(EStatType type) 
        {
            return StatSet.Stats[type].FinalValue;
        }

        protected override void OnDispose()
        {
            _statFactory = null;
            StatSet = null;
            StatSetProvider = null;
        }
    }
}
