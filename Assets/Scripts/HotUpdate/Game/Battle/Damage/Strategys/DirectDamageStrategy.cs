using Core.Log;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Property;
using HotUpdate.Game.Battle.Skill;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.Game.Battle.Damage.Strategys
{
    /// <summary>
    /// 直接伤害计算策略
    /// </summary>
    public class DirectDamageStrategy : IDamageStrategy
    {
        //private IBattleEntityObject attacker;
        //private IBattleEntityObject defender;
        //private SkillInfo skillInfo;

        ////技能倍率数组
        //private int[] skillMuls;

        public void CalcDamage(IBattleEntityObject attacker, IBattleEntityObject defender, SkillInfo skillInfo, out DamageResult damageResult)
        {
            if (attacker == null || defender == null)
            {
                Logger.LogError(ELogTags.Battle, "直接伤害计算参数为null");
            }

            //this.attacker = attacker;
            //this.defender = defender;
            //this.skillInfo = skill.SkillInfo;
            ////this.skillMuls = TextUtility.SplitToIntArr(skillInfo.f_skill_mul, 2);

            ////最终伤害 = 基础伤害区(固定伤害(可选) + 伤害系数 * 基础属性(攻) * 技能倍率(1 + 伤害增加 * 技能伤害加成) * 防御减免率(1 - 减伤率 + 防御降低)) * 暴击区(暴击时 * (1 + 暴击伤害倍率)) * 抗性减免区(1 - 有效抗性 + 抗性降低)
            ////计算基础伤害区
            //int finalDamage = CalcBaseDamageZone();
            ////计算暴击伤害区
            //finalDamage = CalcCritDamageZone(finalDamage);
            ////计算防御减免区
            //finalDamage = CalcDefendZone(finalDamage);
            ////计算抗性减免区
            //finalDamage = CalcResistanceZone(finalDamage);
            //return finalDamage;

            var critValue = attacker.GetComponent<PropertyComponent>().GetPropertyValue(E_DynamicPropertyType.TotalCrit);
            var critRate = critValue / 100f;
            var isCrit = Random.Range(0, 1) < critRate;
            damageResult = new DamageResult(attacker, defender, Random.Range(30, 70), (E_ElementType)skillInfo.f_elementType, (E_DamageType)skillInfo.f_damageType, isCrit, skillInfo.f_id, skillInfo.f_toughenValue);
        }

        ///// <summary>
        ///// 计算基础伤害区
        ///// </summary>
        ///// <param name="damage"></param>
        ///// <returns></returns>
        //private int CalcBaseDamageZone()
        //{
        //    //基础伤害值 = 角色属性 * 对应系数(所有角色属性以攻击为主) = 基础值 * (1 + 大攻击%) + 小攻击固定值(该部分算法为角色基础攻击 + 装备额外攻击)

        //    //获取伤害模型
        //    E_DamageModel damageModel = (E_DamageModel)skillInfo.f_dmg_model;
        //    //记录最终属性值
        //    int finalPropertyValue = 0;
        //    switch (damageModel)
        //    {
        //        case E_DamageModel.Life:
        //            //生命值大攻击百分比加成：= 装备加成 + 天赋加成 + Buff效果
        //            float totalHpPercentBonus = (0 + 0 + attacker.BuffController.GetTotalHpPercentBonus()) / 100f;
        //            //小攻击(生命值固定值加成)：= 装备加成 + 天赋加成 + Buff效果
        //            int totalHpBuildBonus = 0 + 0 + attacker.BuffController.GetTotalHpBuildBonus();
        //            //最终生命值 = (角色基础生命值 + 装备基础生命值) * (1 + 大攻击%) + 小攻击
        //            finalPropertyValue = (int)((attacker.GetProperty<BaseProperty>().F_basicHp + 0) * (1 + totalHpPercentBonus) + totalHpBuildBonus);
        //            break;
        //        case E_DamageModel.NormalAttack:
        //            //攻击力大攻击百分比加成：= 装备加成 + 天赋加成 + Buff效果
        //            float totalAtkPercentBonus = (0 + 0 + attacker.BuffController.GetTotalAtkPercentBonus()) / 100f;
        //            //小攻击(攻击力固定值加成)：= 装备加成 + 天赋加成 + Buff效果
        //            int totalAtkBuildBonus = 0 + 0 + attacker.BuffController.GetTotalAtkBuildBonus();
        //            //最终攻击力 = (角色基础攻击力 + 装备基础攻击力) * (1 + 大攻击%) + 小攻击
        //            finalPropertyValue = (int)((attacker.GetProperty<BaseProperty>().F_basicAtk + 0) * (1 + totalAtkPercentBonus) + totalAtkBuildBonus);
        //            break;
        //        case E_DamageModel.Defend:
        //            //防御力大攻击百分比加成：= 装备加成 + 天赋加成 + Buff效果
        //            float totalDefPercentBonus = (0 + 0 + attacker.BuffController.GetTotalDefPercentBonus()) / 100f;
        //            //小攻击(防御力固定值加成)：= 装备加成 + 天赋加成 + Buff效果
        //            int totalDefBuildBonus = (0 + 0 + attacker.BuffController.GetTotalDefBuildBonus());
        //            //最终防御力 = (角色基础防御力 + 装备基础防御力) * (1 + 大攻击%) + 小攻击
        //            finalPropertyValue = (int)((attacker.GetProperty<BaseProperty>().F_basicDef + 0) * (1 + totalDefPercentBonus) + totalDefBuildBonus);
        //            break;
        //    }

        //    //基础伤害 = 伤害模型数值 * 技能倍率(根据角色当前技能等级获取)
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
        //        //暴击伤害 = 原伤害 * (1 + 暴击伤害倍率)
        //        return (int)(damage * (1 + critDmgRate));
        //    }
        //    //未暴击
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
        //     * 防御区转化为 伤害减免率，公式：伤害减免率(防御承伤率) = 攻击方等级系数 / (敌方实际防御 + 攻击方等级系数)
        //     * 
        //     * 其中：
        //     * 攻击方等级系数 = 200 + 攻击方等级 * 10
        //     * 敌方实际防御 = 敌方基础防御 * (1 + 敌方防御加成) * (1 - 防御降低百分比) * (1 - 无视防御百分比)
        //    */

        //    //防御力大攻击百分比加成：= Buff效果加成
        //    float totalDefPercentBonus = defender.BuffController.GetTotalDefPercentBonus() / 100f;
        //    //小攻击(防御力固定值加成)：= Buff效果加成
        //    int totalDefBuildBonus = defender.BuffController.GetTotalDefBuildBonus();
        //    //最终防御力 = 角色基础防御 * (1 + 大攻击%) + 小攻击
        //    int totalDefValue = (int)(defender.GetProperty<BaseProperty>().F_basicDef + (1 + totalDefPercentBonus) + totalDefBuildBonus);
        //    //防御降低百分比之和 = Buff效果影响
        //    float totalSubDefPercent = defender.BuffController.GetTotalSubDefPercent() / 100f;
        //    //无视防御百分比之和 = Buff效果影响
        //    float totalIgnoreDefPercent = attacker.BuffController.GetTotalIgnoreDefPercent() / 100f;
        //    //防御减免率
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
        //     * 抗性减免率(百分比) = Clamp(-100, 1 - (敌方属性抗性 + 抗性降低 - 攻击方抗性穿透), 90)
        //     * 敌方属性抗性 = 基础属性抗性
        //     * 抗性降低 = Buff效果(正值增加抗性，负值降低)
        //     * 攻击方抗性穿透 = Buff效果
        //     */

        //    return damage;
        //}
    }
}