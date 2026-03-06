using HotUpdate.Core.Battle.Damage.Data;
using HotUpdate.Core.Battle.Object;

namespace HotUpdate.Core.Battle.Damage
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
