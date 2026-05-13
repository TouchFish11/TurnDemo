using System.Collections.Generic;
using System.Linq;
using Core.AssetBundles.Management;
using Core.UI;
using Core.UI.ViewController;
using HotUpdate.Game.Battle.UI.ActionLine;
using HotUpdate.Game.Battle.UI.BattlePoint;
using HotUpdate.Game.Battle.UI.Role;
using HotUpdate.Game.Battle.UI.SkillKey;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.Game.Battle.UI.Base
{
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
        /// ��������
        /// </summary>
        [InjectUI(1)] public RectTransform OperatorArea { get; private set; }

        /// <summary>
        /// ���״̬����
        /// </summary>
        [InjectUI(1)] public RectTransform PlayerArea { get; private set; }

        /// <summary>
        /// Ŀ��������
        /// </summary>
        [InjectUI(1)] public RectTransform SelectMarkerArea { get; private set; }

        /// <summary>
        /// ����״̬����
        /// </summary>
        [InjectUI(1)] public RectTransform MonsterStateArea { get; private set; }

        /// <summary>
        /// ս����������
        /// </summary>
        [InjectUI(1)] public RectTransform BattleStateTipArea { get; private set; }

        /// <summary>
        /// ״̬�ı�����
        /// </summary>
        [InjectUI(1)] public RectTransform BuffTextArea { get; private set; }

        /// <summary>
        /// ս����Ϣ����
        /// </summary>
        [InjectUI(1)] public RectTransform BattleMsgArea { get; private set; }

        /// <summary>
        /// ���˺�����
        /// </summary>
        [InjectUI(1)] public RectTransform TotalDmgArea { get; private set; }

        /// <summary>
        /// ������ʾ����
        /// </summary>
        [InjectUI(1)] public RectTransform PaintingDisplayArea { get; private set; }
        
        /// <summary>
        /// 
        /// </summary>
        [InjectUI(1)] public RectTransform ActingTipArea { get; private set; }

        /// <summary>
        /// �ж�������
        /// </summary>
        public Transform ActionBarContent => svActionbar.content;

        /// <summary>
        /// ս��������
        /// </summary>
        public Transform PointContent => svPoint.content;

        /// <summary>
        /// �ȴ���������
        /// </summary>
        public Transform WaitQueueContent => svWaitQueueArea.content;

        /// <summary>
        /// ���ܼ���
        /// </summary>
        public ToggleGroup SkillKeyGroup => binder.GetControl<ToggleGroup>(nameof(OperatorArea));
        
        /// <summary>
        /// �ж���ʾUI����
        /// </summary>
        public ActingTipUI ActingTipUI { get; private set; }
        
        
        // �ж�������UI�б�
        private readonly List<PoolObject<ActionGridUI>> actions = new();
        // ���ܰ���UI�б�
        private readonly List<PoolObject<SkillKeyUI>> skillKeyUIs = new();
        // ��ɫ״̬UI�б�
        private readonly List<PoolObject<RoleStateUI>> roleStateUIs = new();
        // ս����UI�б�
        private readonly List<PoolObject<BattlePointUI>> battlePointUIs = new();
        // ѡ����UI�б�
        private readonly List<PoolObject> selectMarkerUIs = new();
        // �ȴ��ж������б�
        private readonly List<PoolObject<WaitingActUI>> waitingActUIs = new();
        // ��ǰ�ۼ��˺�
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
        /// �������˺�
        /// </summary>
        /// <param name="dmg"></param>
        public void UpdateTotalDmg(long dmg)
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
        /// �����սἼ��ʾ
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
        /// ����ս������
        /// </summary>
        /// <param name="current"></param>
        public void UpdateBattlePointCount(int current)
        {
            txtCount.text = current.ToString();
        }
        
        /// <summary>
        /// ͨ��ID��ȡ��ɫ״̬UI
        /// ʹ��Linq��ѯ
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns>δ�ҵ�����null</returns>
        public RoleStateUI GetRoleStateUIById(int roleId)
        {
            return roleStateUIs.FirstOrDefault(r => r.Obj.RoleId == roleId).Obj;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="waitingActUI"></param>
        public void CacheWaitingCommmand(PoolObject<WaitingActUI> waitingActUI)
        {
            waitingActUIs.Add(waitingActUI);
        }

        public void ClearWaitingActUI()
        {
            foreach (var waitingActUI in waitingActUIs)
            {
                waitingActUI.Collect();
            }
            waitingActUIs.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearActionBar()
        {
            foreach (var actionGridUI in actions)
            {
                actionGridUI.Collect();
            }
            actions.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionGridUI"></param>
        public void UpdateAcitonbar(PoolObject<ActionGridUI> actionGridUI)
        {
            actions.Add(actionGridUI);
        }

        /// <summary>
        /// ��ȡ���е��ж�����
        /// </summary>
        /// <returns></returns>
        public List<ActionGridUI> GetActionGridUIs()
        {
            return actions.ConvertAll(p => p.Obj);
        }

        /// <summary>
        /// ���ò���UI
        /// </summary>
        /// <param name="skillKeyUIs"></param>
        public void SetOperator(List<PoolObject<SkillKeyUI>> skillKeyUIs)
        {
            foreach (var skillKeyUI in this.skillKeyUIs)
            {
                skillKeyUI.Collect();
            }
            this.skillKeyUIs.Clear();
            this.skillKeyUIs.AddRange(skillKeyUIs);
        }

        /// <summary>
        /// �������UI
        /// </summary>
        public void ClearOperator()
        {
            foreach (var skillKeyUI in skillKeyUIs)
            {
                skillKeyUI.Collect();
            }
            skillKeyUIs.Clear();
        }

        /// <summary>
        /// ����ս������
        /// </summary>
        /// <param name="current"></param>
        /// <param name="battlePointUIs"></param>
        public void UpdateBattlePointCount(int current, IEnumerable<PoolObject<BattlePointUI>> battlePointUIs)
        {
            foreach (var battlePointUI in this.battlePointUIs)
            {
                battlePointUI.Collect();
            }
            this.battlePointUIs.Clear();
            this.battlePointUIs.AddRange(battlePointUIs);
        }
        
        /// <summary>
        ///  缓存目标标记
        /// </summary>
        /// <param name="selectMarkerUI"></param>
        public void AddSelectMarker(PoolObject selectMarkerUI)
        {
            selectMarkerUIs.Add(selectMarkerUI);
        }
        
        /// <summary>
        /// 清理所有标记
        /// </summary>
        public void ClearSelectMarkers()
        {
            foreach (var selectMarkerUI in selectMarkerUIs)
            {
                selectMarkerUI.Collect();
            }
            selectMarkerUIs.Clear();
        }

        /// <summary>
        /// ��ʼ����ɫ״̬UI
        /// </summary>
        /// <param name="roleStateUI"></param>
        public void InitRoleStateUI(PoolObject<RoleStateUI> roleStateUI)
        {
            roleStateUIs.Add(roleStateUI);
        }

        /// <summary>
        /// �����ۼ��˺��ı�
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
