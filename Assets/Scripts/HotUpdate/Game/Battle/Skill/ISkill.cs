using System.Collections;
using System.Collections.Generic;
using HotUpdate.Base;
using HotUpdate.Common.Config.ExcelInfo.Info;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Property;
using HotUpdate.Game.Battle.TargetSelect;

namespace HotUpdate.Game.Battle.Skill
{
    /// <summary>
    /// 技能接口
    /// </summary>
    public interface ISkill
    {
        /// <summary>
        /// 技能信息
        /// </summary>
        SkillInfo SkillInfo { get; }

        /// <summary>
        /// 施法者
        /// </summary>
        IBattleEntityObject Caster { get; }

        /// <summary>
        /// 主目标
        /// </summary>
        IBattleEntityObject MainTarget { get; }

        /// <summary>
        /// 所有目标
        /// </summary>
        List<IBattleEntityObject> AllTargets { get; }

        /// <summary>
        /// 属性组件
        /// </summary>
        IPropertyComponent PropertyComponent { get; }
        
        /// <summary>
        /// 目标选择策略
        /// </summary>
        ITargetSelectStrategy TargetSelectStrategy { get; }

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
        
        /// <summary>
        /// 恢复能量
        /// </summary>
        void RecoverEnergy();

        /// <summary>
        /// 设置目标选择策略
        /// </summary>
        /// <param name="targetSelectStrategy">目标选择策略</param>
        void SetTargetSelectStrategy(ITargetSelectStrategy targetSelectStrategy);
    }
}