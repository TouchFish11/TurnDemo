using Framework;
using Game.Battle;
using System;
using System.Collections.Generic;

/// <summary>
/// 目标选择管理器
/// ——存储玩家回合选中的目标
/// </summary>
public class TargetSelectManager : SingletonBase<TargetSelectManager>, ITargetSelectManager
{
    // 目标选择接口
    private ITargetSelect targetSelect;

    private TargetSelectManager()
    {
#if EDITOR_TEST_AB || !UNITY_EDITOR
        targetSelect = new BattleTargetSelect();
#else
        targetSelect = new BattleTargetSelect();
#endif
        // 注册到定位器中
        ServiceLocator.Instance.Register<ITargetSelectManager>(Instance);
        ServiceLocator.Instance.Get<IBattleManager>().GetContext().GetEventBus().AddListener<SelectSkillEvent>(OnSelectSkillEvent);
    }

    public void ActiveSelectTarget(int skillId)
    {
        targetSelect.ActiveSelectTarget(skillId);
    }

    public void InActiveSelectTarget()
    {
        targetSelect.InActiveSelectTarget();
    }

    private void OnSelectSkillEvent(SelectSkillEvent selectSkillEvent)
    {
        // 当选择的技能改变时，也要触发目标选择UI的改变
        targetSelect.UpdateSkillSelect(selectSkillEvent.SkillId);
    }

    public void RegisterTargetSelectionChanged(Action<(IBattleEntityObject, List<IBattleEntityObject>)> onTargetSelectChanged)
    {
        targetSelect.OnTargetSelectionChanged += onTargetSelectChanged;
    }

    public void CancelTargetSelectionChanged(Action<(IBattleEntityObject, List<IBattleEntityObject>)> onTargetSelectChanged)
    {
        targetSelect.OnTargetSelectionChanged -= onTargetSelectChanged;
    }

    /// <summary>
    /// 获取主目标
    /// </summary>
    /// <returns></returns>
    public IBattleEntityObject GetMainTarget()
    {
        return targetSelect.GetMainTarget();
    }

    /// <summary>
    /// 获取目标列表（包含主目标）
    /// </summary>
    /// <returns></returns>
    public List<IBattleEntityObject> GetTargets()
    {
        return targetSelect.GetTargets();
    }
}
