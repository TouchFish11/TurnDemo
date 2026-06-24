using HotUpdate.Base.Component;
using HotUpdate.Game.Battle.Object.Monster;
using HotUpdate.Game.Battle.Object.Role;
using HotUpdate.Game.Battle.Skill.Base;

namespace HotUpdate.Game.Battle.Skill.Component
{
    /// <summary>
    /// 怪物技能组件
    /// </summary>
    [ComponentId(typeof(MonsterSkillComponent))]
    public class MonsterSkillComponent : SkillComponent
    {
        protected override void OnBattleInit()
        {
            base.OnBattleInit();
            var monsterObject = (IMonsterObject)BattleEntity;
            skillComponentCore.InitSkill(((IMonsterObject)BattleEntity).MonsterInfo.f_skillIds, monsterObject.SkillFactory);
            AddCastCondition(monsterObject.DefaultCastCondition);
            AddTargetSelectStrategy(monsterObject.DefaultTargetSelectStrategy);
        }
    }
}
