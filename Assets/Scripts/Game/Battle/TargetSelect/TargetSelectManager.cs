using Framework;
using Game;
using Game.Battle;
using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// 目标选择管理器
/// ——存储玩家回合选中的目标
/// </summary>
public class TargetSelectManager : SingletonBase<TargetSelectManager>, ITargetSelectManager
{
    // 目标列表（包含主目标）
    private readonly List<IBattleEntityObject> _selectedTargets = new List<IBattleEntityObject>();
    // 主目标
    private IBattleEntityObject _mainTarget;
    // 当前技能信息
    private SkillInfo skillInfo;
    // 战斗上下文接口
    private IBattleContext battleContext;
    // 施法者
    private IBattleEntityObject caster;
    // 当前目标选择策略
    private ITargetSelectStrategy currentSelectStrategy;

    private TargetSelectManager()
    {
        battleContext = ServiceLocator.Get<IBattleManager>().GetContext();
        battleContext.GetEventBus().AddListener<SelectSkillEvent>(OnSelectSkillEvent);
    }

    public void ActiveSelectTarget()
    {
        BattleInputHandler.Instance.OnLeftDrag += SelectPreviousMainTarget;
        BattleInputHandler.Instance.OnRightDrag += SelectNextMainTarget;
        BattleInputHandler.Instance.OnSelectedObject += SelectClickMainTarget;
    }

    public void InActiveSelectTarget()
    {
        BattleInputHandler.Instance.OnLeftDrag -= SelectPreviousMainTarget;
        BattleInputHandler.Instance.OnRightDrag -= SelectNextMainTarget;
        BattleInputHandler.Instance.OnSelectedObject -= SelectClickMainTarget;
    }

    public void SetSelectTargetStrategy<T>() where T : class, ITargetSelectStrategy
    {
        currentSelectStrategy = IFactory.GetTypeInstance<TargetSelectStrategyFactory, T>();
    }

    public void ReSelectTarget(IBattleContext context, IBattleEntityObject caster, SkillInfo skillInfo)
    {
        // 当选择的技能改变时，也要触发目标选择UI的改变
        SelectMainTarget(context, caster, skillInfo);
        UpdateTargets();
    }

    /// <summary>
    /// 选择技能事件回调
    /// </summary>
    /// <param name="selectSkillEvent"></param>
    private void OnSelectSkillEvent(SelectSkillEvent selectSkillEvent)
    {
        // TODO：暂时这样处理，之后考虑如何兼容SetSelectTargetStrategy方法的设置，而不会被回调覆盖
        if (selectSkillEvent.Caster is PlayerObject)
        {
            SetSelectTargetStrategy<PlayerBaseTargetSelectStrategy>();
        }
        else if(selectSkillEvent.Caster is MonsterObject)
        {
            SetSelectTargetStrategy<MonsterBaseTargetSelectStrategy>();
        }

        // 当选择的技能改变时，也要触发目标选择UI的改变
        this.skillInfo = BinaryDataManager.Instance.GetConfig<SkillInfoContainer>(E_ConfigLoadType.Editor).dataDic[selectSkillEvent.SkillId];
        SelectMainTarget(selectSkillEvent.Context, selectSkillEvent.Caster, skillInfo);
        UpdateTargets();
    }

    /// <summary>
    /// 选择主目标
    /// </summary>
    /// <param name="context"></param>
    /// <param name="caster"></param>
    /// <param name="skillInfo"></param>
    private void SelectMainTarget(IBattleContext context, IBattleEntityObject caster, SkillInfo skillInfo)
    {
        // 若当前目标为空 或 当前选中的目标已经死亡 或 强制重新选择主目标 都需要重新选择目标；
        if (_mainTarget == null || _mainTarget.GetComponent<PropertyComponent>().IsDeath || this.caster == null || this.caster != caster)
        {
            // 缓存施法者
            this.caster = caster;
            // 根据规则选择主目标
            _mainTarget = currentSelectStrategy.SelectMainTarget(context, caster, skillInfo);
            //LogManager.Log($"主目标：{_mainTarget}");
        }
    }

    /// <summary>
    /// 更新所有目标
    /// 触发选择目标事件
    /// </summary>
    private void UpdateTargets()
    {
        // 记录选择的所有目标
        _selectedTargets.Clear();
        _selectedTargets.AddRange(BattleUtil.GetRangeTargets(E_CharacterType.PlayerCharacter, _mainTarget, skillInfo.f_skillRangeType));
        // 分发目标选择变化事件，更新目标标记UI、行动轴UI
        battleContext.GetEventBus().TriggerEvent(new SelectTargetEvent(battleContext, _mainTarget, _selectedTargets));
    }

    public IBattleEntityObject GetMainTarget()
    {
        return _mainTarget;
    }


    public List<IBattleEntityObject> GetTargets()
    {
        return _selectedTargets;
    }

    /// <summary>
    /// 选择下一个主目标
    /// </summary>
    private void SelectNextMainTarget()
    {
        E_SkillTargetType targetType = (E_SkillTargetType)skillInfo.f_SkillTargetType;
        List<IBattleEntityObject> targets = new List<IBattleEntityObject>(targetType == E_SkillTargetType.Friend ? BattleManager.Instance.GetContext().GetPlayerObjects() :
            BattleManager.Instance.GetContext().GetMonsterObjects());

        // 只有最后一个目标，不用处理
        if (targets.Count < 1)
            return;

        // 获取主目标所在列表的位置
        int mainIndex = targets.IndexOf(_mainTarget);
        // 检查是否有下一个目标
        if (mainIndex + 1 < targets.Count)
        {
            // 切换主目标
            _mainTarget = targets[++mainIndex];
            // 更新目标选择
            UpdateTargets();
        }
    }

    /// <summary>
    /// 选择上一个主目标
    /// </summary>
    private void SelectPreviousMainTarget()
    {
        E_SkillTargetType targetType = (E_SkillTargetType)skillInfo.f_SkillTargetType;
        List<IBattleEntityObject> targets = new List<IBattleEntityObject>(targetType == E_SkillTargetType.Friend ? BattleManager.Instance.GetContext().GetPlayerObjects() :
     BattleManager.Instance.GetContext().GetMonsterObjects());

        // 只有最后一个目标，不用处理
        if (targets.Count < 1)
            return;

        // 获取主目标所在列表的位置
        int mainIndex = targets.IndexOf(_mainTarget);
        // 检查是否有下一个目标
        if (mainIndex - 1 >= 0)
        {
            //有，切换主目标
            _mainTarget = targets[--mainIndex];
            //更新目标选择
            UpdateTargets();
        }
    }

    /// <summary>
    /// 选择点击的主目标
    /// </summary>
    /// <param name="mainTarget"></param>
    private void SelectClickMainTarget(IBattleEntityObject mainTarget)
    {
        _mainTarget = mainTarget;
        //更新目标
        UpdateTargets();
    }
}
