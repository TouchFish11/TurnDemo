using System;
using System.Collections.Generic;
using Core.UI;
using Core.UI.ViewController;
using HotUpdate.UI.Battle.ActionLine;
using HotUpdate.UI.Battle.BattlePoint;
using HotUpdate.UI.Battle.Role;
using HotUpdate.UI.Battle.SkillKey;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HotUpdate.UI.Battle.Base
{
    /// <summary>
    /// 战斗界面
    /// </summary>
    public class BattleView : UIView, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        #region UI组件

        [InjectUI] private ScrollRect svActionbar;
        [InjectUI] private ScrollRect svPoint;
        [InjectUI] private ScrollRect svWaitQueueArea;

        [InjectUI] private TextMeshProUGUI txtCount;
        [InjectUI] private TextMeshProUGUI txtDmg;
        [InjectUI] private TextMeshProUGUI txtActingTip;
        [InjectUI] private TextMeshProUGUI txtUltimateTip;
        [InjectUI] private TextMeshProUGUI txtTitle;

        [InjectUI] private Image imgActingIcon;
        [InjectUI] private Image imgIcon;
        
        /// <summary>
        /// 操作区域根节点
        /// </summary>
        [InjectUI(1)] public RectTransform OperatorArea { get; private set; }

        /// <summary>
        /// 我方状态根节点
        /// </summary>
        [InjectUI(1)] public RectTransform PlayerArea { get; private set; }

        /// <summary>
        /// 目标标记根节点
        /// </summary>
        [InjectUI(1)] public RectTransform SelectMarkerArea { get; private set; }

        /// <summary>
        /// 怪物状态根节点
        /// </summary>
        [InjectUI(1)] public RectTransform MonsterStateArea { get; private set; }

        /// <summary>
        /// 战斗状态提示根节点
        /// </summary>
        [InjectUI(1)] public RectTransform BattleStateTipArea { get; private set; }

        /// <summary>
        /// 状态文本根节点
        /// </summary>
        [InjectUI(1)] public RectTransform BuffTextArea { get; private set; }

        /// <summary>
        /// 战斗信息根节点
        /// </summary>
        [InjectUI(1)] public RectTransform BattleMsgArea { get; private set; }

        /// <summary>
        /// 总伤害根节点
        /// </summary>
        [InjectUI(1)] public RectTransform TotalDmgArea { get; private set; }

        /// <summary>
        /// 立绘展示根节点
        /// </summary>
        [InjectUI(1)] public RectTransform PaintingDisplayArea { get; private set; }
        
        /// <summary>
        /// 行动提示根节点
        /// </summary>
        [InjectUI(1)] public RectTransform ActingTipArea { get; private set; }

        #endregion
        
        /// <summary>
        /// 技能按键UI列表
        /// </summary>
        public List<SkillKeyUI> SkillKeyUIs { get; } = new();
        
        /// <summary>
        /// 角色状态UI列表
        /// </summary>
        public List<RoleStateBar> RoleStateUIs { get; } = new();
        
        /// <summary>
        /// 战技点UI列表
        /// </summary>
        public List<BattlePointUI> BattlePointUIs { get; } = new();
        
        /// <summary>
        /// 选择标记UI列表
        /// </summary>
        public List<SelectMarkerUI> SelectMarkerUIs { get; } = new();
        
        // 当前累计伤害
        private long currentCalcDamage;
        
        /// <summary>
        /// 行动条格子UI列表
        /// </summary>
        public List<ActionGridUI> ActionGridUis { get; } = new();
        
        /// <summary>
        /// 等待行动UI列表
        /// </summary>
        public List<WaitingActUI> WaitingActUIs { get; } = new();
        
        /// <summary>
        /// 行动条内容
        /// </summary>
        public RectTransform ActionBarContent => svActionbar.content;

        /// <summary>
        /// 战技点UI根节点
        /// </summary>
        public RectTransform PointContent => svPoint.content;

        /// <summary>
        /// 等待队列内容
        /// </summary>
        public RectTransform WaitQueueContent => svWaitQueueArea.content;

        /// <summary>
        /// 技能按键组
        /// </summary>
        public ToggleGroup SkillKeyGroup => binder.GetControl<ToggleGroup>(nameof(OperatorArea));
        
        /// <summary>
        /// 行动提示UI对象
        /// </summary>
        public ActingTipUI ActingTipUI { get; private set; }
        
        /// <summary>
        /// 当前执行指令格子对象
        /// </summary>
        public ActionExecuteGridUI ActionExecuteGridUI { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            
            BattleStateTipArea.gameObject.SetActive(false);
            TotalDmgArea.gameObject.SetActive(false);
            PaintingDisplayArea.gameObject.SetActive(false);

            ActingTipUI = ActingTipArea.gameObject.AddComponent<ActingTipUI>();
            ActionExecuteGridUI = ActionBarContent.GetComponentInChildren<ActionExecuteGridUI>(true);
        }

        protected override void Start()
        {
            ActingTipUI.Init(imgActingIcon, txtActingTip);
            ActingTipUI.gameObject.SetActive(false);
        }

        public void InitActionExecuteGrid(ActionExecuteGridLogic actionExecuteGridLogic)
        {
            ActionExecuteGridUI.Init(actionExecuteGridLogic);
        }

        public void InitActingTip()
        {
            
        }

        /// <summary>
        /// 更新累计总伤害
        /// </summary>
        /// <param name="dmg"></param>
        public void UpdateCumulativeTotalDmg(long dmg)
        {
            txtDmg.text = dmg.ToString();
        }

        /// <summary>
        /// 设置战斗状态提示区域文本
        /// 若是true则为战斗结束，否则为战斗开始
        /// </summary>
        /// <param name="isBattleOver"></param>
        public void SetBattleStateTipAreaText(bool isBattleOver)
        {
            txtTitle.text = isBattleOver ? "结束战斗" : "战斗开始";
        }

        /// <summary>
        /// 更新终结技显示
        /// </summary>
        /// <param name="isShow"></param>
        /// <param name="icon"></param>
        /// <param name="tip"></param>
        public void UpdateUltimateShow(bool isShow, Sprite icon, string tip)
        {
            PaintingDisplayArea.gameObject.SetActive(isShow);
            if (!isShow)
            {
                return;
            }
            
            imgIcon.sprite = icon;
            txtUltimateTip.text = tip;
        }

        /// <summary>
        /// 更新战技点数量
        /// </summary>
        /// <param name="current"></param>
        public void UpdateBattlePointCount(int current)
        {
            txtCount.text = current.ToString();
        }
        
        /// <summary>
        /// 设置累计伤害文本
        /// </summary>
        /// <param name="dmg"></param>
        /// <param name="isClear"></param>
        /// <returns></returns>
        public long SetCumulativeDamage(int dmg, bool isClear)
        {
            if (!isClear)
            {
                currentCalcDamage += dmg;
            }
            else
            {
                currentCalcDamage = 0;
            }

            return currentCalcDamage;
        }
        
        // 激活拖拽的最小偏移
        private const float activateThreshold = 2f;
        // 拖拽阈值（超过该距离判定为拖拽，否则为点击）
        private const float dragThreshold = 50f;
        // 累计偏移量
        private float nowDeltaX;

        public event Action OnClick;
        
        public event Action<float> OnDragging;

        public event Action<bool> OnRebound;

        public event Action OnLeftDrag;
        
        public event Action OnRightDrag;
        
        
        protected override void OnPointerDown(PointerEventData eventData)
        {
            // 仅处理左键/主键；eventData.pressPosition 已经是按下坐标，无需手动记录
            if (eventData.button != PointerEventData.InputButton.Left) 
                return;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            // 等价于原代码"进入拖拽状态"：重置累计偏移
            nowDeltaX = 0;
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) 
                return;

            // 核心修正：eventData.delta.x 就是本帧 X 偏移增量，替代 lastMouseX 手工计算
            var deltaX = eventData.delta.x;
            nowDeltaX += deltaX;
            OnDragging?.Invoke(deltaX);

            if (Mathf.Abs(nowDeltaX) > dragThreshold)
            {
                if (nowDeltaX > 0) 
                    OnRightDrag?.Invoke();
                else if (nowDeltaX < 0) 
                    OnLeftDrag?.Invoke();
                nowDeltaX = 0;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // 只有"拖拽中松手"才会走到这里，等价于原释放分支里的 OnRebound(true)
            if (eventData.button != PointerEventData.InputButton.Left) 
                return;
            OnRebound?.Invoke(true);
        }

        protected override void OnPointerClick(PointerEventData eventData)
        {
            // 只有"按下后未拖动就松开"才会走到这里
            if (eventData.button != PointerEventData.InputButton.Left) 
                return;
            OnClick?.Invoke();
        }
    }
}