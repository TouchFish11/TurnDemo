using GameHotUpdate.Battle.Context;
using GameHotUpdate.Battle.Damage.Data;
using GameHotUpdate.Battle.Object;

namespace GameHotUpdate.Battle.Damage
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
