using System;
using Core.DI;
using Core.Exceptions;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Object.Monster;
using HotUpdate.Game.Battle.Object.Role;
using HotUpdate.Game.Battle.Property.New.Test.StatSystem.Providers;
using HotUpdate.Game.Battle.Property.New.Test.StatSystem.Resources;

namespace HotUpdate.Game.Battle.Property.New.Test.StatSystem
{
    /// <summary>
    /// 属性组件
    /// </summary>
    public class StatsComponent : BattleComponent
    {
        [Inject] private StatFactory _statFactory;
        
        private IStatConfigProvider _statSetProvider;
        // 属性集合对象
        private StatSet _statSet;
        
        /// <summary>
        /// 属性修改器更新事件
        /// </summary>
        public event Action<Stat> OnChange; 
        
        /// <summary>
        /// 护盾更新事件
        /// </summary>
        public event Action<float> OnShieldChange;
        
        /// <summary>
        /// 终结技资源更新事件（curt, max）
        /// </summary>
        public event Action<float, float> OnResourceChange; 

        protected override void OnBattleInit()
        {
            switch (BattleEntity)
            {
                case PlayerObject playerObject:
                    _statSet = new RoleStatSet();
                    _statSetProvider = new RoleStatConfigProvider(playerObject.RoleInfo);
                    ((RoleStatSet)_statSet).UltimateResource = new EnergyResource(playerObject.RoleInfo.f_maxEnergy, 0);
                    break;
                case MonsterObject monsterObject:
                    _statSet = new MonsterStatSet();
                    _statSetProvider = new MonsterStatConfigProvider(monsterObject.MonsterInfo);
                    break;
                default:
                    throw ExceptionHelper.Throw($"{nameof(BattleEntity)}:{BattleEntity.GetType().Name}");
            }
            
            foreach (var value in Enum.GetValues(typeof(EStatType)))
            {
                var statType = (EStatType)value;
                var stat = new Stat(statType, _statFactory.DefaultBaseValueFormula, _statFactory.DefaultLinearFormula);
                stat.AddBaseModifier(new StatModifier(EModifierType.Flat, _statSetProvider.GetConfigValue(statType)));
                _statSet.Stats.Add(statType, stat);
            }
        }

        /// <summary>
        /// 更新当前护盾量
        /// </summary>
        /// <param name="amount"></param>
        public void UpdateShield(float amount)
        {
            _statSet.CurrentShiled += amount;   
            OnShieldChange?.Invoke(_statSet.CurrentShiled);
        }

        /// <summary>
        /// 更新终结技资源
        /// </summary>
        /// <param name="amount"></param>
        public void UpdateResource(float amount)
        {
            var ultimateResources = ((RoleStatSet)_statSet).UltimateResource;
            ultimateResources.Gain(amount);
            OnResourceChange?.Invoke(ultimateResources.CurrentValue, ultimateResources.MaxValue);
        }
        
        /// <summary>
        /// 添加指定类型的基础属性修改器
        /// </summary>
        /// <param name="statType"></param>
        /// <param name="modifier"></param>
        /// <exception cref="Exception"></exception>
        public void AddBaseStat(EStatType statType, IStatModifier modifier)
        {
            if (modifier == null)
                throw ExceptionHelper.Throw($"{nameof(ArgumentNullException)}:{nameof(modifier)}");
            
            var stat = _statSet.Stats[statType];
            stat.AddBaseModifier(modifier);
            OnChange?.Invoke(stat);
        }

        /// <summary>
        /// 移除指定类型的基础属性修改器
        /// </summary>
        /// <param name="statType"></param>
        /// <param name="modifierId"></param>
        /// <param name="modifier"></param>
        /// <returns></returns>
        public bool RemoveBaseStat(EStatType statType, long modifierId, out IStatModifier modifier)
        {
            var stat = _statSet.Stats[statType];
            var remove = stat.RemoveBaseModifier(modifierId, out modifier);
            OnChange?.Invoke(stat);
            return remove;
        }
        
        /// <summary>
        /// 添加指定类型的属性修改器
        /// </summary>
        /// <param name="statType"></param>
        /// <param name="modifier"></param>
        /// <exception cref="Exception"></exception>
        public void AddFinalStat(EStatType statType, IStatModifier modifier)
        {
            if (modifier == null)
                throw ExceptionHelper.Throw($"{nameof(ArgumentNullException)}:{nameof(modifier)}");
            
            var stat = _statSet.Stats[statType];
            stat.AddModifier(modifier);
            OnChange?.Invoke(stat);
        }

        /// <summary>
        /// 移除指定类型的属性修改器
        /// </summary>
        /// <param name="statType"></param>
        /// <param name="modifierId"></param>
        /// <param name="modifier"></param>
        /// <returns></returns>
        public bool RemoveFinalStat(EStatType statType, long modifierId, out IStatModifier modifier)
        {
            var stat = _statSet.Stats[statType];
            var remove = _statSet.Stats[statType].RemoveModifier(modifierId, out modifier);
            OnChange?.Invoke(stat);
            return remove;
        }

        /// <summary>
        /// 更新指定属性白值的计算逻辑
        /// </summary>
        /// <param name="statType"></param>
        /// <param name="formula"></param>
        public void UpdateBaseStatFormula(EStatType statType, IStatFormula formula)
        {
            _statSet.Stats[statType].SetBaseFormula(formula);
        }
        
        /// <summary>
        /// 更新指定属性的计算逻辑
        /// </summary>
        /// <param name="statType"></param>
        /// <param name="formula"></param>
        public void UpdateStatFormula(EStatType statType, IStatFormula formula)
        {
            _statSet.Stats[statType].SetFormula(formula);
        }

        /// <summary>
        /// 获取指定属性
        /// </summary>
        /// <param name="statType"></param>
        /// <returns></returns>
        public Stat GetStat(EStatType statType)
        {
            return _statSet.Stats[statType];
        }
    
        /// <summary>
        /// 获取指定属性的最终值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public float GetFinalValue(EStatType type) 
        {
            return _statSet.Stats[type].FinalValue;
        }

        protected override void OnBattleDestroy()
        {
            _statFactory = null;
            _statSet = null;
            _statSetProvider = null;
            OnChange = null;
            OnShieldChange = null;
            OnResourceChange = null;
        }
    }
}
