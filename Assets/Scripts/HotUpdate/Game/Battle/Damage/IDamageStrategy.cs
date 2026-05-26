using HotUpdate.Base;
using HotUpdate.Common.Config.ExcelInfo.Info;

namespace HotUpdate.Game.Battle.Damage
{
    /// <summary>
    /// 伤害策略接口
    /// </summary>
    public interface IDamageStrategy
    {
        /// <summary>
        /// 计算伤害
        /// </summary>
        /// <param name="source">伤害来源</param>
        /// <param name="target">伤害目标</param>
        /// <param name="skillInfo">技能信息</param>
        /// <param name="damageResult">伤害结果</param>
        void CalcDamage(IBattleEntityObject source, IBattleEntityObject target, SkillInfo skillInfo, out DamageResult damageResult);
    }
}
