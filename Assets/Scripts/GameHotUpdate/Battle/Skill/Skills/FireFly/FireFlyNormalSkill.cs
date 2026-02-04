using System.Collections;
using Core.Config;
using Core.Service;
using Game.Animation;
using Game.Battle.Context;
using Game.Battle.Objects;
using Game.Battle.Status;
using Game.VFX;
using GameHotUpdate.Animation;
using UnityEngine;

namespace GameHotUpdate.Battle.Skill.Skills.FireFly
{
    /// <summary>
    /// FireFly�չ�
    /// </summary>
    public class FireFlyNormalSkill : PlayerSkill
    {
        private static WaitForSeconds _waitForSeconds0_35 = new WaitForSeconds(0.35f);

        private readonly string rollState = "Roll";
        private readonly string attackState = "Attack";

        private Vector3 localVfx = new Vector3(-90, 180, 0);
        private Transform vfxTrans;

        public FireFlyNormalSkill(IBattleEntityObject caster, int skillId, IStatusAddStrategy statusAddStrategy) : base(caster, skillId, statusAddStrategy)
        {

        }

        protected override IEnumerator OnCast(IBattleContext context)
        {
            // ���Ŷ���
            BattleAnimationComponent animationComponent = Caster.GetComponent<BattleAnimationComponent>();
            animationComponent.SetAnimationState((E_AnimationType)SkillInfo.f_animationType);
            Animator animator = animationComponent.GetAnimator();

            // �ȴ������л�Ϊ��������
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).IsName(rollState));

            // ������Ч
            projectileTrans = new ProjectileTrans(Caster.GameObject.transform.position, Quaternion.identity);
            vFXInfo = new VFXInfo();
            ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_NormalSkill_Wave, projectileTrans, default, vFXInfo);

            // ��ʼƥ��Ŀ��
            Vector3 matchPos = MainTarget.GameObject.transform.position - Vector3.forward * 1.5f;
            Quaternion matchRot = Quaternion.identity;
            MatchTargetWeightMask mask = new MatchTargetWeightMask(new Vector3(1, 0, 1), 0);
            animator.MatchTarget(matchPos, matchRot, AvatarTarget.Body, mask, 0.28f);

            // �ȴ������л�Ϊ�չ�����
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).IsName(attackState));

            // ������Ч
            projectileTrans = new ProjectileTrans(Caster.SubGameObject.transform.position + Vector3.up, Quaternion.Euler(180, 180, 0));
            projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);
            vFXInfo = new VFXInfo();
            ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_FireFlyNormalSkill, projectileTrans, projectileData, vFXInfo);

            // �ȴ���������Ч����
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).normalizedTime >= 0.9f && !vFXInfo.IsAlive);
            yield return new WaitForSeconds(0.15f);

            // �ص���ʼλ��
            animator.transform.localPosition = Vector3.zero;
        }
    }
}
