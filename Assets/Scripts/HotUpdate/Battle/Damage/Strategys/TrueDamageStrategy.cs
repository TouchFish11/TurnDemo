using Core.Log;
using HotUpdate.Battle.Damage.Data;
using HotUpdate.Battle.Object;

namespace HotUpdate.Battle.Damage.Strategys
{
    /// <summary>
    /// 真实伤害处理策略类
    /// </summary>
    public class TrueDamageStrategy : IDamageStrategy
    {
        public void CalcDamage(IBattleEntityObject source, IBattleEntityObject target, SkillInfo skillInfo, out DamageResult damageResult)
        {
            LogManager.Log("真实伤害策略执行");
            damageResult = new DamageResult();
        }
    }
}
