using System.Collections;
using Core.Service;
using HotUpdate.Battle.Event.Skill;
using HotUpdate.Battle.Skill.Component;
using HotUpdate.Core.Animation;
using HotUpdate.Core.Battle;
using HotUpdate.Core.Battle.Event;
using HotUpdate.Core.Battle.Object;
using HotUpdate.Core.Battle.Property;
using HotUpdate.Core.Battle.Skill;
using HotUpdate.Core.VFX;
using UnityEngine;

namespace HotUpdate.Battle.Skill.Base
{
    /// <summary>
    /// 终结技（必杀技）抽象基类
    /// 所有角色的终极技能都需继承此类并实现核心释放逻辑
    /// </summary>
    public abstract class UltimateSkill : PlayerSkill
    {
        /// <summary>
        /// 技能组件引用（用于判断技能释放状态）
        /// </summary>
        private readonly PlayerSkillComponent skillComponent;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="caster">技能释放者（战斗实体）</param>
        /// <param name="skillId">技能ID</param>
        protected UltimateSkill(IBattleEntityObject caster, int skillId) : base(caster, skillId)
        {
            // 从释放者身上获取技能组件，用于后续判断释放状态
            skillComponent = Caster.GetComponent<PlayerSkillComponent>();
        }

        /// <summary>
        /// 终结技不需要重写方法
        /// </summary>
        protected sealed override void InitProjectile()
        {
            
        }
        
        /// <summary>
        /// 初始化终结技弹射物数据和Pose特效
        /// 终结技子类重写
        /// </summary>
        protected abstract void InitProjectileAndPoseVfx();

        /// <summary>
        /// 终结技触发处理逻辑
        /// </summary>
        /// <returns>协程迭代器</returns>
        private IEnumerator OnUltimateTrigger()
        {
            // 终结技动画Pose
            Caster.GetComponent<IBattleAnimationComponent>().SetUltimatePose();
            InitProjectileAndPoseVfx();
            yield return ServiceLocator.Get<IBattleEventScheduler>().PreUltimateCastDispatch(Caster, SkillInfo);
        }

        /// <summary>
        /// 终结技释放前逻辑
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private void OnPreUltimateCast(IBattleContext context)
        {
            // 移除Pose特效
            ServiceLocator.Get<IVFXManager>().RemoveVFX(vFXInfo);
            // 清空释放者当前能量（终结技消耗所有能量）
            PropertyComponent.SetPropertyValue(E_DynamicPropertyType.CurrentEnergy, 0);
            // 初始化技能目标
            ServiceLocator.Get<ISkillManager>().InitSkillTarget(this);
            // 终结释放通用逻辑、禁用输入、更新UI显示
            context.GetEventBus().TriggerEvent(new UltimateCastEvent(context));
        }
        
        /// <summary>
        /// 终结技核心释放逻辑
        /// 子类必须实现此方法，定义具体的技能效果（如伤害、控制、召唤等）
        /// </summary>
        /// <param name="context">战斗上下文</param>
        /// <returns>协程迭代器</returns>
        protected abstract IEnumerator OnUltimateCast(IBattleContext context);
        
        /// <summary>
        /// 技能释放核心流程（重写父类方法）
        /// 封装终结技释放的通用流程，子类仅需实现具体释放逻辑
        /// </summary>
        /// <param name="context">战斗上下文（包含战斗场景、实体、规则等核心信息）</param>
        /// <returns>协程迭代器</returns>
        protected sealed override IEnumerator OnCast(IBattleContext context)
        {
            // 执行终结技触发逻辑
            yield return OnUltimateTrigger();
            
            // 待技能组件确认释放（阻塞直到释放条件满足）
            yield return new WaitUntil(() => skillComponent.IsRelease);
            
            // 执行终结技释放前逻辑
            OnPreUltimateCast(context);
            
            // 执行具体的终结技释放逻辑（子类实现）
            yield return OnUltimateCast(context);
        }
    }
}