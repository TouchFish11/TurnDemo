using System.Collections.Generic;
using Core.Log;
using Core.Mono.MonoFunction;
using HotUpdate.Battle.Damage.Strategys;
using HotUpdate.Battle.Event.General;
using HotUpdate.Battle.Utility;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Damage;
using HotUpdate.Core.Battle.Damage.Data;
using HotUpdate.Core.Battle.Object;
using HotUpdate.Core.Battle.Skill;

namespace HotUpdate.Battle.Damage
{
    /// <summary>
    /// 伤害计算管理器
    /// </summary>
    public class DamageCalcManager : IDamageCalcManager, IDestroyable
    {
        // 伤害计算策略缓存
        private readonly Dictionary<E_DamageType, IDamageStrategy> _strategys = new();
        private IBattleContext context;
        
        public DamageCalcManager(IBattleContext context)
        {
            this.context = context;
            // 注册策略
            _strategys.Add(E_DamageType.Direct, new DirectDamageStrategy());
            _strategys.Add(E_DamageType.Dot, new DotDamageStrategy());
            _strategys.Add(E_DamageType.Break, new BreakDamageStrategy());
            _strategys.Add(E_DamageType.True, new TrueDamageStrategy());

            // 监听击破事件
            context.GetEventBus().AddListener<ToughnessBrokenEvent>(OnToughnessBrokenEvent);
            // 监听Dot事件
            context.GetEventBus().AddListener<CalcDotDamageEvent>(OnCalcDotDamageEvent);
        }

        /// <summary>
        /// 计算技能伤害
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="skillInfo"></param>
        /// <param name="damageResult"></param>
        public void CalcSkillDamage(IBattleEntityObject source, IBattleEntityObject target,SkillInfo skillInfo, out DamageResult damageResult)
        {
            var damageType = skillInfo.f_damageType.ToDamageType();
            if (_strategys.TryGetValue(damageType, out var strategy))
            {
                strategy.CalcDamage(source, target, skillInfo, out damageResult);
            }
            else
            {
                damageResult = default;
                LogManager.LogError($"{nameof(DamageCalcManager)}.{nameof(CalcBrokenDamage)}：未注册伤害策略，{damageType}");
            }
        }

        /// <summary>
        /// 击破事件回调
        /// </summary>
        /// <param name="toughnessBrokenEvent"></param>
        private void OnToughnessBrokenEvent(ToughnessBrokenEvent toughnessBrokenEvent)
        {
            CalcBrokenDamage(toughnessBrokenEvent.Breaker, toughnessBrokenEvent.Target, 
                toughnessBrokenEvent.SkillId, toughnessBrokenEvent.ResilienceValue);
        }

        /// <summary>
        /// 计算Dot伤害事件回调
        /// </summary>
        /// <param name="calcDotDamageEvent"></param>
        private void OnCalcDotDamageEvent(CalcDotDamageEvent calcDotDamageEvent)
        {
            CalcDotDamage(calcDotDamageEvent.DotDamageCalcData);
        }

        /// <summary>
        /// 计算击破伤害
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="skillId"></param>
        /// <param name="resilienceValue"></param>
        private void CalcBrokenDamage(IBattleEntityObject source, IBattleEntityObject target, int skillId, int resilienceValue)
        {
            if (_strategys.TryGetValue(E_DamageType.Break, out var strategy))
            {
                // 转换成击破伤害计算策略
                (strategy as BreakDamageStrategy).CalcBreakDamage(source, target, skillId, resilienceValue,
                    out var damageResult);
                target.TakeDamage(damageResult);
            }
            else
            {
                LogManager.LogError($"{nameof(DamageCalcManager)}.{nameof(CalcBrokenDamage)}：未注册击破伤害策略");
            }
        }
        
        /// <summary>
        /// 计算Dot伤害
        /// </summary>
        /// <param name="dotDamageCalcData"></param>
        public void CalcDotDamage(DotDamageCalcData dotDamageCalcData)
        {
            if (_strategys.TryGetValue(E_DamageType.Dot, out var strategy))
            {
                // 转换成击破伤害计算策略
                (strategy as DotDamageStrategy).CalcDotDamage(dotDamageCalcData, out var damageResult);
                dotDamageCalcData.target.TakeDamage(damageResult);
            }
            else
            {
                LogManager.LogError($"{nameof(DamageCalcManager)}.{nameof(CalcBrokenDamage)}：未注册Dot伤害策略");
            }
        }

        public void OnDestroy()
        {
            _strategys.Clear();
            // 监听击破事件
            context.GetEventBus().RemoveListener<ToughnessBrokenEvent>(OnToughnessBrokenEvent);
            // 监听Dot事件
            context.GetEventBus().RemoveListener<CalcDotDamageEvent>(OnCalcDotDamageEvent);
            context = null;
        }
    }
}
