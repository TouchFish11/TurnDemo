using Core.UI;
using HotUpdate.Game.Battle.Object;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.UI.Battle.ActionLine
{
    /// <summary>
    /// 行动格子UI组件
    /// 负责战斗场景中行动格子的视觉表现、选中状态、闪烁动画、位移动画等逻辑
    /// </summary>
    public class ActionGridUI : UIBehaviourBase, ILogicView<ActionGridUI, ActionGridLogic>
    {
        // 选中状态的背景图片
        [InjectUI] public Image imgSelect;
        // 行动格子的图标图片
        [InjectUI] public Image imgIcon;
        // 行动值显示文本
        [InjectUI] public TextMeshProUGUI txtActionValue;
        /// 闪烁特效的根节点
        [InjectUI(1)] public RectTransform Flashing { get; set; }
        
        // 选中框水平移动的范围
        [SerializeField] public float moveRange = 3f;
        // 选中框移动的速度
        [SerializeField] public float moveSpeed = 5f;
        // 闪烁动画的速度
        [SerializeField] public float falshSpeed = 1.5f;
        // 格子滑动速度
        [SerializeField] public float slidingSpeed = 5f;
        // 格子间间隙
        [SerializeField] public float space = 10f;

        private ActionGridLogic _actionGridLogic;
        
        /// <summary>
        /// 选中框的矩形变换组件
        /// </summary>
        public RectTransform ImgSelectRect { get; private set; }
        
        /// <summary>
        /// 闪烁特效下的所有图片组件
        /// </summary>
        public Image[] Images { get; private set; }
        
        /// <summary>
        /// 自身UI变换组件
        /// </summary>
        public RectTransform RectTransform { get; private set; }
        
        /// <summary>
        /// 选中框的初始本地位置（用于位移动画复位）
        /// </summary>
        public Vector3 InitLocalPos { get; private set; }
        
        /// <summary>
        /// 当前格子是否处于选中状态
        /// </summary>
        public bool IsSelect => _actionGridLogic.IsSelect;

        /// <summary>
        /// 绑定的战斗实体对象
        /// </summary>
        public IBattleEntityObject BattleEntity => _actionGridLogic.BattleEntity;
        
        protected override void Awake()
        {
            base.Awake();
            // 获取选中框的矩形变换组件并记录初始位置
            ImgSelectRect = imgSelect.rectTransform;
            InitLocalPos = ImgSelectRect.localPosition;
            RectTransform = (RectTransform)transform;
            // 初始状态隐藏选中框和闪烁特效
            imgSelect.gameObject.SetActive(false);
            Images = Flashing.GetComponentsInChildren<Image>();
            Flashing.gameObject.SetActive(false);
        }

        protected override void OnEnable()
        {
            _actionGridLogic?.OnEnable();
        }

        public void Init(ActionGridLogic logic)
        {
            _actionGridLogic = logic;
        }
        
        /// <summary>
        /// 检查并更新选中状态
        /// </summary>
        /// <param name="battleEntity">当前选中的战斗实体</param>
        public void CheckSelect(IBattleEntityObject battleEntity)
        {
            // 判断当前格子绑定的实体是否为选中实体
            _actionGridLogic.CheckSelect(battleEntity);
        }

        /// <summary>
        /// 设置格子图标
        /// </summary>
        /// <param name="icon"></param>
        public void SetIcon(Sprite icon)
        {
            imgIcon.sprite = icon;
        }

        /// <summary>
        /// 设置剩余行动值
        /// </summary>
        /// <param name="remainActionValue"></param>
        public void SetActionValue(float remainActionValue)
        {
            txtActionValue.text = remainActionValue.ToString();
        }
        
        /// <summary>
        /// 设置UI格子滑动到的目标位置索引
        /// </summary>
        /// <param name="targetIndex"></param>
        public void SetSlideTarget(int targetIndex)
        {
            _actionGridLogic.SetSlideTarget(targetIndex);
        }

        protected override void OnDisable()
        {
            _actionGridLogic?.OnDisable();
        }

        protected override void OnDestroy()
        {
            _actionGridLogic.Dispose();
            base.OnDestroy();
        }
    }
}