using System.Collections;
using Core.Config;
using Core.Service;
using Game.Battle.Context;
using Game.Battle.Objects;
using Game.Battle.Skill.Handler;
using Game.Battle.Status;
using Game.VFX;
using GameHotUpdate.Animation;
using GameHotUpdate.Battle.Event;
using UnityEngine;

namespace GameHotUpdate.Battle.Skill.Skills.Herta
{
    /// <summary>
    /// Herta�սἼ
    /// </summary>
    public class HertaUltimateSkill : UltimateSkill
    {
        private readonly string ultimateAttackState = "UltimateAttack";

        public HertaUltimateSkill(IBattleEntityObject caster, int skillId, IStatusAddStrategy statusAddStrategy) : base(caster, skillId, statusAddStrategy)
        {

        }

        protected override IEnumerator OnPreUltimateCast(IBattleContext context)
        {
            yield return base.OnPreUltimateCast(context);

            // ����Ԥ������������սἼpose���սἼ����
            Caster.GetComponent<BattleAnimationComponent>().SetUltimatePose();
            projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);
            projectileTrans = new ProjectileTrans(Caster.GameObject.transform.position, Quaternion.identity);
            vFXInfo = new VFXInfo();
            // ������Ч
            ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_HeartUltimatePose, projectileTrans, projectileData, vFXInfo);
        }

        protected override IEnumerator OnUltimateCast(IBattleContext context)
        {
            // �Ƴ�Pose��Ч
            ServiceLocator.Get<IVFXManager>().RemoveVFX(vFXInfo);
            context.GetEventBus().TriggerEvent(new SkillCastEvent(context, this, 0));
            BattleAnimationComponent animationComponent = Caster.GetComponent<BattleAnimationComponent>();
            // �ȴ������л�Ϊ�սἼ��������
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).IsName(ultimateAttackState));

            projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);
            projectileTrans = new ProjectileTrans(MainTarget.GameObject.transform.position, Quaternion.identity);
            ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_HeartUltimateSkill, projectileTrans, projectileData, vFXInfo);
            // �ȴ���������Ч����
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).normalizedTime >= 0.9f && !vFXInfo.IsAlive);
        }
    }
}
