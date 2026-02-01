using Game.Battle.Context;
using Game.Battle.Objects;

namespace Game.Battle.Damage
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
