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

namespace GameHotUpdate.Battle.Skill.Skills.FireFly
{
    /// <summary>
    /// FireFly�սἼ
    /// </summary>
    public class FireFlyUltimateSkill : UltimateSkill
    {
        private static WaitForSeconds _waitForSeconds0_25 = new WaitForSeconds(0.25f);
        private readonly string ultimateAttackState = "UltimateAttack";

        public FireFlyUltimateSkill(IBattleEntityObject caster, int skillId, ISkillCastPostHandler postHandler, IStatusAddStrategy statusAddStrategy) : base(caster, skillId, postHandler, statusAddStrategy)
        {

        }

        protected override void OnPreUltimateCast(IBattleContext context)
        {
            base.OnPreUltimateCast(context);

            // ����Ԥ������������սἼpose���սἼ����
            Caster.GetComponent<BattleAnimationComponent>().SetUltimatePose();

            projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);
            projectileTrans = new ProjectileTrans(Caster.GameObject.transform.position, Quaternion.identity);
            vFXInfo = new VFXInfo();
            // ������Ч
            ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_FireFlyUltimatePose, projectileTrans, projectileData, vFXInfo);
        }

        protected override IEnumerator OnUltimateCast(IBattleContext context)
        {
            // �Ƴ�Pose��Ч
            ServiceLocator.Get<IVFXManager>().RemoveVFX(vFXInfo);

            // ���͵���Ŀ����ǰ
            Vector3 targetPos = MainTarget.GameObject.transform.position;
            Caster.GameObject.transform.position = targetPos - Vector3.forward;

            yield return _waitForSeconds0_25;

            context.GetEventBus().TriggerEvent(new SkillCastEvent(context, this, 0));
            BattleAnimationComponent animationComponent = Caster.GetComponent<BattleAnimationComponent>();
            // �ȴ������л�Ϊ�սἼ����
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).IsName(ultimateAttackState));

            projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);
            projectileTrans = new ProjectileTrans(Caster.GameObject.transform.position + Vector3.up * 0.9f, Quaternion.identity);
            // ������Ч
            ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_FireFlyUltimateSkill, projectileTrans, projectileData, vFXInfo);
            // �ȴ���������
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).normalizedTime >= 0.9f);

            // �ص���ʼλ��
            targetPos = BattlePoint.Instance.GetPlayerTransByIndex(Caster.EntityPosIndex).position;
            Caster.GameObject.transform.position = targetPos;

            yield return _waitForSeconds0_25;
        }
    }
}
