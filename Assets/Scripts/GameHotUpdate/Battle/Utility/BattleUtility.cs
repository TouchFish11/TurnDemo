using Game.Battle.Context;
using Game.Battle.Enum;
using GameHotUpdate.Battle.Event.UI;
using GameHotUpdate.Battle.Property;

namespace GameHotUpdate.Battle.Utility
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
    }
}
