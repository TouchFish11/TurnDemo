using Core.DI;
using HotUpdate.Game.Battle.Object.Monster.Slime.Skill;
using HotUpdate.Game.Battle.Skill.Component;
using HotUpdate.Game.Battle.Skill.Factory;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Monster.Slime
{
    /// <summary>
    /// 史莱姆
    /// </summary>
    public class Slime : MonsterObject
    {
        protected override ISkillFactory GetSkillFactory()
        {
            return DIContainer.Create<SlimeSkillFactory>();
        }

        public override int SelectSkill()
        {
            // 随机从技能列表中选择一个技能ID
            var skillComponent = GetComponent<ISkillComponent>();
            var index = Random.Range(0, skillComponent.SkillCount);
            var skillId = skillComponent.GetSkillIdByIndex(index);
            return skillComponent.GetSkill(skillId).SkillContext.SkillInfo.f_id;
        }
    }
}