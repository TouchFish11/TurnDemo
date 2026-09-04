using Core.UI;
using HotUpdate.Game.Battle.Object;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.UI.Battle.ActionLine
{
    /// <summary>
    /// 行动执行格子
    /// </summary>
    public class ActionExecuteGridUI : UIBehaviourBase, ILogicView<ActionExecuteGridUI, ActionExecuteGridLogic>
    {
        // 行动格子的图标图片
        [InjectUI] public Image imgIcon;
        [InjectUI] public Button btnClick;
        
        // 闪烁动画的速度
        [SerializeField] public float falshSpeed = 1.5f;
        
        // 闪烁特效的根节点
        [InjectUI(1)] public RectTransform Flashing { get; set; }
        [InjectUI(1)] public RectTransform ClickSelect { get; private set; }
        
        // 闪烁特效下的所有图片组件
        public Image[] Images { get; private set; }
        
        public RectTransform RectTransform => transform as RectTransform;
        
        public IBattleEntityObject BattleEntity => _logic.BattleEntity;
        
        private ActionExecuteGridLogic _logic;
        
        protected override void Awake()
        {
            base.Awake();
            
            // 初始状态隐藏闪烁特效
            Images = Flashing.GetComponentsInChildren<Image>();
            Flashing.gameObject.SetActive(false);
            ClickSelect.gameObject.SetActive(false);
            imgIcon.color = new Color(imgIcon.color.r, imgIcon.color.g, imgIcon.color.b, 0);
        }

        public void Init(ActionExecuteGridLogic logic)
        {
            _logic = logic;
        }
        
        /// <summary>
        /// 初始化UI数据
        /// </summary>
        /// <param name="icon">格子显示的图标</param>
        /// <param name="battleEntity"></param>
        public void UpdateGrid(Sprite icon, IBattleEntityObject battleEntity)
        {
            _logic.UpdateGrid(icon, battleEntity);
        }
        
        /// <summary>
        /// 检查并更新选中状态
        /// </summary>
        /// <param name="battleEntity">当前选中的战斗实体</param>
        public bool CheckSelect(IBattleEntityObject battleEntity)
        {
            return _logic.CheckSelect(battleEntity);
        }
        
        /// <summary>
        /// 点击的选中，其它的隐藏，互斥
        /// </summary>
        /// <param name="isSelect"></param>
        public void SetClickSelect(bool isSelect)
        {
            _logic.SetClickSelect(isSelect);
            ClickSelect.gameObject.SetActive(isSelect);
        }
        
        protected override void OnButtonClick(string btnName)
        {
            if (btnName == nameof(btnClick))
            {
                SetClickSelect(true);
            }
        }

        protected override void OnDisable()
        {
            _logic?.Dispose();
        }
    }
}
