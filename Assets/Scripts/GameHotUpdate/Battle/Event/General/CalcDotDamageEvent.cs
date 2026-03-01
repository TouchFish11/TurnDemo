using GameHotUpdate.Battle.Context;
using GameHotUpdate.Battle.Damage.Data;

namespace GameHotUpdate.Battle.Event.General
{
    /// <summary>
    /// 计算Dot伤害事件
    /// 传递计算数据给伤害计算管理器计算伤害
    /// </summary>
    public class CalcDotDamageEvent : BattleEvent
    {
        public DotDamageCalcData DotDamageCalcData {get; }
        
        public CalcDotDamageEvent(IBattleContext context, DotDamageCalcData damageCalcData) : base(context)
        {
            DotDamageCalcData = damageCalcData;
        }
    }
}
