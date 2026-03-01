using System.Collections;
using Core.Pool;
using Core.Service;
using GameHotUpdate.Animation;
using GameHotUpdate.Animation.Component;
using GameHotUpdate.Battle.Context;
using GameHotUpdate.Battle.Skill.Base;
using GameHotUpdate.Config;
using GameHotUpdate.VFX;
using UnityEngine;

namespace GameHotUpdate.Battle.Object.Role.Priest.Skill
{
    /// <summary>
    /// 牧师普攻技能类
    /// 继承自玩家技能基类，实现牧师普通攻击的核心逻辑
    /// </summary>
    public class PriestNormalSkill : PlayerSkill
    {
        // 普攻动画状态名称常量
        private const string attackState = "NormalAttack";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="caster">施法者战斗实体</param>
        /// <param name="skillId">技能ID</param>
        public PriestNormalSkill(IBattleEntityObject caster, int skillId) : base(caster, skillId)
        {
        }
        
        /// <summary>
        /// 初始化投射物数据
        /// </summary>
        protected override void InitProjectile()
        {
            // 初始化投射物核心数据（施法者、主目标、所有目标、当前技能实例）
            projectileData = new ProjectileData(Caster, MainTarget, AllTargets, this);
            // 初始化投射物变换信息（位置为目标物体位置，旋转为默认）
            projectileTrans = new ProjectileTrans(MainTarget.GameObject.transform.position, Quaternion.identity);
            // 初始化特效信息对象
            vFXInfo = ServiceLocator.Get<IPoolManager>().GetData<VFXInfo>();
        }

        /// <summary>
        /// 技能释放核心逻辑（协程）
        /// </summary>
        /// <param name="context">战斗上下文，包含战斗场景核心数据</param>
        /// <returns>协程迭代器</returns>
        protected override IEnumerator OnCast(IBattleContext context)
        {
            // 获取施法者的动画组件，用于播放普攻动画
            var animationComponent = Caster.GetComponent<BattleAnimationComponent>();
            // 设置技能动画状态（动画类型从技能配置中读取）
            animationComponent.SetAnimationState((E_AnimationType)SkillInfo.f_animationType);
            
            // 等待动画播放到普攻状态（确保动画执行到攻击帧再执行后续逻辑）
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).IsName(attackState));
            
            // 创建牧师普攻特效（通过特效管理器加载指定特效资源）
            ServiceLocator.Get<IVFXManager>().CreateVFX(ResKeyCollection.VFX_Priest_NormalSkill, projectileTrans, projectileData, vFXInfo);
            
            // 等待动画播放到90%以上且特效已结束，确保技能流程完整后再结束协程
            yield return new WaitUntil(() => animationComponent.GetCurrentAnimatorStateInfo(AnimationComponent.Skill_Layer_Name).normalizedTime >= 0.9f && !vFXInfo.IsAlive);
        }
    }
}