using HotUpdate.Base.Battle;
using HotUpdate.Base.Battle.Damage.Data;
using HotUpdate.Base.Battle.Event;

namespace HotUpdate.Game.Battle.Event.General
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
