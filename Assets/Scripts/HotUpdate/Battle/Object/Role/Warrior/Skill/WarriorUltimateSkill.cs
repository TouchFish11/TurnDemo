using System.Collections;
using Core.Service;
using Core.Utility;
using HotUpdate.Battle.Skill.Base;
using HotUpdate.Common;
using HotUpdate.Core.Animation;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Object;
using HotUpdate.Core.VFX;
using UnityEngine;

namespace HotUpdate.Battle.Object.Role.Warrior.Skill
{
    /// <summary>
    /// 战士终结技技能核心逻辑
    /// 处理终结技的前置特效、瞬移到目标、动画播放、特效创建、位置重置
    /// </summary>
    public class WarriorUltimateSkill : UltimateSkill
    {
        // 复用0.25秒等待对象
        private static readonly WaitForSeconds _waitForSeconds0_25 = new(0.25f);
        // 终结技攻击动画状态名称
        private const string ultimateAttackState = "UltimateAttack";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="caster">施法者实体</param>
        /// <param name="skillId">技能ID</param>
        public WarriorUltimateSkill(IBattleEntityObject caster, int skillId) : base(caster, skillId)
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
            await ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_WarriorUltimatePose, projectileTrans, projectileData, vFXInfo);
        }

        /// <summary>
        /// 终结技释放核心逻辑
        /// </summary>
        /// <param name="context">战斗上下文</param>
        /// <returns>协程迭代器</returns>
        protected override IEnumerator OnUltimateCast(IBattleContext context)
        {
            // 瞬移到目标身前（目标位置向前偏移，避免重叠）
            var targetPos = MainTarget.GameObject.transform.position;
            Caster.GameObject.transform.position = targetPos - Vector3.forward;

            // 等待0.25秒（瞬移后缓冲）
            yield return _waitForSeconds0_25;

            // 切换终结技动画
            var animationComponent = Caster.GetComponent<IBattleAnimationComponent>(); 
            animationComponent.SetAnimationState(SkillInfo.f_animationType);
            
            // 等待动画切换到终结技攻击状态
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).IsName(ultimateAttackState));

            // 初始化终结技核心特效数据（位置上移0.9米，避免穿模）
            projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);
            projectileTrans = new ProjectileTrans(Caster.GameObject.transform.position + Vector3.up * 0.9f, Quaternion.identity);
            
            // 创建终结技核心攻击特效
            yield return TaskUtility.WaitForTask(ServiceLocator.Get<IVFXManager>().CreateVFX(
                ResKeyCollection.VFX_WarriorUltimateSkill,
                projectileTrans, projectileData, vFXInfo));
            
            // 等待动画播放到90%（确保特效播放完成）
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).normalizedTime >= 0.9f);

            // 重置角色位置到战斗初始点位
            targetPos = context.GetProxy().BattlePoint.GetRoleTransByIndex(Caster.EntityPosIndex).position;
            Caster.GameObject.transform.position = targetPos;

            // 等待0.25秒（位移后缓冲）
            yield return _waitForSeconds0_25;
        }
    }
}