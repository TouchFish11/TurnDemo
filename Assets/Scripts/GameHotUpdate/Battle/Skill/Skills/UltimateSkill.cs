using System.Collections;
using Core.Reflection;
using Core.Service;
using Game.Battle.Context;
using Game.Battle.Enum;
using Game.Battle.Objects;
using Game.Battle.Skill.Component;
using Game.Battle.Skill.Handler;
using Game.Battle.Skill.Interface;
using Game.Battle.Status;
using Game.Battle.TargetSelect;
using Game.UI.Battle;
using GameHotUpdate.Battle.TargetSelect.Strategys;
using UnityEngine;

namespace GameHotUpdate.Battle.Skill.Skills
{
    /// <summary>
    /// �սἼ����
    /// </summary>
    public abstract class UltimateSkill : PlayerSkill
    {
        private readonly ISkillComponent skillComponent;

        protected UltimateSkill(IBattleEntityObject caster, int skillId, ISkillCastPostHandler postHandler, IStatusAddStrategy statusAddStrategy) : base(caster, skillId, postHandler, statusAddStrategy)
        {
            skillComponent = Caster.GetComponent<SkillComponent>();
        }

        protected override IEnumerator OnCast(IBattleContext context)
        {
            // �սἼ�ͷ�ǰ
            OnPreUltimateCast(context);
            // �ȴ�����
            yield return new WaitUntil(() => skillComponent.IsRelease);
            // ȷ����������Ŀ��
            ServiceLocator.Get<ISkillManager>().InitSkillTarget(this);
            // ����Ŀ��ѡ��
            ServiceLocator.Get<ITargetSelectManager>().InActiveSelectTarget();
            // �������UI����
            ServiceLocator.Get<IBattleUIScheduler>().UltimateCasting();
            // �սἼ�ͷ�
            yield return OnUltimateCast(context);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// �սἼ�ͷ�ǰ
        /// </summary>
        /// <param name="context"></param>
        protected virtual void OnPreUltimateCast(IBattleContext context)
        {
            // ����������
            BattlePoint.BattlePoint.Instance.ActiveCamera(Caster);
            // ���¿���
            context.GetTurnManager().UpdateEntityLookAt(Caster);
            // ����Ŀ��ѡ��
            ServiceLocator.Get<ITargetSelectManager>().ActiveSelectTarget();
            // ��������Ŀ��ѡ��
            var strategy = ServiceLocator.Get<IFactoryManager>()
                .GetFactory<ITargetSelectStrategyFactory, TargetSelectStrategyFactory>()
                .GetTargetSelectStrategy<PlayerBaseTargetSelectStrategy>();
            ServiceLocator.Get<ITargetSelectManager>().SelectTarget(context, Caster, SkillInfo, strategy);
            // �����սἼ���UI��ʾ
            ServiceLocator.Get<IBattleUIScheduler>().UltimateTriggerChangeUI(Caster, SkillInfo);
            // ��ʱ�������������������ʾ
            PropertyComponent.SetPropertyValue(E_DynamicPropertyType.CurrentEnergy, 0);
        }

        /// <summary>
        /// �սἼ�ͷ�
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected abstract IEnumerator OnUltimateCast(IBattleContext context);
    }
}
