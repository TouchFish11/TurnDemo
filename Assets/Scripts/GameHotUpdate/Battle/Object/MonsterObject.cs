using System.Collections;
using Core.Config;
using Core.DataPersistence.Binary;
using Core.Reflection;
using Core.Service;
using Core.Utility;
using Game.Animation;
using Game.Battle.Command;
using Game.Battle.Context;
using Game.Battle.Skill.Component;
using Game.VFX;
using GameHotUpdate.Animation;
using GameHotUpdate.Battle.Command;
using GameHotUpdate.Battle.Event.Turn;
using GameHotUpdate.Battle.Event.UI;
using GameHotUpdate.Battle.ResponsibilityChain.DamageChain;
using GameHotUpdate.Battle.Toughness;
using UnityEngine;

namespace GameHotUpdate.Battle.Object
{
    /// <summary>
    /// 怪物战斗对象
    /// 继承自BattleObject，封装了怪物的基础属性、战斗行为等核心逻辑
    /// </summary>
    public abstract class MonsterObject : BattleObject
    {
        /// <summary>
        /// 怪物配置信息（从配置表加载）
        /// 包含怪物ID、技能ID列表、组件名称列表等基础配置
        /// </summary>
        public MonsterInfo MonsterInfo { get; private set; }

        /// <summary>
        /// 基础初始化方法
        /// 加载怪物配置信息，为战斗初始化做准备
        /// </summary>
        /// <param name="id">怪物配置ID</param>
        public override void BaseInit(int id)
        {
            base.BaseInit(id);
            // 从二进制配置管理器中加载对应ID的怪物配置
            MonsterInfo = ServiceLocator.Get<IBinaryDataManager>().GetConfig<MonsterInfoContainer>(EConfigLoadType.Excel).dataDic[id];
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
            // 初始化伤害链
            damageChain = DamageChainBuilder.GetMonsterDamageChain();
            // 根据配置的组件名称列表，为怪物添加对应的战斗组件（如韧性组件、动画组件等）
            AddComponents(TextUtility.Split(MonsterInfo.f_comNames, 2));
        }

        public override void ExecuteAction()
        {
            base.ExecuteAction();
            // 随机从技能列表中选择一个技能ID
            var skillIds = this.GetComponent<SkillComponent>().GetSkillIds();
            // TODO：可以封装随机选择的策略类，用于玩家/怪物AI
            var skillId = skillIds[Random.Range(0, skillIds.Count)];
            // 释放选中的技能
            CastSkill(skillId);
        }

        public override void CastSkill(int skillId)
        {
            var skillComponent = this.GetComponent<SkillComponent>();
            // 能否释放
            if (!skillComponent.CanCast(skillId))
            {
                return;
            }
            
            // 获取技能数据
            var skillData = skillComponent.GetSkillData(skillId);
            var toughnessComponent = this.GetComponent<ToughnessComponent>();
            // 获取怪物行动指令
            var actCommand = ServiceLocator.Get<IFactoryManager>().GetFactory<ICommandFactory, CommandFactory>()
                .GetMonsterActCommand(toughnessComponent, skillData);
            // 发送指令
            Context.GetEventBus().TriggerEvent(new InsertCommandEvent(Context, actCommand));
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
            StartCoroutine(ServiceLocator.Get<IAnimationPlayManager>().
                WaitForAnimOver(GetComponent<BattleAnimationComponent>(), 
                AnimationComponent.Battle_Layer_Name, 
                E_AnimationType.Death));
            
            // 等待死亡特效播放完毕（协程阻塞，直到特效销毁）
            yield return new WaitUntil(() => !vFXInfo.IsAlive);
        }

        /// <summary>
        /// 怪物对象禁用时的清理逻辑
        /// 触发怪物死亡事件
        /// </summary>
        public override void Destroy()
        {
            Context.GetEventBus().TriggerEvent(new MonsterDeadEvent(Context, this));
            base.Destroy();
        }
    }
}