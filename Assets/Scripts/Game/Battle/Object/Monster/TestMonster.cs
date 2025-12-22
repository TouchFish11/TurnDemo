using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMonsterSkillFactory : SkillFactory
{
    public override ISkill CreateSkill(int skillId)
    {
        return skillId switch
        {
            101 => new TestMonsterSkill(skillId),
            _ => null,
        };
    }
}

public class TestMonster : MonsterObject
{
    public override void BattleInit(int roleId, IBattleContext context)
    {
        base.BattleInit(roleId, context);

        // 初始化技能组件
        this.GetComponent<SkillComponent>().InitSkills(MonsterInfo.f_skillIds, new TestMonsterSkillFactory());
    }
}
