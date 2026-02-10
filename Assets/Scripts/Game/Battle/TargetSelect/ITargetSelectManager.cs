using System;
using System.Collections.Generic;
using Game.Battle.Context;
using Game.Battle.Objects;

namespace Game.Battle.TargetSelect
{
    /// <summary>
    /// Ŀ��ѡ��������ӿ�
    /// </summary>
    public interface ITargetSelectManager
    {
        /// <summary>
        /// ����Ŀ��ѡ��
        /// </summary>
        void ActiveSelectTarget();

        /// <summary>
        /// ʧ��Ŀ��ѡ��
        /// </summary>
        void InActiveSelectTarget();

        /// <summary>
        /// ��ȡ��Ŀ��
        /// </summary>
        /// <returns></returns>
        IBattleEntityObject GetMainTarget();

        /// <summary>
        /// ��ȡĿ���б���������Ŀ�꣩
        /// </summary>
        /// <returns></returns>
        List<IBattleEntityObject> GetTargets();

        /// <summary>
        /// ѡ��Ŀ��
        /// </summary>
        /// <param name="context"></param>
        /// <param name="caster"></param>
        /// <param name="skillInfo"></param>
        /// <param name="targetSelectStrategy"></param>
        void SelectTarget(IBattleContext context, IBattleEntityObject caster, SkillInfo skillInfo, ITargetSelectStrategy targetSelectStrategy);

        /// <summary>
        /// ��ʼ��
        /// </summary>
        /// <param name="battleContext"></param>
        void Init(IBattleContext battleContext);

        /// <summary>
        /// 主目标选择变化
        /// </summary>
        event Action<IBattleEntityObject> OnSelectChanged;
    }
}
