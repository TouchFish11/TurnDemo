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
    /// 法师终结技逻辑类
    /// 继承自终结技基类，实现法师的特效、动画和释放逻辑
    /// </summary>
    public class WizardUltimateSkill : UltimateSkill
    {
        // 终结技技攻击动画状态名称
        private readonly string ultimateAttackState = "UltimateAttack";
        
        public WizardUltimateSkill(IBattleEntityObject caster, int skillId, BinaryDataManager binaryDataManager) : base(caster, skillId, binaryDataManager)
        {
        }

        /// <summary>
        /// 初始化投射物和释放姿势的特效数据
        /// </summary>
        protected override async void InitProjectileAndPoseVfx()
        {
            // 初始化投射物数据（施法者、主目标、所有目标、当前技能）
            projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);
            // 初始化投射物变换信息（基于施法者位置，无旋转）
            projectileTrans = new ProjectileTrans(Caster.GameObject.transform.position, Quaternion.identity);
            // 初始化特效信息容器
            vFXInfo = new VFXInfo();
            // 创建释放姿势的特效
            await DIContainer.GetInstance<IVFXManager>().CreateVFX(AssetKeys.VFX_WizardUltimatePose, projectileTrans, projectileData, vFXInfo);
        }

        /// <summary>
        /// 终结技释放的核心逻辑
        /// </summary>
        /// <param name="context">战斗上下文</param>
        /// <returns>协程迭代器</returns>
        protected override IEnumerator OnUltimateCast(IBattleContext context)
        {
            // 获取施法者的动画组件
            var animationComponent = Caster.GetComponent<IBattleAnimationComponent>();
            // 设置技能对应的动画状态
            animationComponent.SetAnimationState(SkillInfo.f_animationType);
            
            // 等待动画切换到终结技攻击状态
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).IsName(ultimateAttackState));

            // 重新初始化投射物数据（目标为主要攻击目标）
            projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);
            // 更新投射物变换信息（基于主目标位置，无旋转）
            projectileTrans = new ProjectileTrans(MainTarget.GameObject.transform.position, Quaternion.identity);
            // 创建终结技核心特效（命中目标处）
            yield return TaskUtility.WaitForTask(DIContainer.GetInstance<IVFXManager>()
                .CreateVFX(AssetKeys.VFX_WizardUltimateSkill, projectileTrans, projectileData, vFXInfo));
            
            // 等待动画播放到90%且特效已结束，确保技能流程完成
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).normalizedTime >= 0.9f && !vFXInfo.IsAlive);
        }
    }
}