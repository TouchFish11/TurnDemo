using System.Collections.Generic;
using HotUpdate.Battle.Object;
using HotUpdate.Battle.Skill.Base;

namespace HotUpdate.VFX
{
    /// <summary>
    /// 弹射物数据
    /// </summary>
    public struct ProjectileData
    {
        public readonly IBattleEntityObject caster;
        public readonly IBattleEntityObject mainTarget;
        public readonly List<IBattleEntityObject> targets;
        public readonly ISkill skill;

        public ProjectileData(IBattleEntityObject caster, IBattleEntityObject mainTarget, List<IBattleEntityObject> targets, ISkill skill)
        {
            this.caster = caster;
            this.mainTarget = mainTarget;
            this.targets = targets;
            this.skill = skill;
        }
    }
}
