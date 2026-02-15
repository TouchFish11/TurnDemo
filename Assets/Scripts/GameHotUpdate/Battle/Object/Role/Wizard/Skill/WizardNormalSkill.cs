using System.Collections;
using Core.Config;
using Core.Service;
using Game.Animation;
using Game.Battle.Context;
using Game.Battle.Objects;
using Game.Battle.Status;
using Game.VFX;
using GameHotUpdate.Animation;
using GameHotUpdate.Battle.Skill.Base;
using UnityEngine;

namespace GameHotUpdate.Battle.Object.Role.Wizard.Skill
{
    /// <summary>
    /// 法师普攻技能类
    /// 处理法师普通攻击的动画、特效及释放逻辑
    /// </summary>
    public class WizardNormalSkill : PlayerSkill
    {
        // 普攻动画状态名称
        private readonly string attackState = "NormalAttack";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="caster">施法者实体</param>
        /// <param name="skillId">技能ID</param>
        /// <param name="statusAddStrategy">状态添加策略</param>
        public WizardNormalSkill(IBattleEntityObject caster, int skillId, IStatusAddStrategy statusAddStrategy) : base(caster, skillId, statusAddStrategy)
        {
        }

        /// <summary>
        /// 初始化投射物数据
        /// 包含施法者、目标、特效等基础数据初始化
        /// </summary>
        protected override void InitProjectile()
        {
            // 初始化投射物核心数据（施法者、主目标、所有目标、当前技能）
            projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);
            // 初始化投射物位置（主目标位置）和旋转
            projectileTrans = new ProjectileTrans(MainTarget.GameObject.transform.position, Quaternion.identity);
            // 初始化特效信息容器
            vFXInfo = new VFXInfo();
        }

        /// <summary>
        /// 技能释放核心逻辑（协程）
        /// 处理动画播放、特效触发、动画等待等流程
        /// </summary>
        /// <param name="context">战斗上下文</param>
        /// <returns>协程迭代器</returns>
        protected override IEnumerator OnCast(IBattleContext context)
        {
            // 获取施法者的动画组件
            var animationComponent = Caster.GetComponent<BattleAnimationComponent>();
            // 设置技能对应的动画状态（从配置表读取动画类型）
            animationComponent.SetAnimationState((E_AnimationType)SkillInfo.f_animationType);
            
            // 等待动画播放到普攻状态（确保动画执行到攻击帧再触发后续逻辑）
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).IsName(attackState));
            
            // 创建普攻特效（从资源配置中获取普攻特效资源）
            ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_WizardNormalSkill, projectileTrans, projectileData, vFXInfo);
            
            // 等待动画播放至90%且特效已结束，确保技能流程完整结束
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).normalizedTime >= 0.9f && !vFXInfo.IsAlive);
        }
    }
}