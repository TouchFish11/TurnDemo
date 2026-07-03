using Core.DI;
using Core.Mono;
using Core.UI;
using HotUpdate.Game.Battle.Object;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate.UI.Battle.ActionLine
{
    /// <summary>
    /// 行动执行格子
    /// </summary>
    public class ActionExecuteGrid : UIBehaviourBase
    {
        // 行动格子的图标图片
        [InjectUI] private Image imgIcon;
        
        // 闪烁动画的速度
        [SerializeField] private float falshSpeed = 1.5f;
        
        // 闪烁特效的根节点
        [InjectUI(1)] private RectTransform Flashing { get; set; }
        
        // 闪烁特效下的所有图片组件
        private Image[] images;
        // 闪烁图片当前的透明度
        private float currentAlpha = 1f;
        // 闪烁动画的计时变量
        private float time;
        
        /// <summary>
        /// 绑定的战斗实体对象
        /// </summary>
        public IBattleEntityObject BattleEntity { get; private set; }
        
        /// <summary>
        /// 只读属性：当前格子是否处于选中状态
        /// </summary>
        public bool IsSelect { get; private set; }
        
        public RectTransform RectTransform => transform as RectTransform;
        
        protected override void Awake()
        {
            base.Awake();
            
            // 初始状态隐藏闪烁特效
            images = Flashing.GetComponentsInChildren<Image>();
            Flashing.gameObject.SetActive(false);
            imgIcon.color = new Color(imgIcon.color.r, imgIcon.color.g, imgIcon.color.b, 0);
        }
        
        /// <summary>
        /// 组件启用时调用（生命周期）
        /// 注册帧更新监听，用于处理动画逻辑
        /// </summary>
        protected override void OnEnable()
        {
            DIContainer.GetInstance<IMonoAdapter>().AddUpdateListener(OnUpdate);
        }

        /// <summary>
        /// 初始化UI数据
        /// </summary>
        /// <param name="icon">格子显示的图标</param>
        /// <param name="battleEntity"></param>
        public void UpdateGrid(Sprite icon, IBattleEntityObject battleEntity)
        {
            BattleEntity = battleEntity;
            imgIcon.sprite = icon;
            var alpha = icon ? 1f : 0f;
            imgIcon.color = new Color(imgIcon.color.r, imgIcon.color.g, imgIcon.color.b, alpha);
        }
        
        /// <summary>
        /// 检查并更新选中状态
        /// </summary>
        /// <param name="battleEntity">当前选中的战斗实体</param>
        public bool CheckSelect(IBattleEntityObject battleEntity)
        {
            // 判断当前格子绑定的实体是否为选中实体
            IsSelect = BattleEntity == battleEntity;
            // 设置闪烁特效状态
            SetFlashing();
            return IsSelect;
        }
        
        /// <summary>
        /// 设置闪烁特效状态
        /// 选中时启用闪烁特效并重置动画参数，未选中时关闭
        /// </summary>
        private void SetFlashing()
        {
            time = 0;
            Flashing.gameObject.SetActive(IsSelect);
            // 重置所有闪烁图片的颜色为白色（初始状态）
            foreach (var image in images)
            {
                image.color = Color.white;
            }
        }
        
        /// <summary>
        /// 闪烁动画
        /// </summary>
        private void FlashAnim()
        {
            if (!IsSelect)
            {
                return;
            }
            
            // 闪烁特效透明度计算（PingPong实现0-1之间的往复变化）
            time += Time.deltaTime * falshSpeed;
            currentAlpha = 1 - Mathf.PingPong(time, 1f);

            // 应用透明度到所有闪烁图片
            var color = new Color(1, 1, 1, currentAlpha);
            foreach (var image in images)
            {
                image.color = color;
            }
        }
        
        /// <summary>
        /// 帧更新逻辑（仅在选中状态下执行）
        /// 处理选中框的水平位移动画和闪烁特效的透明度动画
        /// </summary>
        private void OnUpdate()
        {
            FlashAnim();
        }
        
        /// <summary>
        /// 组件禁用时调用（生命周期）
        /// 移除帧更新监听，避免无效计算
        /// </summary>
        protected override void OnDisable()
        {
            DIContainer.GetInstance<IMonoAdapter>().RemoveUpdateListener(OnUpdate);
        }
    }
}
