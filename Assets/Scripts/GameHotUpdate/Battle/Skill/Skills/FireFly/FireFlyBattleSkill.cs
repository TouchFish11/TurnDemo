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
    /// FireFlyս��
    /// </summary>
    public class FireFlyBattleSkill : PlayerSkill
    {
        private readonly string battleAttackState = "BattleAttack";

        public FireFlyBattleSkill(IBattleEntityObject caster, int skillId, IStatusAddStrategy statusAddStrategy) : base(caster, skillId, statusAddStrategy)
        {

        }

        protected override void OnPreCast(IBattleContext context)
        {
            base.OnPreCast(context);
            projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);
            projectileTrans = new ProjectileTrans(MainTarget.GameObject.transform.position, Quaternion.LookRotation(-Caster.GameObject.transform.right));
            vFXInfo = new VFXInfo();
        }

        protected override IEnumerator OnCast(IBattleContext context)
        {
            // ���Ŷ���
            context.GetEventBus().TriggerEvent(new SkillCastEvent(context, this, 0));
            BattleAnimationComponent animationComponent = Caster.GetComponent<BattleAnimationComponent>();
            // �ȴ������л�Ϊս������
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).IsName(battleAttackState));
            // ������Ч
            ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_FireFlyBattleSkill, projectileTrans, projectileData, vFXInfo);
            // �ȴ���������Ч����
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).normalizedTime >= 0.9f && !vFXInfo.IsAlive);
        }
    }
}
