using Framework;
using Game.Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能基类
/// </summary>
public abstract class Skill : ISkill
{
    public SkillInfo SkillInfo { get; private set; }

    public float DamageCoefficient { get; }

    public IBattleEntityObject Caster { get; private set; }

    public IBattleEntityObject MainTarget { get; private set; }

    public List<IBattleEntityObject> AllTargets { get; private set; }

    private float waitTime = 1f;

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
    public IEnumerator Cast(IBattleContext context)
    {
        // 通用处理逻辑
        // 处理战技点
        context.CurentBattlePointCount -= SkillInfo.f_costBP;
        yield return OnCast(context);


        yield return new WaitForSeconds(waitTime);

        // 暂时写这里
        // 减少行动次数
        this.Caster.SubActCount();
    }

    protected abstract IEnumerator OnCast(IBattleContext context);

    /// <summary>
    /// 测试
    /// </summary>
    /// <param name="battleEntity"></param>
    /// <param name="count"></param>
    public void MulTest(IBattleEntityObject battleEntity, int count)
    {
        for (int i = 0; i < count; i++)
        {
            DamageCalcManager.Instance.CalcDamage(Caster, battleEntity, this, out DamageResult result);
            battleEntity.TakeDamage(result);
        }
    }
}