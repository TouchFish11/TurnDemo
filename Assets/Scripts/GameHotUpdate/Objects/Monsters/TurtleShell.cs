using Game.Battle.Context;
using Game.Battle.Skill.Component;
using GameHotUpdate.Skill.Fatory.Monsters;

namespace GameHotUpdate.Objects.Monsters
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