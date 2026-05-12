using HotUpdate.Base.Battle.Damage.Data;
using HotUpdate.Base.Battle.Object;
using HotUpdate.Common.Config.ExcelInfo.Info;

namespace HotUpdate.Base.Battle.Damage
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDamageStrategy
    {
        /// <summary>
        /// �����˺�
        /// </summary>
        /// <param name="source">������</param>
        /// <param name="target">������</param>
        /// <param name="skillInfo">��������</param>
        /// <returns></returns>
        void CalcDamage(IBattleEntityObject source, IBattleEntityObject target, SkillInfo skillInfo, out DamageResult damageResult);
    }
}
