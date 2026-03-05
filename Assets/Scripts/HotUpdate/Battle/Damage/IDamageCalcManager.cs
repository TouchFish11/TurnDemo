using HotUpdate.Battle.Context;
using HotUpdate.Battle.Damage.Data;
using HotUpdate.Battle.Object;

namespace HotUpdate.Battle.Damage
{
    /// <summary>
    /// �˺�����������ӿ�
    /// </summary>
    public interface IDamageCalcManager
    {
        void CalcSkillDamage(IBattleEntityObject source, IBattleEntityObject target, SkillInfo skillInfo, out DamageResult damageResult);

        /// <summary>
        /// ��ʼ��
        /// </summary>
        /// <param name="context"></param>
        void Init(IBattleContext context);
    }
}
