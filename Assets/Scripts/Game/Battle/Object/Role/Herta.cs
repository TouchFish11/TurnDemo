using Game;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Herta技能工厂类
/// </summary>
public class HertaSkillFactory : SkillFactory
{
    public override ISkill CreateSkill(int skillId)
    {
        switch (skillId)
        {
            case 20:
                return new HertaNormalSkill(skillId, GetSkillCastPostHandler<BaseSkillCastPostHandler>());
            case 21:
                return new HertaBattleSkill(skillId, GetSkillCastPostHandler<BaseSkillCastPostHandler>());
            case 22:
                return new HertaUltimateSkill(skillId, GetSkillCastPostHandler<BaseUltimateSkillCastPostHandler>());
            default:
                return null;
        }
    }
}

/// <summary>
/// Herta角色类
/// </summary>
public class Herta : PlayerObject
{
    public override void BattleInit(int roleId, IBattleContext context)
    {
        base.BattleInit(roleId, context);

        // 初始化技能组件
        this.GetComponent<SkillComponent>().InitSkills(RoleInfo.f_skillIds, new HertaSkillFactory());
    }
}
