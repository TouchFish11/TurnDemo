using Framework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Game.Battle;
using Game;

/// <summary>
/// 战斗输入处理器
/// </summary>
public class BattleInputHandler : SingletonAutoMono<BattleInputHandler>
{
    // 滑动开始位置
    private Vector3 _dragStartPosition;
    // 是否正在滑动
    private bool _isDragging;
    // 滑动阈值
    private const float DragThreshold = 100f;
    // 是否启用
    //private bool _isEnable;

    // 当前选择的技能ID
    private int skillId;

    /// <summary>
    /// 选中对象事件
    /// </summary>
    public event UnityAction<IBattleEntityObject> OnSelectedObject;
    /// <summary>
    /// 左滑动一定阈值时
    /// </summary>
    public event UnityAction OnLeftDrag;
    /// <summary>
    /// 右滑动一定阈值时
    /// </summary>
    public event UnityAction OnRightDrag;

    private void Awake()
    {
        MonoManager.Instance.AddUpdateListener(OnUpdate);
        ServiceLocator.Get<IBattleManager>().GetContext().GetEventBus().AddListener<SelectSkillEvent>(OnSelectSkillEvent);
    }

    private void OnSelectSkillEvent(SelectSkillEvent selectSkillEvent)
    {
        this.skillId = selectSkillEvent.SkillId;
    }

    /// <summary>
    /// 处理点击拖曳输入
    /// </summary>
    private void OnUpdate()
    {
        InputHandle();
    }

    private void InputHandle()
    {
        // 处理按下逻辑
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // 记录按下瞬间的位置，为了与长按拖曳时的阈值判断
            _dragStartPosition = Input.mousePosition;
        }

        // 处理滑动输入
        if (Mouse.current.leftButton.isPressed)
        {
            // 满足拖曳条件才能滑动，设置合理的阈值能避免点击时的轻微抖动被误判为拖拽
            if (!_isDragging && Vector2.Distance(Input.mousePosition, _dragStartPosition) > DragThreshold)
            {
                _isDragging = true;
            }

            // 正在拖曳
            if (_isDragging)
            {
                // 根据拖拽方向切换目标
                float dragDeltaX = Input.mousePosition.x - _dragStartPosition.x;
                // 上述是为了避免误判，这里是处理拖曳多少偏移量才能切换目标
                if (Mathf.Abs(dragDeltaX) > DragThreshold)
                {
                    // 向右拖拽，选择下一个敌人为主目标
                    if (dragDeltaX > 0)
                    {
                        OnRightDrag?.Invoke();
                    }
                    // 向左拖拽，选择上一个敌人为主目标
                    else if (dragDeltaX < 0)
                    {
                        OnLeftDrag?.Invoke();
                    }

                    // 重新记录拖曳起始位置
                    _dragStartPosition = Input.mousePosition;
                }
            }
        }

        // 处理抬起逻辑
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            // 取消拖曳
            this._isDragging = false;

            SkillInfo skillInfo = BinaryDataManager.Instance.GetConfig<SkillInfoContainer>(E_ConfigLoadType.Editor).dataDic[skillId];
            //如何取获取当前行动对象的技能ID，获取其中的技能目标类型，为了后续的射线检测能作用到正确的目标层级
            E_SkillTargetType targetType = skillInfo.f_skillRangeType.ToSkillTargetType();

            int layerMask = 0;
            switch (targetType)
            {
                case E_SkillTargetType.Friend:
                    layerMask = 1 << LayerMask.NameToLayer("PlayerObject");
                    break;
                case E_SkillTargetType.Enemy:
                    layerMask = 1 << LayerMask.NameToLayer("MonsterObject");
                    break;
            }

            // 鼠标选中主目标
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo, 500, layerMask))
            {
                // 获取记录点击主角色
                BattleObject currentMainTarget = hitInfo.collider.GetComponent<BattleObject>();
                // 执行选选择对象事件
                OnSelectedObject?.Invoke(currentMainTarget);
                LogManager.Log($"命中目标，{currentMainTarget}");
            }
            else
            {
                //LogManager.Log($"未命中目标");
            }
        }
    }

    private void OnDisable()
    {
        OnSelectedObject = null;
        OnLeftDrag = null;
        OnRightDrag = null;
    }
}
