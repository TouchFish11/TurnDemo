using HotUpdate.Base;
using HotUpdate.Game.Battle.Context;
using HotUpdate.Game.Battle.Object.Monster.Slime.Skill;
using HotUpdate.Game.Battle.Skill.Component;
using UnityEngine;

namespace HotUpdate.Game.Battle.Object.Monster.Slime
{
    /// <summary>
    /// 史莱姆
    /// </summary>
    public class Slime : MonsterObject
    {
        public override void BattleInit(int monsterId, IBattleContext context)
        {
            base.BattleInit(monsterId, context);
            
            GetComponent<SkillComponent>().InitSkills(MonsterInfo.f_skillIds, new SlimeSkillFactory());
        }

        public override int SelectSkill()
        {
            // 随机从技能列表中选择一个技能ID
            var skillIds = GetComponent<SkillComponent>().GetSkillIds();
            return skillIds[Random.Range(0, skillIds.Count)];
        }
    }
}