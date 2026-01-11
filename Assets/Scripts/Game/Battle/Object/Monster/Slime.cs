using Framework;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Slime技能工厂类
/// </summary>
public class SlimeSkillFactory : SkillFactory
{
    public override ISkill CreateSkill(IBattleEntityObject caster, int skillId)
    {
        switch (skillId)
        {
            case 101:
                return new SlimeSkill(caster, skillId,
                                      IFactory.GetTypeInstance<SkillCastPostHandlerFactory, BaseSkillCastPostHandler>(),
                                      IFactory.GetTypeInstance<StatusAddStrategyFactory, SlimeSkillStatusStrategy>());
            default:
                LogManager.Log($"未找到技能ID， skillId = {skillId}");
                return null;
        }
    }
}

public class Slime : MonsterObject
{
    public override void BattleInit(int roleId, IBattleContext context)
    {
        base.BattleInit(roleId, context);

        // 初始化技能组件
        this.GetComponent<SkillComponent>().InitSkills(MonsterInfo.f_skillIds, new SlimeSkillFactory());
    }
}
