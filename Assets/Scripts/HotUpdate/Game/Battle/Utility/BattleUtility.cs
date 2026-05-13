using System.Collections.Generic;
using HotUpdate.Base.Battle;
using HotUpdate.Base.Battle.Object;
using HotUpdate.Base.Battle.Property;
using HotUpdate.Base.Battle.Skill;
using HotUpdate.Game.Battle.Event.UI;
using HotUpdate.Game.Battle.Property;
using UnityEngine;
using Logger = Core.Log.Logger;

namespace HotUpdate.Game.Battle.Utility
{
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
        /// 初始化顺序
        /// 用于选取第一个行动的实体
        /// </summary>
        public static void InitOrder(IBattleContext context)
        {
            // 初始化所有角色的行动值
            foreach (var battleEntityObject in context.GetAliveEntitys())
            {
                var speed = battleEntityObject.GetComponent<PropertyComponent>().GetPropertyValue(E_DynamicPropertyType.CurrentSpeed);
                // 初始化行动值
                battleEntityObject.SetActionValue(CalcActionValue(speed));
            }

            // 基于行动值初始化行动顺序
            context.Sort((b1, b2) =>
            {
                // 比较行动值确定行动顺序。行动值低，越先行动
                if (b1.ActionValue < b2.ActionValue)
                {
                    return -1;
                }

                return b1.ActionValue > b2.ActionValue ? 1 : 0;
            });

            // TODO：暂时这样处理：第一个行动的实体行动值为0，后续可能根据算法优化
            context.GetFirstBattleEntity().SetActionValue(0);
            // 事件分发传递，更新行动轴UI显示
            context.GetEventBus().TriggerEvent(new ActionBarSortPostEvent(context, context.GetAliveEntitys()));
        }

        /// <summary>
        /// 计算行动值
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        public static float CalcActionValue(float speed)
        {
            // 计算行动值，基准行动值 / 速度 * 修正系数
            return BASE_ACTION_VALUE / speed * SPEED_CORRECTION;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mainTarget"></param>
        /// <param name="rangeType"></param>
        /// <param name="filterObjects"></param>
        /// <param name="finalTargets"></param>
        public static void GetRangeTargets(IBattleEntityObject mainTarget, int rangeType, List<IBattleEntityObject> filterObjects, List<IBattleEntityObject> finalTargets)
        {
            switch ((E_SkillRangeType)rangeType)
            {
                case E_SkillRangeType.Single:
                    // ֻ������Ŀ��
                    finalTargets.Add(mainTarget);
                    break;
                case E_SkillRangeType.Diffusion:
                    // ������Ŀ�������Ŀ��
                    finalTargets.Add(mainTarget);
                    if (filterObjects.Count > 1)
                    {
                        var mainIndex = filterObjects.IndexOf(mainTarget);
                        // �����
                        if (mainIndex == 0)
                        {
                            finalTargets.Add(filterObjects[mainIndex + 1]);
                        }
                        // ���Ҷ�
                        else if (mainIndex == filterObjects.Count - 1)
                        {
                            finalTargets.Add(filterObjects[mainIndex - 1]);
                        }
                        // ��������/��
                        else
                        {
                            finalTargets.Add(filterObjects[mainIndex - 1]);
                            finalTargets.Add(filterObjects[mainIndex + 1]);
                        }
                    }
                    break;
                case E_SkillRangeType.All:
                    //����ȫ��Ŀ��
                    finalTargets.AddRange(filterObjects);
                    break;
                default:
                    Logger.LogError($"{nameof(rangeType)}, {rangeType}");
                    break;
            }
        }
        
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

        public static E_SkillType ToSkillType(this int i)
        {
            return (E_SkillType)i;
        }
        
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
        
        public static E_ElementType ToElementType(this int i)
        {
            return (E_ElementType)i;
        }
        
        public static E_DamageType ToDamageType(this int i)
        {
            return (E_DamageType)i;
        }
    }
}
