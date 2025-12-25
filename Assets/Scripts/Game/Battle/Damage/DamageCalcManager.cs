using Framework;
using Game.Battle;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 伤害计算管理器
/// </summary>
public class DamageCalcManager : SingletonBase<DamageCalcManager>
{
    //策略字典
    private readonly Dictionary<E_DamageType, IDamageStrategy> _strategyDic = new Dictionary<E_DamageType, IDamageStrategy>();
    //当前总伤害
    private int _currentTotalDamage;

    private DamageCalcManager()
    {
        // 初始化具体策略
        _strategyDic.Add(E_DamageType.Direct, new DirectDamageStrategy());
        _strategyDic.Add(E_DamageType.Dot, new DotDamageStrategy());
        _strategyDic.Add(E_DamageType.Break, new  BreakDamageStrategy());
        _strategyDic.Add(E_DamageType.True, new TrueDamageStrategy());
    }

    /// <summary>
    /// 计算伤害
    /// </summary>
    /// <param name="source">攻击者</param>
    /// <param name="target">目标</param>
    /// <param name="damageType">伤害类型</param>
    /// <param name="skill">技能对象</param>
    /// <returns>最终伤害</returns>
    public void CalcDamage(IBattleEntityObject source, IBattleEntityObject target, ISkill skill, out DamageResult damageResult)
    {
        E_DamageType damageType = skill.SkillInfo.f_damageType.ToDamageType();
        if (_strategyDic.TryGetValue(damageType, out IDamageStrategy strategy))
        {
            //计算每次最终伤害
            strategy.CalcDamage(source, target, skill, out damageResult);
            source.Context.GetEventBus().TriggerEvent(new OnTakeDamageEvent(source.Context, damageResult));
        }
        else
        {
            damageResult = default;
            LogManager.LogError("未实现对应的伤害策略");
        }
    }

    ///// <summary>
    ///// 计算Dot伤害
    ///// </summary>
    ///// <param name="attacker">攻击者</param>
    ///// <param name="target">目标</param>
    ///// <param name="damageType">伤害类型</param>
    ///// <param name="extraData"></param>
    ///// <returns>最终伤害</returns>
    //public void CalcDotDamage(IBattleTarget source, IBattleTarget target, IDotBuff dot)
    //{
    //    if (_strategyDic.TryGetValue(E_DamageType.Dot, out IDamageStrategy strategy))
    //    {
    //        UIMgr.Instance.GetPanel<BattlePanel>((panel) =>
    //        {
    //            //计算最终伤害
    //            int tempDmg = dot.CalcDamage();
    //            target.ProcessDamage(new DamageResult());
    //            //分发事件
    //            //EventCenter.Instance.EventTrigger(E_EventType.OnApplyDamage, new ApplyDamageEvent(attacker, target, tempDmg));
    //            //显示伤害
    //            CreateDamageText(tempDmg, target);
    //            //显示累计伤害
    //            panel.UpdateCumulativeDamageText(dmg: _currentTotalDamage += tempDmg);
    //        });
    //    }
    //    else
    //    {
    //        DebugMgr.LogError("未实现对应的策略");
    //    }
    //}

    /// <summary>
    /// 清除伤害缓存
    /// </summary>
    public void ClearDamage()
    {
        // 重置伤害累计
        _currentTotalDamage = 0;
        //隐藏伤害显示
        //UIMgr.Instance.GetPanel<BattlePanel>((panel) => { panel.UpdateCumulativeDamageText(false); });
    }
}
