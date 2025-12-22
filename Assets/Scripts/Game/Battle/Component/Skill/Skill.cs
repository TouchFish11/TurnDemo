using Framework;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;



/// <summary>
/// 技能基类
/// </summary>
public abstract class Skill : ISkill
{
    public SkillInfo SkillInfo { get; private set; }

    public float DamageCoefficient { get; }

    public E_ElementType PropertyType { get; }

    public IBattleEntityObject Caster { get; private set; }

    public IBattleEntityObject MainTarget { get; private set; }

    public List<IBattleEntityObject> AllTargets { get; private set; }

    protected Skill(int skillId)
    {
        SkillInfo = BinaryDataMgr.Instance.GetConfig<SkillInfoContainer>(E_ConfigLoadType.Editor).dataDic[skillId];
    }

    public void Init(IBattleEntityObject caster, IBattleEntityObject mainTarget, List<IBattleEntityObject> allTargets)
    {
        Caster = caster;
        MainTarget = mainTarget;
        AllTargets = allTargets;
    }

    // 一定是通过技能对象实例来驱动角色释放技能行为的
    public abstract IEnumerator Cast(IBattleContext context);
}