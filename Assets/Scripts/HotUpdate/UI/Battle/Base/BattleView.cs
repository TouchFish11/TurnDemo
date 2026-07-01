using System.Collections.Generic;
using System.Linq;
using Core.AssetBundles.Management;
using Core.UI;
using Core.UI.ViewController;
using HotUpdate.UI.Battle.ActionLine;
using HotUpdate.UI.Battle.BattlePoint;
using HotUpdate.UI.Battle.Role;
using HotUpdate.UI.Battle.SkillKey;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.UI.Battle.Base
{
    /// <summary>
    /// 战斗界面
    /// </summary>
    public class BattleView : UIView
    {
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

        /// <summary>
        /// 行动条内容
        /// </summary>
        public Transform ActionBarContent => svActionbar.content;

        /// <summary>
        /// 战技点UI根节点
        /// </summary>
        public Transform PointContent => svPoint.content;

        /// <summary>
        /// 等待队列内容
        /// </summary>
        public Transform WaitQueueContent => svWaitQueueArea.content;

        /// <summary>
        /// 技能按键组
        /// </summary>
        public ToggleGroup SkillKeyGroup => binder.GetControl<ToggleGroup>(nameof(OperatorArea));
        
        /// <summary>
        /// 行动提示UI实例
        /// </summary>
        public ActingTipUI ActingTipUI { get; private set; }
        
        
        // 行动条格子UI列表
        private readonly List<ActionGridUI> actions = new();
        // 技能按键UI列表
        private readonly List<SkillKeyUI> skillKeyUIs = new();
        // 角色状态UI列表
        private readonly List<RoleStateUI> roleStateUIs = new();
        // 战技点UI列表
        private readonly List<BattlePointUI> battlePointUIs = new();
        // 选择标记UI列表
        private readonly List<SelectMarkerUI> selectMarkerUIs = new();
        // 等待行动UI列表
        private readonly List<WaitingActUI> waitingActUIs = new();
        // 当前累计伤害
        private long currentCalcDamage;

        protected override void Awake()
        {
            base.Awake();

            BattleStateTipArea.gameObject.SetActive(false);
            TotalDmgArea.gameObject.SetActive(false);
            PaintingDisplayArea.gameObject.SetActive(false);

            ActingTipUI = ActingTipArea.gameObject.AddComponent<ActingTipUI>();
        }

        protected override void Start()
        {
            ActingTipUI.Init(imgActingIcon, txtActingTip);
            ActingTipUI.gameObject.SetActive(false);
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
        /// 通过ID获取角色状态UI
        /// 使用Linq查询
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns>未找到返回null</returns>
        public RoleStateUI GetRoleStateUIById(int roleId)
        {
            return roleStateUIs.FirstOrDefault(r => r.RoleId == roleId);
        }
        
        /// <summary>
        /// 缓存等待指令UI
        /// </summary>
        /// <param name="waitingActUI"></param>
        public void CacheWaitingCommmand(WaitingActUI waitingActUI)
        {
            waitingActUIs.Add(waitingActUI);
        }

        public void ClearWaitingActUI(ObjectSpawner spawner)
        {
            foreach (var waitingActUI in waitingActUIs)
            {
                spawner.Release(waitingActUI);
            }
            waitingActUIs.Clear();
        }

        /// <summary>
        /// 清空行动条
        /// </summary>
        /// <param name="spawner"></param>
        public void ClearActionBar(ObjectSpawner spawner)
        {
            foreach (var actionGridUI in actions)
            {
                spawner.Release(actionGridUI);
            }
            actions.Clear();
        }

        /// <summary>
        /// 更新行动条
        /// </summary>
        /// <param name="actionGridUI"></param>
        public void UpdateAcitonbar(ActionGridUI actionGridUI)
        {
            actions.Add(actionGridUI);
        }

        /// <summary>
        /// 获取所有的行动条格子UI
        /// </summary>
        /// <returns></returns>
        public List<ActionGridUI> GetActionGridUIs()
        {
            return actions.ConvertAll(p => p);
        }

        /// <summary>
        /// 设置操作UI
        /// </summary>
        /// <param name="skillKeyUIs"></param>
        /// <param name="spawner"></param>
        public void SetOperator(List<SkillKeyUI> skillKeyUIs, ObjectSpawner spawner)
        {
            foreach (var skillKeyUI in this.skillKeyUIs)
            {
                spawner.Release(skillKeyUI);
            }
            this.skillKeyUIs.Clear();
            this.skillKeyUIs.AddRange(skillKeyUIs);
        }

        /// <summary>
        /// 清空操作UI
        /// </summary>
        /// <param name="spawner"></param>
        public void ClearOperator(ObjectSpawner spawner)
        {
            foreach (var skillKeyUI in skillKeyUIs)
            {
                spawner.Release(skillKeyUI);
            }
            skillKeyUIs.Clear();
        }

        /// <summary>
        /// 更新战点数量
        /// </summary>
        /// <param name="current"></param>
        /// <param name="battlePointUIs"></param>
        /// <param name="spawner"></param>
        public void UpdateBattlePointCount(int current, IEnumerable<BattlePointUI> battlePointUIs, ObjectSpawner spawner)
        {
            foreach (var battlePointUI in this.battlePointUIs)
            {
                spawner.Release(battlePointUI);
            }
            this.battlePointUIs.Clear();
            this.battlePointUIs.AddRange(battlePointUIs);
        }
        
        /// <summary>
        /// 缓存目标标记
        /// </summary>
        /// <param name="selectMarkerUI"></param>
        public void AddSelectMarker(SelectMarkerUI selectMarkerUI)
        {
            selectMarkerUIs.Add(selectMarkerUI);
        }

        /// <summary>
        /// 清理所有标记
        /// </summary>
        /// <param name="spawner"></param>
        public void ClearSelectMarkers(ObjectSpawner spawner)
        {
            foreach (var selectMarkerUI in selectMarkerUIs)
            {
                spawner.Release(selectMarkerUI);
            }
            selectMarkerUIs.Clear();
        }

        /// <summary>
        /// 初始化角色状态UI
        /// </summary>
        /// <param name="roleStateUI"></param>
        public void InitRoleStateUI(RoleStateUI roleStateUI)
        {
            roleStateUIs.Add(roleStateUI);
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
    }
}