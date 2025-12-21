using Framework;
using Game.Battle;
using GameLogic.BattleMoudule.Entity;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 技能基类
/// </summary>
public abstract class Skill : ISkill
{
    public SkillInfo SkillInfo { get; private set; }

    public float DamageCoefficient { get; }

    public E_PropertyType PropertyType { get; }

    public IBattleEntityObject Caster { get; private set; }

    public IBattleEntityObject MainTarget { get; private set; }

    public List<IBattleEntityObject> AllTargets { get; private set; }

    public void Init(SkillInfo skillInfo, IBattleEntityObject caster, IBattleEntityObject mainTarget, List<IBattleEntityObject> allTargets)
    {
        SkillInfo = skillInfo;
        Caster = caster;
        MainTarget = mainTarget;
        AllTargets = allTargets;
    }

    // 一定是通过技能对象实例来驱动角色释放技能行为的
    public abstract IEnumerator Cast(IBattleContext context);
}