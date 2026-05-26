using System.Collections;
using Core.DI;
using Core.Utility;
using HotUpdate.Base;
using HotUpdate.Base.Component;
using HotUpdate.Base.Utility;
using HotUpdate.Common.Generated;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.VFX;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Role.Warrior.Skill
{
    /// <summary>
    /// 战士战技技能核心逻辑
    /// 处理战技的动画播放、特效创建及流程等待
    /// </summary>
    public class WarriorBattleSkill : PlayerSkill
    {
        // 战斗攻击动画状态名称
        private const string battleAttackState = "BattleAttack";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="caster">施法者实体</param>
        /// <param name="skillId">技能ID</param>
        public WarriorBattleSkill(IBattleEntityObject caster, int skillId) : base(caster, skillId)
        {
        }

        /// <summary>
        /// 初始化投射物数据（战技所需的特效、目标等数据）
        /// </summary>
        protected override void InitProjectile()
        {
            // 初始化投射物数据（施法者、主目标、所有目标、当前技能）
            projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);
            // 设置投射物位置（主目标位置）和旋转（朝向施法者右侧反方向）
            projectileTrans = new ProjectileTrans(MainTarget.GameObject.transform.position, 
                Quaternion.LookRotation(-Caster.GameObject.transform.right));
            // 初始化特效信息
            vFXInfo = new VFXInfo();
        }

        /// <summary>
        /// 技能释放核心逻辑
        /// </summary>
        /// <param name="context">战斗上下文（包含战斗场景、实体等信息）</param>
        /// <returns>协程迭代器</returns>
        protected override IEnumerator OnCast(IBattleContext context)
        {
            // 获取施法者的动画组件
            var animationComponent = Caster.GetComponent<IBattleAnimationComponent>();
            // 切换到技能配置的动画状态
            animationComponent.SetAnimationState(SkillInfo.f_animationType);
            // 等待动画切换到战斗攻击状态
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).IsName(battleAttackState));
            // 创建战技特效
            yield return TaskUtility.WaitForTask(DIContainer.GetInstance<IVFXManager>()
                .CreateVFX(AssetKeys.VFX_WarriorBattleSkill, projectileTrans, projectileData, vFXInfo));
            // 等待动画播放到90%且特效已结束
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationUtility.Skill_Layer_Name).normalizedTime >= 0.9f && !vFXInfo.IsAlive);
        }
    }
}