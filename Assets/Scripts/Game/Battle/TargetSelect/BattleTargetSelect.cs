using Framework;
using Game.Battle;
using System.Collections.Generic;

/// <summary>
/// 战斗目标选择
/// </summary>
public class BattleTargetSelect : ITargetSelect
{
    // 目标列表（包含主目标）
    private readonly List<IBattleEntityObject> _selectedTargets = new List<IBattleEntityObject>();
    // 主目标
    private IBattleEntityObject _mainTarget;
    // 当前技能信息
    private SkillInfo skillInfo;

    /// <summary>
    /// 激活目标选择
    /// </summary>
    /// <param name="skillInfo"></param>
    public void ActiveSelectTarget(SkillInfo skillInfo)
    {
        BattleInputHandler.Instance.OnLeftDrag += SelectPreviousMainTarget;
        BattleInputHandler.Instance.OnRightDrag += SelectNextMainTarget;
        BattleInputHandler.Instance.OnSelectedObject += SelectClickMainTarget;

        //_isEnable = true;
        DefaultSelectTarget();
    }

    /// <summary>
    /// 失活目标选择
    /// </summary>
    public void InActiveSelectTarget()
    {
        BattleInputHandler.Instance.OnLeftDrag -= SelectPreviousMainTarget;
        BattleInputHandler.Instance.OnRightDrag -= SelectNextMainTarget;
        BattleInputHandler.Instance.OnSelectedObject -= SelectClickMainTarget;

        // 清除UI
        UIManager.Instance.GetView<BattleController>().ClearTargetSelectMasker();
    }

    /// <summary>
    /// 默认选择目标逻辑
    /// </summary>
    private void DefaultSelectTarget()
    {
        //若当前目标为空 或 当前选中的目标已经死亡 或 强制重新选择主目标 都需要重新选择目标；
        if (_mainTarget == null || _mainTarget.GetComponent<PropertyComponent>().IsDeath)
        {
            // 根据规则选择主目标
            _mainTarget = BattleUtil.GetMainTarget(this.skillInfo, BattleManager.Instance.GetContext());
        }

        UpdateTargets();
    }

    public IBattleEntityObject GetMainTarget()
    {
        return _mainTarget;
    }

    public List<IBattleEntityObject> GetTargets()
    {
        return _selectedTargets;
    }


    public void UpdateTargets()
    {
        //TODO：暂时这样写，后续优化
        // 每次重新选择目标时，都将所有角色设置为未选中
        List<IBattleEntityObject> battleEntities = new List<IBattleEntityObject>(BattleManager.Instance.GetContext().GetAllBattleEntity());
        for (int i = 0; i < battleEntities.Count; i++)
        {
            // 通知UI，设置为未选中
            // battleEntities[i].SetSelectFlag(false);
        }

        E_SkillTargetType skillTargetType = (E_SkillTargetType)skillInfo.f_targetType;
        // 记录选择的所有目标
        _selectedTargets.Clear();
        _selectedTargets.AddRange(BattleUtil.GetRangeTargets(E_CharacterType.PlayerCharacter, _mainTarget, skillInfo.f_skillRangeType));
        // 设置为选中
        //for (int i = 0; i < targets.Count; i++)
        //{
        //    (targets[i] as IActionable).SetSelectFlag(true);
        //}

        //设置角色技能所有目标
        //chooser.SetTargets(targets);
        //TODO：分发事件处理，UI战斗界面处理，批量选择目标
        //UpdateSelectMasker(targets);
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

        //只有最后一个目标，不用处理
        if (targets.Count < 1)
            return;

        //获取主目标所在列表的位置
        int mainIndex = targets.IndexOf(_mainTarget);
        //检查是否有下一个目标
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
