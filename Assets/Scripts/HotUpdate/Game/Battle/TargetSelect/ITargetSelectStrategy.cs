using System.Collections.Generic;
using HotUpdate.Base;
using HotUpdate.Common.Config.ExcelInfo.Info;
using HotUpdate.Game.Battle.Object;

namespace HotUpdate.Game.Battle.TargetSelect
{
    /// <summary>
    /// 技能目标选择策略
    /// </summary>
    public interface ITargetSelectStrategy
    {
        /// <summary>
        /// 优先级
        /// 越高越先执行
        /// </summary>
        public int Priority { get; }

        /// <summary>
        /// 选择主目标
        /// </summary>
        /// <param name="targets"></param>
        /// <param name="caster"></param>
        /// <param name="skillInfo"></param>
        /// <returns></returns>
        IBattleEntityObject SelectMainTarget(List<IBattleEntityObject> targets, IBattleEntityObject caster, SkillInfo skillInfo);
    }
}
