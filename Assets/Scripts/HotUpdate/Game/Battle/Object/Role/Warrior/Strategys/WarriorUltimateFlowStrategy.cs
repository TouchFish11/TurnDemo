using System.Collections;
using Core.Utility;
using HotUpdate.Base.Component;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Warrior.Strategys
{
    public class WarriorUltimateFlowStrategy : UltimateFlowStrategy
    {
        private static readonly WaitForSeconds s_waitForSeconds0_1 = new(0.1f);
        // 终结技攻击动画状态名称
        private const string UltimateAttackState = "UltimateAttack";
        
        protected override IEnumerator OnExecuteFlow()
        {
            // 瞬移到目标身前（目标位置向前偏移，避免重叠）
            skillContext.Caster.GameObject.transform.position = skillContext.MainTarget.GameObject.transform.position - Vector3.forward;

            // 等待0.1秒（瞬移后缓冲）
            yield return s_waitForSeconds0_1;

            // 切换终结技动画
            var animationComponent = skillContext.Caster.GetComponent<IBattleAnimationComponent>(); 
            animationComponent.SetAnimationState(skillContext.SkillInfo.f_animationType);
            
            // 等待动画切换到终结技攻击状态
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).IsName(UltimateAttackState));

            // 初始化终结技核心特效数据（位置上移0.9米，避免穿模）
            skillContext.ProjectileData = new ProjectileData(skillContext.Caster, skillContext.MainTarget, skillContext.AllTargets, this);
            skillContext.ProjectileTrans = new ProjectileTrans(skillContext.Caster.GameObject.transform.position + Vector3.up * 0.9f, Quaternion.identity);

            skillContext.VFXInfo = poolManager.GetData<VFXInfo>();
            // 创建终结技核心攻击特效
            yield return TaskUtility.WaitForTask(
                vfxManager.CreateVFX(AssetKeys.VFX_WarriorUltimateSkill, skillContext.ProjectileTrans, skillContext.ProjectileData, skillContext.VFXInfo), 
                projectile => skillContext.Projectile = projectile);
            
            // 等待动画播放到90%（确保特效播放完成）
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).normalizedTime >= 0.9f);

            // 重置角色位置到战斗初始点位
            skillContext.Caster.GameObject.transform.position = battleCoordinator.GetRoleTransByIndex(skillContext.Caster.EntityPosIndex);

            // 等待0.1秒（位移后缓冲）
            yield return s_waitForSeconds0_1;
        }
    }
}
