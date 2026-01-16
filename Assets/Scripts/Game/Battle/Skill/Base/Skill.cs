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
    // 弹射物数据
    protected ProjectileData projectileData;
    // 弹射物Transform
    protected ProjectileTrans projectileTrans;
    // 特效信息
    protected VFXInfo vFXInfo;
    // buffId数组
    protected int[] statusIds;
    private readonly float waitTime = 0.85f;

    public SkillInfo SkillInfo { get; private set; }

    public IBattleEntityObject Caster { get; private set; }

    public IBattleEntityObject MainTarget { get; private set; }

    public List<IBattleEntityObject> AllTargets { get; private set; }

    public IPropertyComponent PropertyComponent { get; private set; }

    public ISkillCastPostHandler SkillCastPostHandler { get; private set; }

    public IStatusAddStrategy StatusAddStrategy { get; private set; }

    public ITargetSelectStrategy TargetSelectStrategy { get; private set; }

    protected Skill(IBattleEntityObject caster, int skillId, ISkillCastPostHandler postHandler, IStatusAddStrategy statusAddStrategy)
    {
        Caster = caster;
        SkillInfo = BinaryDataManager.Instance.GetConfig<SkillInfoContainer>(E_ConfigLoadType.Editor).dataDic[skillId];
        SkillCastPostHandler = postHandler;
        statusIds = TextUtility.SplitToIntArr(SkillInfo.f_statusId, 2);
        StatusAddStrategy = statusAddStrategy;
        PropertyComponent = Caster.GetComponent<PropertyComponent>();
    }

    public virtual void Init(IBattleEntityObject mainTarget, List<IBattleEntityObject> allTargets)
    {
        MainTarget = mainTarget;
        AllTargets = allTargets;
    }

    public void SetTargetSelectStrategy(ITargetSelectStrategy targetSelectStrategy)
    {
        TargetSelectStrategy = targetSelectStrategy;
    }

    /// <summary>
    /// 技能释放前
    /// 进行目标选择、初始化技能目标
    /// 先调用父类虚方法
    /// </summary>
    /// <param name="context"></param>
    protected virtual void OnPreCast(IBattleContext context)
    {
        // 选择目标
        ServiceLocator.Get<ITargetSelectManager>().SelectTarget(context, Caster, SkillInfo, TargetSelectStrategy);
        // 初始化技能目标
        ServiceLocator.Get<ISkillManager>().InitSkillTarget(this);
    }

    public IEnumerator Cast(IBattleContext context)
    {
        // 技能释放前
        OnPreCast(context);
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
        // TODO：考虑移动到SkillCastPostHandler中
        // 清空战斗界面显示的伤害总文本
        BattleUIScheduler.Instance.BattleController.GetBattleUI().ClearActiveDamageTextUI();
        // 清空总伤害累计显示UI
        BattleUIScheduler.Instance.BattleController.GetBattleUI().UpdateCumulativeDamage(false, 0);

        yield return SkillCastPostHandler.OnHnadle(this);
    }

    /// <summary>
    /// 技能释放攻击后恢复能量
    /// 子类调用，在造成伤害的时候恢复能量
    /// </summary>
    public virtual void RecoverEnergy()
    {
        int newValue = PropertyComponent.GetPropertyValue(E_DynamicPropertyType.CurrentEnergy);
        PropertyComponent.SetPropertyValue(E_DynamicPropertyType.CurrentEnergy, newValue + SkillInfo.f_recoveryEnergy);
    }
}