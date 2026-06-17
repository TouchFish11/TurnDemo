using System.Collections;
using Core.DI;
using Core.Serialize.Binary;
using Core.Utility;
using HotUpdate.Base.Component;
using HotUpdate.Base.Utility;

using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Warrior.Skill
{
    /// <summary>
    /// 战士终结技技能核心逻辑
    /// 处理终结技的前置特效、瞬移到目标、动画播放、特效创建、位置重置
    /// </summary>
    public class WarriorUltimateSkill : UltimateSkill
    {
        // 复用等待对象
        private static readonly WaitForSeconds s_waitForSeconds0_1 = new(0.1f);
        // 终结技攻击动画状态名称
        private const string ultimateAttackState = "UltimateAttack";
        
        public WarriorUltimateSkill(IBattleEntityObject caster, int skillId, BinaryDataManager binaryDataManager) : base(caster, skillId, binaryDataManager)
        {
        }

        /// <summary>
        /// 初始化终结技前置特效和投射物数据
        /// </summary>
        protected override async void InitProjectileAndPoseVfx()
        {
            // 初始化投射物基础数据
            projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);
            projectileTrans = new ProjectileTrans(Caster.GameObject.transform.position, Quaternion.identity);
            vFXInfo = new VFXInfo();
            
            // 创建终结技前置POSE特效（展示特效）
            await DIContainer.GetInstance<IVFXManager>().CreateVFX(AssetKeys.VFX_WarriorUltimatePose, projectileTrans, projectileData, vFXInfo);
        }

        /// <summary>
        /// 终结技释放核心逻辑
        /// </summary>
        /// <param name="context">战斗上下文</param>
        /// <returns>协程迭代器</returns>
        protected override IEnumerator OnUltimateCast(IBattleContext context)
        {
            // 瞬移到目标身前（目标位置向前偏移，避免重叠）
            Caster.GameObject.transform.position = MainTarget.GameObject.transform.position - Vector3.forward;

            // 等待0.1秒（瞬移后缓冲）
            yield return s_waitForSeconds0_1;

            // 切换终结技动画
            var animationComponent = Caster.GetComponent<IBattleAnimationComponent>(); 
            animationComponent.SetAnimationState(SkillInfo.f_animationType);
            
            // 等待动画切换到终结技攻击状态
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).IsName(ultimateAttackState));

            // 初始化终结技核心特效数据（位置上移0.9米，避免穿模）
            projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);
            projectileTrans = new ProjectileTrans(Caster.GameObject.transform.position + Vector3.up * 0.9f, Quaternion.identity);
            
            // 创建终结技核心攻击特效
            yield return TaskUtility.WaitForTask(DIContainer.GetInstance<IVFXManager>().CreateVFX(
                AssetKeys.VFX_WarriorUltimateSkill,
                projectileTrans, projectileData, vFXInfo));
            
            // 等待动画播放到90%（确保特效播放完成）
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).normalizedTime >= 0.9f);

            // 重置角色位置到战斗初始点位
            Caster.GameObject.transform.position = battleCoordinator.GetRoleTransByIndex(Caster.EntityPosIndex);

            // 等待0.1秒（位移后缓冲）
            yield return s_waitForSeconds0_1;
        }
    }
}