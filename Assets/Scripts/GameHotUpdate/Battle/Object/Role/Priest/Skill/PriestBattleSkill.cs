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

namespace GameHotUpdate.Battle.Object.Role.Priest.Skill
{
    /// <summary>
    /// 牧师战技技能逻辑类
    /// 继承自玩家技能基类，实现牧师普通攻击（战技）的核心逻辑，包括动画播放、特效创建、流程等待等
    /// </summary>
    public class PriestBattleSkill : PlayerSkill
    {
        // 动画状态名称常量：普攻攻击状态（与Animator中状态名对应）
        private const string battleAttackState = "BattleAttack";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="caster">施法者实体（当前释放技能的牧师）</param>
        /// <param name="skillId">技能ID（用于读取技能配置）</param>
        /// <param name="statusAddStrategy">状态添加策略（处理技能附带的状态效果）</param>
        public PriestBattleSkill(IBattleEntityObject caster, int skillId, IStatusAddStrategy statusAddStrategy) : base(caster, skillId, statusAddStrategy)
        {
        }
        
        /// <summary>
        /// 初始化投射物数据（重写基类方法）
        /// 初始化技能投射物相关的基础数据、位置信息、特效信息
        /// </summary>
        protected override void InitProjectile()
        {
            // 初始化投射物核心数据（关联施法者、目标、技能本身）
            projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);
            // 初始化投射物位置（以主目标的位置为基准，旋转为默认）
            projectileTrans = new ProjectileTrans(MainTarget.GameObject.transform.position, Quaternion.identity);
            // 初始化特效信息容器（用于记录特效的生命周期等状态）
            vFXInfo = new VFXInfo();
        }

        /// <summary>
        /// 技能释放核心逻辑（重写基类方法，协程执行）
        /// 处理技能释放过程中的动画、特效、流程等待等逻辑
        /// </summary>
        /// <param name="context">战斗上下文（包含战斗场景、规则等核心信息）</param>
        /// <returns>协程迭代器</returns>
        protected override IEnumerator OnCast(IBattleContext context)
        {
            // 获取施法者身上的战斗动画组件，用于控制普攻动画播放
            var animationComponent = Caster.GetComponent<BattleAnimationComponent>();
            // 从技能配置中读取动画类型，并设置到动画组件中（触发对应动画播放）
            animationComponent.SetAnimationState((E_AnimationType)SkillInfo.f_animationType);
            
            // 等待动画播放到"战技"状态（确保动画执行到帧再执行后续的特效逻辑，保证表现和逻辑同步）
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).IsName(battleAttackState));
            
            // 通过特效管理器创建牧师战技特效
            ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_Priest_BattleSkill, projectileTrans, projectileData, vFXInfo);
            
            // 等待动画播放进度达到90%以上，且特效已播放完毕，确保技能的动画和特效都完成后再结束技能流程，避免流程提前终止导致表现异常
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).normalizedTime >= 0.9f && !vFXInfo.IsAlive);
        }
    }
}