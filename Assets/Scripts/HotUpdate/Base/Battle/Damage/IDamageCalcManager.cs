using HotUpdate.Base.Battle.Damage.Data;
using HotUpdate.Base.Battle.Object;
using HotUpdate.Common.Config.ExcelInfo.Info;

namespace HotUpdate.Base.Battle.Damage
{
    /// <summary>
    /// 伤害计算管理器接口
    /// </summary>
    public interface IDamageCalcManager
    {
        /// <summary>
        /// 计算技能伤害
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="skillInfo"></param>
        /// <param name="damageResult"></param>
        void CalcSkillDamage(IBattleEntityObject source, IBattleEntityObject target, SkillInfo skillInfo, out DamageResult damageResult);
    }
}
