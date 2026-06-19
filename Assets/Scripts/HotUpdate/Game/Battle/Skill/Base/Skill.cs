using System.Collections;
using System.Collections.Generic;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Skill.Base.Flow;
using UnityEngine;

namespace HotUpdate.Game.Battle.Skill.Base
{
    /// <summary>
    /// 技能基类
    /// 所有战斗技能的抽象基类，定义技能释放的核心流程和通用逻辑
    /// 子类需实现具体的技能释放前/释放中逻辑
    /// </summary>
    public class Skill : ISkill
    {
        public SkillContext SkillContext { get; }

        private ISkillFlow _skillFlow;
        
        /// <summary>
        /// 技能释放后等待时间（用于战斗UI/逻辑缓冲，单位：秒）
        /// </summary>
        public const float WaitTime = 0.85f;

        /// <summary>
        /// 技能基类构造函数
        /// </summary>
        /// <param name="skillContext"></param>
        protected Skill(SkillContext skillContext)
        {
            SkillContext = skillContext;
            
            // // TODO：Test
            // // 怪物效果流程：
            // _effects.Add(new MonsterPreNode(this));   // 通用逻辑
            // _effects.Add(new TargetSelectNode(this));     // 目标选择
            // _effects.Add(new ProjectileInitNode(this));   // 初始化弹射物
            // _effects.Add(new SkillExecuteNode(this));     // 执行技能（角色动画、伤害、buff、特效）
            //
            // // 玩家角色效果流程：
            // if (非终结技能)
            // {
            //     _effects.Add(new TargetSelectNode(this));     // 目标选择
            //     _effects.Add(new SkillPointCastNode(this));     // 消耗战技点
            //     _effects.Add(new ProjectileInitNode(this));   // 初始化弹射物
            //     _effects.Add(new NonUltimateSkillExecuteNode(this));     // 非终结技触发事件可以写在这里
            // }
            // else // 终结技
            // {
            //     _effects.Add(new UltimateDisplayIllustrationNode(this));  // 显示立绘
            //     // 再“展示Pose 或 播放动画”
            //     if (展示pose)
            //     {
            //         _effects.Add(new UltimateWaitTriggerNode(this));   // 等待触发也暂时抽成效果，此时玩家可以滑动选择目标，只有pose才有这个效果
            //         // 等到确认触发后在固定目标
            //         _effects.Add(new TargetSelectNode(this));     // 目标选择，这个只会执行一次
            //         _effects.Add(new ProjectileInitNode(this));   // 初始化弹射物
            //     }
            //     else  // 播放动画
            //     {
            //         // 然后一般都是进入一个强化状态
            //         // ...
            //     }
            //     
            //     _effects.Add(new UltimateSkillExecuteNode(this));
            // }
        }

        /// <summary>
        /// 初始化技能目标信息
        /// </summary>
        /// <param name="mainTarget">主要目标</param>
        /// <param name="allTargets">所有目标列表</param>
        public virtual void Init(IBattleEntityObject mainTarget, List<IBattleEntityObject> allTargets)
        {
            SkillContext.MainTarget = mainTarget;
            SkillContext.AllTargets = allTargets;
        }

        public void SetFlow(ISkillFlow skillFlow)
        {
            _skillFlow = skillFlow;
        }
        
        /// <summary>
        /// 技能释放核心流程（协程方法）
        /// 封装技能释放的完整生命周期：前处理 -> 释放中 -> 等待缓冲 -> 后处理
        /// </summary>
        /// <param name="context">战斗上下文</param>
        /// <returns>协程迭代器</returns>
        public IEnumerator Cast(IBattleContext context)
        {
            // 执行技能流程
            yield return _skillFlow.Run();
            // 等待缓冲时间
            yield return new WaitForSeconds(WaitTime);
        }
    }
}