using Core.DI;
using HotUpdate.Game.Battle.Object.Monster.TurtleShell.Skill;
using HotUpdate.Game.Battle.Skill;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Monster.TurtleShell
{
    public class TurtleShell : MonsterObject
    {
        protected override ISkillFactory GetSkillFactory()
        {
            return DIContainer.Create<TurtleShellSkillFactory>();
        }

        public override int SelectSkill()
        {
            // 随机从技能列表中选择一个技能ID
            var skillComponent = GetComponent<ISkillComponent>();
            var index = Random.Range(0, skillComponent.SkillCount);
            return skillComponent.GetSkill(index).SkillContext.SkillInfo.f_id;
        }
    }
}