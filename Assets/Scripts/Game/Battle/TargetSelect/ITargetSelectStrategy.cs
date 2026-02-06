using System.Collections.Generic;
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
        /// <param name="targets"></param>
        /// <param name="caster"></param>
        /// <param name="skillInfo"></param>
        /// <returns></returns>
        IBattleEntityObject SelectMainTarget(List<IBattleEntityObject> targets, IBattleEntityObject caster, SkillInfo skillInfo);
    }
}
