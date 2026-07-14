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