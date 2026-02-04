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

namespace GameHotUpdate.Battle.Skill.Skills.Herta
{
    /// <summary>
    /// Hertaս��
    /// </summary>
    public class HertaBattleSkill : PlayerSkill
    {
        private readonly string battleAttackState = "BattleAttack";

        public HertaBattleSkill(IBattleEntityObject caster, int skillId, IStatusAddStrategy statusAddStrategy) : base(caster, skillId, statusAddStrategy)
        {

        }

        protected override void OnPreCast(IBattleContext context)
        {
            base.OnPreCast(context);
            projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);
            projectileTrans = new ProjectileTrans(MainTarget.GameObject.transform.position, Quaternion.identity);
            vFXInfo = new VFXInfo();
        }

        protected override IEnumerator OnCast(IBattleContext context)
        {
            // ���Ŷ���
            BattleAnimationComponent animationComponent = Caster.GetComponent<BattleAnimationComponent>();
            animationComponent.SetAnimationState((E_AnimationType)SkillInfo.f_animationType);
            
            // �ȴ������л�Ϊս������
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).IsName(battleAttackState));
            // ������Ч
            ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_HertaBattleSkill, projectileTrans, projectileData, vFXInfo);
            // �ȴ���������
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).normalizedTime >= 0.9f && !vFXInfo.IsAlive);
        }
    }
}
