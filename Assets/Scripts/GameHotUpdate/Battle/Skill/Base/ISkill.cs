using System.Collections;
using System.Collections.Generic;
using GameHotUpdate.Battle.Context;
using GameHotUpdate.Battle.Object;
using GameHotUpdate.Battle.Property;
using GameHotUpdate.Battle.TargetSelect.Strategys;

namespace GameHotUpdate.Battle.Skill.Base
{
    /// <summary>
    /// ���ܽӿ�
    /// </summary>
    public interface ISkill
    {
        /// <summary>
        /// ��������
        /// </summary>
        SkillInfo SkillInfo { get; }

        /// <summary>
        /// ʩ����
        /// </summary>
        IBattleEntityObject Caster { get; }

        /// <summary>
        /// ��Ŀ��
        /// </summary>
        IBattleEntityObject MainTarget { get; }

        /// <summary>
        /// ����Ŀ��
        /// </summary>
        List<IBattleEntityObject> AllTargets { get; }

        /// <summary>
        /// �������
        /// </summary>
        IPropertyComponent PropertyComponent { get; }
        
        /// <summary>
        /// Ŀ��ѡ�����
        /// </summary>
        ITargetSelectStrategy TargetSelectStrategy { get; }

        /// <summary>
        /// �ͷż���
        /// ͨ�����ܶ���ʵ����������ɫ�ͷż�����Ϊ��
        /// </summary>
        /// <param name="context"></param>
        IEnumerator Cast(IBattleContext context);

        /// <summary>
        /// ��ʼ������
        /// </summary>
        /// <param name="mainTarget"></param>
        /// <param name="allTargets"></param>
        void Init(IBattleEntityObject mainTarget, List<IBattleEntityObject> allTargets);
        
        void RecoverEnergy();

        /// <summary>
        /// ����Ŀ��ѡ�����
        /// </summary>
        /// <param name="targetSelectStrategy"></param>
        void SetTargetSelectStrategy(ITargetSelectStrategy targetSelectStrategy);
    }
}
