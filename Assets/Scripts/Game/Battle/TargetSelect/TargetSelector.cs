using Framework;
using Game.Battle;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 目标选择器
/// ——根据玩家角色技能范围确定选择的目标
/// </summary>
public class TargetSelector : SingletonAutoMono<TargetSelector>
{
    ////当前选择的主目标
    //private IBattleEntityObject _currentMainTarget;
    //////滑动开始位置
    ////private Vector3 _dragStartPosition;
    //////是否正在滑动
    ////private bool _isDragging;
    //////滑动阈值
    ////private const float DragThreshold = 100f;
    //////是否启用
    ////private bool _isEnable;

    ///// <summary>
    ///// 启用目标选择
    ///// </summary>
    ///// <param name="chooser">选择者</param>
    ///// <param name="skillId">技能ID</param>
    ///// <param name="isNew">是否重新选择主目标</param>
    //public void ActiveSelectTarget(bool isNew = false)
    //{
    //    BattleInputHandler.Instance.OnLeftDrag += SelectPreviousMainTarget;
    //    BattleInputHandler.Instance.OnRightDrag += SelectNextMainTarget;
    //    BattleInputHandler.Instance.OnSelectedObject += SelectClickMainTarget;

    //    //_isEnable = true;
    //    UpdateTarget(isNew);
    //}

    ///// <summary>
    ///// 禁用目标选择
    ///// </summary>
    //public void InActiveSelectTarget()
    //{
    //    BattleInputHandler.Instance.OnLeftDrag -= SelectPreviousMainTarget;
    //    BattleInputHandler.Instance.OnRightDrag -= SelectNextMainTarget;
    //    BattleInputHandler.Instance.OnSelectedObject -= SelectClickMainTarget;
    //    //_isEnable = false;
    //    //清除UI
    //    UIMgr.Instance.GetPanel<BattlePanel>((panel) => panel.ClearTargetSelectMasker());
    //}

    ///// <summary>
    ///// 更新目标
    ///// </summary>
    ///// <param name="skillId">选择者</param>
    ///// <param name="isNew">是否重新选择主目标</param>
    //private void UpdateTarget(bool isNew = false)
    //{
    //    //若当前目标为空 或 当前选中的目标已经死亡 或 强制重新选择主目标 都需要重新选择目标；
    //    if (_currentMainTarget == null || _currentMainTarget.IsDeath || isNew)
    //    {
    //        //根据规则选择主目标
    //        _currentMainTarget = BattleManager.Instance.GetMainTarget();
    //    }

    //    //TODO：暂时这样写，后续优化
    //    //每次重新选择目标时，都将所有角色设置为未选中
    //    for (int i = 0; i < BattleManager.Instance.CharactersManager.GetAllActCharacter().Count; i++)
    //    {
    //        BattleManager.Instance.CharactersManager.GetAllActCharacter()[i].SetSelectFlag(false);
    //    }

    //    T_SkillInfo skillInfo = SkillStateService.GetCurrentSkillInfo();

    //    //获取要选择的所有目标
    //    List<IBattleTarget> targets = BattleManager.Instance.GetRangeTargets(E_CharacterType.PlayerCharacter, _currentMainTarget, skillInfo.f_skillRangeType);

    //    //设置为选中
    //    for (int i = 0; i < targets.Count; i++)
    //    {
    //        (targets[i] as IActionable).SetSelectFlag(true);
    //    }

    //    //设置角色技能所有目标
    //    //chooser.SetTargets(targets);
    //    //TODO：分发事件处理，UI战斗界面处理，批量选择目标
    //    //UpdateSelectMasker(targets);

    //    // TODO：更新目标选择中间层数据，触发UI、操作缓存的更新
    //    TargetSelectManager.Instance.UpdateTargetSelection(_currentMainTarget, targets);
    //}

    ////private void Update()
    ////{
    ////    if (!_isEnable)
    ////        return;

    ////    //OnUpdate();
    ////}

    ///// <summary>
    ///// TODO：这里的方法迁移到玩家输入模块中处理
    ///// 处理点击拖曳输入
    ///// </summary>
    ////private void OnUpdate()
    ////{
    ////    //处理按下逻辑
    ////    if (Mouse.current.leftButton.wasPressedThisFrame)
    ////    {
    ////        //记录按下瞬间的位置，为了与长按拖曳时的阈值判断
    ////        _dragStartPosition = Input.mousePosition;
    ////    }

    ////    //处理滑动输入
    ////    if (Mouse.current.leftButton.isPressed)
    ////    {
    ////        //满足拖曳条件才能滑动，设置合理的阈值能避免点击时的轻微抖动被误判为拖拽
    ////        if (!_isDragging && Vector2.Distance(Input.mousePosition, _dragStartPosition) > DragThreshold)
    ////        {
    ////            _isDragging = true;
    ////        }

    ////        //正在拖曳
    ////        if (_isDragging)
    ////        {
    ////            // 根据拖拽方向切换目标
    ////            float dragDeltaX = Input.mousePosition.x - _dragStartPosition.x;
    ////            //上述是为了避免误判，这里是处理拖曳多少偏移量才能切换目标
    ////            if (Mathf.Abs(dragDeltaX) > DragThreshold)
    ////            {
    ////                // 向右拖拽，选择下一个敌人为主目标
    ////                if (dragDeltaX > 0)
    ////                {
    ////                    SelectNextMainTarget();
    ////                }
    ////                // 向左拖拽，选择上一个敌人为主目标
    ////                else if(dragDeltaX < 0)
    ////                {
    ////                    SelectPreviousMainTarget();
    ////                }

    ////                //重新记录拖曳起始位置
    ////                _dragStartPosition = Input.mousePosition;
    ////            }
    ////        }
    ////    }

    ////    //处理抬起逻辑
    ////    if (Mouse.current.leftButton.wasReleasedThisFrame)
    ////    {
    ////        //取消拖曳
    ////        this._isDragging = false;

    ////        T_SkillInfo skillInfo = SkillStateService.GetCurrentSkillInfo();

    ////        //如何取获取当前行动对象的技能ID，获取其中的技能目标类型，为了后续的射线检测能作用到正确的目标层级
    ////        E_TargetType targetType = (E_TargetType)skillInfo.f_target_type;

    ////        int layerMask = 0;
    ////        switch (targetType)
    ////        {
    ////            case E_TargetType.Friend:
    ////                layerMask = 1 << LayerMask.NameToLayer("PlayerCharacter");
    ////                break;
    ////            case E_TargetType.Enemy:
    ////                layerMask = 1 << LayerMask.NameToLayer("MonsterCharacter");
    ////                break;
    ////        }

    ////        //鼠标选中主目标
    ////        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, 100, layerMask))
    ////        {
    ////            //获取记录点击主角色
    ////            _currentMainTarget = hitInfo.collider.GetComponentInParent<Character>();
    ////            //更新目标
    ////            UpdateTarget(true);
    ////        }
    ////    }
    ////}

    ///// <summary>
    ///// 选择下一个主目标
    ///// </summary>
    //private void SelectNextMainTarget()
    //{
    //    //通过技能状态服务中间层获取技能目标类型
    //    E_TargetType targetType = (E_TargetType)SkillStateService.GetCurrentSkillInfo().f_target_type;

    //    List<IBattleTarget> targets = targetType == E_TargetType.Friend ? BattleManager.Instance.CharactersManager.GetAllActPlayerCharacter() : BattleManager.Instance.CharactersManager.GetAllActMonsterCharacter();

    //    //只有最后一个目标，不用处理
    //    if (targets.Count < 1)
    //        return;

    //    //获取主目标所在列表的位置
    //    int mainIndex = targets.IndexOf(_currentMainTarget);

    //    //检查是否有下一个目标
    //    if (mainIndex + 1 < targets.Count)
    //    {
    //        //有，切换主目标
    //        _currentMainTarget = targets[++mainIndex];
    //        //更新目标选择
    //        UpdateTarget();
    //    }
    //}

    ///// <summary>
    ///// 选择上一个主目标
    ///// </summary>
    //private void SelectPreviousMainTarget()
    //{
    //    E_TargetType targetType = (E_TargetType)SkillStateService.GetCurrentSkillInfo().f_target_type;

    //    List<IBattleTarget> targets = targetType == E_TargetType.Friend ? BattleManager.Instance.CharactersManager.GetAllActPlayerCharacter() : BattleManager.Instance.CharactersManager.GetAllActMonsterCharacter();

    //    //只有最后一个目标，不用处理
    //    if (targets.Count < 1)
    //        return;

    //    //获取主目标所在列表的位置
    //    int mainIndex = targets.IndexOf(_currentMainTarget);

    //    //检查是否有下一个目标
    //    if (mainIndex - 1 >= 0)
    //    {
    //        //有，切换主目标
    //        _currentMainTarget = targets[--mainIndex];
    //        //更新目标选择
    //        UpdateTarget();
    //    }
    //}

    ///// <summary>
    ///// 选择点击的主目标
    ///// </summary>
    ///// <param name="mainTarget"></param>
    //private void SelectClickMainTarget(IBattleTarget mainTarget)
    //{
    //    _currentMainTarget = mainTarget;
    //    //更新目标
    //    UpdateTarget(true);
    //}
}
