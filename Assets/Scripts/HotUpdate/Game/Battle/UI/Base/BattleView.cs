using Core.UI;
using Core.UI.ViewController;
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
    }
}
