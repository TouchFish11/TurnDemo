using Game.Battle.Objects;

namespace Game.Battle.Damage
{
    /// <summary>
    /// �˺��������Խӿ�
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
