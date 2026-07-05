using System.Collections;
using Core.Utility;
using HotUpdate.Base.Utility;
using HotUpdate.Game.Animation.Component;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Warrior.Skill.Normal
{
    public class WarriorNormalSkillCastPhaseStrategy : SkillCastPhaseStrategy
    {
        // 翻滚动画状态名称
        private const string RollState = "Roll";
        // 攻击动画状态名称
        private const string AttackState = "Attack";
        
        public override IEnumerator Execute()
        {
            var caster = SkillContext.Caster;
            var mainTarget = SkillContext.MainTarget;
            var skillInfo = SkillContext.SkillInfo;
            
            // 获取动画组件并切换到普攻动画
            var animationComponent = caster.GetComponent<BattleAnimationComponent>();
            animationComponent.SetAnimationState(skillInfo.f_animationType);
            var animator = animationComponent.Animator;

            // 等待动画切换到翻滚状态
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).IsName(RollState));

            // 初始化第一段普攻特效（波浪特效）
            var projectileTrans = new ProjectileTrans(caster.GameObject.transform.position, Quaternion.identity);
            var vFXInfo = poolManager.GetData<VFXInfo>();
            yield return TaskUtility.WaitForTask(vfxManager.CreateVFX(AssetKeys.VFX_NormalSkill_Wave, projectileTrans, default, vFXInfo));

            // 动画匹配目标位置（让角色朝向/移动到目标位置）
            var matchPos = mainTarget.GameObject.transform.position - Vector3.forward * 1.5f; // 目标前1.5米位置
            var matchRot = Quaternion.identity;
            var mask = new MatchTargetWeightMask(new Vector3(1, 0, 1), 0); // 仅匹配X/Z轴
            animator.MatchTarget(matchPos, matchRot, AvatarTarget.Body, mask, 0.28f); // 0.28秒内完成匹配

            // 等待动画切换到攻击状态
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).IsName(AttackState));

            // 初始化第二段普攻特效（核心攻击特效）
            SkillContext.ProjectileTrans = new ProjectileTrans(caster.SubGameObject.transform.position + Vector3.up, Quaternion.Euler(180, 180, 0));
            SkillContext.ProjectileData = new ProjectileData(caster, mainTarget, SkillContext.AllTargets, SkillContext);
            SkillContext.VFXInfo = poolManager.GetData<VFXInfo>();
            var task = vfxManager.CreateVFX(AssetKeys.VFX_WarriorNormalSkill, projectileTrans, SkillContext.ProjectileData, SkillContext.VFXInfo);
            yield return SkillHelper.WaitForCreateVFX(SkillContext, task);
        }
    }
}
