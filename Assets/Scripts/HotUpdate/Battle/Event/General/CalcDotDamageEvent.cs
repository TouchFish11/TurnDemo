using HotUpdate.Battle.Context;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Damage;
using HotUpdate.Core.Battle.Damage.Data;
using HotUpdate.Core.Battle.Event;

namespace HotUpdate.Battle.Event.General
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
