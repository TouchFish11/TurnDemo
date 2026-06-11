using System.Collections.Generic;
using HotUpdate.Base;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Event.UI;
using HotUpdate.Game.Battle.Property;
using HotUpdate.Game.Battle.Skill;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.Game.Battle.Utility
{
    /// <summary>
    /// 战斗工具类
    /// </summary>
    public static class BattleUtility
    {
        /// <summary>
        /// 基础行动值
        /// </summary>
        public const float BASE_ACTION_VALUE = 10000f;

        /// <summary>
        /// 速度修正系数（平衡不同速度区间）
        /// </summary>
        public const float SPEED_CORRECTION = 1.0f;
        
        /// <summary>
        /// 初始化行动顺序
        /// 计算所有存活实体的行动值并排序，选出第一个行动的实体
        /// </summary>
        /// <param name="context">战斗上下文</param>
        public static void InitOrder(IBattleContext context)
        {
            // 初始化所有角色的行动值
            foreach (var battleEntityObject in context.GetAliveEntitys())
            {
                var speed = battleEntityObject.GetComponent<PropertyComponent>().GetPropertyValue(E_DynamicPropertyType.CurrentSpeed);
                // 根据速度计算行动值
                battleEntityObject.SetActionValue(CalcActionValue(speed));
            }

            // 基于行动值升序排列（行动值越小越先行动）
            context.Sort((b1, b2) =>
            {
                if (b1.ActionValue < b2.ActionValue)
                {
                    return -1;
                }

                return b1.ActionValue > b2.ActionValue ? 1 : 0;
            });

            // 将首个行动实体的行动值置为0，保证其最先行动
            context.GetFirstBattleEntity().SetActionValue(0);
            // 触发事件，通知行动轴UI更新
            context.GetEventBus().TriggerEvent(new ActionBarSortPostEvent(context, context.GetAliveEntitys()));
        }

        /// <summary>
        /// 计算行动值
        /// 行动值 = 基础行动值 / 速度 * 修正系数
        /// </summary>
        /// <param name="speed">当前速度</param>
        /// <returns>行动值</returns>
        public static float CalcActionValue(float speed)
        {
            return BASE_ACTION_VALUE / speed * SPEED_CORRECTION;
        }
        
        /// <summary>
        /// 根据技能范围和主目标获取受击目标列表
        /// </summary>
        /// <param name="mainTarget">主目标实体</param>
        /// <param name="rangeType">技能范围类型（枚举值）</param>
        /// <param name="filterObjects">可被选择的目标列表</param>
        /// <param name="finalTargets">最终受击目标列表（输出）</param>
        public static void GetRangeTargets(IBattleEntityObject mainTarget, int rangeType, List<IBattleEntityObject> filterObjects, List<IBattleEntityObject> finalTargets)
        {
            switch ((E_SkillRangeType)rangeType)
            {
                case E_SkillRangeType.Single:
                    // 仅当前目标
                    finalTargets.Add(mainTarget);
                    break;
                case E_SkillRangeType.Diffusion:
                    // 主目标及其相邻目标
                    finalTargets.Add(mainTarget);
                    if (filterObjects.Count > 1)
                    {
                        var mainIndex = filterObjects.IndexOf(mainTarget);
                        // 目标在最左端，只取右侧相邻
                        if (mainIndex == 0)
                        {
                            finalTargets.Add(filterObjects[mainIndex + 1]);
                        }
                        // 目标在最右端，只取左侧相邻
                        else if (mainIndex == filterObjects.Count - 1)
                        {
                            finalTargets.Add(filterObjects[mainIndex - 1]);
                        }
                        // 目标在中间，取左右两侧相邻
                        else
                        {
                            finalTargets.Add(filterObjects[mainIndex - 1]);
                            finalTargets.Add(filterObjects[mainIndex + 1]);
                        }
                    }
                    break;
                case E_SkillRangeType.All:
                    // 全体目标
                    finalTargets.AddRange(filterObjects);
                    break;
                default:
                    Logger.LogError($"{nameof(rangeType)}, {rangeType}");
                    break;
            }
        }
        
        /// <summary>
        /// 将技能范围类型数值转换为中文描述
        /// </summary>
        /// <param name="i">技能范围类型数值</param>
        /// <returns>中文描述</returns>
        public static string ToSkillRangeTypeText(this int i)
        {
            E_SkillRangeType skillRangeType = (E_SkillRangeType)i;
            return skillRangeType switch
            {
                E_SkillRangeType.Single => "单体",
                E_SkillRangeType.Diffusion => "扩散",
                E_SkillRangeType.All => "全体",
                _ => "None"
            };
        }

        /// <summary>
        /// 将整型数值转换为技能类型枚举
        /// </summary>
        /// <param name="i">技能类型数值</param>
        /// <returns>技能类型枚举</returns>
        public static E_SkillType ToSkillType(this int i)
        {
            return (E_SkillType)i;
        }
        
        /// <summary>
        /// 根据元素类型数值返回对应的颜色
        /// </summary>
        /// <param name="i">元素类型数值</param>
        /// <returns>颜色</returns>
        public static Color ToElementTypeColor(this int i)
        {
            E_ElementType elementType = (E_ElementType)i;
            return elementType switch
            {
                E_ElementType.Fire => Color.red,
                E_ElementType.Ice => Color.blue,
                E_ElementType.Physical => Color.white,
                E_ElementType.Quantum => new Color(128, 0, 128),
                E_ElementType.Wind => Color.green,
                _ => Color.white
            };
        }
        
        /// <summary>
        /// 将整型数值转换为元素类型枚举
        /// </summary>
        /// <param name="i">元素类型数值</param>
        /// <returns>元素类型枚举</returns>
        public static E_ElementType ToElementType(this int i)
        {
            return (E_ElementType)i;
        }
        
        /// <summary>
        /// 将整型数值转换为伤害类型枚举
        /// </summary>
        /// <param name="i">伤害类型数值</param>
        /// <returns>伤害类型枚举</returns>
        public static E_DamageType ToDamageType(this int i)
        {
            return (E_DamageType)i;
        }
    }
}