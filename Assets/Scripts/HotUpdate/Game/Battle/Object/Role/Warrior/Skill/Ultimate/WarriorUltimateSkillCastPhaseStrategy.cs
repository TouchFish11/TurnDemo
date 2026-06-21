using System.Collections;
using Core.Utility;
using HotUpdate.Base.Component;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Warrior.Skill.Ultimate
{
    public class WarriorUltimateSkillCastPhaseStrategy : SkillCastPhaseStrategy
    {
        // 终结技攻击动画状态名称
        private const string UltimateAttackState = "UltimateAttack";
        
        public override IEnumerator Execute()
        {
            var caster = SkillContext.Caster;
            var mainTarget = SkillContext.MainTarget;
            
            // 瞬移到目标身前（目标位置向前偏移，避免重叠）
            caster.GameObject.transform.position = mainTarget.GameObject.transform.position - Vector3.forward;
            
            // 等待0.1秒（瞬移后缓冲）
            yield return SkillHelper.Delay(100);
            
            // 切换终结技动画
            var animationComponent = caster.GetComponent<IBattleAnimationComponent>(); 
            animationComponent.SetAnimationState(SkillContext.SkillInfo.f_animationType);
            
            // 等待动画切换到终结技攻击状态
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).IsName(UltimateAttackState));
            
            // 初始化终结技核心特效数据（位置上移0.9米，避免穿模）
            SkillContext.ProjectileData = new ProjectileData(caster, mainTarget, SkillContext.AllTargets, SkillContext);
            SkillContext.ProjectileTrans = new ProjectileTrans(caster.GameObject.transform.position + Vector3.up * 0.9f, Quaternion.identity);
            SkillContext.VFXInfo = poolManager.GetData<VFXInfo>();
            
            // 创建终结技核心攻击特效
            yield return TaskUtility.WaitForTask(
                vfxManager.CreateVFX(AssetKeys.VFX_WarriorUltimateSkill, SkillContext.ProjectileTrans, SkillContext.ProjectileData, SkillContext.VFXInfo), 
                projectile => SkillContext.Projectile = projectile);
        }
    }
}
