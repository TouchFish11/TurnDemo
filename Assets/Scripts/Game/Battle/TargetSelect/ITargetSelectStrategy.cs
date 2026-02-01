using Game.Battle.Context;
using Game.Battle.Objects;

namespace Game.Battle.TargetSelect
{
    /// <summary>
    /// Ŀ��ѡ����Խӿ�
    /// </summary>
    public interface ITargetSelectStrategy
    {
        /// <summary>
        /// ���ȼ�
        /// Խ��Խ��ִ��
        /// </summary>
        public int Priority { get; }

        /// <summary>
        /// ѡ����Ŀ��
        /// </summary>
        /// <param name="context"></param>
        /// <param name="caster"></param>
        /// <param name="skillInfo"></param>
        /// <returns></returns>
        IBattleEntityObject SelectMainTarget(IBattleContext context, IBattleEntityObject caster, SkillInfo skillInfo);
    }
}
