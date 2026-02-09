using System.Collections;
using System.Collections.Generic;
using Core.DataPersistence.Binary;
using Core.Service;
using Core.Utility;
using Game.Battle.Context;
using Game.Battle.Enum;
using Game.Battle.Objects;
using Game.Battle.Skill;
using Game.Battle.Skill.Handler;
using Game.Battle.Status;
using Game.Battle.TargetSelect;
using Game.Property;
using Game.UI.Battle;
using Game.VFX;
using GameHotUpdate.Battle.UI.Base;
using GameHotUpdate.Property;
using UnityEngine;

namespace GameHotUpdate.Battle.Skill
{
    /// <summary>
    /// 技能基类
    /// 所有战斗技能的抽象基类，定义技能释放的核心流程和通用逻辑
    /// 子类需实现具体的技能释放前/释放中逻辑
    /// </summary>
    public abstract class Skill : ISkill
    {
        // 投射物数据（如子弹、技能弹道等数据）
        protected ProjectileData projectileData;
        // 投射物变换组件（控制投射物的位置/旋转等）
        protected ProjectileTrans projectileTrans;
        // 视觉特效信息（技能特效的配置数据）
        protected VFXInfo vFXInfo;
        // 技能附带的Buff/状态ID数组
        protected int[] statusIds;
        // 技能释放后等待时间（用于战斗UI/逻辑缓冲，单位：秒）
        private readonly float waitTime = 0.85f;

        /// <summary>
        /// 技能配置信息（从配置表加载的技能基础属性）
        /// </summary>
        public SkillInfo SkillInfo { get; private set; }

        /// <summary>
        /// 技能释放者（释放该技能的战斗实体，如角色、怪物）
        /// </summary>
        public IBattleEntityObject Caster { get; private set; }

        /// <summary>
        /// 技能主要目标（技能优先作用的单个目标）
        /// </summary>
        public IBattleEntityObject MainTarget { get; private set; }

        /// <summary>
        /// 技能所有目标（技能作用的全部目标列表，含主要目标）
        /// </summary>
        public List<IBattleEntityObject> AllTargets { get; private set; }

        /// <summary>
        /// 释放者的属性组件（用于读取/修改释放者的属性，如攻击力、能量等）
        /// </summary>
        public IPropertyComponent PropertyComponent { get; private set; }

        /// <summary>
        /// 技能释放后置处理器（处理技能释放完成后的附加逻辑）
        /// </summary>
        public ISkillCastPostHandler SkillCastPostHandler { get; private set; }

        /// <summary>
        /// 状态添加策略（定义如何给目标添加Buff/DeBuff等状态）
        /// </summary>
        public IStatusAddStrategy StatusAddStrategy { get; private set; }

        /// <summary>
        /// 目标选择策略（定义技能如何选择作用目标）
        /// </summary>
        public ITargetSelectStrategy TargetSelectStrategy { get; private set; }

        /// <summary>
        /// 技能基类构造函数
        /// </summary>
        /// <param name="caster">技能释放者</param>
        /// <param name="skillId">技能ID（用于从配置表加载技能信息）</param>
        /// <param name="statusAddStrategy">状态添加策略实例</param>
        protected Skill(IBattleEntityObject caster, int skillId, IStatusAddStrategy statusAddStrategy)
        {
            // 初始化释放者
            Caster = caster;
            // 从二进制配置管理器加载技能配置信息
            SkillInfo = ServiceLocator.Get<IBinaryDataManager>().GetConfig<SkillInfoContainer>(EConfigLoadType.Excel).dataDic[skillId];
            // 解析技能配置中的状态ID（分割字符串为int数组，分隔符为2？注：此处需确认分割规则，2为自定义分隔符标识）
            statusIds = TextUtility.SplitToIntArr(SkillInfo.f_statusId, 2);
            // 初始化状态添加策略
            StatusAddStrategy = statusAddStrategy;
            // 获取释放者的属性组件
            PropertyComponent = Caster.GetComponent<PropertyComponent>();
        }

        /// <summary>
        /// 初始化技能目标信息
        /// </summary>
        /// <param name="mainTarget">主要目标</param>
        /// <param name="allTargets">所有目标列表</param>
        public virtual void Init(IBattleEntityObject mainTarget, List<IBattleEntityObject> allTargets)
        {
            MainTarget = mainTarget;
            AllTargets = allTargets;
        }

        /// <summary>
        /// 设置技能目标选择策略
        /// </summary>
        /// <param name="targetSelectStrategy">目标选择策略实例</param>
        public void SetTargetSelectStrategy(ITargetSelectStrategy targetSelectStrategy)
        {
            TargetSelectStrategy = targetSelectStrategy;
        }

        /// <summary>
        /// 技能释放前的预处理逻辑（抽象方法）
        /// 子类需实现：目标筛选、技能前摇、状态初始化等释放前操作
        /// </summary>
        /// <param name="context">战斗上下文</param>
        protected abstract void OnPreCast(IBattleContext context);

        /// <summary>
        /// 技能释放核心流程（协程方法）
        /// 封装技能释放的完整生命周期：前处理 -> 释放中 -> 等待缓冲 -> 后处理
        /// </summary>
        /// <param name="context">战斗上下文</param>
        /// <returns>协程迭代器</returns>
        public IEnumerator Cast(IBattleContext context)
        {
            // 执行释放前预处理逻辑
            OnPreCast(context);
            // 执行具体的技能释放逻辑（子类实现）
            yield return OnCast(context);
            // 等待缓冲时间，保证战斗UI/逻辑的稳定性
            yield return new WaitForSeconds(waitTime);
            // 执行释放后处理逻辑
            OnPostCast();
        }

        /// <summary>
        /// 技能释放中逻辑（抽象协程方法）
        /// 子类需实现：技能伤害计算、特效播放、目标命中、状态附加等核心逻辑
        /// </summary>
        /// <param name="context">战斗上下文</param>
        /// <returns>协程迭代器</returns>
        protected abstract IEnumerator OnCast(IBattleContext context);

        /// <summary>
        /// 技能释放后处理逻辑
        /// 负责清理战斗UI、更新累计伤害等收尾操作
        /// </summary>
        protected void OnPostCast()
        {
            // 更新累计伤害UI（参数1：是否强制刷新，参数2：重置数值为0）
            ((BattleController)ServiceLocator.Get<IBattleUIScheduler>().BattleController).BattleUiManager.UpdateCumulativeDamage(false, 0);
        }

        /// <summary>
        /// 技能释放后恢复能量（蓝量/怒气等）
        /// 从技能配置中读取恢复值，更新释放者的当前能量属性
        /// </summary>
        public virtual void RecoverEnergy()
        {
            // 获取释放者当前能量值
            int newValue = PropertyComponent.GetPropertyValue(E_DynamicPropertyType.CurrentEnergy);
            // 累加技能配置的能量恢复值并更新
            PropertyComponent.SetPropertyValue(E_DynamicPropertyType.CurrentEnergy, newValue + SkillInfo.f_recoveryEnergy);
        }
    }
}