using HotUpdate.Core.Battle.Damage.Data;
using HotUpdate.Core.Battle.Object;

namespace HotUpdate.Core.Battle.Damage
{
    public interface IDamageCalcManager
    {
        void CalcSkillDamage(IBattleEntityObject source, IBattleEntityObject target, SkillInfo skillInfo, out DamageResult damageResult);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        void Init(IBattleContext context);
    }
}
