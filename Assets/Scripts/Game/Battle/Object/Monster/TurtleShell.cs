using Framework;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// TurtleShell技能工厂类
/// </summary>
public class TurtleShellSkillFactory : SkillFactory
{
    public override ISkill CreateSkill(int skillId)
    {
        switch (skillId)
        {
            case 102:
                return new TurtleShellSkill(skillId);
            default:
                LogManager.Log($"未找到技能ID， skillId = {skillId}");
                return null;
        }
    }
}

public class TurtleShell : MonsterObject
{
    public override void BattleInit(int roleId, IBattleContext context)
    {
        base.BattleInit(roleId, context);

        // 初始化技能组件
        this.GetComponent<SkillComponent>().InitSkills(MonsterInfo.f_skillIds, new TurtleShellSkillFactory());
    }
}
