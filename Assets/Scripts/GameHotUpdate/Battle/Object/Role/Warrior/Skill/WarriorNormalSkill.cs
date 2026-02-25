using System.Collections;
using Core.Service;
using Game.Animation;
using Game.Battle.Context;
using Game.Battle.Objects;
using Game.Battle.Status;
using Game.VFX;
using GameHotUpdate.Animation;
using GameHotUpdate.Battle.Skill.Base;
using GameHotUpdate.Config;
using UnityEngine;

namespace GameHotUpdate.Battle.Object.Role.Warrior.Skill
{
    /// <summary>
    /// 战士普攻技能核心逻辑
    /// 处理普攻的两段特效、动画匹配目标、位置重置等流程
    /// </summary>
    public class WarriorNormalSkill : PlayerSkill
    {
        // 复用0.05秒等待对象
        private static WaitForSeconds _waitForSeconds0_05 = new(0.05f);

        // 翻滚动画状态名称
        private const string rollState = "Roll";
        // 攻击动画状态名称
        private const string attackState = "Attack";
        // 特效本地偏移角度
        private Vector3 localVfx = new Vector3(-90, 180, 0);
        private Transform vfxTrans;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="caster">施法者实体</param>
        /// <param name="skillId">技能ID</param>
        /// <param name="statusAddStrategy">状态添加策略</param>
        public WarriorNormalSkill(IBattleEntityObject caster, int skillId, IStatusAddStrategy statusAddStrategy) : base(caster, skillId, statusAddStrategy)
        {
        }

        /// <summary>
        /// 初始化投射物
        /// </summary>
        protected override void InitProjectile()
        {
            /* 该技能不需要实现 */
        }

        /// <summary>
        /// 普攻释放核心逻辑
        /// </summary>
        /// <param name="context">战斗上下文</param>
        /// <returns>协程迭代器</returns>
        protected override IEnumerator OnCast(IBattleContext context)
        {
            // 获取动画组件并切换到普攻动画
            var animationComponent = Caster.GetComponent<BattleAnimationComponent>();
            animationComponent.SetAnimationState((E_AnimationType)SkillInfo.f_animationType);
            var animator = animationComponent.GetAnimator();

            // 等待动画切换到翻滚状态
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).IsName(rollState));

            // 初始化第一段普攻特效（波浪特效）
            projectileTrans = new ProjectileTrans(Caster.GameObject.transform.position, Quaternion.identity);
            vFXInfo = new VFXInfo();
            ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_NormalSkill_Wave, projectileTrans, default, vFXInfo);

            // 动画匹配目标位置（让角色朝向/移动到目标位置）
            var matchPos = MainTarget.GameObject.transform.position - Vector3.forward * 1.5f; // 目标前1.5米位置
            var matchRot = Quaternion.identity;
            var mask = new MatchTargetWeightMask(new Vector3(1, 0, 1), 0); // 仅匹配X/Z轴
            animator.MatchTarget(matchPos, matchRot, AvatarTarget.Body, mask, 0.28f); // 0.28秒内完成匹配

            // 等待动画切换到攻击状态
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).IsName(attackState));

            // 初始化第二段普攻特效（核心攻击特效）
            projectileTrans = new ProjectileTrans(Caster.SubGameObject.transform.position + Vector3.up, Quaternion.Euler(180, 180, 0));
            projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);
            vFXInfo = new VFXInfo();
            ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_WarriorNormalSkill, projectileTrans, projectileData, vFXInfo);

            // 等待动画播放到90%且特效结束
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).normalizedTime >= 0.9f && !vFXInfo.IsAlive);

            // 重置角色本地位置（防止动画位移残留）
            animator.transform.localPosition = Vector3.zero;
        }
    }
}