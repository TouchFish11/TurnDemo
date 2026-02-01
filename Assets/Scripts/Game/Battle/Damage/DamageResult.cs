using Game.Battle.Enum;
using Game.Battle.Objects;
using Game.Battle.Skill.Enum;

namespace Game.Battle.Damage
{
    /// <summary>
    /// �˺����
    /// </summary>
    public readonly struct DamageResult
    {
        /// <summary>
        /// �˺���Դ
        /// </summary>
        public IBattleEntityObject Source { get; }

        /// <summary>
        /// Ŀ��
        /// </summary>
        public IBattleEntityObject Target { get; }

        /// <summary>
        /// �����˺�
        /// </summary>
        public int FinalDamage { get; }

        /// <summary>
        /// �˺�����
        /// </summary>
        public E_ElementType ElementType { get; }

        /// <summary>
        /// �˺�����
        /// </summary>
        public E_DamageType DamageType { get; }

        /// <summary>
        /// �Ƿ񱩻�
        /// </summary>
        public bool IsCrit { get; }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        public SkillInfo SkillInfo { get; }

        public DamageResult(IBattleEntityObject source, IBattleEntityObject target, int finalDamage, E_ElementType elementType, E_DamageType damageType, bool isCrit, SkillInfo skillInfo)
        {
            Source = source;
            Target = target;
            FinalDamage = finalDamage;
            ElementType = elementType;
            DamageType = damageType;
            IsCrit = isCrit;
            SkillInfo = skillInfo;
        }
    }
}
