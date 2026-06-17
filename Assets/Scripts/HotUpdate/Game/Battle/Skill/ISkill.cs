using System.Collections;
using System.Collections.Generic;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Skill.Base;
using HotUpdate.Game.Battle.TargetSelect;

namespace HotUpdate.Game.Battle.Skill
{
    /// <summary>
    /// 技能接口
    /// </summary>
    public interface ISkill
    {
        /// <summary>
        /// 技能上下文
        /// </summary>
        SkillContext SkillContext { get; }
        
        /// <summary>
        /// 释放技能
        /// 通过技能对象实现不同角色释放技能行为
        /// </summary>
        /// <param name="context">战斗上下文</param>
        IEnumerator Cast(IBattleContext context);

        /// <summary>
        /// 初始化技能
        /// </summary>
        /// <param name="mainTarget">主目标</param>
        /// <param name="allTargets">所有目标列表</param>
        void Init(IBattleEntityObject mainTarget, List<IBattleEntityObject> allTargets);

        void SetEffects(List<ISkillNode> effects);
    }
}