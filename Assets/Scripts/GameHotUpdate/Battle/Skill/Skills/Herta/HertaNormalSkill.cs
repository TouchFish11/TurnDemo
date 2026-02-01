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
    /// Herta�չ�
    /// </summary>
    public class HertaNormalSkill : PlayerSkill
    {
        private readonly string attackState = "NormalAttack";

        public HertaNormalSkill(IBattleEntityObject caster, int skillId, ISkillCastPostHandler postHandler, IStatusAddStrategy statusAddStrategy) : base(caster, skillId, postHandler, statusAddStrategy)
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
            context.GetEventBus().TriggerEvent(new SkillCastEvent(context, this, 0));
            BattleAnimationComponent animationComponent = Caster.GetComponent<BattleAnimationComponent>();
            // �ȴ������л�Ϊ�չ�����
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).IsName(attackState));
            // ������Ч
            ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_HertaNormalSkill, projectileTrans, projectileData, vFXInfo);
            // �ȴ���������
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).normalizedTime >= 0.9f && !vFXInfo.IsAlive);
        }
    }
}
