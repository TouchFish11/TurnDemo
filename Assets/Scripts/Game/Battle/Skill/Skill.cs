using Framework;
using Game;
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

    public IBattleEntityObject Caster { get; private set; }

    public IBattleEntityObject MainTarget { get; private set; }

    public List<IBattleEntityObject> AllTargets { get; private set; }

    public IDamageCalcManager DamageCalcManager { get; private set; }

    public IPropertyComponent PropertyComponent { get; private set; }

    public ISkillCastPostHandler SkillCastPostHandler { get; private set; }

    private readonly float waitTime = 1f;

    protected Skill(int skillId, ISkillCastPostHandler postHandler)
    {
        SkillInfo = BinaryDataManager.Instance.GetConfig<SkillInfoContainer>(E_ConfigLoadType.Editor).dataDic[skillId];
        DamageCalcManager = ServiceLocator.Instance.Get<IDamageCalcManager>();
        SkillCastPostHandler = postHandler;
    }

    public virtual void Init(IBattleEntityObject caster, IBattleEntityObject mainTarget, List<IBattleEntityObject> allTargets)
    {
        Caster = caster;
        MainTarget = mainTarget;
        AllTargets = allTargets;

        PropertyComponent = Caster.GetComponent<PropertyComponent>();
    }

    // 一定是通过技能对象实例来驱动角色释放技能行为的
    public IEnumerator Cast(IBattleContext context)
    {
        // 通用处理逻辑

        // 处理战技点
        context.ConsumeSkillPoint(SkillInfo.f_costBP);

        // 处理动画相关内容
        yield return OnCast(context);

        // 等待时间，优化战斗表现
        yield return new WaitForSeconds(waitTime);

        // 释放结束后处理
        yield return OnPostCast();
    }

    /// <summary>
    /// 技能释放时
    /// 处理动画、特性相关内容
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected abstract IEnumerator OnCast(IBattleContext context);

    /// <summary>
    /// 技能释放后
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator OnPostCast()
    {
        // TODO：暂时直接清空战斗界面显示的伤害总文本
        ServiceLocator.Instance.Get<IUIManager>().GetView<BattleController>().GetBattleUI().ClearDamageTextUI();
        yield return SkillCastPostHandler.OnHnadle(this);
    }

    /// <summary>
    /// 技能释放攻击后恢复能量
    /// </summary>
    protected virtual void RecoverEnergy()
    {
        int newValue = PropertyComponent.GetPropertyValue(E_DynamicPropertyType.CurrentEnergy);
        PropertyComponent.SetPropertyValue(E_DynamicPropertyType.CurrentEnergy, newValue + SkillInfo.f_recoveryEnergy);
    }

    /// <summary>
    /// 测试
    /// </summary>
    /// <param name="battleEntity"></param>
    /// <param name="count"></param>
    public void MulTest(IBattleEntityObject battleEntity, int count)
    {
        for (int i = 0; i < count; i++)
        {
            DamageCalcManager.CalcSkillDamage(Caster, battleEntity, this.SkillInfo, out DamageResult result);
            battleEntity.TakeDamage(result);
        }
    }
}