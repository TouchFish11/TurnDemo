using System;
using Core.Exceptions;
using HotUpdate.Base.ECModule;
using HotUpdate.Game.Battle.Core;
using HotUpdate.Game.Battle.Event.General;
using HotUpdate.Game.Battle.Property.New.Test.StatSystem;

namespace HotUpdate.Game.Battle.Property
{
    /// <summary>
    /// 属性组件抽象类
    /// 封装战斗实体的核心属性管理逻辑，包括属性读写、加成配置、属性变更事件触发等
    /// 所有具体的战斗属性组件需继承此类实现
    /// </summary>
    [ComponentCore(typeof(StatsComponentCore))]
    public abstract class StatsComponent : BattleComponent
    {
        protected StatsComponentCore StatsComponentCore { get; private set; }
        
        /// <summary>
        /// 当前生命值
        /// </summary>
        public float CurrentHp => StatsComponentCore.StatSet.CurrentHp;
        
        /// <summary>
        /// 当前护盾值
        /// </summary>
        public float CurrentShiled => StatsComponentCore.StatSet.CurrentShiled;
        
        protected override void OnBattleInit()
        {
            StatsComponentCore = (StatsComponentCore)ComponentCore;
            StatsComponentCore.InitStats();
        }

        /// <summary>
        /// 更新当前生命值
        /// </summary>
        /// <param name="amount">变化量</param>
        public void UpdateHealth(float amount)
        {
            StatsComponentCore.UpdateHealth(amount);
            BattleEntity.Context.EventBus.TriggerEvent(new CurrentHpChangedEvent(BattleEntity.Context, StatsComponentCore.StatSet.CurrentHp, StatsComponentCore.StatSet.Stats[EStatType.Hp].FinalValue, BattleEntity));
        }
        
        /// <summary>
        /// 更新当前护盾量
        /// </summary>
        /// <param name="amount"></param>
        public void UpdateShield(float amount)
        {
            StatsComponentCore.UpdateShield(amount); 
            BattleEntity.Context.EventBus.TriggerEvent(new ShieldChangedEvent(BattleEntity.Context, StatsComponentCore.StatSet.CurrentShiled, BattleEntity, amount));
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

            StatsComponentCore.AddBaseStat(statType, modifier);
            BattleEntity.Context.EventBus.TriggerEvent(new StatChangedEvent(BattleEntity.Context, GetStat(statType)));
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
            var remove = StatsComponentCore.RemoveBaseStat(statType, modifierId, out modifier);
            BattleEntity.Context.EventBus.TriggerEvent(new StatChangedEvent(BattleEntity.Context, GetStat(statType)));
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

            StatsComponentCore.AddFinalStat(statType, modifier);
            BattleEntity.Context.EventBus.TriggerEvent(new StatChangedEvent(BattleEntity.Context, GetStat(statType)));
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
            var remove = StatsComponentCore.RemoveFinalStat(statType, modifierId, out modifier);
            BattleEntity.Context.EventBus.TriggerEvent(new StatChangedEvent(BattleEntity.Context, GetStat(statType)));
            return remove;
        }

        /// <summary>
        /// 更新指定属性白值的计算逻辑
        /// </summary>
        /// <param name="statType"></param>
        /// <param name="formula"></param>
        public void UpdateBaseStatFormula(EStatType statType, IStatFormula formula)
        {
            StatsComponentCore.UpdateBaseStatFormula(statType, formula);
        }
        
        /// <summary>
        /// 更新指定属性的计算逻辑
        /// </summary>
        /// <param name="statType"></param>
        /// <param name="formula"></param>
        public void UpdateStatFormula(EStatType statType, IStatFormula formula)
        {
            StatsComponentCore.UpdateStatFormula(statType, formula);
        }

        /// <summary>
        /// 获取指定属性对象
        /// </summary>
        /// <param name="statType"></param>
        /// <returns></returns>
        public Stat GetStat(EStatType statType)
        {
            return StatsComponentCore.GetStat(statType);
        }
        
        /// <summary>
        /// 获取指定属性的最终值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public float GetFinalValue(EStatType type) 
        {
            return StatsComponentCore.GetFinalValue(type);
        }
    }
}