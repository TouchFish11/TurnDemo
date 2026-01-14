using Framework;
using Game.Battle;
using UnityEngine;

/// <summary>
/// 直伤处理策略类
/// </summary>
public class DirectDamageStrategy : IDamageStrategy
{
    //private IBattleEntityObject attacker;
    //private IBattleEntityObject defender;
    //private SkillInfo skillInfo;

    ////技能倍率数组
    //private int[] skillMuls;

    /// <summary>
    /// 计算直伤
    /// </summary>
    /// <param name="attacker">攻击者</param>
    /// <param name="defender">防御者</param>
    /// <param name="skill">额外数据</param>
    /// <returns></returns>
    public void CalcDamage(IBattleEntityObject attacker, IBattleEntityObject defender, SkillInfo skillInfo, out DamageResult damageResult)
    {
        if (attacker == null || defender == null)
        {
            LogManager.LogError("直伤计算策略参数为null");
        }

        //this.attacker = attacker;
        //this.defender = defender;
        //this.skillInfo = skill.SkillInfo;
        ////this.skillMuls = TextUtility.SplitToIntArr(skillInfo.f_skill_mul, 2);

        ////最终伤害 = 「基础伤害区（基础伤害(可选) + 伤害倍率 * 基于属性）」 * 「暴击乘区倍率（1 + 暴击率 * 暴击伤害）」* 「防御乘区倍率」 * 「抗性乘区倍率（1 - 有效抗性 + 抗性降低）」
        ////计算基础伤害
        //int finalDamage = CalcBaseDamageZone();
        ////计算暴击伤害
        //finalDamage = CalcCritDamageZone(finalDamage);
        ////计算防御乘区
        //finalDamage = CalcDefendZone(finalDamage);
        ////计算抗性乘区
        //finalDamage = CalcResistanceZone(finalDamage);
        //return finalDamage;

        int critValue = attacker.GetComponent<PropertyComponent>().GetPropertyValue(E_DynamicPropertyType.TotalCrit);
        float critRate = critValue / 100f;
        bool isCrit = Random.Range(0, 1) < critRate;
        damageResult = new DamageResult(attacker, defender, Random.Range(30, 70), skillInfo.f_elementType.ToElementType(), skillInfo.f_damageType.ToDamageType(), isCrit, skillInfo);
    }

    ///// <summary>
    ///// 计算基础伤害区
    ///// </summary>
    ///// <param name="damage"></param>
    ///// <returns></returns>
    //private int CalcBaseDamageZone()
    //{
    //    //基础伤害区 = 角色三维 × 对应倍率。其中角色三维以攻击为例，攻击力 = 白值 × (1 + 大攻击) + 小攻击。白值即基础攻击力，算法为角色基础攻击力 + 光锥基础攻击力

    //    //获取伤害模型
    //    E_DamageModel damageModel = (E_DamageModel)skillInfo.f_dmg_model;
    //    //记录最终属性
    //    int finalPropertyValue = 0;
    //    switch (damageModel)
    //    {
    //        case E_DamageModel.Life:
    //            //大生命（生命百分比加成）= 光锥加成 + 仪器加成 + Buff效果
    //            float totalHpPercentBonus = (0 + 0 + attacker.BuffController.GetTotalHpPercentBonus()) / 100f;
    //            //小生命（生命固定数值加成）= 光锥加成 + 仪器加成 + Buff效果
    //            int totalHpBuildBonus = 0 + 0 + attacker.BuffController.GetTotalHpBuildBonus();
    //            //最终生命值 = （角色基础生命值 + 光锥基础生命值）* (1 + 大生命) + 小生命
    //            finalPropertyValue = (int)((attacker.GetProperty<BaseProperty>().F_basicHp + 0) * (1 + totalHpPercentBonus) + totalHpBuildBonus);
    //            break;
    //        case E_DamageModel.NormalAttack:
    //            //大攻击（攻击百分比加成）= 光锥加成 + 仪器加成 + Buff效果
    //            float totalAtkPercentBonus = (0 + 0 + attacker.BuffController.GetTotalAtkPercentBonus()) / 100f;
    //            //小攻击（攻击固定数值加成）= 光锥加成 + 仪器加成 + Buff效果
    //            int totalAtkBuildBonus = 0 + 0 + attacker.BuffController.GetTotalAtkBuildBonus();
    //            //最终攻击力 = （角色基础攻击力 + 光锥基础攻击力）* (1 + 大攻击) + 小攻击
    //            finalPropertyValue = (int)((attacker.GetProperty<BaseProperty>().F_basicAtk + 0) * (1 + totalAtkPercentBonus) + totalAtkBuildBonus);
    //            break;
    //        case E_DamageModel.Defend:
    //            //大防御（防御百分比加成）= 光锥加成 + 仪器加成 + Buff效果
    //            float totalDefPercentBonus = (0 + 0 + attacker.BuffController.GetTotalDefPercentBonus()) / 100f;
    //            //小防御（防御固定数值加成）= 光锥加成 + 仪器加成 + Buff效果
    //            int totalDefBuildBonus = (0 + 0 + attacker.BuffController.GetTotalDefBuildBonus());
    //            //最终防御力 = （角色基础防御力 + 光锥基础防御力）* (1 + 大防御) + 小防御
    //            finalPropertyValue = (int)((attacker.GetProperty<BaseProperty>().F_basicDef + 0) * (1 + totalDefPercentBonus) + totalDefBuildBonus);
    //            break;
    //    }

    //    //最终伤害 = 最终模型属性 * 技能倍率(根据角色当前技能等级获取)
    //    return (int)(finalPropertyValue * (this.skillMuls[0] / 100f));
    //}

    ///// <summary>
    ///// 计算暴击区
    ///// </summary>
    //private int CalcCritDamageZone(int damage)
    //{
    //    //获取暴击率
    //    float critRate = attacker.GetComponent<PropertyComponent>().GetProperty<BattleProperty>().F_crit / 100f;
    //    float critDmgRate = attacker.GetProperty<BaseProperty>().F_critDmg / 100f;
    //    //是否暴击
    //    bool isCrit = Random.Range(0, 1f) < critRate;
    //    //暴击
    //    if(isCrit)
    //    {
    //        //最终伤害 = 总伤害 *（1 + 暴击伤害倍率）
    //        return (int)(damage * (1 + critDmgRate));
    //    }
    //    //无暴击
    //    else
    //    {
    //        return damage;
    //    }
    //}

    ///// <summary>
    ///// 计算防御区
    ///// </summary>
    ///// <param name="damage"></param>
    ///// <returns></returns>
    //private int CalcDefendZone(int damage)
    //{
    //    /*
    //     * 防御力转化为 “伤害倍率“公式：伤害倍率（防御乘区）= 攻击方等级系数 / (敌方实际防御 + 攻击方等级系数)
    //     * 
    //     * 其中：
    //     * 攻击方等级系数 = 200 + 攻击方等级 × 10
    //     * 敌方实际防御 = 敌方基础防御 × (1 + 敌方防御加成) × (1 - 减防百分比) × (1 - 无视防御百分比)
    //    */

    //    //大防御（防御百分比加成）= Buff效果加成
    //    float totalDefPercentBonus = defender.BuffController.GetTotalDefPercentBonus() / 100f;
    //    //小防御（防御固定数值加成）= Buff效果加成
    //    int totalDefBuildBonus = defender.BuffController.GetTotalDefBuildBonus();
    //    //最终防御力 = 角色基础防御力 * (1 + 大防御) + 小防御
    //    int totalDefValue = (int)(defender.GetProperty<BaseProperty>().F_basicDef + (1 + totalDefPercentBonus) + totalDefBuildBonus);
    //    //减防百分比之和 = Buff效果影响
    //    float totalSubDefPercent = defender.BuffController.GetTotalSubDefPercent() / 100f;
    //    //无视防御百分比之和 = Buff效果影响
    //    float totalIgnoreDefPercent = attacker.BuffController.GetTotalIgnoreDefPercent() / 100f;
    //    //防御倍率
    //    float damageRate = (200 + attacker.GetProperty<BaseProperty>().F_lev * 10) / 
    //                       (totalDefValue * (1 - totalSubDefPercent) * (1 - totalIgnoreDefPercent) + 200 + attacker.GetProperty<BaseProperty>().F_lev * 10);

    //    return (int)(damage * damageRate);
    //}

    ///// <summary>
    ///// 计算抗性区
    ///// </summary>
    ///// <param name="damage"></param>
    ///// <returns></returns>
    //private int CalcResistanceZone(int damage)
    //{
    //    /*
    //     * 对应属性抗性：
    //     * 抗性乘区倍率(百分比) = Clamp(-100, 1 - (敌方基础抗性 + 抗性增减量 - 攻击者抗性穿透), 90)
    //     * 敌方基础抗性 = 敌人自身基础属性
    //     * 抗性增减量 = Buff效果（正值为提升，负值为降低）
    //     * 攻击者抗性穿透 = Buff效果
    //     */

    //    return damage;
    //}
}
