using System.Collections.Generic;
using System.Linq;
using Core.AssetBundles.Management;
using Core.UI;
using Core.UI.ViewController;
using HotUpdate.Game.Battle.UI;
using HotUpdate.UI.Battle.ActionLine;
using HotUpdate.UI.Battle.BattlePoint;
using HotUpdate.UI.Battle.Role;
using HotUpdate.UI.Battle.SkillKey;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.UI.Battle.Base
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
        private readonly List<ActionGridUI> actions = new();
        // ���ܰ���UI�б�
        private readonly List<SkillKeyUI> skillKeyUIs = new();
        // ��ɫ״̬UI�б�
        private readonly List<RoleStateUI> roleStateUIs = new();
        // ս����UI�б�
        private readonly List<BattlePointUI> battlePointUIs = new();
        // ѡ����UI�б�
        private readonly List<SelectMarkerUI> selectMarkerUIs = new();
        // �ȴ��ж������б�
        private readonly List<WaitingActUI> waitingActUIs = new();
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
            return roleStateUIs.FirstOrDefault(r => r.RoleId == roleId);
        }
        
        /// <summary>
        /// 
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
        /// 
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
        /// 
        /// </summary>
        /// <param name="actionGridUI"></param>
        public void UpdateAcitonbar(ActionGridUI actionGridUI)
        {
            actions.Add(actionGridUI);
        }

        /// <summary>
        /// ��ȡ���е��ж�����
        /// </summary>
        /// <returns></returns>
        public List<ActionGridUI> GetActionGridUIs()
        {
            return actions.ConvertAll(p => p);
        }

        /// <summary>
        /// ���ò���UI
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
        /// �������UI
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
        /// ����ս������
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
        ///  缓存目标标记
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
        /// ��ʼ����ɫ״̬UI
        /// </summary>
        /// <param name="roleStateUI"></param>
        public void InitRoleStateUI(RoleStateUI roleStateUI)
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
