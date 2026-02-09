using Core.UI;
using Core.UI.MVC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameHotUpdate.Battle.UI.Base
{
    public class BattleView : UIView
    {
        [Inject] private ScrollRect svActionbar;
        [Inject] private ScrollRect svPoint;
        [Inject] private ScrollRect svWaitQueueArea;

        [Inject] private TextMeshProUGUI txtCount;
        [Inject] private TextMeshProUGUI txtDmg;
        [Inject] private TextMeshProUGUI txtActingTip;
        [Inject] private TextMeshProUGUI txtUltimateTip;

        [Inject] private Image imgActingIcon;
        [Inject] private Image imgIcon;
        
        /// <summary>
        /// ��������
        /// </summary>
        [Inject(1)] public RectTransform OperatorArea { get; private set; }

        /// <summary>
        /// ���״̬����
        /// </summary>
        [Inject(1)] public RectTransform PlayerArea { get; private set; }

        /// <summary>
        /// Ŀ��������
        /// </summary>
        [Inject(1)] public RectTransform SelectMarkerArea { get; private set; }

        /// <summary>
        /// ����״̬����
        /// </summary>
        [Inject(1)] public RectTransform MonsterStateArea { get; private set; }

        /// <summary>
        /// ս����������
        /// </summary>
        [Inject(1)] public RectTransform BattleOverArea { get; private set; }

        /// <summary>
        /// ״̬�ı�����
        /// </summary>
        [Inject(1)] public RectTransform BuffTextArea { get; private set; }

        /// <summary>
        /// ս����Ϣ����
        /// </summary>
        [Inject(1)] public RectTransform BattleMsgArea { get; private set; }

        /// <summary>
        /// ���˺�����
        /// </summary>
        [Inject(1)] public RectTransform TotalDmgArea { get; private set; }

        /// <summary>
        /// ������ʾ����
        /// </summary>
        [Inject(1)] public RectTransform PaintingDisplayArea { get; private set; }
        
        /// <summary>
        /// 
        /// </summary>
        [Inject(1)] public RectTransform ActingTipArea { get; private set; }

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

            BattleOverArea.gameObject.SetActive(false);
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
