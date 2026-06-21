using System.Collections.Generic;
using HotUpdate.Game.Battle.Object;
using HotUpdate.Game.Battle.Skill.Base;

namespace HotUpdate.Game.VFX
{
    /// <summary>
    /// 弹射物数据
    /// </summary>
    public struct ProjectileData
    {
        public readonly IBattleEntityObject caster;
        public readonly IBattleEntityObject mainTarget;
        public readonly List<IBattleEntityObject> targets;
        public readonly SkillContext SkillContext;

        public ProjectileData(IBattleEntityObject caster, IBattleEntityObject mainTarget, List<IBattleEntityObject> targets, SkillContext skillContext)
        {
            this.caster = caster;
            this.mainTarget = mainTarget;
            this.targets = targets;
            SkillContext = skillContext;
        }
    }
}
