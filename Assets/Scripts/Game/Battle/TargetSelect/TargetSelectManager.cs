using Framework;
using Game.Battle;
using System;
using System.Collections.Generic;

/// <summary>
/// 目标选择管理器
/// ——存储玩家回合选中的目标
/// </summary>
public class TargetSelectManager : SingletonBase<TargetSelectManager>
{
    // 目标选择接口
    private ITargetSelect targetSelect;

    /// <summary>
    /// 目标选择变化事件
    /// </summary>
    public event Action<(IBattleEntityObject maintarget, List<IBattleEntityObject> selectedTargets)> OnTargetSelectionChanged;

    private TargetSelectManager()
    {
#if EDITOR_TEST_AB || !UNITY_EDITOR
        targetSelect = new BattleTargetSelect();
#else
        targetSelect = new MockTargetSelect();
#endif
    }

    public void ActiveSelectTarget(SkillInfo skillInfo)
    {
        targetSelect.ActiveSelectTarget(skillInfo);
        // 分发目标选择变化事件
        OnTargetSelectionChanged?.Invoke((targetSelect.GetMainTarget(), targetSelect.GetTargets()));
    }

    public void InActiveSelectTarget()
    {
        targetSelect.InActiveSelectTarget();
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
