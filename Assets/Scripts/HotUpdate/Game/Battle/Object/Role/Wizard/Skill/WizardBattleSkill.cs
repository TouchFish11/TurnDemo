using System.Collections;
using Core.DI;
using Core.Utility;
using HotUpdate.Base;
using HotUpdate.Base.Animation;
using HotUpdate.Base.Component;
using HotUpdate.Base.Utility;
using HotUpdate.Common;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Wizard.Skill
{
    /// <summary>
    /// 法师战技技能核心逻辑类
    /// 继承自玩家技能基类，实现法师战技的释放、动画、特效等核心逻辑
    /// </summary>
    public class WizardBattleSkill : PlayerSkill
    {
        // 战斗攻击动画状态名（与Animator中状态名对应）
        private readonly string battleAttackState = "BattleAttack";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="caster">技能释放者（战斗实体）</param>
        /// <param name="skillId">技能ID</param>
        public WizardBattleSkill(IBattleEntityObject caster, int skillId) : base(caster, skillId)
        {
        }

        /// <summary>
        /// 初始化投射物数据（技能弹道/特效载体相关）
        /// </summary>
        protected override void InitProjectile()
        {
            // 初始化投射物核心数据（释放者、主目标、所有目标、当前技能实例）
            projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);
            // 初始化投射物变换信息（位置为目标位置，旋转默认）
            projectileTrans = new ProjectileTrans(MainTarget.GameObject.transform.position, Quaternion.identity);
            // 初始化特效信息容器
            vFXInfo = new VFXInfo();
        }

        /// <summary>
        /// 技能释放核心协程逻辑
        /// </summary>
        /// <param name="context">战斗上下文（包含战斗场景、规则等核心信息）</param>
        /// <returns>协程迭代器</returns>
        protected override IEnumerator OnCast(IBattleContext context)
        {
            // 获取释放者的动画组件，用于播放技能动画
            var animationComponent = Caster.GetComponent<IBattleAnimationComponent>();
            // 设置动画状态（从技能配置中读取动画类型）
            animationComponent.SetAnimationState(SkillInfo.f_animationType);
            
            // 等待动画播放到"战斗攻击"状态（确保动画执行到攻击帧再触发后续逻辑）
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).IsName(battleAttackState));
            
            // 触发技能特效：通过特效管理器创建战技特效
            yield return TaskUtility.WaitForTask(DIContainer.GetInstance<IVFXManager>()
                .CreateVFX(ResKeyCollection.VFX_WizardBattleSkill, projectileTrans, projectileData, vFXInfo));
            
            // 等待动画播放至90%且特效已结束，确保技能完整执行后再结束协程
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).normalizedTime >= 0.9f && !vFXInfo.IsAlive);
        }
    }
}