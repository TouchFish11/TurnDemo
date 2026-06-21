using HotUpdate.Game.Battle.Object.Monster.TurtleShell.Skill;
using HotUpdate.Game.Battle.Skill.Component;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Monster.TurtleShell
{
    public class TurtleShell : MonsterObject
    {
        protected override void OnBattleInit()
        {
            GetComponent<SkillComponent>().InitSkills(MonsterInfo.f_skillIds, new TurtleShellSkillFactory());
        }

        public override int SelectSkill()
        {
            // 随机从技能列表中选择一个技能ID
            var skillComponent = GetComponent<SkillComponent>();
            var index = Random.Range(0, skillComponent.SkillCount);
            return skillComponent.GetSkill(index).SkillContext.SkillInfo.f_id;
        }
    }
}