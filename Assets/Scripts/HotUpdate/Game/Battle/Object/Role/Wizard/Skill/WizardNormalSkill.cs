using System.Collections;
using Core.DI;
using Core.Serialize.Binary;
using Core.Utility;
using HotUpdate.Base;
using HotUpdate.Base.Component;
using HotUpdate.Base.Utility;

using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Wizard.Skill
{
    /// <summary>
    /// 法师普攻技能类
    /// 处理法师普通攻击的动画、特效及释放逻辑
    /// </summary>
    public class WizardNormalSkill : PlayerSkill
    {
        // 普攻动画状态名称
        private readonly string attackState = "NormalAttack";
        
        public WizardNormalSkill(IBattleEntityObject caster, int skillId, BinaryDataManager binaryDataManager) : base(caster, skillId, binaryDataManager)
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
            var animationComponent = Caster.GetComponent<IBattleAnimationComponent>();
            // 设置技能对应的动画状态（从配置表读取动画类型）
            animationComponent.SetAnimationState(SkillInfo.f_animationType);
            
            // 等待动画播放到普攻状态（确保动画执行到攻击帧再触发后续逻辑）
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).IsName(attackState));
            
            // 创建普攻特效（从资源配置中获取普攻特效资源）
            yield return TaskUtility.WaitForTask(DIContainer.GetInstance<IVFXManager>()
                .CreateVFX(AssetKeys.VFX_WizardNormalSkill, projectileTrans, projectileData, vFXInfo));
            
            // 等待动画播放至90%且特效已结束，确保技能流程完整结束
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).normalizedTime >= 0.9f && !vFXInfo.IsAlive);
        }
    }
}