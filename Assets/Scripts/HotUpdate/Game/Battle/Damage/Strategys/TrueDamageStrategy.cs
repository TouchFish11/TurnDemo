using Core.Log;
using HotUpdate.Base;

using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.Damage.Strategys
{
    /// <summary>
    /// 真实伤害处理策略类
    /// </summary>
    public class TrueDamageStrategy : IDamageStrategy
    {
        public void CalcDamage(IBattleEntityObject source, IBattleEntityObject target, SkillInfo skillInfo, out DamageResult damageResult)
        {
            Logger.Log("真实伤害策略执行");
            damageResult = new DamageResult();
        }
    }
}
