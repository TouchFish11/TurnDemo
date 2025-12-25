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

    private TargetSelectManager()
    {
#if EDITOR_TEST_AB || !UNITY_EDITOR
        targetSelect = new BattleTargetSelect();
#else
        targetSelect = new BattleTargetSelect();


#endif
    }

    public void ActiveSelectTarget(int skillId)
    {
        targetSelect.ActiveSelectTarget(skillId);
    }

    public void InActiveSelectTarget()
    {
        targetSelect.InActiveSelectTarget();
    }

    /// <summary>
    /// 更新技能选择
    /// </summary>
    /// <param name="skillId"></param>
    public void UpdateSkillSelect(int skillId)
    {
        // 当选择的技能改变时，也要触发目标选择UI的改变
        targetSelect.UpdateSkillSelect(skillId);

        // 设置技能ID，滑动时能根据设置好的ID读取配置信息，进行范围判断
        BattleInputHandler.Instance.SetSkillId(skillId);
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
