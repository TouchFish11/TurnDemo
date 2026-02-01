using System.Collections;
using System.Collections.Generic;
using Core.Config;
using Core.DataPersistence.Binary;
using Core.Reflection;
using Core.Service;
using Core.Utility;
using Game.Animation;
using Game.Battle;
using Game.Battle.Command;
using Game.Battle.Context;
using Game.Battle.Damage;
using Game.Battle.Toughness;
using Game.VFX;
using GameHotUpdate.Animation;
using GameHotUpdate.Battle.Event.UI;
using UnityEngine;

namespace GameHotUpdate.Objects
{
    /// <summary>
    /// 怪物战斗对象
    /// 继承自BattleObject，封装了怪物的基础属性、战斗行为等核心逻辑
    /// </summary>
    public class MonsterObject : BattleObject
    {
        /// <summary>
        /// 怪物配置信息（从配置表加载）
        /// 包含怪物ID、技能ID列表、组件名称列表等基础配置
        /// </summary>
        public MonsterInfo MonsterInfo { get; private set; }

        /// <summary>
        /// 怪物可释放的技能ID列表
        /// 战斗初始化时从MonsterInfo中解析填充
        /// </summary>
        private readonly List<int> skillIds = new();

        /// <summary>
        /// 基础初始化方法
        /// 加载怪物配置信息，为战斗初始化做准备
        /// </summary>
        /// <param name="id">怪物配置ID</param>
        public override void BaseInit(int id)
        {
            base.BaseInit(id);
            // 从二进制配置管理器中加载对应ID的怪物配置
            MonsterInfo = ServiceLocator.Get<IBinaryDataManager>().GetConfig<MonsterInfoContainer>(EConfigLoadType.Editor).dataDic[id];
        }

        /// <summary>
        /// 战斗初始化方法
        /// 初始化怪物的技能列表和战斗组件，为进入战斗做最终准备
        /// </summary>
        /// <param name="monsterId">怪物ID（与BaseInit的id一致）</param>
        /// <param name="context">战斗上下文（包含战斗管理器、回合管理器等核心战斗环境）</param>
        public override void BattleInit(int monsterId, IBattleContext context)
        {
            base.BattleInit(monsterId, context);

            // 解析配置中的技能ID字符串（分隔符为2），填充到技能列表
            skillIds.AddRange(TextUtility.SplitToIntArr(MonsterInfo.f_skillIds, 2));
            // 根据配置的组件名称列表，为怪物添加对应的战斗组件（如韧性组件、动画组件等）
            AddComponents(TextUtility.Split(MonsterInfo.f_comNames, 2));
        }

        /// <summary>
        /// 受击前的处理逻辑
        /// 主要用于扣除怪物韧性值，是韧性系统的核心触发点
        /// </summary>
        /// <param name="damageResult">伤害结果对象（包含伤害来源、元素类型、技能信息等）</param>
        protected override void OnPreTakeDamage(DamageResult damageResult)
        {
            // 获取韧性组件，根据伤害信息扣除对应韧性
            GetComponent<IToughnessComponent>().ReduceToughness(damageResult.Source, damageResult.ElementType, damageResult.SkillInfo);
        }

        /// <summary>
        /// 怪物行动逻辑的核心协程
        /// 执行流程：先恢复韧性（若需）→ 随机选择技能释放
        /// 注：该方法为怪物AI的核心入口，mono协程特性使其能异步等待操作完成
        /// </summary>
        /// <returns>协程迭代器</returns>
        protected override IEnumerator OnExceuteAction()
        {
            // 第一步：执行韧性恢复逻辑（若韧性被击破则等待恢复完成）
            yield return RestoreToughness();

            // 第二步：随机从技能列表中选择一个技能ID
            int skillId = skillIds[Random.Range(0, skillIds.Count)];
            // 释放选中的技能
            CastSkill(skillId);
        }

        /// <summary>
        /// 韧性恢复协程
        /// 逻辑：仅当韧性被击破时触发 → 创建韧性恢复指令 → 插入到回合队列 → 等待恢复完成
        /// </summary>
        /// <returns>协程迭代器</returns>
        private IEnumerator RestoreToughness()
        {
            // 获取当前怪物的韧性组件
            var toughnessComponent = GetComponent<IToughnessComponent>();
            
            // 若韧性未被击破，直接退出协程（无需恢复）
            if (!toughnessComponent.IsToughnessBroken())
            {
                yield break;
            }

            // 通过工厂管理器创建韧性恢复指令
            var command = ServiceLocator.Get<IFactoryManager>()
                .GetFactory<ICommandFactory, CommandFactory>()
                .GetToughnessCommand(toughnessComponent);
            
            // 将韧性恢复指令插入到战斗回合管理器的指令队列中
            ServiceLocator.Get<IBattleManager>().GetContext().GetTurnManager().InsertCommand(command);
            
            // 等待韧性值恢复至最大值（协程阻塞，直到条件满足）
            yield return new WaitUntil(() => toughnessComponent.CurrentToughnessValue == toughnessComponent.MaxToughnessVaue);
        }

        /// <summary>
        /// 怪物死亡处理协程
        /// 执行流程：播放死亡特效 → 播放死亡动画 → 等待特效结束
        /// </summary>
        /// <returns>协程迭代器</returns>
        public override IEnumerator Die()
        {
            // 初始化特效信息对象（用于追踪特效生命周期）
            var vFXInfo = new VFXInfo();
            
            // 创建并播放怪物死亡特效
            // 参数说明：特效资源Key → 特效挂载节点 → 投射数据（关联当前怪物）→ 特效信息（用于后续判断）
            ServiceLocator.Get<IVFXManager>().CreateVFX(
                ResKeyCollection.VFX_MonsterDead, 
                new ProjectileTrans(transform, false), 
                new ProjectileData(this, null, null, null), 
                vFXInfo);
            // 播放怪物死亡动画，并等待动画播放完成
            this.StartCoroutine(ServiceLocator.Get<IAnimationPlayManager>().
                WaitForAnimOver(GetComponent<BattleAnimationComponent>(), 
                AnimationComponent.Battle_Layer_Name, 
                E_AnimationType.Death));
            
            // 等待死亡特效播放完毕（协程阻塞，直到特效销毁）
            yield return new WaitUntil(() => !vFXInfo.IsAlive);
        }

        /// <summary>
        /// 怪物对象禁用时的清理逻辑
        /// 主要用于触发UI层的怪物死亡事件，更新战斗界面状态
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            // 触发怪物死亡事件，通知UI层移除该怪物的状态显示
            Context.GetEventBus().TriggerEvent(new MonsterDeadEvent(Context, this));
        }
    }
}