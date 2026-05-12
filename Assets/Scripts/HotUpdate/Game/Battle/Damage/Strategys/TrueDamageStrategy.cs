using HotUpdate.Base.Battle.Damage;
using HotUpdate.Base.Battle.Damage.Data;
using HotUpdate.Base.Battle.Object;
using HotUpdate.Common.Config.ExcelInfo.Info;

namespace HotUpdate.Game.Battle.Damage.Strategys
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
