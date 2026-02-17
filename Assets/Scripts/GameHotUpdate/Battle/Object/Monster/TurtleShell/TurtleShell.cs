using Game.Battle.Context;
using Game.Battle.Skill.Component;
using GameHotUpdate.Battle.Object.Monster.TurtleShell.Skill;
using UnityEngine;

namespace GameHotUpdate.Battle.Object.Monster.TurtleShell
{
    public class TurtleShell : MonsterObject
    {
        public override void BattleInit(int monsterId, IBattleContext context)
        {
            base.BattleInit(monsterId, context);
            
            GetComponent<SkillComponent>().InitSkills(MonsterInfo.f_skillIds, new TurtleShellSkillFactory());
        }
        
        protected override int SelectSkill()
        {
            // 随机从技能列表中选择一个技能ID
            var skillIds = this.GetComponent<SkillComponent>().GetSkillIds();
            return skillIds[Random.Range(0, skillIds.Count)];
        }
    }
}