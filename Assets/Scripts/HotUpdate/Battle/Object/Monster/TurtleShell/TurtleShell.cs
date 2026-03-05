using HotUpdate.Battle.Context;
using HotUpdate.Battle.Object.Monster.TurtleShell.Skill;
using HotUpdate.Battle.Skill.Component;
using UnityEngine;

namespace HotUpdate.Battle.Object.Monster.TurtleShell
{
    public class TurtleShell : MonsterObject
    {
        public override void BattleInit(int monsterId, IBattleContext context)
        {
            base.BattleInit(monsterId, context);
            
            GetComponent<SkillComponent>().InitSkills(MonsterInfo.f_skillIds, new TurtleShellSkillFactory());
        }
        
        public override int SelectSkill()
        {
            // 随机从技能列表中选择一个技能ID
            var skillIds = GetComponent<SkillComponent>().GetSkillIds();
            return skillIds[Random.Range(0, skillIds.Count)];
        }
    }
}