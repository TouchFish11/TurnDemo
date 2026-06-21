using System.Collections;
using Core.Utility;
using HotUpdate.Base.Component;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Warrior.Skill.Battle
{
    public class WarriorBattleSkillCastPhaseStrategy : SkillCastPhaseStrategy
    {
        // 战斗攻击动画状态名称
        private const string BattleAttackState = "BattleAttack";
        
        public override IEnumerator Execute()
        {
            // 获取施法者的动画组件
            var animationComponent = SkillContext.Caster.GetComponent<IBattleAnimationComponent>();
            // 切换到技能配置的动画状态
            animationComponent.SetAnimationState(SkillContext.SkillInfo.f_animationType);
            // 等待动画切换到战斗攻击状态
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).IsName(BattleAttackState));
            // 创建战技特效
            var task = vfxManager.CreateVFX(AssetKeys.VFX_WarriorBattleSkill, SkillContext.ProjectileTrans, SkillContext.ProjectileData, SkillContext.VFXInfo);
            yield return TaskUtility.WaitForTask(task, projectile => SkillContext.Projectile = projectile);
        }
    }
}
