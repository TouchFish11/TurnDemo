using Game.Battle.Context;
using Game.Battle.Skill.Component;
using GameHotUpdate.Battle.Object.Monster.TurtleShell.Skill;

namespace GameHotUpdate.Battle.Object.Monster.TurtleShell
{
    public class TurtleShell : MonsterObject
    {
        public override void BattleInit(int monsterId, IBattleContext context)
        {
            base.BattleInit(monsterId, context);
            
            GetComponent<SkillComponent>().InitSkills(MonsterInfo.f_skillIds, new TurtleShellSkillFactory());
        }
    }
}