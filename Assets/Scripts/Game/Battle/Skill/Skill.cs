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

    public float DamageCoefficient { get; }

    public IBattleEntityObject Caster { get; private set; }

    public IBattleEntityObject MainTarget { get; private set; }

    public List<IBattleEntityObject> AllTargets { get; private set; }

    public IDamageCalcManager DamageCalcManager { get; set; }

    private float waitTime = 1f;

    protected Skill(int skillId)
    {
        SkillInfo = BinaryDataManager.Instance.GetConfig<SkillInfoContainer>(E_ConfigLoadType.Editor).dataDic[skillId];
        DamageCalcManager = ServiceLocator.Instance.Get<IDamageCalcManager>();
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
        context.ConsumeSkillPoint(SkillInfo.f_costBP);
        // TODO：暂时直接触发对应动画，之后根据具体技能的时机触发
        if ((E_SkillType)SkillInfo.f_SkillType != E_SkillType.UltimateSkill)
        {
            context.GetEventBus().TriggerEvent(new SkillCastEvent(context, this, 0));
        }

        // TODO：暂时直接恢复能量，后续优化为造成伤害时恢复能量
        PlayerPropertyComponent playerPropertyComponent = Caster.GetComponent<PlayerPropertyComponent>();
        if (playerPropertyComponent != null)
        {
            int newValue = playerPropertyComponent.GetPropertyValue(E_DynamicPropertyType.CurrentEnergy);
            Caster.GetComponent<PropertyComponent>().SetPropertyValue(E_DynamicPropertyType.CurrentEnergy, newValue + SkillInfo.f_recoveryEnergy);
        }

        yield return OnCast(context);

        // 等待时间，优化战斗表现
        yield return new WaitForSeconds(waitTime);

        yield return OnPostCast();

        // TODO：暂时这样判断，后续优化
        if ((E_SkillType)SkillInfo.f_SkillType != E_SkillType.UltimateSkill)
        {
            // 减少行动次数
            this.Caster.SubActCount();
        }
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
        // TODO：暂时直接情况战斗界面显示的伤害总文本
        ServiceLocator.Instance.Get<IUIManager>().GetView<BattleController>().ClearDamageTextUI();
        yield break;
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