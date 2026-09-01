using HotUpdate.Base.ECModule;
using HotUpdate.Game.Battle.Object.Monster;

namespace HotUpdate.Game.Battle.Skill.Component
{
    /// <summary>
    /// 怪物技能组件
    /// </summary>
    [ComponentId]
    public class MonsterSkillComponent : SkillComponent
    {
        protected override string InitSkillIds()
        {
            return ((IMonsterObject)BattleEntity).MonsterInfo.f_skillIds;
        }
    }
}
