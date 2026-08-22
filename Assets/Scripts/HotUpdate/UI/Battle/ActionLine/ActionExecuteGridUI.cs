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
        
        // 闪烁动画的速度
        [SerializeField] public float falshSpeed = 1.5f;
        
        // 闪烁特效的根节点
        [InjectUI(1)] public RectTransform Flashing { get; set; }
        
        // 闪烁特效下的所有图片组件
        public Image[] Images { get; private set; }
        
        public RectTransform RectTransform => transform as RectTransform;
        
        private ActionExecuteGridLogic _logic;
        
        protected override void Awake()
        {
            base.Awake();
            
            // 初始状态隐藏闪烁特效
            Images = Flashing.GetComponentsInChildren<Image>();
            Flashing.gameObject.SetActive(false);
            imgIcon.color = new Color(imgIcon.color.r, imgIcon.color.g, imgIcon.color.b, 0);
        }
        
        /// <summary>
        /// 组件启用时调用（生命周期）
        /// 注册帧更新监听，用于处理动画逻辑
        /// </summary>
        protected override void OnEnable()
        {
            _logic?.OnEnable();
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
        /// 组件禁用时调用（生命周期）
        /// 移除帧更新监听，避免无效计算
        /// </summary>
        protected override void OnDisable()
        {
            _logic?.OnDisable();
        }

        protected override void OnDestroy()
        {
            _logic.Dispose();
            base.OnDestroy();
        }
    }
}
