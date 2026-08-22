using Core.DI;
using Core.Mono;
using Core.Pool;
using Core.Time;
using Core.UI;
using HotUpdate.Game.Battle.Object;
using UnityEngine;

namespace HotUpdate.UI.Battle.ActionLine
{
    public class ActionExecuteGridLogic : IUILogic<ActionExecuteGridUI, ActionExecuteGridLogic>, IPoolData
    {
        [Inject] private IMonoAdapter _monoAdapter;
        [Inject] private IPoolManager _poolManager;
        
        // 闪烁图片当前的透明度
        public float currentAlpha = 1f;
        // 闪烁动画的计时变量
        public float time;
        
        public ActionExecuteGridUI View { get; private set; }
        
        /// <summary>
        /// 当前格子是否处于选中状态
        /// </summary>
        public bool IsSelect { get; private set; }
        
        /// <summary>
        /// 绑定的战斗实体对象
        /// </summary>
        public IBattleEntityObject BattleEntity { get; private set; }
        
        public void OnEnable()
        {
            _monoAdapter.AddUpdateListener(OnUpdate);
        }
        
        /// <summary>
        /// 初始化UI数据
        /// </summary>
        /// <param name="icon">格子显示的图标</param>
        /// <param name="battleEntity"></param>
        public void UpdateGrid(Sprite icon, IBattleEntityObject battleEntity)
        {
            BattleEntity = battleEntity;
            View.imgIcon.sprite = icon;
            var alpha = icon ? 1f : 0f;
            View.imgIcon.color = new Color(View.imgIcon.color.r, View.imgIcon.color.g, View.imgIcon.color.b, alpha);
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
            View.Flashing.gameObject.SetActive(IsSelect);
            // 重置所有闪烁图片的颜色为白色（初始状态）
            foreach (var image in View.Images)
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
            time += TimeUtil.DeltaTime * View.falshSpeed;
            currentAlpha = 1 - Mathf.PingPong(time, 1f);

            // 应用透明度到所有闪烁图片
            var color = new Color(1, 1, 1, currentAlpha);
            foreach (var image in View.Images)
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
        
        public void OnDisable()
        {
            _monoAdapter.RemoveUpdateListener(OnUpdate);
        }
        
        public void Dispose()
        {
            _poolManager.PushData(this);
        }

        void IPoolData.ResetData()
        {
            
        }
    }
}
